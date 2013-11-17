CREATE TABLE [dbo].[User](
	[ID] [varchar](20) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[EmailAddress] [nvarchar](200) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[PasswordHash] [varbinary](2048) NOT NULL,
	[PasswordSalt] [varbinary](256) NOT NULL,
	[LoginAttempts] [int] NOT NULL,
	[LastLoginDate] [datetime2] NULL,
	[CreatedDate] [datetime2] NOT NULL,
	[LastUpdatedDate] [datetime2] NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ( [ID] ASC ),
)
GO

CREATE INDEX [IX_User_Name] ON [dbo].[User] (LastName, FirstName)
GO

CREATE TABLE dbo.Applicant(
    [ID] [uniqueidentifier] NOT NULL,
    [FirstName] [nvarchar](100) NOT NULL,
    [LastName] [nvarchar](100) NOT NULL,
    [EmailAddress] [nvarchar](200) NOT NULL,
    [CreatedDate] [datetime2] NOT NULL,
    [CreatedBy] [varchar](50) NOT NULL,
    [LastUpdatedDate] [datetime2] NOT NULL,
    [LastUpdatedBy] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Applicant] PRIMARY KEY CLUSTERED ( [ID] ASC )
)
GO

CREATE INDEX [IX_Applicant_CreatedDate] ON [dbo].[Applicant] ([CreatedDate])
GO

CREATE TABLE [dbo].[Question](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](MAX) NULL,
	[QuestionBody] [uniqueidentifier] NOT NULL,	
	[QuestionTypeID] [smallint] NOT NULL,
	[CodedTestID] [int] NULL,
	[Tests] [uniqueidentifier] NULL,
	[LastTestRunOn] [datetime2] NULL,
    [CreatedDate] [datetime2] NOT NULL,
    [CreatedBy] [varchar](50) NOT NULL,
    [LastUpdatedDate] [datetime2] NOT NULL,
    [LastUpdatedBy] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ( [ID] ASC )
)
GO

CREATE INDEX [IX_Question_CreatedDate] ON [dbo].[Question] ([CreatedDate])
GO

CREATE TABLE [dbo].[QuestionSet](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime2] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[LastUpdatedDate] [datetime2] NULL,
	[LastUpdatedBy] [varchar](50) NULL,
    CONSTRAINT [PK_QuestionSet] PRIMARY KEY CLUSTERED ( [ID] ASC )
) 
GO

CREATE INDEX [IX_QuestionSet_CreatedDate] ON [dbo].[QuestionSet] ([CreatedDate])
GO

CREATE TABLE [dbo].[QuestionSetQuestion](
	[QuestionSetID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_QuestionSetQuestion] PRIMARY KEY CLUSTERED  ([QuestionSetID] ASC, [QuestionID] ASC ),
    CONSTRAINT [FK_QuestionSetQuestion_Question] FOREIGN KEY([QuestionID]) REFERENCES [dbo].[Question] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_QuestionSetQuestion_QuestionSet] FOREIGN KEY([QuestionSetID]) REFERENCES [dbo].[QuestionSet] ([ID]) ON DELETE CASCADE
) 
GO

CREATE INDEX [IX_QuestionSetQuestion_QuestionSet] ON [dbo].[QuestionSetQuestion] ([QuestionSetID])
GO

CREATE TABLE [dbo].[Interview](
	[ID] [uniqueidentifier] NOT NULL,
	[ApplicantID] [uniqueidentifier] NOT NULL,
	[SentDate] [datetime2] NULL,
	[MinutesAllowed] [int] NOT NULL,
	[StartedDate] [datetime2] NULL,
	[CreatedDate] [datetime2] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[LastUpdatedDate] [datetime2] NULL,
	[LastUpdatedBy] [varchar](50) NULL,
    CONSTRAINT [PK_Interview] PRIMARY KEY CLUSTERED ([ID] ASC ),
    CONSTRAINT [FK_Interview_Applicant] FOREIGN KEY([ApplicantID]) REFERENCES [dbo].[Applicant] ([ID]) ON DELETE CASCADE
) 
GO

CREATE INDEX [IX_Interview_Applicant] ON [dbo].[Interview] ([ApplicantID])
GO

CREATE INDEX [IX_Interview_CreatedDate] ON [dbo].[Interview] ([CreatedDate])
GO

CREATE TABLE [dbo].[InterviewQuestion](
    [ID] uniqueidentifier NOT NULL,
	[InterviewID] [uniqueidentifier] NOT NULL,
	[QuestionID] [uniqueidentifier] NOT NULL,
	[LastAttemptID] [uniqueidentifier] NULL,
	[LastTestRunOn] [datetime2] NULL,
	[Randomizer] [bigint] NULL,
    CONSTRAINT [PK_InterviewQuestion] PRIMARY KEY CLUSTERED ([ID] ASC ),
    CONSTRAINT [FK_InterviewQuestion_Question] FOREIGN KEY([QuestionID]) REFERENCES [dbo].[Question] ([ID]),
    CONSTRAINT [FK_InterviewQuestion_Interview] FOREIGN KEY([InterviewID]) REFERENCES [dbo].[Interview] ([ID]) ON DELETE CASCADE
) 
GO

CREATE INDEX [IX_InterviewQuestion_Interview] ON [dbo].[InterviewQuestion] ([InterviewID])
GO

CREATE TABLE dbo.Attempt (
    [ID] [uniqueidentifier] NOT NULL,
	[InterviewQuestionID] [uniqueidentifier] NOT NULL,
	[Output] [uniqueidentifier] NOT NULL,	
	[Code] [uniqueidentifier] NOT NULL,	
	[CreatedDate] [datetime2] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[LastUpdatedDate] [datetime2] NOT NULL,
	[LastUpdatedBy] [varchar](50) NOT NULL,
    CONSTRAINT [PK_Attempt] PRIMARY KEY CLUSTERED ( [ID] ASC ),
    CONSTRAINT [FK_Attempt_InterviewQuestion] FOREIGN KEY ([InterviewQuestionID]) REFERENCES dbo.InterviewQuestion ([ID]) ON DELETE CASCADE,
) 
GO

ALTER TABLE [dbo].[InterviewQuestion] ADD CONSTRAINT [FK_InterviewQuestion_LastAttempt] FOREIGN KEY([LastAttemptID]) REFERENCES [dbo].[Attempt] ([ID])
GO

CREATE INDEX [IX_Attempt_InterviewQuestion] ON [dbo].[Attempt] ([InterviewQuestionID])
GO

CREATE TABLE dbo.TestResult (
    [ID] [uniqueidentifier] NOT NULL,
	[AttemptID] [uniqueidentifier] NOT NULL,
	[TestID] [uniqueidentifier] NULL,
	[Passed] bit NOT NULL,	
	[TestName] varchar(100) NULL,
	[InputString] varchar(100) NULL,
	[ExpectedOutputString] varchar(100) NULL,
	[OutputString] varchar(100) NULL,
	[CreatedDate] [datetime2] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[LastUpdatedDate] [datetime2] NOT NULL,
	[LastUpdatedBy] [varchar](50) NOT NULL,
    CONSTRAINT [PK_TestResult] PRIMARY KEY CLUSTERED ([ID] ASC ),
    CONSTRAINT [FK_TestResult_Attempt] FOREIGN KEY ([AttemptID]) REFERENCES dbo.Attempt ([ID]) ON DELETE CASCADE
) 
GO

CREATE INDEX [IX_TestResult_Attempt] ON [dbo].[TestResult] ([AttemptID])
GO