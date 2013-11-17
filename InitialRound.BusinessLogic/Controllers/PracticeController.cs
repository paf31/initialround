///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Classes.Services;
using InitialRound.Common.Exceptions;
using InitialRound.Common.Extensions;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class PracticeController
    {
        private static readonly long[] input =
            new long[] 
            {
                400514,
                863414,
                250147,
                49861,
                365565,
                820569,
                979888,
                638548,
                449056,
                365148
            };

        public static string InputString
        {
            get
            {
                return string.Join("\n", input);
            }
        }

        public static TestResult[] ValidateOutput(string output)
        {
            var parsedOutput = output.Lines();

            if (input.Length > parsedOutput.Length)
            {
                throw new ValidationException(string.Format("Invalid output. Expected {0} lines, found {1}.", input.Length, parsedOutput.Length));
            }

            return input.Select((value, n) => new { value, index = n + 1 })
                .Zip(parsedOutput, 
                    (value, outputValue) => new TestResult
                    {
                        TestName = "Test Case #" + value.index,
                        Success = outputValue.EqualsLenient((value.value * value.value).ToString()),
                        Input = value.value.ToString(),
                        ExpectedOutput = (value.value * value.value).ToString(),
                        Output = outputValue
                    })
                .ToArray();
        }
    }
}
