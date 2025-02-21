using BoletinServiceWorker;
using BoletinServiceWorker.Data;
using BoletinServiceWorker.Helpers;
using BoletinServiceWorker.Services;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// * configure serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddLogging(logginBuilder =>{
    logginBuilder.AddSerilog();
});

builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("WorkerSettings"));
builder.Services.Configure<TwillioSettings>(builder.Configuration.GetSection("TwillioSettings"));
builder.Services.AddHttpClient("WhatsappService", client => {
    client.BaseAddress = new Uri(builder.Configuration["WhatsAppSettings:Endpoint"]!);
    client.DefaultRequestHeaders.Add("x-token", builder.Configuration["WhatsAppSettings:Token"]);
});
builder.Services.AddDbContext<SicemContext>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddScoped<TwillioService>();
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();