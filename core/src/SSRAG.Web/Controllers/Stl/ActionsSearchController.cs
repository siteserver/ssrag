﻿using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Core.StlParser.Models;
using SSRAG.Repositories;
using SSRAG.Services;

namespace SSRAG.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsSearchController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public ActionsSearchController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        private static NameValueCollection GetPostCollection(StlSearchRequest request)
        {
            var formCollection = new NameValueCollection();
            if (request != null)
            {
                foreach (var key in request.GetKeys())
                {
                    var value = request.Get(key);
                    if (value != null)
                    {
                        formCollection[key] = request.Get(key).ToString();
                    }
                }
            }

            return formCollection;
        }
    }
}
