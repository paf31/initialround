using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitialRound.Web.Classes.ApplicantService
{
    public class GetApplicantsResponseItem
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string LastUpdatedBy { get; set; }

        public string LastUpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
    }
}
