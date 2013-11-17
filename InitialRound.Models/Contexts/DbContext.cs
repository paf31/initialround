using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using dbo = InitialRound.Models.Schema.dbo;
using System.Data.Common;

namespace InitialRound.Models.Contexts
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext(string connectionString)
            : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dbo::Attempt>();
            modelBuilder.Entity<dbo::Applicant>();
            modelBuilder.Entity<dbo::Question>();
            modelBuilder.Entity<dbo::QuestionSet>();
            modelBuilder.Entity<dbo::QuestionSetQuestion>();
            modelBuilder.Entity<dbo::Interview>();
            modelBuilder.Entity<dbo::InterviewQuestion>();
            modelBuilder.Entity<dbo::TestResult>();

            modelBuilder.Entity<dbo::Interview>()
                .HasRequired(i => i.Applicant)
                .WithMany()
                .HasForeignKey(i => i.ApplicantID);

            modelBuilder.Entity<dbo::InterviewQuestion>()
                .HasRequired(i => i.Question)
                .WithMany()
                .HasForeignKey(i => i.QuestionID);

            modelBuilder.Entity<dbo::InterviewQuestion>()
                .HasRequired(i => i.LastAttempt)
                .WithMany()
                .HasForeignKey(i => i.LastAttemptID);

            modelBuilder.Entity<dbo::InterviewQuestion>()
                .HasRequired(i => i.Interview)
                .WithMany()
                .HasForeignKey(i => i.InterviewID);

            modelBuilder.Entity<dbo::Attempt>()
                .HasRequired(i => i.InterviewQuestion)
                .WithMany()
                .HasForeignKey(i => i.InterviewQuestionID);

            modelBuilder.Entity<dbo::QuestionSetQuestion>()
                .HasRequired(i => i.Question)
                .WithMany()
                .HasForeignKey(i => i.QuestionID);

            modelBuilder.Entity<dbo::QuestionSetQuestion>()
                .HasRequired(i => i.QuestionSet)
                .WithMany()
                .HasForeignKey(i => i.QuestionSetID);

            modelBuilder.Entity<dbo::TestResult>();

            modelBuilder.Entity<dbo::TestResult>()
                .HasRequired(i => i.Attempt)
                .WithMany(a => a.TestResults)
                .HasForeignKey(i => i.AttemptID);

            modelBuilder.Entity<dbo::User>();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<dbo::Attempt> Attempts { get; set; }
        public DbSet<dbo::Applicant> Applicants { get; set; }
        public DbSet<dbo::Question> Questions { get; set; }
        public DbSet<dbo::QuestionSet> QuestionSets { get; set; }
        public DbSet<dbo::QuestionSetQuestion> QuestionSetQuestions { get; set; }
        public DbSet<dbo::Interview> Interviews { get; set; }
        public DbSet<dbo::InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<dbo::TestResult> TestResults { get; set; }
        public DbSet<dbo::User> Users { get; set; }
    }
}
