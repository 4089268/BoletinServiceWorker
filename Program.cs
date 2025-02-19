using BoletinServiceWorker;
using BoletinServiceWorker.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<SicemContext>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();