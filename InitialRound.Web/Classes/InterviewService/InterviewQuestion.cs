using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.InterviewService
{
    public class InterviewQuestion
    {
        public Guid ID { get; set; }

        public string QuestionBody { get; set; }

        public string Name { get; set; }

        public bool Submitted { get; set; }
    }
}
