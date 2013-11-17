using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InitialRound.Models.Schema.dbo
{
    [Table("Interview", Schema = "dbo")]
    public class Interview
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ApplicantID { get; set; }
        public DateTime? StartedDate { get; set; }
        public int MinutesAllowed { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }

        public virtual Applicant Applicant { get; set; }
    }
}
