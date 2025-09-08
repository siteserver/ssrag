using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Core.Utils.Office;
using SSRAG.Dto;
using SSRAG.Core.Utils;
using System;

namespace SSRAG.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteExport)]
        public async Task<ActionResult<StringResult>> Export([FromBody] ExportRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsers))
            {
                return Unauthorized();
            }

            var fileName = $"用户_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.xlsx";
            var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager);
            await excelObject.CreateExcelFileForUsersAsync(filePath, request.DepartmentId);

            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };

            // const string fileName = "用户.xlsx";
            // var filePath = _pathManager.GetTemporaryFilesPath(fileName);

            // var excelObject = new ExcelObject(_databaseManager, _pathManager);
            // await excelObject.CreateExcelFileForUsersAsync(filePath, null);

            // var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            // return new StringResult
            // {
            //     Value = downloadUrl
            // };
        }
    }
}
