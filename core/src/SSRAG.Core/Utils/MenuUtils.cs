using System.Collections.Generic;

namespace SSRAG.Core.Utils
{
    public static class MenuUtils
    {
        public const string IdSite = "site";
        public const string IdSiteContentsAll = "site_contents_all";
        public const string IdSiteFormAll = "site_form_all";

        public static string GetFormPermission(int formId)
        {
            return $"site_form_{formId}";
        }

        public static class AppPermissions
        {

            public const string AppsManagement = "sys_apps_management";
            public const string AppsChats = "sys_apps_chats";
            public const string PluginsAdd = "sys_plugins_add";
            public const string PluginsManagement = "sys_plugins_management";
            public const string SettingsSitesAdd = "sys_settings_sitesAdd";
            public const string SettingsSites = "sys_settings_sites";
            public const string SettingsSitesUrl = "sys_settings_sitesUrl";
            public const string SettingsSitesTemplates = "sys_settings_sitesTemplates";
            public const string SettingsSitesTemplatesOnline = "sys_settings_sitesTemplatesOnline";
            public const string SettingsSitesTables = "sys_settings_sitesTables";
            public const string SettingsAdministrators = "sys_settings_administrators";
            public const string SettingsAdministratorsRole = "sys_settings_administratorsRole";
            public const string SettingsUsers = "sys_settings_users";
            public const string SettingsUsersDepartment = "sys_settings_usersDepartment";
            public const string SettingsUsersGroup = "sys_settings_usersGroup";
            public const string SettingsUsersStyle = "sys_settings_usersStyle";
            public const string SettingsUsersConfig = "sys_settings_usersConfig";
            public const string SettingsHomeConfig = "sys_settings_homeConfig";
            public const string SettingsHomeMenus = "sys_settings_homeMenus";
            public const string SettingsConfigsModels = "sys_settings_configsModels";
            public const string SettingsConfigsAccessTokens = "sys_settings_configsAccessTokens";
            public const string SettingsConfigsAdministrators = "sys_settings_configsAdministrators";
            public const string SettingsAnalysisAdminLogin = "sys_settings_analysisAdminLogin";
            public const string SettingsAnalysisSiteContent = "sys_settings_analysisSiteContent";
            public const string SettingsAnalysisUser = "sys_settings_analysisUser";
            public const string SettingsLogsSite = "sys_settings_logsSite";
            public const string SettingsLogsAdmin = "sys_settings_logsAdmin";
            public const string SettingsLogsUser = "sys_settings_logsUser";
            public const string SettingsLogsError = "sys_settings_logsError";
            public const string SettingsLogsConfig = "sys_settings_logsConfig";
            public const string SettingsUtilitiesCache = "sys_settings_utilitiesCache";
            public const string SettingsUtilitiesParameters = "sys_settings_utilitiesParameters";
            public const string SettingsUtilitiesEncrypt = "sys_settings_utilitiesEncrypt";
        }

        public static class SitePermissions
        {
            public const string Contents = "site_contents";
            public const string Channels = "site_channels";
            public const string ContentsSearch = "site_contentsSearch";
            public const string ContentsCheck = "site_contentsCheck";
            public const string FormList = "site_formList";
            public const string FormTemplates = "site_formTemplates";
            public const string ChannelsTranslate = "site_channelsTranslate";
            public const string ContentsReplace = "site_contentsReplace";
            public const string ContentsRecycle = "site_contentsRecycle";
            public const string Templates = "site_templates";
            public const string Specials = "site_specials";
            public const string TemplatesMatch = "site_templatesMatch";
            public const string TemplatesIncludes = "site_templatesIncludes";
            public const string TemplatesAssets = "site_templatesAssets";
            public const string TemplatesPreview = "site_templatesPreview";
            public const string TemplatesReference = "site_templatesReference";
            public const string SettingsSite = "site_settingsSite";
            public const string SettingsContent = "site_settingsContent";
            public const string SettingsChannelGroup = "site_settingsChannelGroup";
            public const string SettingsContentGroup = "site_settingsContentGroup";
            public const string SettingsContentTag = "site_settingsContentTag";
            public const string SettingsStyleContent = "site_settingsStyleContent";
            public const string SettingsStyleChannel = "site_settingsStyleChannel";
            public const string SettingsStyleSite = "site_settingsStyleSite";
            public const string SettingsStyleRelatedField = "site_settingsStyleRelatedField";
            public const string SettingsCrossSiteTrans = "site_settingsCrossSiteTrans";
            public const string SettingsCrossSiteTransChannels = "site_settingsCrossSiteTransChannels";
            public const string SettingsCreateRule = "site_settingsCreateRule";
            public const string SettingsCreate = "site_settingsCreate";
            public const string SettingsCreateTrigger = "site_settingsCreateTrigger";
            public const string SettingsUploadImage = "site_settingsUploadImage";
            public const string SettingsUploadVideo = "site_settingsUploadVideo";
            public const string SettingsUploadAudio = "site_settingsUploadAudio";
            public const string SettingsUploadFile = "site_settingsUploadFile";
            public const string SettingsWaterMark = "site_settingsWaterMark";
            public const string CreateIndex = "site_createIndex";
            public const string CreateChannels = "site_createChannels";
            public const string CreateContents = "site_createContents";
            public const string CreateFiles = "site_createFiles";
            public const string CreateSpecials = "site_createSpecials";
            public const string CreateAll = "site_createAll";
            public const string CreateStatus = "site_createStatus";
        }

        public static class ContentPermissions
        {
            public const string View = "content_view";
            public const string Add = "content_add";
            public const string Edit = "content_edit";
            public const string Delete = "content_delete";
            public const string Translate = "content_translate";
            public const string Arrange = "content_arrange";
            public const string CheckLevel1 = "content_checkLevel1";
            public const string CheckLevel2 = "content_checkLevel2";
            public const string CheckLevel3 = "content_checkLevel3";
            public const string CheckLevel4 = "content_checkLevel4";
            public const string CheckLevel5 = "content_checkLevel5";
            public const string Create = "content_create";
        }
    }
}
