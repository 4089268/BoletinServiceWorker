using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Models;

public class MessageResult
{
    public string MessageId { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    // Static methods for success and failure
    public static MessageResult Success(string message) => new MessageResult { IsSuccess = true, Message = message };
    public static MessageResult Failure(string message) => new MessageResult { IsSuccess = false, Message = message };
}
