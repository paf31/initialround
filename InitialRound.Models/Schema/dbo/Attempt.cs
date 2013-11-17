using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InitialRound.Models.Schema.dbo
{
    [Table("Attempt", Schema = "dbo")]
    public class Attempt
    {
        [Key]
        public Guid ID { get; set; }
        public Guid InterviewQuestionID { get; set; }
        public Guid Output { get; set; }
        public Guid Code { get; set; }
        public long? Randomizer { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }

        public virtual InterviewQuestion InterviewQuestion { get; set; }

        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
