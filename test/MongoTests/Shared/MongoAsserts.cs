//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using Newtonsoft.Json.Linq;
//using Xunit;
//
//namespace MongoTests.Shared
//{
//    public static class MongoAsserts
//    {
//        public static void BsonEqual(string expected, BsonDocument actual)
//        {
//            if (!BsonDocument.TryParse(expected, out var expectedBson))
//            {
//                throw new
//            }
//
//            Assert.Equal(expectedBson, actual);
//
//            var expectedJ = JToken.Parse(expected);
//            var actualJ = JToken.Parse(actual);
//            bool equals = JToken.DeepEquals(expectedJ, actualJ);
//
//            if (!equals)
//            {
//                // throws an exception with a consistent message from xUnit
//                Assert.Equal(expectedJ.ToString(), actualJ.ToString());
//            }
//        }
//    }
//}
