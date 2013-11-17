using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using InitialRound.BusinessLogic.Classes;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Enums;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.Common.Extensions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.Models.Contexts;
using InitialRound.Web.Classes.InterviewService;
using E = InitialRound.Models.Schema.dbo;
using System.Collections;
using InitialRound.BusinessLogic.Properties;
using InitialRound.Web.Classes.AccountService;
using InitialRound.BusinessLogic.Classes.Services;
using System.Threading.Tasks;
using System.Net;
using InitialRound.BusinessLogic;
using System.Data.Entity.Core.Objects;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class InterviewService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetInterviewsResponse GetInterviews(GetInterviewsRequest request)
        {
            GetInterviewsResponse response = new GetInterviewsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                Common.Helpers.ValidationHelper.Assert(request.StartAt < 1E4, "StartAt is too large.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                IQueryable<E::Interview> Interviews = context.Interviews;

                if (!string.IsNullOrEmpty(request.Name))
                {
                    Interviews = Interviews.Where(q => (q.Applicant.FirstName + " " + q.Applicant.LastName).Contains(request.Name));
                }

                if (request.Status.HasValue)
                {
                    switch ((InterviewStatus)request.Status.Value)
                    {
                        case InterviewStatus.Created:
                            Interviews = Interviews
                                .Where(i => !i.SentDate.HasValue);
                            break;
                        case InterviewStatus.WaitingForApplicant:
                            Interviews = Interviews
                                .Where(i => i.SentDate.HasValue)
                                .Where(i => !i.StartedDate.HasValue);
                            break;
                        case InterviewStatus.InProgress:
                            Interviews = Interviews
                                .Where(i => i.SentDate.HasValue)
                                .Where(i => i.StartedDate.HasValue)
                                .Where(i => EntityFunctions.AddMinutes(i.StartedDate.Value, i.MinutesAllowed) >= DateTime.UtcNow);
                            break;
                        case InterviewStatus.Completed:
                            Interviews = Interviews
                                .Where(i => i.SentDate.HasValue)
                                .Where(i => i.StartedDate.HasValue)
                                .Where(i => EntityFunctions.AddMinutes(i.StartedDate.Value, i.MinutesAllowed) < DateTime.UtcNow);
                            break;
                    }
                }

                if (request.ApplicantID.HasValue)
                {
                    Interviews = Interviews.Where(q => q.ApplicantID == request.ApplicantID.Value);
                }

                response.TotalCount = Interviews.Count();

                var interviews = Interviews.OrderByDescending(a => a.CreatedDate)
                    .Skip(request.StartAt)
                    .Take(InitialRound.Web.Properties.Settings.Default.PageSize)
                    .Select(interview => new
                    {
                        Interview = interview,
                        Applicant = new
                        {
                            FirstName = interview.Applicant.FirstName,
                            LastName = interview.Applicant.LastName
                        }
                    });

                response.Results = interviews.AsEnumerable().Select(interview => new GetInterviewsResponseItem
                {
                    ID = interview.Interview.ID,
                    Name = string.IsNullOrEmpty(interview.Applicant.FirstName) && string.IsNullOrEmpty(interview.Applicant.LastName)
                         ? "No Name"
                         : interview.Applicant.FirstName + " " + interview.Applicant.LastName,
                    Status = StatusHelper.GetInterviewStatus(interview.Interview).GetDescription(),
                    MinutesRemaining = interview.Interview.StartedDate.HasValue
                        ? Math.Max(0, interview.Interview.MinutesAllowed - (int)(DateTime.UtcNow - interview.Interview.StartedDate.Value).TotalMinutes)
                        : interview.Interview.MinutesAllowed,
                    LastUpdatedBy = interview.Interview.LastUpdatedBy,
                    LastUpdatedDate = interview.Interview.LastUpdatedDate.ToShortDateString(),
                    CreatedBy = interview.Interview.CreatedBy,
                    CreatedDate = interview.Interview.CreatedDate.ToShortDateString()
                }).ToArray();
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetInterviewResponse GetInterview(GetInterviewRequest request)
        {
            GetInterviewResponse response = new GetInterviewResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var interview = context.Interviews
                    .Where(q => q.ID == request.InterviewID)
                    .Select(i => new
                    {
                        Interview = i,
                        Applicant = new
                        {
                            FirstName = i.Applicant.FirstName,
                            LastName = i.Applicant.LastName
                        }
                    })
                    .FirstOrDefault();

                response.ApplicantID = interview.Interview.ApplicantID;
                response.ApplicantName = interview.Applicant.FirstName + " " + interview.Applicant.LastName;

                InterviewStatus status = StatusHelper.GetInterviewStatus(interview.Interview);

                response.Status = (short)status;
                response.StatusText = status.GetDescription();
                response.MinutesAllowed = interview.Interview.MinutesAllowed;

                if (interview.Interview.StartedDate.HasValue)
                {
                    response.TimeRemaining = TimeHelper.CalculateTimeOffset(DateTime.UtcNow, interview.Interview.StartedDate.Value.AddMinutes(interview.Interview.MinutesAllowed));
                }

                response.Questions = context.InterviewQuestions
                    .Where(i => i.InterviewID == request.InterviewID)
                    .Select(iq => new InterviewQuestionItem
                    {
                        Name = iq.Question.Name,
                        ID = iq.QuestionID,
                        InterviewQuestionID = iq.ID
                    })
                    .ToArray();

                response.LastUpdatedBy = interview.Interview.LastUpdatedBy;
                response.LastUpdatedDate = interview.Interview.LastUpdatedDate.ToShortDateString();
                response.CreatedBy = interview.Interview.CreatedBy;
                response.CreatedDate = interview.Interview.CreatedDate.ToShortDateString();
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CreateInterviewResponse CreateInterview(CreateInterviewRequest request)
        {
            CreateInterviewResponse response = new CreateInterviewResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");

                Common.Helpers.ValidationHelper.Assert(request.QuestionSetID.HasValue || (request.QuestionIDs != null && request.QuestionIDs.Any()), "No questions specified");
                Common.Helpers.ValidationHelper.Assert(request.MinutesAllowed > 0, "Minutes allowed must be positive");
                Common.Helpers.ValidationHelper.Assert(request.MinutesAllowed <= Constants.MaxInterviewDuration, "Time allocated must not be greater than two hours.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Interview newInterview = new E::Interview();

                newInterview.ID = Guid.NewGuid();
                newInterview.ApplicantID = request.ApplicantID;
                newInterview.MinutesAllowed = request.MinutesAllowed;
                newInterview.CreatedBy = authToken.Username;
                newInterview.CreatedDate = DateTime.UtcNow;
                newInterview.LastUpdatedBy = authToken.Username;
                newInterview.LastUpdatedDate = DateTime.UtcNow;

                context.Interviews.Add(newInterview);

                AddQuestions(context, newInterview.ID, request.UseQuestionSet, request.QuestionSetID, request.QuestionIDs);

                context.SaveChanges();

                response.InterviewID = newInterview.ID;
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public QuickStartResponse QuickStart(QuickStartRequest request)
        {
            QuickStartResponse response = new QuickStartResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");

                Common.Helpers.ValidationHelper.Assert(request.MinutesAllowed > 0, "Minutes allowed must be positive");
                Common.Helpers.ValidationHelper.Assert(request.MinutesAllowed <= Constants.MaxInterviewDuration, "Time allocated must not be greater than two hours.");

                Common.Helpers.ValidationHelper.ValidateEmailAddress(request.EmailAddress);

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                int numberOfQuestions = context.QuestionSetQuestions.Count(q => q.QuestionSetID == request.QuestionSetID);

                if (numberOfQuestions == 0)
                {
                    throw new Common.Exceptions.ValidationException("The question set has no questions.");
                }

                E::Applicant newApplicant = new E::Applicant();

                newApplicant.ID = Guid.NewGuid();
                newApplicant.FirstName = string.Empty;
                newApplicant.LastName = string.Empty;
                newApplicant.EmailAddress = request.EmailAddress;
                newApplicant.CreatedBy = authToken.Username;
                newApplicant.CreatedDate = DateTime.UtcNow;
                newApplicant.LastUpdatedBy = authToken.Username;
                newApplicant.LastUpdatedDate = DateTime.UtcNow;

                context.Applicants.Add(newApplicant);

                E::Interview newInterview = new E::Interview();

                newInterview.ID = Guid.NewGuid();
                newInterview.ApplicantID = newApplicant.ID;
                newInterview.MinutesAllowed = request.MinutesAllowed;
                newInterview.CreatedBy = authToken.Username;
                newInterview.CreatedDate = DateTime.UtcNow;
                newInterview.LastUpdatedBy = authToken.Username;
                newInterview.LastUpdatedDate = DateTime.UtcNow;

                if (request.SendInvitation)
                {
                    newInterview.SentDate = DateTime.UtcNow;
                }

                context.Interviews.Add(newInterview);

                AddQuestions(context, newInterview.ID, true, request.QuestionSetID, null);

                context.SaveChanges();

                if (request.SendInvitation)
                {
                    EmailController.SendInvitationEmail(newInterview.ID, authToken.Username);
                }

                response.InterviewID = newInterview.ID;
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void EditInterview(EditInterviewRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");

                Common.Helpers.ValidationHelper.Assert(request.MinutesAllowed > 0, "Minutes allowed must be positive");
                Common.Helpers.ValidationHelper.Assert(request.QuestionIDs != null, "No questions specified");
                Common.Helpers.ValidationHelper.Assert(request.QuestionIDs.Any(), "No questions specified");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Interview interview = context.Interviews.FirstOrDefault(q => q.ID == request.InterviewID);

                InterviewStatus status = StatusHelper.GetInterviewStatus(interview);

                if (status > InterviewStatus.WaitingForApplicant)
                {
                    throw new Common.Exceptions.ValidationException("Cannot edit an interview which has already started.");
                }

                E::InterviewQuestion[] interviewQuestions = context.InterviewQuestions.Where(iq => iq.InterviewID == request.InterviewID).ToArray();

                foreach (E::InterviewQuestion interviewQuestion in interviewQuestions)
                {
                    if (!request.QuestionIDs.Contains(interviewQuestion.QuestionID))
                    {
                        context.InterviewQuestions.Remove(interviewQuestion);
                    }
                }

                foreach (Guid questionId in request.QuestionIDs.Except(interviewQuestions.Select(iq => iq.QuestionID)))
                {
                    E::InterviewQuestion newInterviewQuestion = new E.InterviewQuestion();
                    newInterviewQuestion.ID = Guid.NewGuid();
                    newInterviewQuestion.InterviewID = request.InterviewID;
                    newInterviewQuestion.QuestionID = questionId;

                    context.InterviewQuestions.Add(newInterviewQuestion);
                }

                interview.MinutesAllowed = request.MinutesAllowed;
                interview.LastUpdatedBy = authToken.Username;
                interview.LastUpdatedDate = DateTime.UtcNow;

                context.SaveChanges();
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteInterview(DeleteInterviewRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Interview interview = context.Interviews
                    .Where(i => i.ID == request.InterviewID)
                    .FirstOrDefault();

                context.Interviews.Remove(interview);

                var attempts = context.Attempts
                    .Where(a => a.InterviewQuestion.InterviewID == request.InterviewID)
                    .Select(t => new
                    {
                        Code = t.Code,
                        Output = t.Output
                    });

                foreach (var attempt in attempts.AsEnumerable())
                {
                    DataController.DeleteBlob(attempt.Code.ToString());
                    DataController.DeleteBlob(attempt.Output.ToString());
                }

                context.SaveChanges();
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public SendInvitationResponse SendInvitation(SendInvitationRequest request)
        {
            SendInvitationResponse response = new SendInvitationResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Interview interview = context.Interviews
                    .Where(i => i.ID == request.InterviewID)
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.Assert(interview != null, "Interview does not exist.");
                Common.Helpers.ValidationHelper.AssertFalse(interview.StartedDate.HasValue, "Interview has already started.");

                bool isNewInvitation = !interview.SentDate.HasValue;

                interview.SentDate = DateTime.UtcNow;

                context.SaveChanges();

                EmailController.SendInvitationEmail(request.InterviewID, authToken.Username);
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetAttemptsResponse GetQuestionAttempts(GetAttemptsRequest request)
        {
            GetAttemptsResponse response = new GetAttemptsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var interview = context.Interviews
                    .Where(i => i.ID == request.InterviewID)
                    .Select(i => new { StartedDate = i.StartedDate })
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.Assert(interview != null, "Interview does not exist.");

                if (interview.StartedDate.HasValue)
                {
                    var attempts =
                        from attempt in context.Attempts
                        where attempt.InterviewQuestionID == request.InterviewQuestionID
                        select new
                        {
                            AttemptID = attempt.ID,
                            AttemptCreatedDate = attempt.CreatedDate,
                        };

                    response.Attempts =
                        (from attempt in attempts.AsEnumerable()
                         orderby attempt.AttemptCreatedDate descending
                         select new GetAttemptsResponseItem
                         {
                             AttemptID = attempt.AttemptID,
                             TimeOffset = TimeHelper.CalculateTimeOffset(interview.StartedDate.Value, attempt.AttemptCreatedDate)
                         }).ToArray();
                }
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetAttemptDetailsResponse GetAttemptDetails(GetAttemptDetailsRequest request)
        {
            GetAttemptDetailsResponse response = new GetAttemptDetailsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var attempt = context.Attempts
                    .Where(a => a.ID == request.AttemptID)
                    .Select(a => new
                    {
                        Code = a.Code,
                        Output = a.Output,
                        QuestionTypeID = a.InterviewQuestion.Question.QuestionTypeID
                    })
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.Assert(attempt != null, "Attempt does not exist.");

                response.Code = DataController.DownloadBlob(attempt.Code.ToString());
                response.Output = DataController.DownloadBlob(attempt.Output.ToString());

                var results = context.TestResults
                    .Where(a => a.AttemptID == request.AttemptID)
                    .Select(a => new
                    {
                        TestName = a.TestName,
                        Success = a.Passed,
                        InputString = a.InputString,
                        ExpectedOutputString = a.ExpectedOutputString,
                        OutputString = a.OutputString
                    });

                response.Results = results.AsEnumerable()
                    .Select(r =>
                    {
                        TestResultSummary summary = new TestResultSummary();

                        summary.TestName = r.TestName;
                        summary.Input = r.InputString;
                        summary.ExpectedOutput = r.ExpectedOutputString;
                        summary.Output = r.OutputString;
                        summary.Success = r.Success;

                        return summary;
                    })
                    .ToArray();
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, authToken == null ? null : authToken.Username);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public LoadInterviewResponse LoadInterview(LoadInterviewRequest request)
        {
            LoadInterviewResponse response = new LoadInterviewResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");

                InterviewToken token = InterviewToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                DbContext context = DataController.CreateDbContext();

                E::Interview interview = context.Interviews
                    .Where(i => i.ID == token.InterviewID)
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.AssertFalse(interview == null, "Invalid token");

                InterviewStatus status = StatusHelper.GetInterviewStatus(interview);

                response.StatusID = (short)status;

                response.MinutesAllowed = interview.MinutesAllowed;

                response.NumberOfQuestions = context.InterviewQuestions
                    .Where(i => i.InterviewID == token.InterviewID)
                    .Count();
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public StartInterviewResponse StartInterview(StartInterviewRequest request)
        {
            StartInterviewResponse response = new StartInterviewResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");

                InterviewToken token = InterviewToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                DbContext context = DataController.CreateDbContext();

                E::Interview interview = context.Interviews
                    .Where(i => i.ID == token.InterviewID)
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.Assert(interview != null, "Invalid token");

                InterviewStatus status = StatusHelper.GetInterviewStatus(interview);

                Common.Helpers.ValidationHelper.Assert(status == InterviewStatus.WaitingForApplicant || status == InterviewStatus.InProgress, "Invalid state");

                int secondsRemaining = TimeHelper.CalculateSecondsRemaining(interview);

                Common.Helpers.ValidationHelper.Assert(secondsRemaining > -10, "Interview has already been completed.");

                response.SecondsRemaining = Math.Max(0, secondsRemaining);

                var questions = context.InterviewQuestions
                    .Where(i => i.InterviewID == token.InterviewID)
                    .Select(i => new
                    {
                        ID = i.ID,
                        Name = i.Question.Name,
                        QuestionBody = i.Question.QuestionBody,
                        QuestionTypeID = i.Question.QuestionTypeID
                    })
                    .ToList();

                var attempts = context.Attempts
                    .Where(a => a.InterviewQuestion.InterviewID == token.InterviewID)
                    .Select(a => new
                    {
                        InterviewQuestionID = a.InterviewQuestionID
                    })
                    .ToList();

                IList<InterviewQuestion> interviewQuestions = new List<InterviewQuestion>();

                foreach (var question in questions)
                {
                    string questionBody = DataController.DownloadBlob(question.QuestionBody.ToString());

                    InterviewQuestion newInterviewQuestion = new InterviewQuestion();

                    newInterviewQuestion.ID = question.ID;
                    newInterviewQuestion.Name = question.Name;
                    newInterviewQuestion.QuestionBody = questionBody;
                    newInterviewQuestion.Submitted = attempts.Any(a => a.InterviewQuestionID == question.ID);

                    interviewQuestions.Add(newInterviewQuestion);
                }

                response.Questions = interviewQuestions.ToArray();

                if (status == InterviewStatus.WaitingForApplicant)
                {
                    interview.StartedDate = DateTime.UtcNow;

                    context.SaveChanges();

                    try
                    {
                        SendInterviewCompletedEmail(token, context, interview);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.Log(ex, interview.CreatedBy);
                    }
                }
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        private static void SendInterviewCompletedEmail(InterviewToken token, DbContext context, E::Interview interview)
        {
            var applicant = context.Applicants
                .Where(a => a.ID == interview.ApplicantID)
                .Select(a => new { FirstName = a.FirstName, LastName = a.LastName, EmailAddress = a.EmailAddress })
                .FirstOrDefault();

            var applicantName = string.IsNullOrEmpty(applicant.FirstName) && string.IsNullOrEmpty(applicant.LastName) ? applicant.EmailAddress : applicant.FirstName + " " + applicant.LastName;

            string recipientName, recipientEmail;

            using (var root = DataController.CreateDbContext())
            {
                var recipient = root.Users
                    .Where(a => a.ID == interview.CreatedBy)
                    .Select(a => new { FirstName = a.FirstName, LastName = a.LastName, EmailAddress = a.EmailAddress })
                    .FirstOrDefault();

                recipientName = string.Format("{0} {1}", recipient.FirstName, recipient.LastName);
                recipientEmail = recipient.EmailAddress;
            }

            EmailController.SendInterviewCompletedEmail(recipientName, applicantName, interview.ID, recipientEmail, TimeSpan.FromMinutes(interview.MinutesAllowed));
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public CreateAttemptTokenResponse CreateAttemptToken(CreateAttemptTokenRequest request)
        {
            CreateAttemptTokenResponse response = new CreateAttemptTokenResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");

                InterviewToken token = InterviewToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                DbContext context = DataController.CreateDbContext();

                long randomizer = Common.Helpers.RandomHelper.RandomLong();

                Guid mostRecentAttemptId = context.Attempts
                    .Where(a => a.InterviewQuestionID == request.QuestionID)
                    .OrderByDescending(a => a.CreatedDate)
                    .Select(a => a.ID)
                    .FirstOrDefault();

                AttemptToken attemptToken = new AttemptToken(request.QuestionID, randomizer, mostRecentAttemptId);
                response.AttemptToken = Convert.ToBase64String(EncryptionHelper.EncryptToken(attemptToken.AsBytes()));
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public DownloadInputResponse DownloadInput(DownloadInputRequest request)
        {
            DownloadInputResponse response = new DownloadInputResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");

                InterviewToken token = InterviewToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                DbContext context = DataController.CreateDbContext();

                var interviewQuestion = context.InterviewQuestions
                    .Where(i => i.ID == request.QuestionID)
                    .Select(t => new
                    {
                        QuestionID = t.QuestionID,
                        QuestionTypeID = t.Question.QuestionTypeID,
                        CodedTestID = t.Question.CodedTestID,
                        TestsID = t.Question.Tests
                    })
                    .FirstOrDefault();

                var attemptToken = AttemptToken.FromBytes(EncryptionHelper.DecryptToken(Convert.FromBase64String(request.AttemptToken)));

                ValidateAttemptToken(context, request.QuestionID, attemptToken, token);

                if (interviewQuestion.QuestionTypeID == (short)QuestionType.Standard)
                {
                    var testsJson = DataController.DownloadBlob(interviewQuestion.TestsID.Value.ToString());
                    var tests = TestsController.FromJson(testsJson).OrderBy(t => t.ID);

                    var randomTests = tests.Shuffle(attemptToken.Random).Take(Settings.Default.TestsExecutedPerQuestion).ToList();

                    var selectedTests =
                        from test in randomTests
                        orderby test.Name
                        select test.Input;

                    response.Input = string.Join("\n", selectedTests);
                }
                else
                {
                    response.Input = CodedQuestionController.GetInput(interviewQuestion.CodedTestID.Value, attemptToken.Random);
                }
            }
            catch (AuthenticationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        private static void ValidateAttemptToken(DbContext context, Guid interviewQuestionId, AttemptToken attemptToken, InterviewToken interviewToken)
        {
            if (attemptToken.InterviewQuestionID != interviewQuestionId)
            {
                throw new Common.Exceptions.ValidationException("Please download the input file again.");
            }

            Guid mostRecentAttemptId = context.Attempts
                .Where(a => a.InterviewQuestionID == interviewQuestionId)
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => a.ID)
                .FirstOrDefault();

            if (attemptToken.MostRecentAttemptID != mostRecentAttemptId)
            {
                throw new Common.Exceptions.ValidationException("Please download the input file again.");
            }
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public PracticeResponse Practice(PracticeRequest request)
        {
            PracticeResponse response = new PracticeResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Output, "Output");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Code, "Code");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Output, "Output", Constants.MaxBlobLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Code, "Code", Constants.MaxBlobLength);

                IEnumerable<TestResult> testResults = PracticeController.ValidateOutput(request.Output);

                IList<RunTestsResult> runTestsResults = new List<RunTestsResult>();

                foreach (TestResult result in testResults)
                {
                    runTestsResults.Add(new RunTestsResult
                    {
                        Success = result.Success,
                        TestName = result.TestName,
                        Input = result.Input.Truncate(100),
                        ExpectedOutput = result.ExpectedOutput.Truncate(100),
                        Output = result.Output.Truncate(100)
                    });
                }

                response.TestResults = runTestsResults.ToArray();
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public SubmitAnswerResponse SubmitAnswer(SubmitAnswerRequest request)
        {
            SubmitAnswerResponse response = new SubmitAnswerResponse();

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Token, "Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Output, "Output");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Code, "Code");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Output, "Output", Constants.MaxBlobLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.Code, "Code", Constants.MaxBlobLength);

                InterviewToken token = InterviewToken.FromBytes(EncryptionHelper.DecryptURL(Convert.FromBase64String(request.Token)));

                DbContext context = DataController.CreateDbContext();

                E::Interview interview = context.Interviews
                    .Where(i => i.ID == token.InterviewID)
                    .FirstOrDefault();

                Common.Helpers.ValidationHelper.Assert(interview != null, "Invalid token");
                Common.Helpers.ValidationHelper.Assert(interview.StartedDate.HasValue, "Invalid state");

                int secondsRemaining = TimeHelper.CalculateSecondsRemaining(interview);

                Common.Helpers.ValidationHelper.Assert(secondsRemaining > -10, "Interview has already been completed.");

                var interviewQuestion = context.InterviewQuestions
                    .Where(i => i.ID == request.ID)
                    .FirstOrDefault();

                if (interviewQuestion == null || interviewQuestion.InterviewID != token.InterviewID)
                {
                    throw new Common.Exceptions.ValidationException("Invalid token");
                }

                if (interviewQuestion.LastTestRunOn.HasValue && DateTime.UtcNow - interviewQuestion.LastTestRunOn.Value < TimeSpan.FromSeconds(5))
                {
                    throw new Common.Exceptions.ValidationException("Please wait at least 5 seconds between test runs.");
                }

                var question = context.Questions
                    .Where(q => q.ID == interviewQuestion.QuestionID)
                    .Select(q => new
                    {
                        CodedTestID = q.CodedTestID,
                        QuestionTypeID = q.QuestionTypeID,
                        Tests = q.Tests
                    })
                    .FirstOrDefault();

                AttemptToken attemptToken = AttemptToken.FromBytes(EncryptionHelper.DecryptToken(Convert.FromBase64String(request.AttemptToken)));

                ValidateAttemptToken(context, request.ID, attemptToken, token);

                E::Attempt attempt = CreateAttempt(request.ID, request.Code, request.Output, attemptToken);

                context.Attempts.Add(attempt);

                interviewQuestion.LastTestRunOn = DateTime.UtcNow;

                IEnumerable<TestResult> testResults;

                if (question.QuestionTypeID == (short)QuestionType.Coded)
                {
                    testResults = CodedQuestionController.RunTests(question.CodedTestID.Value, request.Output, attemptToken.Random);
                }
                else
                {
                    var testsJson = DataController.DownloadBlob(question.Tests.Value.ToString());
                    var tests = TestsController.FromJson(testsJson).OrderBy(t => t.ID);

                    var randomTests = tests.Shuffle(attemptToken.Random).Take(Settings.Default.TestsExecutedPerQuestion).ToList();

                    var parsedOutput = request.Output.Lines();

                    if (randomTests.Count > parsedOutput.Length)
                    {
                        throw new Common.Exceptions.ValidationException(string.Format("Invalid output. Expected {0} lines, found {1}.", randomTests.Count, parsedOutput.Length));
                    }

                    testResults =
                        (from pair in randomTests.OrderBy(t => t.Name).AsEnumerable().Zip(parsedOutput, Tuple.Create)
                         select new TestResult
                         {
                             TestID = pair.Item1.ID,
                             TestName = pair.Item1.Name,
                             Success = pair.Item1.ExpectedOutput.EqualsLenient(pair.Item2),
                             Input = pair.Item1.Input,
                             ExpectedOutput = pair.Item1.ExpectedOutput,
                             Output = pair.Item2
                         })
                         .ToArray();
                }

                IList<RunTestsResult> runTestsResults = new List<RunTestsResult>();

                foreach (TestResult result in testResults)
                {
                    E::TestResult testResult = CreateTestResult(attempt.ID, result.TestID, result);

                    context.TestResults.Add(testResult);

                    runTestsResults.Add(new RunTestsResult
                    {
                        Success = result.Success,
                        TestName = result.TestName,
                        Input = result.Input.Truncate(100),
                        ExpectedOutput = result.ExpectedOutput.Truncate(100),
                        Output = result.Output.Truncate(100)
                    });
                }

                response.TestResults = runTestsResults.ToArray();

                context.SaveChanges();

                // Set the LastAttemptID only when we run tests, so that we have something to show in the results page
                interviewQuestion.LastAttemptID = attempt.ID;

                context.SaveChanges();
            }
            catch (Common.Exceptions.ValidationException ex)
            {
                throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                ExceptionHelper.Log(ex, null);
                throw new WebFaultException<string>("An unknown error has occurred.", System.Net.HttpStatusCode.InternalServerError);
            }

            return response;
        }

        private static E::TestResult CreateTestResult(Guid attemptId, Guid? testId, BusinessLogic.Classes.Services.TestResult result)
        {
            E::TestResult testResult = new E::TestResult();

            testResult.ID = Guid.NewGuid();
            testResult.AttemptID = attemptId;
            testResult.TestName = result.TestName;
            testResult.OutputString = result.Output.Truncate(100);
            testResult.ExpectedOutputString = result.ExpectedOutput.Truncate(100);
            testResult.InputString = result.Input.Truncate(100);
            testResult.Passed = result.Success;
            testResult.CreatedBy = string.Empty;
            testResult.LastUpdatedBy = string.Empty;
            testResult.CreatedDate = DateTime.UtcNow;
            testResult.LastUpdatedDate = DateTime.UtcNow;

            return testResult;
        }

        private static E::Attempt CreateAttempt(Guid interviewQuestionId, string code, string output, AttemptToken attemptToken)
        {
            E::Attempt attempt = new E::Attempt();

            attempt.ID = Guid.NewGuid();
            attempt.InterviewQuestionID = interviewQuestionId;
            attempt.Code = Guid.NewGuid();
            attempt.Output = Guid.NewGuid();
            attempt.CreatedBy = string.Empty;
            attempt.LastUpdatedBy = string.Empty;
            attempt.CreatedDate = DateTime.UtcNow;
            attempt.LastUpdatedDate = DateTime.UtcNow;
            attempt.Randomizer = attemptToken.Random;

            DataController.UploadBlob(attempt.Code.ToString(), code);
            DataController.UploadBlob(attempt.Output.ToString(), output);

            return attempt;
        }

        private static void AddQuestions(DbContext context, Guid interviewId, bool useQuestionSet, Guid? questionSetId, Guid[] questionIds)
        {
            if (useQuestionSet)
            {
                questionIds = context.QuestionSetQuestions
                    .Where(q => q.QuestionSetID == questionSetId.Value)
                    .Select(q => q.QuestionID)
                    .ToArray();
            }

            foreach (Guid questionId in questionIds)
            {
                E::InterviewQuestion newInterviewQuestion = new E.InterviewQuestion();

                newInterviewQuestion.ID = Guid.NewGuid();
                newInterviewQuestion.InterviewID = interviewId;
                newInterviewQuestion.QuestionID = questionId;

                context.InterviewQuestions.Add(newInterviewQuestion);
            }
        }
    }
}
