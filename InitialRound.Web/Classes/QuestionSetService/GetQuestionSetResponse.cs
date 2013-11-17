using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionSetService
{
    public class GetQuestionSetResponse
    {
        public string Name { get; set; }

        public QuestionSetQuestionItem[] Questions { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
    }
}