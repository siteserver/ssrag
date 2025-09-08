﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSRAG.Models;

namespace SSRAG.Services
{
    public partial interface IPathManager
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        string GetContentRootPath(params string[] paths);

        string GetAdminUrl(params string[] paths);

        string GetHomeUrl(params string[] paths);

        string GetApiHostUrl(Site site, params string[] paths);

        string GetUploadFileName(string fileName);

        string GetWebUrl(Site site);

        string GetAssetsUrl(Site site);

        string ParsePath(string virtualPath);

        Task UploadAsync(IFormFile file, string filePath);

        Task UploadAsync(byte[] bytes, string filePath);

        Task<(bool success, string filePath, string errorMessage)> UploadImageAsync(Site site, IFormFile file);

        Task<(bool success, string filePath, string errorMessage)> UploadFileAsync(Site site, IFormFile file);
    }
}
