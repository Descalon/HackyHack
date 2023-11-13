using Hackvip.Domain;
using Hackvip.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddGrpc();
services.AddHttpClient<OracleRequestService>(client => {
    var apiBaseAddress = builder.Configuration["OracleApiUrl"] ?? throw new Exception("Oracle API Key not found");
    client.BaseAddress = new Uri(apiBaseAddress);
});


services.AddGrpcClient<Repository.RepositoryClient>(o => {
    var dbAddress = builder.Configuration["DatabaseAccessor"] ?? throw new Exception("DatabaseAccessor Key not found");
    o.Address = new (dbAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<IngressService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
