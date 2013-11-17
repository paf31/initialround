using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.QuestionSetService
{
    public class GetQuestionSetsResponse
    {
        public int TotalCount { get; set; }

        public GetQuestionSetsResponseItem[] Results { get; set; }
    }
}