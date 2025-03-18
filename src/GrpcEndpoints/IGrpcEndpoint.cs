using Grpc.Core;

namespace GrpcEndpoints;

/// <summary>
/// Generic interface for gRPC service endpoints.
/// </summary>
/// <typeparam name="TRequest">The type of request message.</typeparam>
/// <typeparam name="TResponse">The type of response message.</typeparam>
public interface IGrpcEndpoint<in TRequest, TResponse>
{
    /// <summary>
    /// Executes the endpoint asynchronously.
    /// </summary>
    /// <param name="request">The request message.</param>
    /// <param name="context">The server call context.</param>
    /// <param name="cancellationToken">A token to cancel the endpoint.</param>
    /// <returns>The response message.</returns>
    Task<TResponse> ExecuteAsync(TRequest request, ServerCallContext context, CancellationToken cancellationToken);
} 