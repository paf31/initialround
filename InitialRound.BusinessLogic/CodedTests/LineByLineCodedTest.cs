///
///

using InitialRound.BusinessLogic.Classes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.Common.Extensions;
using InitialRound.Common.Exceptions;

namespace InitialRound.BusinessLogic.CodedTests
{
    public abstract class LineByLineCodedTest<TTestData> : ICodedTest
    {
        protected readonly int numberOfTestCases;

        public LineByLineCodedTest(int numberOfTestCases)
        {
            this.numberOfTestCases = numberOfTestCases;
        }

        protected abstract TTestData GenerateTestCase(Random random);

        protected abstract string ToInput(TTestData testData);

        protected abstract TestResult ValidateOutput(TTestData testData, string output);

        protected Classes.Services.TestResult Expect(string testName, TTestData testData, string expectedOutput, string output)
        {
            return new TestResult
            {
                ExpectedOutput = expectedOutput,
                Output = output,
                Success = expectedOutput.EqualsLenient(output),
                TestName = testName,
                Input = ToInput(testData)
            };
        }

        private IEnumerable<TTestData> GenerateTestCases(long randomizer)
        {
            Random random = new Random((int)randomizer);

            for (int i = 0; i < numberOfTestCases; i++)
            {
                yield return GenerateTestCase(random);
            }
        }

        public string GetInput(long randomizer)
        {
            var testCases = GenerateTestCases(randomizer);

            return string.Join("\n", testCases.Select(ToInput));
        }

        public IEnumerable<Classes.Services.TestResult> RunTests(string output, long randomizer)
        {
            var testCases = GenerateTestCases(randomizer).ToArray();

            var outputs = output.Lines();

            if (testCases.Length != outputs.Length)
            {
                throw new ValidationException(string.Format("Invalid output. Expected {0} lines, found {1}.", testCases.Length, outputs.Length));
            }

            return testCases.Zip(outputs, ValidateOutput);
        }
    }
}
