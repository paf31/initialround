using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.InterviewService
{
    public class GetInterviewsResponseItem
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public int MinutesRemaining { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
    }
}
