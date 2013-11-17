///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class CuttingSquares : LineByLineCodedTest<CuttingSquares.TestCase>
    {
        private readonly int minSize;
        private readonly int maxSize;

        public CuttingSquares(int numberOfTestCases, int minSize, int maxSize)
            : base(numberOfTestCases)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
        }

        public class TestCase
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }

        protected override CuttingSquares.TestCase GenerateTestCase(Random random)
        {
            return new TestCase
            {
                Width = random.Next(maxSize - minSize + 1) + minSize,
                Height = random.Next(maxSize - minSize + 1) + minSize
            };
        }

        protected override string ToInput(CuttingSquares.TestCase testData)
        {
            return string.Format("{0},{1}", testData.Width, testData.Height);
        }

        protected override Classes.Services.TestResult ValidateOutput(CuttingSquares.TestCase testData, string output)
        {
            int w = testData.Width;
            int h = testData.Height;

            IList<int> sizesCut = new List<int>();

            while (w != h)
            {
                if (w > h)
                {
                    w = w - h;
                    sizesCut.Add(h);
                }
                else
                {
                    h = h - w;
                    sizesCut.Add(w);
                }
            }

            sizesCut.Add(w);

            string expectedOutput = string.Join(",", sizesCut);

            return Expect("Random Test Case", testData, expectedOutput, output);
        }
    }
}
