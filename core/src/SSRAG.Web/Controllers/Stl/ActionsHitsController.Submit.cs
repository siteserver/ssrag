using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Configuration;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Stl
{
    public partial class ActionsHitsController
    {
        [HttpPost, Route(Constants.RouteStlActionsHits)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            
            try
            {
                var hits = await _contentRepository.GetHitsAsync(request.SiteId, request.ChannelId, request.ContentId);
                if (request.AutoIncrease)
                {
                    hits++;
                    await _contentRepository.UpdateHitsAsync(request.SiteId, request.ChannelId, request.ContentId, hits);
                }
                
                return new IntResult
                {
                    Value = hits
                };
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }
        }
    }
}
