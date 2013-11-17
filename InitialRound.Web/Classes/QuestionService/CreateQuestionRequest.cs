using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.QuestionService
{
    public class CreateQuestionRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }

        public string Name { get; set; }

        public string QuestionBody { get; set; }

        public QuestionTest[] Tests { get; set; }
    }
}