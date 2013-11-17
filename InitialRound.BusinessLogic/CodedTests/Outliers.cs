///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Classes.Services;
using InitialRound.BusinessLogic.Exceptions;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class Outliers : LineByLineCodedTest<Outliers.TestCase>
    {
        private readonly int maxValue;
        private readonly int minNumberOfValues;
        private readonly int maxNumberOfValues;
        private readonly int minSmoothness;
        private readonly int maxSmoothness;
        private readonly int cutoff;

        public Outliers(int numberOfTestCases, int maxValue, int minNumberOfValues, int maxNumberOfValues, int minSmoothness, int maxSmoothness, int cutoff)
            : base(numberOfTestCases)
        {
            this.maxValue = maxValue;
            this.minNumberOfValues = minNumberOfValues;
            this.maxNumberOfValues = maxNumberOfValues;
            this.minSmoothness = minSmoothness;
            this.maxSmoothness = maxSmoothness;
            this.cutoff = cutoff;
        }

        public class TestCase
        {
            public int[] Values { get; set; }
        }

        protected override Outliers.TestCase GenerateTestCase(Random random)
        {
            var smoothness = minSmoothness + random.Next(maxSmoothness - minSmoothness + 1);

            var numberOfValues = minNumberOfValues + random.Next(maxNumberOfValues - minNumberOfValues + 1);

            var randoms = Enumerable.Range(0, int.MaxValue).Select(_ => random.Next(maxValue * 2 + 1) - maxValue);

            var values = Centralize(randoms, smoothness).Take(numberOfValues);

            return new TestCase { Values = values.ToArray() };
        }

        private static IEnumerable<int> Centralize(IEnumerable<int> random, int n)
        {
            using (var e = random.GetEnumerator())
            {
                while (true)
                {
                    int total = 0;

                    for (int j = 0; j < n; j++)
                    {
                        e.MoveNext();
                        total += e.Current;
                    }

                    yield return total;
                }
            }
        }

        protected override string ToInput(Outliers.TestCase testData)
        {
            return string.Join(",", testData.Values);
        }

        protected override Classes.Services.TestResult ValidateOutput(Outliers.TestCase testData, string output)
        {
            var sum = testData.Values.Sum();
            var count = testData.Values.Count();

            var rhs = cutoff * cutoff * (testData.Values.Select(x => x * x * count * count).Sum() - sum * sum);

            var expectedOutput = testData.Values.Where(x => count * (count * x - sum) * (count * x - sum) > rhs).OrderBy(n => n);

            int[] actualOutput;

            if (ParseOutput(output, out actualOutput))
            {
                return new TestResult
                {
                    ExpectedOutput = string.Join(",", expectedOutput),
                    Output = output,
                    Success = Enumerable.SequenceEqual(expectedOutput, actualOutput.OrderBy(n => n)),
                    TestName = "Random Test Case",
                    Input = string.Join(",", actualOutput.OrderBy(n => n))
                };
            }
            else
            {
                return new TestResult
                {
                    ExpectedOutput = "Expected a comma separated string of integers",
                    Output = output,
                    Success = false,
                    TestName = "Random Test Case",
                    Input = string.Join(",", actualOutput.OrderBy(n => n))
                };
            }
        }

        private bool ParseOutput(string output, out int[] values)
        {
            var parts = output.Split(',');

            var list = new List<int>();

            foreach (var part in parts)
            {
                int value;

                if (int.TryParse(part.Trim(), out value))
                {
                    list.Add(value);
                }
                else
                {
                    values = new int[0];
                    return false;
                }
            }

            values = list.ToArray();
            return true;
        }
    }
}
