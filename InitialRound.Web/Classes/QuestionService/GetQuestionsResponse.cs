using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionService
{
    public class GetQuestionsResponse
    {
        public int TotalCount { get; set; }

        public GetQuestionsResponseItem[] Results { get; set; }
    }
}