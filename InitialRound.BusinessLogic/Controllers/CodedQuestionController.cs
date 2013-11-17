///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Classes.Services;
using InitialRound.BusinessLogic.CodedTests;
using InitialRound.BusinessLogic.Properties;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class CodedQuestionController
    {
        public const int GeneralizedFizzBuzz = 0;
        public const int InOrderToPostOrder = 1;
        public const int MakingIslands = 1000;
        public const int Outliers = 1001;
        public const int SortedLCS = 1002;
        public const int CuttingSquares = 1003;
        public const int Median = 1004;
        public const int BuyingWater = 1005;
        public const int NonConsecutiveSum = 1006;
        public const int LargestInitialSegmentMedian = 1007;

        public static string GetInput(int codedTestId, long randomizer)
        {
            ICodedTest codedTest = CreateCodedTest(codedTestId);
            return codedTest.GetInput(randomizer);
        }

        public static IEnumerable<TestResult> RunTests(int codedTestId, string output, long randomizer)
        {
            ICodedTest codedTest = CreateCodedTest(codedTestId);
            return codedTest.RunTests(output, randomizer);
        }

        public static ICodedTest CreateCodedTest(int codedTestId)
        {
            switch (codedTestId)
            {
                case GeneralizedFizzBuzz:
                    return new GeneralizedFizzBuzz(Settings.Default.TestsExecutedPerQuestion, 5, 20, 5);
                case InOrderToPostOrder:
                    return new InOrderToPostOrder(Settings.Default.TestsExecutedPerQuestion, 7, 100);
                case MakingIslands:
                    return new MakingIslands(Settings.Default.TestsExecutedPerQuestion, 5, 20, 3);
                case Outliers:
                    return new Outliers(Settings.Default.TestsExecutedPerQuestion, 10, 10, 100, 1, 10, 2);
                case SortedLCS:
                    return new SortedLCS(Settings.Default.TestsExecutedPerQuestion, 0, 100, 20, 100, 1000000);
                case CuttingSquares:
                    return new CuttingSquares(Settings.Default.TestsExecutedPerQuestion, 1, 100);
                case Median:
                    return new Median(Settings.Default.TestsExecutedPerQuestion, 1, 100, 1000);
                case BuyingWater:
                    return new BuyingWater(Settings.Default.TestsExecutedPerQuestion, 0, 5, 0, 1000000, 1, 1000, 1, 10000);
                case NonConsecutiveSum:
                    return new NonConsecutiveSum(Settings.Default.TestsExecutedPerQuestion, 10, 100, 1000000);
                case LargestInitialSegmentMedian:
                    return new LargestInitialSegmentMedian(Settings.Default.TestsExecutedPerQuestion, 10, 100, -1000000, 1000000);
            }

            throw new ArgumentException("codedTestId");
        }
    }
}
