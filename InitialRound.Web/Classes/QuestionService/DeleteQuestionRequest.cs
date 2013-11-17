using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionService
{
    public class DeleteQuestionRequest
    {
        public Guid QuestionID { get; set; }
        
        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
    }
}