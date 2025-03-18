using Grpc.Core;

namespace GrpcEndpoints.Example.Endpoints;

[GrpcEndpoint(typeof(Greeter.GreeterBase), nameof(Greeter.GreeterBase.SayGoodbye))]
public class SayGoodbye(ILogger<SayGoodbye> logger) : IGrpcEndpoint<GoodbyeRequest, GoodbyeReply>
{
    public Task<GoodbyeReply> ExecuteAsync(GoodbyeRequest request, ServerCallContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saying goodbye to {Name}", request.Name);
        
        return Task.FromResult(new GoodbyeReply
        {
            Message = $"Goodbye {request.Name}, hope to see you again soon!"
        });
    }
} 