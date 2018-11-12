using Framework;

namespace WebAppTests.Shared
{
    public class TestCollectionOrderer : TestCollectionOrdererBase
    {
        private static readonly string[] Collections =
        {
            "user operations",
            "task list operations",
        };

        public TestCollectionOrderer()
            : base(Collections) { }
    }
}
