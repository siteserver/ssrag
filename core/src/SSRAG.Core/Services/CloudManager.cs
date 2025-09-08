using System;
using System.Threading.Tasks;
using SSRAG.Core.Utils;
using SSRAG.Repositories;
using SSRAG.Services;
using SSRAG.Utils;
using SSRAG.Models;
using SSRAG.Enums;
using SSRAG.Dto;

namespace SSRAG.Core.Services
{
    public partial class CloudManager : ICloudManager
    {

        private const string RouteBackup = "backup";
        private const string RouteGetDownloadUrl = "clouds/actions/getDownloadUrl";
        private const string RouteGetOssCredentials = "clouds/actions/getOssCredentials";
        private const string RouteCensor = "censor";
        private const string RouteCensorAddWhiteList = "censor/actions/addWhiteList";
        private const string RouteSpell = "spell";
        private const string RouteSpellAddWhiteList = "spell/actions/addWhiteList";
        private const string RouteVod = "vod";
        public const string DomainDns = "https://a.ssrag.net";
        public const string DataZipFileName = "ssrag-data.zip";
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IStorageFileRepository _storageFileRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CloudManager(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IConfigRepository configRepository, IStorageFileRepository storageFileRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _configRepository = configRepository;
            _storageFileRepository = storageFileRepository;
            _errorLogRepository = errorLogRepository;
        }


        public class GetDownloadUrlResult
        {
            public string DownloadUrl { get; set; }
        }
    }
}
