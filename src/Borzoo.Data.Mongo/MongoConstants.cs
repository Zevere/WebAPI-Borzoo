namespace Borzoo.Data.Mongo
{
    public static class MongoConstants
    {
        public static class Collections
        {
            public static class Users
            {
                public const string Name = "users";

                public static class Indexes
                {
                    public const string Username = "username";
                }
            }

            public static class TaskLists
            {
                public const string Name = "task-lists";

                public static class Indexes
                {
                    public const string OwnerListName = "owner_list-name";
                }
            }

            public static class TaskItems
            {
                public const string Name = "task-items";

                public static class Indexes
                {
                    public const string ListTaskName = "list-task_name";
                }
            }
        }
    }
}
