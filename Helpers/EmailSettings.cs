using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Helpers;

public class EmailSettings
{
    public string Host {get;set;} = default!;
    public int Port {get;set;}
    public string UserName {get;set;} = default!;
    public string Password {get;set;} = default!;
    public string From {get;set;} = default!;
}
