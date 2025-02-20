using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using BoletinServiceWorker.Helpers;
using BoletinServiceWorker.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BoletinServiceWorker.Services;

public class TwillioService
{
    private readonly TwillioSettings settings;
    private ILogger<TwillioService> logger;

    public TwillioService(IOptions<TwillioSettings> settings, ILogger<TwillioService> logger)
    {
        this.settings = settings.Value;
        this.logger = logger;
    }

    public async Task<MessageResult> SendMessage(string phoneNumber, string message, string lada = "52")
    {
        try
        {
            TwilioClient.Init(settings.SID, settings.Token);
            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(settings.PhoneNumber),
                to: new Twilio.Types.PhoneNumber($"+{lada}{phoneNumber}")
            );
            return MessageResult.Success(messageResource.Body);
        }
        catch (System.Exception ex)
        {
            return MessageResult.Failure(ex.Message);
        }
    }

}
