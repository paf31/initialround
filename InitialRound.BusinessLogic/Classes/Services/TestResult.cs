///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.BusinessLogic.Classes.Services
{
    public class TestResult
    {
        public string TestName { get; set; }

        public Guid? TestID { get; set; }

        public bool Success { get; set; }

        public string Input { get; set; }

        public string ExpectedOutput { get; set; }

        public string Output { get; set; }
    }
}
