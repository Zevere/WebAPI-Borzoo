using Framework;
using Xunit;

[assembly: TestCaseOrderer(Constants.TestCaseOrderer, Constants.AssemblyName)]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
