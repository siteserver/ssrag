using System;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSRAG.Configuration;
using SSRAG.Models;
using SSRAG.Repositories;

namespace SSRAG.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class ErrorController : ControllerBase
    {
        public const string Route = "error";

        private readonly IErrorLogRepository _errorLogRepository;

        public ErrorController(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public class GetResult
        {
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string Summary { get; set; }
            public DateTime? CreatedDate { get; set; }
        }
    }
}
