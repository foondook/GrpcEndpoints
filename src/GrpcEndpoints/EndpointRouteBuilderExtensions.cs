using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace GrpcEndpoints;

/// <summary>
/// Extension methods for mapping dynamic gRPC services to endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps dynamic gRPC services to endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="serviceBaseTypes">The gRPC service base types to map.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGrpcEndpoints(
        this IEndpointRouteBuilder endpoints, 
        params Type[] serviceBaseTypes)
    {
        // Get the generic method info for MapGrpcService<T>
        var methodInfo = typeof(GrpcEndpointRouteBuilderExtensions)
            .GetMethod(
                nameof(GrpcEndpointRouteBuilderExtensions.MapGrpcService),
                BindingFlags.Public | BindingFlags.Static);

        if (methodInfo == null)
        {
            throw new InvalidOperationException("Could not find MapGrpcService method");
        }

        foreach (var baseType in serviceBaseTypes)
        {
            // Create a generic version of the method for the specific service type
            var genericMethod = methodInfo.MakeGenericMethod(baseType);
            
            // Call the method with the endpoints as the argument
            genericMethod.Invoke(null, [endpoints]);
        }

        return endpoints;
    }
    
    /// <summary>
    /// Maps a dynamic gRPC service to endpoints.
    /// </summary>
    /// <typeparam name="TService">The gRPC service base type.</typeparam>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapGrpcEndpoints<TService>(
        this IEndpointRouteBuilder endpoints)
        where TService : class
    {
        return MapGrpcEndpoints(endpoints, typeof(TService));
    }
} 