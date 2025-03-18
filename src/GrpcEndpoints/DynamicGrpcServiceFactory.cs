using System.Reflection;
using Castle.DynamicProxy;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcEndpoints;

/// <summary>
/// Factory for creating dynamic gRPC service implementations.
/// </summary>
public class DynamicGrpcServiceFactory
{
    private static readonly ProxyGenerator _proxyGenerator = new();
    
    /// <summary>
    /// Creates a dynamic implementation of a gRPC service.
    /// </summary>
    /// <param name="serviceBaseType">The gRPC service base type.</param>
    /// <param name="serviceProvider">The service provider for resolving endpoints.</param>
    /// <returns>An instance of the dynamic service.</returns>
    public static object CreateDynamicService(Type serviceBaseType, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceBaseType);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        // Find all methods in the base service type
        var serviceMethods = serviceBaseType.GetMethods(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        
        // Create a proxy class that extends the service base type
        return _proxyGenerator.CreateClassProxy(
            serviceBaseType,
            new DynamicGrpcServiceInterceptor(serviceBaseType, serviceProvider));
    }
    
    private class DynamicGrpcServiceInterceptor : IInterceptor
    {
        private readonly Type _serviceBaseType;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _endpointTypesByMethod;

        public DynamicGrpcServiceInterceptor(Type serviceBaseType, IServiceProvider serviceProvider)
        {
            _serviceBaseType = serviceBaseType;
            _serviceProvider = serviceProvider;
            _endpointTypesByMethod = BuildEndpointLookup();
        }

        private Dictionary<string, Type> BuildEndpointLookup()
        {
            var result = new Dictionary<string, Type>();
            
            // Find all endpoint types marked with our attribute
            var endpointTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<GrpcEndpointAttribute>() != null);
                
            foreach (var endpointType in endpointTypes)
            {
                var attribute = endpointType.GetCustomAttribute<GrpcEndpointAttribute>();
                if (attribute?.ServiceType == _serviceBaseType)
                {
                    result[attribute.MethodName] = endpointType;
                }
            }
            
            return result;
        }

        public void Intercept(IInvocation invocation)
        {
            // Check if this method should be handled by an endpoint
            if (_endpointTypesByMethod.TryGetValue(invocation.Method.Name, out var endpointType))
            {
                // Create a scope to resolve scoped services
                var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                
                // Get or create the endpoint instance from the scope
                var endpoint = scope.ServiceProvider.GetService(endpointType);
                if (endpoint == null)
                {
                    invocation.Proceed();
                    return;
                }
                
                // Find and invoke the ExecuteAsync method
                var executeMethod = endpointType.GetMethod("ExecuteAsync");
                if (executeMethod != null)
                {
                    var request = invocation.Arguments[0];
                    var context = invocation.Arguments[1] as ServerCallContext;
                    
                    var result = executeMethod.Invoke(endpoint, [
                        request, 
                        context, 
                        context?.CancellationToken ?? CancellationToken.None
                    ]);
                    
                    // Handle the Task<TResponse> return type
                    if (result is Task task)
                    {
                        invocation.ReturnValue = task;
                    }
                    return;
                }
            }
            
            // If no matching endpoint found or other issue, proceed with base implementation
            invocation.Proceed();
        }
    }
} 