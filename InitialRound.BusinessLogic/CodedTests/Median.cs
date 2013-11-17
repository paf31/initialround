///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class Median : LineByLineCodedTest<Median.TestCase>
    {
        private readonly int minNumberOfValues;
        private readonly int maxNumberOfValues;
        private readonly int maxValue;

        public Median(int numberOfTestCases, int minNumberOfValues, int maxNumberOfValues, int maxValue)
            : base(numberOfTestCases)
        {
            this.minNumberOfValues = minNumberOfValues;
            this.maxNumberOfValues = maxNumberOfValues;
            this.maxValue = maxValue;
        }

        public class TestCase
        {
            public int[] Values { get; set; }
        }

        protected override Median.TestCase GenerateTestCase(Random random)
        {
            int numberOfValues = random.Next(maxNumberOfValues - minNumberOfValues + 1) + minNumberOfValues;

            return new TestCase
            {
                Values = Enumerable.Range(0, numberOfValues).Select(_ => random.Next(maxValue)).ToArray()
            };
        }

        protected override string ToInput(Median.TestCase testData)
        {
            return string.Join(",", testData.Values);
        }

        protected override Classes.Services.TestResult ValidateOutput(Median.TestCase testData, string output)
        {
            int[] sortedValues = testData.Values.OrderBy(v => v).ToArray();

            int median;

            if (sortedValues.Length % 2 == 0)
            {
                median = (sortedValues[sortedValues.Length / 2] + sortedValues[sortedValues.Length / 2 - 1]) / 2;
            }
            else
            {
                median = sortedValues[(sortedValues.Length - 1) / 2];
            }

            string expectedOutput = median.ToString();

            return Expect("Random Test Case", testData, expectedOutput, output);
        }
    }
}
