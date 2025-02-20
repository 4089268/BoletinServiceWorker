using System.Text.Json;
using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using BoletinServiceWorker.Helpers;
using BoletinServiceWorker.Models;
using BoletinServiceWorker.Services;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<Worker> _logger;
    private readonly WorkerSettings workerSettings;

    public Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger, IConfiguration configuration, IOptions<WorkerSettings> options)
    {
        this._logger = logger;
        this._serviceScopeFactory = serviceScopeFactory;
        this.workerSettings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            using( var scope = this._serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SicemContext>();
                var whatsAppService = scope.ServiceProvider.GetRequiredService<WhatsAppService>();

                await DoWork(stoppingToken, context, whatsAppService);
            }

            await Task.Delay(workerSettings.Delay, stoppingToken);
        }
    }

    private async Task DoWork(CancellationToken stoppingToken, SicemContext context, WhatsAppService whatsAppService)
    {
        var envios = 0;
        var boletines = this.GetBoletinesPendientes(context);
        
        foreach (var boletin in boletines)
        {
            var mensajes = GetMensajes(context, boletin.Id);
            var destinatarios = GetDestinatariosPendientes(context, boletin.Id);

            // * check if has messages and destinatarios
            if(!destinatarios.Any() || !mensajes.Any())
            {
                await FinishBoletin(context, boletin.Id);
                continue;
            }

            foreach(Destinatario dest in destinatarios)
            {
                // * check if a cancelation was requested
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested. Exiting boletines loop.");
                    return; // Stop execution
                }

                // * validate the maximum of messages to send
                if(envios >= workerSettings.MaxEnvios)
                {
                    return;
                }

                // * send messages
                var results = await EnviarMensage(whatsAppService, dest, mensajes);
                await UdpateDesti(context, dest, results);

                envios += mensajes.Count();
            }

            // * check if a all messages was sent
            var allSent = !context.Destinatarios.Where(item => item.BoletinId == boletin.Id && item.FechaEnvio == null).Any();
            if(allSent)
            {
                await FinishBoletin(context, boletin.Id);
            }

            // * validate the maximum of messages to send
            if(envios >= workerSettings.MaxEnvios)
            {
                return;
            }
        }

    }

    private IEnumerable<OprBoletin> GetBoletinesPendientes(SicemContext context)
    {
        return context.OprBoletins.Where(item => item.FinishedAt == null).ToList();
    }

    private IEnumerable<Destinatario> GetDestinatariosPendientes(SicemContext context, Guid boletinId)
    {
        return context.Destinatarios.Where(item => item.BoletinId == boletinId && item.FechaEnvio == null )
         .Take(100)
         .ToList();
    }

    private IEnumerable<BoletinMensaje> GetMensajes(SicemContext context, Guid boletinId)
    {
        return context.BoletinMensajes.Where(item => item.BoletinId == boletinId)
         .Take(100)
         .ToList();
    }

    private async Task FinishBoletin(SicemContext context, Guid boletinId)
    {
        var boletin = context.OprBoletins.FirstOrDefault(item => item.Id == boletinId);
        if(boletin == null)
        {
            return;
        }

        boletin.FinishedAt = DateTime.Now;
        await context.SaveChangesAsync();
    }

    private async Task UdpateDesti(SicemContext context, Destinatario dest, IEnumerable<MessageResult> results)
    {
        var allMessagesOk = results.All( item => item.IsSuccess);
        if (allMessagesOk)
        {
            dest.Resultado = "Enviado";
        }
        else
        {
            dest.Resultado = "Error";
        }
        dest.Error = !allMessagesOk;
        dest.FechaEnvio = DateTime.Now;
        dest.EnvioMetadata = JsonSerializer.Serialize(new {results });

        context.Destinatarios.Update(dest);
        await context.SaveChangesAsync();
    }

    private async Task<IEnumerable<MessageResult>> EnviarMensage(WhatsAppService whatsAppService, Destinatario destinatario, IEnumerable<BoletinMensaje> boletinMensajes)
    {
        var response = new List<MessageResult>();

        foreach(var mes in boletinMensajes)
        {
            MessageResult result1;
            // * Send eth message an retrive the response
            if(mes.EsArchivo == true)
            {
                // * send file
                result1 = await whatsAppService.SendFile(
                    destinatario.Telefono.ToString(),
                    mes.Mensaje,
                    mes.MimmeType!,
                    mes.FileName ?? "archivo",
                    $"{destinatario.Lada}1"
                );
            }
            else
            {
                // * send message
                result1 = await whatsAppService.SendMessage(
                    destinatario.Telefono.ToString(),
                    mes.Mensaje,
                    $"{destinatario.Lada}1"
                );
            }

            result1.MessageId = mes.Id.ToString();
            response.Add(result1);
            await Task.Delay(1000);
        }

        return response;
    }
}
