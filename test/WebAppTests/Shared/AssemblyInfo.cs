using Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(TestConstants.TestCaseOrderer, TestConstants.AssemblyName)]
