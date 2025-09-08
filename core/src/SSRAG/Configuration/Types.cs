namespace SSRAG.Configuration
{
    public static class Types
    {
        public static class Claims
        {
            public const string UserName = "user_name";
            public const string AdminName = "admin_name";
            public const string Role = "role";
            public const string IsPersistent = "is_persistent";
        }

        public static class Roles
        {
            public const string Administrator = nameof(Administrator);
            public const string User = nameof(User);
        }

        public static class PermissionTypes
        {
            public const string App = "app";
            public const string Channel = "channel";
            public const string Content = "content";
        }

        public static class MenuTypes
        {
            public const string App = "app";
            public const string Channels = "channels";
            public const string Channel = "channel";
            public const string Contents = "contents";
            public const string Content = "content";
            public const string Editor = "editor";
        }

        public static class TableTypes
        {
            public const string Content = "content";
            public const string Custom = "custom";
        }
    }
}
