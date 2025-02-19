using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SicemContext context;
    private readonly int delay;

    public Worker(ILogger<Worker> logger, SicemContext context, IConfiguration configuration)
    {
        this._logger = logger;
        this.context = context;
        this.delay = configuration.GetValue<int>("DelayAfterBoletin", 18000);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var envios = 0;
            var boletines = this.GetBoletinesPendientes();
            foreach ( var boletin in boletines)
            {
                var mensajes = GetMensajes(boletin.Id);
                var destinatarios = GetDestinatariosPendientes(boletin.Id);

                foreach (Destinatario dest in destinatarios)
                {
                    await EnviarMensage(dest, mensajes);
                    envios += mensajes.Count();
                }
            }

            await Task.Delay(this.delay, stoppingToken);
        }
    }

    private IEnumerable<OprBoletin> GetBoletinesPendientes()
    {
        return this.context.OprBoletins.Where(item => item.FinishedAt == null).ToList();
    }

    private IEnumerable<Destinatario> GetDestinatariosPendientes(Guid boletinId)
    {
        return this.context.Destinatarios.Where(item => item.BoletinId == boletinId && item.FechaEnvio == null )
         .Take(100)
         .ToList();
    }

    private IEnumerable<BoletinMensaje> GetMensajes(Guid boletinId)
    {
        return this.context.BoletinMensajes.Where(item => item.BoletinId == boletinId)
         .Take(100)
         .ToList();
    }

    private async Task<object> EnviarMensage(Destinatario destinatario, IEnumerable<BoletinMensaje> boletinMensajes)
    {
        var response = new List<object>();

        foreach(var mes in boletinMensajes)
        {
            // TODO: Send eth message an retrive the response
            if(mes.EsArchivo == true)
            {
                // * send file
            }
            else
            {
                // * send message
            }
        }

        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}
