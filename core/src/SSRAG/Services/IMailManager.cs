using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;

namespace SSRAG.Services
{
    public interface IMailManager
    {
        Task<bool> IsMailEnabledAsync();

        Task<MailSettings> GetMailSettingsAsync();

        Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody);

        Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string url, List<KeyValuePair<string, string>> items);
    }
}
