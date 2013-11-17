///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.Common.Extensions;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class GeneralizedFizzBuzz : LineByLineCodedTest<GeneralizedFizzBuzz.TestCase>
    {
        private readonly int maxNumberOfDivisors;
        private readonly int maxDivisor;
        private readonly int wordSize;

        public GeneralizedFizzBuzz(int numberOfTests, int maxNumberOfDivisors, int maxDivisor, int wordSize)
            : base(numberOfTests)
        {
            this.maxNumberOfDivisors = maxNumberOfDivisors;
            this.maxDivisor = maxDivisor;
            this.wordSize = wordSize;
        }

        public class TestCase
        {
            public Tuple<int, string>[] Divisors { get; set; }
        }

        protected override GeneralizedFizzBuzz.TestCase GenerateTestCase(Random random)
        {
            int numberOfDivisors = 2 + random.Next(maxNumberOfDivisors - 2);

            var divisors = new Tuple<int, string>[numberOfDivisors];

            for (int i = 0; i < numberOfDivisors; i++)
            {
                int divisor = 2 + random.Next(maxDivisor - 2);
                string word = random.NextString(wordSize);

                divisors[i] = Tuple.Create(divisor, word);
            }

            return new TestCase { Divisors = divisors };
        }

        protected override string ToInput(GeneralizedFizzBuzz.TestCase testData)
        {
            return string.Format("{0};{1}",
                string.Join(",", testData.Divisors.Select(d => d.Item1)),
                string.Join(",", testData.Divisors.Select(d => d.Item2)));
        }

        protected override Classes.Services.TestResult ValidateOutput(GeneralizedFizzBuzz.TestCase testData, string output)
        {
            IList<string> parts = new List<string>();

            for (int i = 1; i <= 100; i++)
            {
                foreach (var divisor in testData.Divisors)
                {
                    if (i % divisor.Item1 == 0)
                    {
                        parts.Add(divisor.Item2);
                        break;
                    }
                }

                parts.Add(i.ToString());
            }

            return Expect("Random Test Case", testData, string.Join(",", parts), output);
        }
    }
}
