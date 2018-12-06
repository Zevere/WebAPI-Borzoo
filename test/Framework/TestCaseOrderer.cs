using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Framework
{
    public class TestCaseOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase =>
            testCases
                .Select(tc => new
                {
                    TestCase = tc,
                    LineNumber = GetTestCaseLineNumber(tc)
                })
                .OrderBy(x => x.LineNumber)
                .Select(x => x.TestCase);

        private int GetTestCaseLineNumber(ITestCase testCase)
        {
            int line;

            var methodInfo = testCase.TestMethod.Method.ToRuntimeMethod();
            var factAttribute = methodInfo.GetCustomAttribute<OrderedFactAttribute>();

            if (factAttribute != null)
            {
                line = factAttribute.LineNumber;
            }
            else
            {
                var theoryAttribute = methodInfo.GetCustomAttribute<OrderedTheoryAttribute>();
                if (theoryAttribute != null)
                {
                    line = theoryAttribute.LineNumber;
                }
                else
                {
                    throw new Exception(
                        $"Test case \"{testCase.DisplayName}\" must have either" +
                        $"{nameof(OrderedFactAttribute)} or {nameof(OrderedTheoryAttribute)}."
                    );
                }
            }

            return line;
        }
    }
}
