///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class SortedLCS : LineByLineCodedTest<SortedLCS.TestCase>
    {
        private readonly int minNumberOfCommonValues;
        private readonly int maxNumberOfCommonValues;
        private readonly int minNumberOfAdditionalValues;
        private readonly int maxNumberOfAdditionalValues;
        private readonly int maxValue;

        public SortedLCS(int numberOfTestCases, int minNumberOfCommonValues, int maxNumberOfCommonValues,
            int minNumberOfAdditionalValues, int maxNumberOfAdditionalValues, int maxValue)
            : base(numberOfTestCases)
        {
            this.minNumberOfCommonValues = minNumberOfCommonValues;
            this.maxNumberOfCommonValues = maxNumberOfCommonValues;
            this.minNumberOfAdditionalValues = minNumberOfAdditionalValues;
            this.maxNumberOfAdditionalValues = maxNumberOfAdditionalValues;
            this.maxValue = maxValue;
        }

        public class TestCase
        {
            public int[] Common { get; set; }

            public int[] Left { get; set; }

            public int[] Right { get; set; }
        }

        protected override SortedLCS.TestCase GenerateTestCase(Random random)
        {
            var numberOfCommonValues = minNumberOfCommonValues + random.Next(maxNumberOfCommonValues - minNumberOfCommonValues + 1);
            var numberOfAdditionalValues1 = minNumberOfAdditionalValues + random.Next(maxNumberOfAdditionalValues - minNumberOfAdditionalValues + 1);
            var numberOfAdditionalValues2 = minNumberOfAdditionalValues + random.Next(maxNumberOfAdditionalValues - minNumberOfAdditionalValues + 1);

            var common = Enumerable.Range(0, numberOfCommonValues).Select(_ => random.Next(maxValue)).OrderBy(n => n).ToArray();

            var values1 = Enumerable.Range(0, numberOfAdditionalValues1).Select(_ => random.Next(maxValue)).Concat(common).OrderBy(n => n).ToArray();
            var values2 = Enumerable.Range(0, numberOfAdditionalValues2).Select(_ => random.Next(maxValue)).Concat(common).OrderBy(n => n).ToArray();

            return new TestCase
            {
                Common = common,
                Left = values1,
                Right = values2
            };
        }

        protected override string ToInput(SortedLCS.TestCase testData)
        {
            return string.Format("{0};{1}", string.Join(",", testData.Left), string.Join(",", testData.Right));
        }

        protected override Classes.Services.TestResult ValidateOutput(SortedLCS.TestCase testData, string output)
        {
            return Expect("Random Test Case", testData, string.Join(",", testData.Common), output);
        }
    }
}
