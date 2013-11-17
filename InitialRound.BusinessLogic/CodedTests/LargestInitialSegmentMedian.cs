///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class LargestInitialSegmentMedian : LineByLineCodedTest<LargestInitialSegmentMedian.TestCase>
    {
        private readonly int minNumberOfValues;
        private readonly int maxNumberOfValues;
        private readonly int minValue;
        private readonly int maxValue;

        public LargestInitialSegmentMedian(int numberOfTestCases, int minNumberOfValues, int maxNumberOfValues, int minValue, int maxValue)
            : base(numberOfTestCases)
        {
            this.minNumberOfValues = minNumberOfValues;
            this.maxNumberOfValues = maxNumberOfValues;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public class TestCase
        {
            public int[] Values { get; set; }
        }

        protected override LargestInitialSegmentMedian.TestCase GenerateTestCase(Random random)
        {
            int numberOfValues = random.Next(maxNumberOfValues - minNumberOfValues + 1) + minNumberOfValues;

            return new TestCase
            {
                Values = Enumerable.Range(0, numberOfValues).Select(_ => random.Next(maxValue - minValue + 1) + minValue).ToArray()
            };
        }

        protected override string ToInput(LargestInitialSegmentMedian.TestCase testData)
        {
            return string.Join(",", testData.Values);
        }

        protected override Classes.Services.TestResult ValidateOutput(LargestInitialSegmentMedian.TestCase testData, string output)
        {
            int maxMedian = Solve(testData);

            return Expect("Random Test Case", testData, maxMedian.ToString(), output);
        }

        private static int Solve(LargestInitialSegmentMedian.TestCase testData)
        {
            SortedDictionary<Tuple<int, int>, int> values = new SortedDictionary<Tuple<int, int>, int>();

            int index = 0;
            int maxMedian = int.MinValue;

            foreach (var value in testData.Values)
            {
                values.Add(Tuple.Create(value, index++), value);

                int median;

                if (values.Count % 2 == 0)
                {
                    median = (values.Values.ElementAt(values.Count / 2) + values.Values.ElementAt(values.Count / 2 - 1)) / 2;
                }
                else
                {
                    median = values.Values.ElementAt((values.Count - 1) / 2);
                }

                if (median > maxMedian)
                {
                    maxMedian = median;
                }
            }

            return maxMedian;
        }
    }
}
