using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.InterviewService
{
    public class InterviewQuestionItem
    {
        public Guid ID { get; set; }

        public Guid InterviewQuestionID { get; set; }

        public string Name { get; set; }
    }
}
