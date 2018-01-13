using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Borzoo.Data.SQLite.Tests.Framework
{
    public class TestCaseOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase =>
            testCases
                .Select(tc => new
                {
                    TestCase = tc,
                    Attrib = tc.TestMethod.Method
                                 .ToRuntimeMethod()
                                 .GetCustomAttribute<OrderedFactAttribute>() ?? throw new
                                 Exception($"All test cases must have {nameof(OrderedFactAttribute)} attribute.")
                })
                .OrderBy(x => x.Attrib.LineNumber)
                .Select(x => x.TestCase);
    }
}