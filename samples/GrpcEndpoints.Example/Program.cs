using GrpcEndpoints;
using GrpcEndpoints.Example;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection(); 

// Register gRPC endpoints and dynamic services
builder.Services.AddGrpcEndpoints();

var app = builder.Build();

// Map services
app.MapGrpcReflectionService();
app.MapGrpcEndpoints<Greeter.GreeterBase>();

app.Run();