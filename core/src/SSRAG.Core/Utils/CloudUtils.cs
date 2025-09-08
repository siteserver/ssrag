using System.Linq;
using System.Threading.Tasks;
using SSRAG.Configuration;
using SSRAG.Services;
using SSRAG.Utils;

namespace SSRAG.Core.Utils
{
    public static class CloudUtils
    {
        public const string CloudApiHost = "https://api.ssrag.com";

        public static class Www
        {
            public static string GetPluginUrl(string userName, string name)
            {
                return PageUtils.Combine(Constants.OfficialHost, $"/plugins/plugin.html?userName={userName}&name={name}");
            }

            public static string GetThemeUrl(string userName, string name)
            {
                return PageUtils.Combine(Constants.OfficialHost, $"/templates/template.html?userName={userName}&name={name}");
            }
        }

        public static class Dl
        {
            private const string Host = "https://dl.ssrag.com";

            public static string GetThemesDownloadUrl(string userName, string name)
            {
                return $"{Host}/themes/{userName}/T_{name}.zip";
            }

            private static string GetCmsDownloadName(string osArchitecture, string version)
            {
                return $"ssrag-{version}-{osArchitecture}";
            }

            private static string GetCmsDownloadUrl(string osArchitecture, string version)
            {
                return $"{Host}/cms/{version}/{GetCmsDownloadName(osArchitecture, version)}.zip";
            }

            public static string GetExtensionsDownloadUrl(string userName, string name, string version)
            {
                return $"{Host}/extensions/{userName}/{name}/{userName}.{name}.{version}.zip";
            }

            public static async Task<string> DownloadCmsAsync(IPathManager pathManager, string osArchitecture, string version)
            {
                var packagesPath = pathManager.GetPackagesPath();
                var name = GetCmsDownloadName(osArchitecture, version);
                var directoryPath = PathUtils.Combine(packagesPath, name);

                var directoryNames = DirectoryUtils.GetDirectoryNames(packagesPath);
                foreach (var directoryName in directoryNames.Where(directoryName => StringUtils.StartsWithIgnoreCase(directoryName, "ssrag-")))
                {
                    DirectoryUtils.DeleteDirectoryIfExists(PathUtils.Combine(packagesPath, directoryName));
                }

                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var filePath = PathUtils.Combine(packagesPath, $"{GetCmsDownloadName(osArchitecture, version)}.zip");

                var url = GetCmsDownloadUrl(osArchitecture, version);
                await RestUtils.DownloadAsync(url, filePath);
                //FileUtils.WriteText(filePath, string.Empty);
                //using (var writer = File.OpenWrite(filePath))
                //{
                //    var client = new RestClient(GetCmsDownloadUrl(osArchitecture, version));
                //    var request = new RestRequest
                //    {
                //        ResponseWriter = responseStream =>
                //        {
                //            using (responseStream)
                //            {
                //                responseStream.CopyTo(writer);
                //            }
                //        }
                //    };

                //    client.DownloadData(request);
                //}

                pathManager.ExtractZip(filePath, directoryPath);

                FileUtils.DeleteFileIfExists(filePath);

                return directoryPath;
            }
        }
    }
}
