using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Dto;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IFormManager
    {
        string GetTemplateDirectoryPath(Site site, bool isSystem, string name);

        Task<string> GetTemplateHtmlAsync(Site site, bool isSystem, string name);

        void SetTemplateHtml(Site site, string name, string html);

        void DeleteTemplate(Site site, string name);

        List<FormTemplate> GetFormTemplates(Site site);

        FormTemplate GetFormTemplate(Site site, string name);

        void Clone(Site site, bool isSystemOriginal, string nameOriginal, string name);

        void Clone(Site site, bool isSystemOriginal, string nameOriginal, string name, string templateHtml);
    }
}
