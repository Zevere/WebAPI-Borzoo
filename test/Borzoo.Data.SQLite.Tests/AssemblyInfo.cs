using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer("Borzoo.Data.SQLite.Tests.TestCaseMethodNameOrderer", "Borzoo.Data.SQLite.Tests")]