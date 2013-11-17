using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InitialRound.Models.Schema.dbo
{
    [Table("QuestionSetQuestion", Schema = "dbo")]
    public class QuestionSetQuestion
    {
        [Key]
        [Column(Order = 0)]
        public Guid QuestionSetID { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid QuestionID { get; set; }

        public virtual QuestionSet QuestionSet { get; set; }

        public virtual Question Question { get; set; }
    }
}
