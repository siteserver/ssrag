﻿using SSRAG.Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSRAG.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SmsCodeType
    {
        [DataEnum(DisplayName = "自定义")] None,

        [DataEnum(DisplayName = "身份验证")] Authentication,

        [DataEnum(DisplayName = "登录确认")] LoginConfirmation,

        [DataEnum(DisplayName = "登录异常")] AbnormalLogin,

        [DataEnum(DisplayName = "用户注册")] Registration,

        [DataEnum(DisplayName = "修改密码")] ChangePassword,

        [DataEnum(DisplayName = "信息变更")] InformationChanges,

        [DataEnum(DisplayName = "任务执行成功")] TaskSuccess,

        [DataEnum(DisplayName = "任务执行失败")] TaskFailure,

        [DataEnum(DisplayName = "工单回复通知")] TicketReplied,
    }
}
