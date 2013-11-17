using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InitialRound.Models.Schema.dbo
{
    [Table("InterviewQuestion", Schema = "dbo")]
    public class InterviewQuestion
    {
        [Key]
        public Guid ID { get; set; }
        public Guid QuestionID { get; set; }
        public Guid InterviewID { get; set; }
        public DateTime? LastTestRunOn { get; set; }
        public Guid? LastAttemptID { get; set; }

        public virtual Question Question { get; set; }

        public virtual Interview Interview { get; set; }

        public virtual Attempt LastAttempt { get; set; }
    }
}
