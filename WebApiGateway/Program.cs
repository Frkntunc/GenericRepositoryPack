using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Reverse proxy config
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();
app.Run();
