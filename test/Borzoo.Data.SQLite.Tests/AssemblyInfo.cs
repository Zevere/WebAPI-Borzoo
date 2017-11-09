using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer("Borzoo.Data.SQLite.Tests.TestCaseMethodNameOrderer", "Borzoo.Data.SQLite.Tests")]