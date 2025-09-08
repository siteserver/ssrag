using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin
{
    public partial class LoadingController
    {
        [HttpPost, Route(Route)]
        public ActionResult<StringResult> Submit([FromBody] SubmitRequest request)
        {
            return new StringResult
            {
                Value = _settingsManager.Decrypt(request.RedirectUrl)
            };
        }
    }
}
