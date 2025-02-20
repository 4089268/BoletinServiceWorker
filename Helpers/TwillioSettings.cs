using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Helpers;

public class TwillioSettings
{
    public string SID {get;set;} = default!;
    public string Token {get;set;} = default!;
    public string PhoneNumber {get;set;} = default!;
}
