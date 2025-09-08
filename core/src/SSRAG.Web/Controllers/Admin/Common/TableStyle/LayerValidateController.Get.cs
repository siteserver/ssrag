using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSRAG.Datory;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Enums;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerValidateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var relatedIdentities = ListUtils.GetIntList(request.RelatedIdentities);
            var style = await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, relatedIdentities);

            var options = ListUtils.GetEnums<ValidateType>().Select(validateType =>
                new Select<string>(validateType.GetValue(), validateType.GetDisplayName()));

            return new GetResult
            {
                Options = options,
                Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues)
            };
        }
    }
}
