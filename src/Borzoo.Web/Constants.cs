namespace Borzoo
{
    public static class Constants
    {
        public static class ZVeerRoutes
        {
            public static class PathParameters
            {
                public const string UserId = "{userId}";

                public const string TaskId = "{taskId}";
            }

            public const string Base = "/zv";

            public const string Users = Base + "/users";

            public const string User = Users + "/" + PathParameters.UserId;

            public const string UserTasks = Users + "/" + PathParameters.UserId + "/tasks";

            public const string Task = UserTasks + "/" + PathParameters.TaskId;
        }

        public static class ZVeerContentTypes
        {
            public static class User
            {
                public const string Full = "application/vnd.zv.user.full+json";

                public const string Pretty = "application/vnd.zv.user.pretty+json";

                public const string Creation = "application/vnd.zv.user.creation+json";
            }
        }
    }
}