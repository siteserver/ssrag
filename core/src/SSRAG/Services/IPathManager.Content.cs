using System.Collections.Specialized;
using System.Threading.Tasks;
using SSRAG.Models;
using SSRAG.Configuration;
using SSRAG.Utils;
using System.Collections.Generic;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        Task<Content> EncodeContentAsync(Site site, Channel channel, Content content, string excludeUrlPrefix = null);

        Task<string> EncodeTextEditorAsync(Site site, string content, string excludePrefix = null);

        Task<Content> DecodeContentAsync(Site site, Channel channel, int contentId);

        Task<Content> DecodeContentAsync(Site site, Channel channel, Content content);

        string DecodeTextEditor(Site site, string content, bool isLocal);

        void PutFilePaths(Site site, Content content, NameValueCollection collection, List<TableStyle> tableStyles);
    }
}
