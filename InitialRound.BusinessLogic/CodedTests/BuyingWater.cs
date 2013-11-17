///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class BuyingWater : LineByLineCodedTest<BuyingWater.TestCase>
    {
        private readonly int minNumberOfValues;
        private readonly int maxNumberOfValues;
        private readonly int minMoney;
        private readonly int maxMoney;
        private readonly int minPricePerUnit;
        private readonly int maxPricePerUnit;
        private readonly int minAvailableUnits;
        private readonly int maxAvailableUnits;

        public BuyingWater(int numberOfTestCases, int minNumberOfValues, int maxNumberOfValues, int minMoney, int maxMoney,
            int minPricePerUnit, int maxPricePerUnit, int minAvailableUnits, int maxAvailableUnits)
            : base(numberOfTestCases)
        {
            this.minNumberOfValues = minNumberOfValues;
            this.maxNumberOfValues = maxNumberOfValues;
            this.minMoney = minMoney;
            this.maxMoney = maxMoney;
            this.minPricePerUnit = minPricePerUnit;
            this.maxPricePerUnit = maxPricePerUnit;
            this.minAvailableUnits = minAvailableUnits;
            this.maxAvailableUnits = maxAvailableUnits;
        }

        public class TestCase
        {
            public int Money { get; set; }

            public TestCaseValue[] Values { get; set; }
        }

        public class TestCaseValue
        {
            public int PricePerUnit { get; set; }

            public int AvailableUnits { get; set; }
        }

        protected override BuyingWater.TestCase GenerateTestCase(Random random)
        {
            int numberOfValues = random.Next(maxNumberOfValues - minNumberOfValues + 1) + minNumberOfValues;

            return new TestCase
            {
                Money = random.Next(maxMoney - minMoney + 1) + minMoney,
                Values = Enumerable.Range(0, numberOfValues).Select(_ =>
                {
                    int pricePerUnit = random.Next(maxPricePerUnit - minPricePerUnit + 1) + minPricePerUnit;
                    int availableUnits = random.Next(maxAvailableUnits - minAvailableUnits + 1) + minAvailableUnits;

                    return new TestCaseValue { PricePerUnit = pricePerUnit, AvailableUnits = availableUnits };
                }).ToArray()
            };
        }

        protected override string ToInput(BuyingWater.TestCase testData)
        {
            return string.Format("{0};{1}", testData.Money, string.Join(";", testData.Values.Select(v => string.Format("{0},{1}", v.PricePerUnit, v.AvailableUnits))));
        }

        protected override Classes.Services.TestResult ValidateOutput(BuyingWater.TestCase testData, string output)
        {
            int remainingMoney = testData.Money;
            int unitsPurchased = 0;

            var values = testData.Values.Select(t => new TestCaseValue
            {
                PricePerUnit = t.PricePerUnit,
                AvailableUnits = t.AvailableUnits
            }).ToArray();

            while (values.Any(v => v.AvailableUnits > 0) && remainingMoney >= values.Where(v => v.AvailableUnits > 0).Select(v => v.PricePerUnit).Min())
            {
                int minIndex = -1;
                int minPricePerUnit = int.MaxValue;

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].AvailableUnits > 0 && values[i].PricePerUnit < minPricePerUnit)
                    {
                        minPricePerUnit = values[i].PricePerUnit;
                        minIndex = i;
                    }
                }

                if (minIndex > -1)
                {
                    int unitsPurchasedAtThisPrice = Math.Min(values[minIndex].AvailableUnits, remainingMoney / values[minIndex].PricePerUnit);

                    values[minIndex].AvailableUnits -= unitsPurchasedAtThisPrice;
                    unitsPurchased+= unitsPurchasedAtThisPrice;
                    remainingMoney -= unitsPurchasedAtThisPrice * values[minIndex].PricePerUnit;
                }
            };

            string expectedOutput = unitsPurchased.ToString();

            return Expect("Random Test Case", testData, expectedOutput, output);
        }
    }
}
