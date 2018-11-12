using Framework;
using WebAppTests.Shared;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

[assembly: TestCollectionOrderer(
    "WebAppTests.Shared." + nameof(TestCollectionOrderer),
    "WebAppTests"
)]

[assembly: TestCaseOrderer(TestConstants.TestCaseOrderer, TestConstants.AssemblyName)]
