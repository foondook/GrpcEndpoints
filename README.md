# GrpcEndpoints

[![NuGet](https://img.shields.io/nuget/v/GrpcEndpoints.svg)](https://www.nuget.org/packages/GrpcEndpoints)
[![NuGet](https://img.shields.io/nuget/dt/GrpcEndpoints.svg)](https://www.nuget.org/packages/GrpcEndpoints)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://www.nuget.org/packages/GrpcEndpoints)

GrpcEndpoints is a lightweight framework that simplifies gRPC service implementation in .NET by using an endpoint-centric approach. It eliminates boilerplate code through attribute-based mapping and dynamic service generation, allowing developers to focus on implementing business logic rather than repetitive service scaffolding.

## Features

- ✅ Eliminate boilerplate service implementation classes
- ✅ Attribute-based mapping of endpoints to gRPC methods
- ✅ Automatic registration of endpoints with dependency injection
- ✅ Dynamic creation of gRPC service implementations
- ✅ Cleaner separation of concerns with endpoint-focused architecture

## Installation

```bash
dotnet add package GrpcEndpoints
```

## Quick Start

### 1. Define a gRPC endpoint

```csharp
[GrpcEndpoint(typeof(Greeter.GreeterBase), nameof(Greeter.GreeterBase.SayHello))]
public class SayHello : IGrpcEndpoint<HelloRequest, HelloReply>
{
    public Task<HelloReply> ExecuteAsync(HelloRequest request, ServerCallContext context, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HelloReply
        {
            Message = $"Hello {request.Name} from GrpcEndpoints!"
        });
    }
}
```

### 2. Register endpoints and services

```csharp
// Program.cs
builder.Services.AddGrpcEndpoints(typeof(Program).Assembly);
builder.Services.AddDynamicGrpcServices(typeof(Greeter.GreeterBase));
```

### 3. Map dynamic services

```csharp
// Program.cs
app.MapGrpcEndpoints(typeof(Greeter.GreeterBase));
```

## License

MIT