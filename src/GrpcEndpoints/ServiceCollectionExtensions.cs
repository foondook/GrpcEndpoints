using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcEndpoints;

/// <summary>
/// Extension methods for registering gRPC endpoints with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all gRPC endpoints from the specified assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for endpoints.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGrpcEndpoints(this IServiceCollection services, params Assembly[]? assemblies)
    {
        // If no assemblies provided, use the calling assembly
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }
        
        // Find and register all endpoint implementations
        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<GrpcEndpointAttribute>() != null);
            
        foreach (var endpointType in endpointTypes)
        {
            // Find the endpoint interface
            var serviceInterface = endpointType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                                     i.GetGenericTypeDefinition() == typeof(IGrpcEndpoint<,>));
                                     
            if (serviceInterface != null)
            {
                // Register the endpoint with its interface
                services.AddScoped(serviceInterface, endpointType);
                services.AddScoped(endpointType);
            }
        }
        
        return services;
    }
    
    /// <summary>
    /// Registers dynamic gRPC service implementations for the specified service base types.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceBaseTypes">The gRPC service base types to create dynamic implementations for.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDynamicGrpcServices(this IServiceCollection services, params Type[] serviceBaseTypes)
    {
        foreach (var baseType in serviceBaseTypes)
        {
            services.AddSingleton(baseType, sp => DynamicGrpcServiceFactory.CreateDynamicService(baseType, sp));
        }
        
        return services;
    }
} 