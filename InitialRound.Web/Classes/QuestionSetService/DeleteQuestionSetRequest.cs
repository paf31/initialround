using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionSetService
{
    public class DeleteQuestionSetRequest
    {
        public Guid QuestionSetID { get; set; }

        public string AuthToken { get; set; }

        public string AntiForgeryToken { get; set; }
    }
}