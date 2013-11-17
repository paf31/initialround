using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.QuestionService
{
    public class GetQuestionResponse
    {
        public string Name { get; set; }

        public bool CanEdit { get; set; }

        public bool IsCodedTest { get; set; }

        public string QuestionBody { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }

        public QuestionTest[] Tests { get; set; }
    }
}