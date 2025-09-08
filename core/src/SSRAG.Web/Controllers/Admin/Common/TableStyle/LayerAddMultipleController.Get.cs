using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils;
using SSRAG.Enums;

namespace SSRAG.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerAddMultipleController
    {
        [HttpGet, Route(Route)]
        public ActionResult<GetResult> Get()
        {
            var styles = new List<Style>
            {
                new Style {
                    InputType = InputType.Text
                }
            };

            return new GetResult
            {
                InputTypes = InputTypeUtils.GetInputTypes(),
                Styles = styles
            };
        }
    }
}
