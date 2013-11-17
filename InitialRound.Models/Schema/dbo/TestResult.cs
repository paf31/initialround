using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InitialRound.Models.Schema.dbo
{
    [Table("TestResult", Schema = "dbo")]
    public class TestResult
    {
        [Key]
        public Guid ID { get; set; }
        public Guid AttemptID { get; set; }
        public Guid? TestID { get; set; }
        public bool Passed { get; set; }
        public string TestName { get; set; }
        public string InputString { get; set; }
        public string ExpectedOutputString { get; set; }
        public string OutputString { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }

        public virtual Attempt Attempt { get; set; }
    }
}
