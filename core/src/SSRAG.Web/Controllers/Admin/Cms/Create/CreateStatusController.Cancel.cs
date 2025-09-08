using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;

namespace SSRAG.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateStatusController
    {
        [HttpPost, Route(RouteActionsCancel)]
        public ActionResult<BoolResult> Cancel([FromBody] SiteRequest request)
        {
            _createManager.ClearAllTask(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
