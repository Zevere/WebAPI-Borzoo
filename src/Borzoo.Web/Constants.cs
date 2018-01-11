﻿namespace Borzoo.Web
{
    public static class Constants
    {
        public static class ZevereRoutes
        {
            public static class PathParameters
            {
                public const string UserId = "{userId}";

                public const string TaskId = "{taskId}";
            }

            public const string Base = "/zv";

            public const string Login = Base + "/login";

            public const string Logout = Base + "/logout";

            public const string Users = Base + "/users";

            public const string User = Users + "/" + PathParameters.UserId;

            public const string UserTasks = Users + "/" + PathParameters.UserId + "/tasks";

            public const string Task = UserTasks + "/" + PathParameters.TaskId;
        }

        public static class ZevereContentTypes
        {
            private const string BaseZevereTypeName = "application/vnd.zv.";

            public const string Empty = BaseZevereTypeName + "empty";

            public static class Login
            {
                public const string Creation = BaseZevereTypeName + "login.creation+json";

                public const string Token = BaseZevereTypeName + "login.token+json";
            }

            public static class User
            {
                public const string Full = BaseZevereTypeName + "user.full+json";

                public const string Pretty = BaseZevereTypeName + "user.pretty+json";

                public const string Creation = BaseZevereTypeName + "user.creation+json";
            }
        }
    }
}