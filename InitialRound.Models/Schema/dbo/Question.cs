using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InitialRound.Models.Schema.dbo
{
    [Table("Question", Schema = "dbo")]
    public class Question
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short QuestionTypeID { get; set; }
        public int? CodedTestID { get; set; }
        public Guid QuestionBody { get; set; }
        public Guid? Tests { get; set; }
        public DateTime? LastTestRunOn { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
