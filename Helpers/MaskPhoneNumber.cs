using BoletinServiceWorker.Data;
using BoletinServiceWorker.Entities;
using Microsoft.Extensions.Options;

namespace BoletinServiceWorker.Helpers;

public static class MaskPhoneNumber
{
    public static string Mask(string phoneNumber)
    {
        // Ensure the phone number is at least 4 characters long
        if (phoneNumber.Length < 4)
            return new string('*', phoneNumber.Length);

        // Mask the first part of the phone number, leaving the last 4 digits
        string masked = new string('*', phoneNumber.Length - 4) + phoneNumber.Substring(phoneNumber.Length - 4);
        return masked;
    }
}
