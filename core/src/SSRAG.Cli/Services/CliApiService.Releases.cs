﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SSRAG.Utils;

namespace SSRAG.Cli.Services
{
    public partial class CliApiService
    {
        public async Task<(bool success, GetReleasesResult result, string failureMessage)> GetReleasesAsync(string version, List<string> pluginIds)
        {
            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlReleases)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new GetReleasesRequest
            //{
            //    IsNightly = isNightly,
            //    Version = version,
            //    PluginIds = pluginIds
            //}), ParameterType.RequestBody);
            //var response = client.Execute<GetReleasesResult>(request);
            //if (!response.IsSuccessful)
            //{
            //    if (response.StatusCode == HttpStatusCode.InternalServerError)
            //    {
            //        Log.Fatal(response.Content);
            //        var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
            //        if (error != null)
            //        {
            //            return (false, null, error.Message);
            //        }
            //    }
            //    return (false, null, response.ErrorMessage);
            //}

            //return (true, response.Data, null);

            var url = GetCliUrl(RestUrlReleases);
            return await RestUtils.PostAsync<GetReleasesRequest, GetReleasesResult>(url, new GetReleasesRequest
            {
                Version = version,
                PluginIds = pluginIds
            });
        }
    }
}
