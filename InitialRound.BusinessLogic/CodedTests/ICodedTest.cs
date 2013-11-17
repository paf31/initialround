///
///

using InitialRound.BusinessLogic.Classes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.CodedTests
{
    public interface ICodedTest
    {
        string GetInput(long randomizer);

        IEnumerable<TestResult> RunTests(string output, long randomizer);
    }
}
