namespace Borzoo.Web
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
            public static class Login
            {
                public const string Creation = "application/vnd.zv.login.creation+json";
                
                public const string Token = "application/vnd.zv.login.token+json";
            }

            public static class User
            {
                public const string Full = "application/vnd.zv.user.full+json";

                public const string Pretty = "application/vnd.zv.user.pretty+json";

                public const string Creation = "application/vnd.zv.user.creation+json";
            }
        }
    }
}