using Framework;

namespace MongoTests.Shared
{
    public class TestCollectionOrderer : TestCollectionOrdererBase
    {
        private static readonly string[] Collections =
        {
            "user repository",
            "task list repository",
        };

        public TestCollectionOrderer()
            : base(Collections) { }
    }
}
