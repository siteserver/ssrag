using System;
using System.Collections.Generic;
using SSRAG.Datory;
using SSRAG.Datory.Annotations;
using SSRAG.Configuration;
using SSRAG.Enums;

namespace SSRAG.Models
{
    [DataTable("ssrag_config")]
    public class Config : Entity
    {
        [DataColumn]
        public string DatabaseVersion { get; set; }

        [DataColumn]
        public DateTime UpdateDate { get; set; }

        [DataColumn(Text = true)]
        public string Configs { get; set; }

        public bool Initialized => Id > 0;
        public bool IsLogSite { get; set; } = true;
        public bool IsLogSiteCreate { get; set; } = false;
        public bool IsLogAdmin { get; set; } = true;
        public bool IsLogUser { get; set; } = true;
        public bool IsLogError { get; set; } = true;
        public bool IsTimeThreshold { get; set; }
        public int TimeThreshold { get; set; } = 60;
        public int AdminUserNameMinLength { get; set; }
        public int AdminPasswordMinLength { get; set; } = 6;
        public PasswordRestriction AdminPasswordRestriction { get; set; } = PasswordRestriction.LetterAndDigit;
        public bool IsAdminLockLogin { get; set; }
        public int AdminLockLoginCount { get; set; } = 3;
        public LockType AdminLockLoginType { get; set; } = LockType.Hours;
        public int AdminLockLoginHours { get; set; } = 3;
        public bool IsAdminEnforcePasswordChange { get; set; }
        public int AdminEnforcePasswordChangeDays { get; set; } = 90;
        public bool IsAdminEnforceLogout { get; set; }
        public int AdminEnforceLogoutMinutes { get; set; } = 960;
        public bool IsAdminCaptchaDisabled { get; set; }
        public bool IsUserRegistrationAllowed { get; set; } = true;
        public List<string> UserRegistrationAttributes { get; set; }
        public bool IsUserRegistrationMobile { get; set; }
        public bool IsUserRegistrationEmail { get; set; }
        public bool IsUserRegistrationGroup { get; set; }
        public bool IsUserRegistrationDisplayName { get; set; }
        public bool IsUserRegistrationChecked { get; set; } = true;
        public bool IsUserUnRegistrationAllowed { get; set; } = true;
        public bool IsUserForceVerifyMobile { get; set; }
        public int UserPasswordMinLength { get; set; } = 6;
        public PasswordRestriction UserPasswordRestriction { get; set; } = PasswordRestriction.LetterAndDigit;
        public int UserRegistrationMinMinutes { get; set; } = 3;
        public bool IsUserLockLogin { get; set; }
        public int UserLockLoginCount { get; set; } = 3;
        public string UserLockLoginType { get; set; } = "Hours";
        public int UserLockLoginHours { get; set; } = 3;
        public string UserDefaultGroupAdminName { get; set; }
        public bool IsUserCaptchaDisabled { get; set; }
        public bool IsCloudAdmin { get; set; }
        public string AdminTitle { get; set; } = Constants.AdminTitle;
        public string AdminFaviconUrl { get; set; }
        public string AdminLogoUrl { get; set; }
        public string AdminLogoLinkUrl { get; set; } = Constants.OfficialHost;
        public string AdminWelcomeHtml { get; set; } = @"欢迎使用 SSRAG 管理后台";
        public bool IsAdminUpdateDisabled { get; set; }
        public bool IsAdminTicketsDisabled { get; set; }
        public bool IsAdminWatermark { get; set; }
        public bool IsHomeClosed { get; set; }
        public string HomeTitle { get; set; } = "用户中心";
        public bool IsHomeLogo { get; set; }
        public string HomeLogoUrl { get; set; }
        public string HomeDefaultAvatarUrl { get; set; }
        public bool IsHomeAgreement { get; set; }
        public string HomeAgreementHtml { get; set; } = @"阅读并接受<a href=""#"">《用户协议》</a>";
        public string HomeWelcomeHtml { get; set; } = @"欢迎使用用户中心";
    }
}
