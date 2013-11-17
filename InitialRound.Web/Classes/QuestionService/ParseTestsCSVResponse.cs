using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.QuestionService
{
    public class ParseTestsCSVResponse
    {
        public CSVTest[] Tests { get; set; }
    }

    public class CSVTest
    {
        public string Name { get; set; }

        public string Input { get; set; }

        public string ExpectedOutput { get; set; }
    }
}
