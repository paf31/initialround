using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionService
{
    public class CreateQuestionResponse
    {
        public Guid QuestionID { get; set; }

        public Guid[] TestIDs { get; set; }
    }
}