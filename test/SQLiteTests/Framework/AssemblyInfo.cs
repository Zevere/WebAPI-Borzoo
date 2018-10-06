using Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(Constants.TestCaseOrderer, Constants.AssemblyName)]