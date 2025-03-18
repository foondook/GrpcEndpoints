namespace GrpcEndpoints;

/// <summary>
/// Marks a class as a gRPC endpoint handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GrpcEndpointAttribute : Attribute
{
    /// <summary>
    /// Gets the gRPC service base type (e.g., MyService.MyServiceBase).
    /// </summary>
    public Type ServiceType { get; }
    
    /// <summary>
    /// Gets the method name on the service (e.g., "GetItem").
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcEndpointAttribute"/> class.
    /// </summary>
    /// <param name="serviceType">The gRPC service base type.</param>
    /// <param name="methodName">The method name on the service.</param>
    public GrpcEndpointAttribute(Type serviceType, string methodName)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }
} 