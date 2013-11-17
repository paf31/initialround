///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class MakingIslands : LineByLineCodedTest<MakingIslands.TestCase>
    {
        private readonly int minDimensions;
        private readonly int maxDimensions;
        private readonly int roughness;

        public MakingIslands(int numberOfTestCases, int minDimensions, int maxDimensions, int roughness)
            : base(numberOfTestCases)
        {
            this.minDimensions = minDimensions;
            this.maxDimensions = maxDimensions;
            this.roughness = roughness;
        }

        public class TestCase
        {
            public int[,] Heights { get; set; }

            public int Dimensions { get; set; }
        }

        protected override MakingIslands.TestCase GenerateTestCase(Random random)
        {
            int dimensions = minDimensions + random.Next(maxDimensions - minDimensions + 1);

            int[,] heights = new int[dimensions, dimensions];

            heights[0, 0] = random.Next(roughness);

            for (int i = 1; i < dimensions; i++)
            {
                heights[i, 0] = heights[i - 1, 0] + random.Next(roughness * 2 + 1) - roughness;
                heights[0, i] = heights[0, i - 1] + random.Next(roughness * 2 + 1) - roughness;
            }

            for (int i = 1; i < dimensions; i++)
            {
                for (int j = 1; j < dimensions; j++)
                {
                    heights[i, j] = (heights[i - 1, j] + heights[i, j - 1] + heights[i - 1, j - 1]) / 3 + random.Next(roughness * 2 + 1) - roughness;
                }
            }

            return new TestCase { Dimensions = dimensions, Heights = heights };
        }

        private static int MaxWaterLevel(int[,] heights, int dimensions)
        {
            var byDepth =
                from x in Enumerable.Range(0, dimensions)
                from y in Enumerable.Range(0, dimensions)
                group new { x, y } by heights[x, y] into height
                orderby height.Key descending
                select height;

            var colors = new Dictionary<Tuple<int, int>, int>();

            var minHeight = int.MaxValue;

            var wasConnected = true;

            foreach (var height in byDepth)
            {
                foreach (var p in height)
                {
                    var pColors = new HashSet<int>();

                    if (colors.ContainsKey(Tuple.Create(p.x - 1, p.y)))
                    {
                        pColors.Add(colors[Tuple.Create(p.x, p.y)] = colors[Tuple.Create(p.x - 1, p.y)]);
                    }

                    if (colors.ContainsKey(Tuple.Create(p.x + 1, p.y)))
                    {
                        pColors.Add(colors[Tuple.Create(p.x, p.y)] = colors[Tuple.Create(p.x + 1, p.y)]);
                    }

                    if (colors.ContainsKey(Tuple.Create(p.x, p.y - 1)))
                    {
                        pColors.Add(colors[Tuple.Create(p.x, p.y)] = colors[Tuple.Create(p.x, p.y - 1)]);
                    }

                    if (colors.ContainsKey(Tuple.Create(p.x, p.y + 1)))
                    {
                        pColors.Add(colors[Tuple.Create(p.x, p.y)] = colors[Tuple.Create(p.x, p.y + 1)]);
                    }

                    if (pColors.Count == 0)
                    {
                        colors[Tuple.Create(p.x, p.y)] = colors.Any() ? colors.Values.Max() + 1 : 0;
                    }
                    else if (pColors.Count > 1)
                    {
                        UpdateAll(colors, pColors);
                    }
                }

                var connected = colors.Values.Distinct().Count() == 1;

                if (!wasConnected && connected)
                {
                    minHeight = height.Key;
                }

                wasConnected = connected;
            }

            return minHeight;
        }

        private static void UpdateAll(IDictionary<Tuple<int, int>, int> colors, HashSet<int> pColors)
        {
            var toUpdate = new List<Tuple<int, int>>();

            foreach (var kv in colors)
            {
                if (pColors.Contains(kv.Value))
                {
                    toUpdate.Add(kv.Key);
                }
            }

            var rep = pColors.First();

            foreach (var key in toUpdate)
            {
                colors[key] = rep;
            }
        }

        protected override string ToInput(MakingIslands.TestCase testData)
        {
            return string.Join(";", Enumerable.Range(0, testData.Dimensions).Select(y => string.Join(",",
                Enumerable.Range(0, testData.Dimensions).Select(x => testData.Heights[x, y]))));
        }

        protected override Classes.Services.TestResult ValidateOutput(MakingIslands.TestCase testData, string output)
        {
            int expected = MaxWaterLevel(testData.Heights, testData.Dimensions);

            return Expect("Random Test Case", testData, expected.ToString(), output);
        }
    }
}
