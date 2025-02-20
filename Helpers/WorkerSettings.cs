using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Helpers;

public class WorkerSettings
{
    public int Delay {get;set;}
    public int MaxEnvios {get;set;}
}
