using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.QuestionService
{
    public class QuestionTest
    {
        public Guid? ID { get; set; }

        public string Name { get; set; }

        public string Input { get; set; }

        public string ExpectedOutput { get; set; }
    }
}
