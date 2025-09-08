using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Models;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Services
{
    public partial class CloudManager
    {
        public async Task<bool> IsMailEnabledAsync()
        {
            return await Task.FromResult(false);
        }

        public async Task<MailSettings> GetMailSettingsAsync()
        {
            return await Task.FromResult(new MailSettings());
        }

        public class SendMailRequest
        {
            public string FromAlias { get; set; }
            public string Mail { get; set; }
            public string Subject { get; set; }
            public string HtmlBody { get; set; }
        }

        public async Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody)
        {
            return await Task.FromResult((false, "未启用邮件功能"));
        }

        public async Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string url, List<KeyValuePair<string, string>> items)
        {
            var htmlBody = await _pathManager.GetMailTemplateHtmlAsync();

            return await SendMailAsync(mail, subject, htmlBody);
        }

        public static async Task SendContentChangedMail(IPathManager pathManager, IMailManager mailManager, IErrorLogRepository errorLogRepository, Site site, Content content, string channelNames, string userName, bool isEdit)
        {
            try
            {
                var mailSettings = await mailManager.GetMailSettingsAsync();
                if (mailSettings == null || !mailSettings.IsMail)
                {
                    return;
                }

                if (isEdit)
                {
                    if (!mailSettings.IsMailContentEdit)
                    {
                        return;
                    }
                }
                else
                {
                    if (!mailSettings.IsMailContentAdd)
                    {
                        return;
                    }
                }

                var action = isEdit ? "修改" : "添加";
                var subject = $"{action}内容";
                var url = await pathManager.GetContentUrlByIdAsync(site, content, false);
                var items = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("内容标题", content.Title),
                    new KeyValuePair<string, string>("栏目", channelNames),
                    new KeyValuePair<string, string>("内容Id", content.Id.ToString()),
                    new KeyValuePair<string, string>($"{action}时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new KeyValuePair<string, string>("执行人", userName)
                };

                await mailManager.SendMailAsync(mailSettings.MailAddress, subject, url, items);
            }
            catch (Exception ex)
            {
                await errorLogRepository.AddErrorLogAsync(ex);
            }
        }
    }
}
