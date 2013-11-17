using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetInterviewsResponse
    {
        public int TotalCount { get; set; }

        public GetInterviewsResponseItem[] Results { get; set; }
    }
}