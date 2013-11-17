using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.QuestionService
{
    public class EditQuestionRequest
    {
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
        
        public Guid QuestionID { get; set; }

        public string Name { get; set; }

        public string QuestionBody { get; set; }

        public QuestionTest[] Tests { get; set; }
    }
}