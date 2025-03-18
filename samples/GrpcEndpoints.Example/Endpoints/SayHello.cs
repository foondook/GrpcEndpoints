using Grpc.Core;

namespace GrpcEndpoints.Example.Endpoints;

[GrpcEndpoint(typeof(Greeter.GreeterBase), nameof(Greeter.GreeterBase.SayHello))]
public class SayHello(ILogger<SayHello> logger) : IGrpcEndpoint<HelloRequest, HelloReply>
{
    public Task<HelloReply> ExecuteAsync(HelloRequest request, ServerCallContext context, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saying hello to {Name}", request.Name);
        
        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name} from GrpcEndpoints!"
        });
    }
} 