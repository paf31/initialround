///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.Common.Extensions;

namespace InitialRound.BusinessLogic.CodedTests
{
    public class InOrderToPostOrder : LineByLineCodedTest<InOrderToPostOrder.TestCase>
    {
        private readonly int maxDepthOfTree;
        private readonly int maxNodeValue;

        public InOrderToPostOrder(int numberOfTests, int maxDepthOfTree, int maxNodeValue)
            : base(numberOfTests)
        {
            this.maxDepthOfTree = maxDepthOfTree;
            this.maxNodeValue = maxNodeValue;
        }

        public class Tree
        {
            public int Value { get; set; }

            public Tree Left { get; set; }

            public Tree Right { get; set; }
        }

        public class TestCase
        {
            public int Depth { get; set; }

            public Tree Tree { get; set; }
        }

        protected override InOrderToPostOrder.TestCase GenerateTestCase(Random random)
        {
            int depth = 3 + random.Next(maxDepthOfTree - 3);

            var tree = GenerateTree(depth, random);

            return new TestCase { Depth = depth, Tree = tree };
        }

        private Tree GenerateTree(int depth, Random random)
        {
            Tree tree = new Tree();

            tree.Value = random.Next(maxNodeValue);

            if (depth > 1)
            {
                tree.Left = GenerateTree(depth - 1, random);
                tree.Right = GenerateTree(depth - 1, random);
            }

            return tree;
        }

        private static IEnumerable<int> InOrder(Tree tree)
        {
            if (tree.Left != null)
            {
                foreach (var value in InOrder(tree.Left))
                {
                    yield return value;
                }
            }

            yield return tree.Value;

            if (tree.Right != null)
            {
                foreach (var value in InOrder(tree.Right))
                {
                    yield return value;
                }
            }
        }

        private static IEnumerable<int> PostOrder(Tree tree)
        {
            if (tree.Left != null)
            {
                foreach (var value in PostOrder(tree.Left))
                {
                    yield return value;
                }
            }

            if (tree.Right != null)
            {
                foreach (var value in PostOrder(tree.Right))
                {
                    yield return value;
                }
            }

            yield return tree.Value;
        }

        protected override string ToInput(InOrderToPostOrder.TestCase testData)
        {
            return string.Join(",", InOrder(testData.Tree));
        }

        protected override Classes.Services.TestResult ValidateOutput(InOrderToPostOrder.TestCase testData, string output)
        {
            var expectedOutput = string.Join(",", PostOrder(testData.Tree));

            return Expect("Random Test Case", testData, expectedOutput, output);
        }
    }
}
