using GrpcEndpoints;
using GrpcEndpoints.Example;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection(); 

// Register gRPC endpoints
builder.Services.AddGrpcEndpoints(typeof(Program).Assembly);
builder.Services.AddDynamicGrpcServices(typeof(Greeter.GreeterBase));

var app = builder.Build();

// Map services
app.MapGrpcReflectionService();
app.MapGrpcEndpoints(typeof(Greeter.GreeterBase));

app.Run();