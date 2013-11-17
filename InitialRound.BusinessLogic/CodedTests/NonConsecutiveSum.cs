///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class NonConsecutiveSum : LineByLineCodedTest<NonConsecutiveSum.TestCase>
    {
        private readonly int minNumberOfValues;
        private readonly int maxNumberOfValues;
        private readonly int maxValue;

        public NonConsecutiveSum(int numberOfTestCases, int minNumberOfValues, int maxNumberOfValues, int maxValue)
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

        protected override NonConsecutiveSum.TestCase GenerateTestCase(Random random)
        {
            int numberOfValues = random.Next(maxNumberOfValues - minNumberOfValues + 1) + minNumberOfValues;

            return new TestCase
            {
                Values = Enumerable.Range(0, numberOfValues).Select(_ => random.Next(maxValue * 2) - maxValue).ToArray()
            };
        }

        protected override string ToInput(NonConsecutiveSum.TestCase testData)
        {
            return string.Join(",", testData.Values);
        }

        private static long MaxNonConsecutiveSum(int[] vals)
        {
            long sumWith = 0;
            long sumWithout = 0;

            for (int i = 0; i < vals.Length; i++)
            {
                long newSumWith = sumWithout + vals[i];
                long newSumWithout = Math.Max(sumWith, sumWithout);
                sumWith = newSumWith;
                sumWithout = newSumWithout;
            }

            return Math.Max(sumWith, sumWithout);
        }

        protected override Classes.Services.TestResult ValidateOutput(NonConsecutiveSum.TestCase testData, string output)
        {
            string expectedOutput = MaxNonConsecutiveSum(testData.Values).ToString();

            return Expect("Random Test Case", testData, expectedOutput, output);
        }
    }
}
