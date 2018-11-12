using Framework;

namespace WebAppTests.Shared
{
    public class TestCollectionOrderer : TestCollectionOrdererBase
    {
        private static readonly string[] Collections =
        {
            "user operations",
        };

        public TestCollectionOrderer()
            : base(Collections) { }
    }
}
