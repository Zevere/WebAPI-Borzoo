namespace Borzoo.Data.Mongo
{
    public static class MongoConstants
    {
        public static class Database
        {
            public const string Test = "borzoo_test";
        }

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
        }
    }
}