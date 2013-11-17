using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.QuestionService
{
    public class RunTestsResponseItem
    {
        public string TestName { get; set; }

        public Guid TestID { get; set; }

        public bool Success { get; set; }

        public string Input { get; set; }

        public string ExpectedOutput { get; set; }

        public string Output { get; set; }
    }
}
