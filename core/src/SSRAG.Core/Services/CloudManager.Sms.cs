using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Enums;

namespace SSRAG.Core.Services
{
    public partial class CloudManager
    {
        private class SendSmsRequest
        {
            public string PhoneNumbers { get; set; }
            public SmsCodeType CodeType { get; set; }
            public string TemplateCode { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
        }

        public async Task<bool> IsSmsEnabledAsync()
        {
            return await Task.FromResult(false);
        }

        public async Task<SmsSettings> GetSmsSettingsAsync()
        {
            return await Task.FromResult(new SmsSettings());
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters)
        {
            return await SendSmsAsync(phoneNumbers, SmsCodeType.None, templateCode, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType,
            Dictionary<string, string> parameters = null)
        {
            return await SendSmsAsync(phoneNumbers, codeType, string.Empty, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, int code)
        {
            var parameters = new Dictionary<string, string> { { "code", code.ToString() } };
            return await SendSmsAsync(phoneNumbers, codeType, string.Empty, parameters);
        }

        public async Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, string templateCode, Dictionary<string, string> parameters)
        {
            return await Task.FromResult((false, "未启用短信功能"));
        }
    }
}
