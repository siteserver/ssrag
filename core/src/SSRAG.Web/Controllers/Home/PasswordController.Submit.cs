using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRAG.Dto;
using SSRAG.Utils;

namespace SSRAG.Web.Controllers.Home
{
    public partial class PasswordController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var password = request.Password;
            var (isValid, errorMessage) = await _userRepository.ChangePasswordAsync(_authManager.UserName, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            await _authManager.AddUserLogAsync("修改密码");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
