using Framework;
using Xunit;

[assembly: TestCaseOrderer(TestConstants.TestCaseOrderer, TestConstants.AssemblyName)]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
