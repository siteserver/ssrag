using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Services
{
    public interface ISmsManager
    {
        Task<bool> IsSmsEnabledAsync();

        Task<SmsSettings> GetSmsSettingsAsync();

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode,
            Dictionary<string, string> parameters = null);

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            Dictionary<string, string> parameters = null);

        Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            int code);
    }
}
