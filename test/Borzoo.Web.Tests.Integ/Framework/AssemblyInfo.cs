using Borzoo.Web.Tests.Integ.Framework;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(TestConstants.TestCaseOrderer, TestConstants.AssemblyName)]