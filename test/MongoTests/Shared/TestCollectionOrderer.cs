using Framework;

namespace MongoTests.Shared
{
    public class TestCollectionOrderer : TestCollectionOrdererBase
    {
        private static readonly string[] Collections =
        {
            "user repository",
        };

        public TestCollectionOrderer()
            : base(Collections) { }
    }
}
