using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Core.StlParser.StlElement;

namespace SSRAG.Web.Controllers.Stl
{
    public partial class ActionsDynamicController
    {
        [HttpPost, Route(Constants.RouteStlActionsDynamic)]
        public async Task<SubmitResult> Submit([FromBody] SubmitRequest request)
        {
            var user = await _authManager.GetUserAsync();

            var dynamicInfo = StlDynamic.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);
            var template = dynamicInfo.YesTemplate;

            return new SubmitResult
            {
                Value = true,
                Html = await StlDynamic.ParseDynamicAsync(_parseManager, dynamicInfo, template)
            };
        }
    }
}
