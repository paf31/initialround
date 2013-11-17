using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InitialRound.BusinessLogic.Enums;

namespace InitialRound.Web.Classes.QuestionService
{
    public class GetQuestionsResponseItem
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
    }
}
