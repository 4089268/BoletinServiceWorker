using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Helpers;

public class WhatsAppSettings
{
    public string Endpoint {get;set;} = default!;
    public string Token {get;set;} = default!;
    public string Suffix {get;set;} = default!;
}
