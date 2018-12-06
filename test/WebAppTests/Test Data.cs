using MongoDB.Bson;
using MongoDB.Driver;

namespace WebAppTests
{
    public static class TestData
    {
        public static void SeedUsers(IMongoDatabase db)
        {
            var usersCollection = db.GetCollection<BsonDocument>("users");

            usersCollection.InsertMany(new[]
            {
                BsonDocument.Parse(@"{
                    name : ""poulad1024"",
                    pass : ""10-pass_phrase-24"",
                    fname : ""Poulad"",
                    joined : new Date(),
                    token : ""n010mLpm010FS1HZFe1S0CKOaj""
                }"),

                BsonDocument.Parse(@"{
                    name : ""franky"",
                    pass : ""passWORD"",
                    fname : ""Frank"",
                    joined : new Date(),
                    token : ""aVc4378nmASDGb5n6JvbAsd87y""
                }"),
            });
        }

        public static void SeedTaskLists(IMongoDatabase db)
        {
            var usersCollection = db.GetCollection<BsonDocument>("task-lists");

            usersCollection.InsertMany(new[]
            {
                BsonDocument.Parse(@"{
                    name : ""a_test_list"",
                    owner : ""poulad1024"",
                    title : ""Test title here"",
                    created : new Date(),
                }"),

                BsonDocument.Parse(@"{
                    name : ""s2nd.list"",
                    owner : ""poulad1024"",
                    title : ""my 2nd list"",
                    created : new Date(),
                }"),

                BsonDocument.Parse(@"{
                    name : ""a_test_list"",
                    owner : ""franky"",
                    title : ""Testing..."",
                    created : new Date(),
                }"),
            });
        }
    }
}
