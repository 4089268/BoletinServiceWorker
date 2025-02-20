using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using BoletinServiceWorker.Helpers;
using BoletinServiceWorker.Models;

namespace BoletinServiceWorker.Services;

public class WhatsAppService
{
    private readonly HttpClient client;
    private readonly WhatsAppSettings settings;
    private ILogger<WhatsAppService> logger;

    public WhatsAppService(IHttpClientFactory httpClientFactory, IOptions<WhatsAppSettings> settings, ILogger<WhatsAppService> logger)
    {
        this.client = httpClientFactory.CreateClient("WhatsappService");
        this.settings = settings.Value;
        this.logger = logger;
    }

    public async Task<MessageResult> SendMessage(string phoneNumber, string message, string lada = "521")
    {
        logger.LogDebug($"Attempt to send the message to the phone {MaskPhoneNumber.Mask(phoneNumber)}");

        // * prepara request
        var _requestPayloadJson = JsonSerializer.Serialize( new{
            tipo = "texto",
            telefono = string.Format("{0}{1}{2}", lada, phoneNumber, settings.Suffix),
            mensaje = message
        });

        var requestHttp = new HttpRequestMessage(){
            Method = HttpMethod.Post,
            RequestUri = new Uri(client.BaseAddress!, "/enviar/mensaje"),
            Content = new StringContent(_requestPayloadJson, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json ),
        };
        requestHttp.Headers.Add("x-token", settings.Token);

        try
        {
            // * send the request
            var response = await client.SendAsync(requestHttp);
            
            // * prosesar respuesta
            response.EnsureSuccessStatusCode();

            this.logger.LogInformation($"Message successfully sent to the phone {MaskPhoneNumber.Mask(phoneNumber)}");
            
            var responseText = await response.Content.ReadAsStringAsync();
            return MessageResult.Success(responseText);
        }
        catch (System.Exception ex)
        {
            this.logger.LogError(ex, $"Fail at send the message to {phoneNumber}");
            return MessageResult.Failure(ex.Message);
        }
    }

    public async Task<MessageResult> SendFile(string phoneNumber, string content, string mimetype, string name, string lada = "521")
    {
        this.logger.LogDebug($"Attempt to send the message to the phone {MaskPhoneNumber.Mask(phoneNumber)}");

        // * prepara request
        var _requestContetnJson = JsonSerializer.Serialize(new {
            tipo = "archivo",
            telefono = string.Format("{0}{1}{2}", lada, phoneNumber, settings.Suffix),
            nombre = name,
            mimetype = mimetype,
            mensaje = content
        });
        var requestHttp = new HttpRequestMessage(){
            Method = HttpMethod.Post,
            RequestUri = new Uri( client.BaseAddress!, "/enviar/mensaje"),
            Content = new StringContent(_requestContetnJson, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json),
        };
        requestHttp.Headers.Add("x-token", settings.Token);

        try
        {
            // * send the request
            var response = await client.SendAsync(requestHttp);
            
            // * prosesar respuesta
            response.EnsureSuccessStatusCode();

            this.logger.LogInformation($"Message successfully sent to the phone {MaskPhoneNumber.Mask(phoneNumber)}");
            var responseText = await response.Content.ReadAsStringAsync();
            return MessageResult.Success(responseText);
        }
        catch(Exception ex)
        {
            this.logger.LogError($"Fail at sending the message to the phone {MaskPhoneNumber.Mask(phoneNumber)}: {ex.Message}", ex);
            return MessageResult.Failure(ex.Message);
        }
    }

}
