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
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.Models.Contexts;
using InitialRound.Web.Classes.QuestionSetService;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.Web.Classes.AccountService;
using InitialRound.BusinessLogic;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionSetService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetQuestionSetsResponse GetQuestionSets(GetQuestionSetsRequest request)
        {
            GetQuestionSetsResponse response = new GetQuestionSetsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                IQueryable<E::QuestionSet> QuestionSets = context.QuestionSets;

                if (!string.IsNullOrEmpty(request.Name))
                {
                    QuestionSets = QuestionSets.Where(q => q.Name.Contains(request.Name));
                }

                response.TotalCount = QuestionSets.Count();

                QuestionSets = QuestionSets.OrderByDescending(a => a.CreatedDate)
                    .Skip(request.StartAt)
                    .Take(InitialRound.Web.Properties.Settings.Default.PageSize);

                response.Results = QuestionSets.AsEnumerable().Select(a => new GetQuestionSetsResponseItem
                {
                    ID = a.ID,
                    Name = a.Name,
                    LastUpdatedBy = a.LastUpdatedBy,
                    LastUpdatedDate = a.LastUpdatedDate.ToShortDateString(),
                    CreatedBy = a.CreatedBy,
                    CreatedDate = a.CreatedDate.ToShortDateString()
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
        public GetQuestionSetResponse GetQuestionSet(GetQuestionSetRequest request)
        {
            GetQuestionSetResponse response = new GetQuestionSetResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                E::QuestionSet questionSet = context.QuestionSets
                    .Where(q => q.ID == request.QuestionSetID)
                    .FirstOrDefault();

                response.Name = questionSet.Name;
                response.Questions =
                    (from questionSetQuestion in context.QuestionSetQuestions
                     where questionSetQuestion.QuestionSetID == request.QuestionSetID
                     join question in context.Questions on questionSetQuestion.QuestionID equals question.ID
                     select new QuestionSetQuestionItem { Name = question.Name, ID = question.ID }).ToArray();
                response.LastUpdatedBy = questionSet.LastUpdatedBy;
                response.LastUpdatedDate = questionSet.LastUpdatedDate.ToShortDateString();
                response.CreatedBy = questionSet.CreatedBy;
                response.CreatedDate = questionSet.CreatedDate.ToShortDateString();
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
        public CreateQuestionSetResponse CreateQuestionSet(CreateQuestionSetRequest request)
        {
            CreateQuestionSetResponse response = new CreateQuestionSetResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Name, "Name");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Name, "Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.Assert(request.QuestionIDs.Any(), "No questions selected.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::QuestionSet newQuestionSet = new E::QuestionSet();

                newQuestionSet.ID = Guid.NewGuid();
                newQuestionSet.Name = request.Name;
                newQuestionSet.CreatedBy = authToken.Username;
                newQuestionSet.CreatedDate = DateTime.UtcNow;
                newQuestionSet.LastUpdatedBy = authToken.Username;
                newQuestionSet.LastUpdatedDate = DateTime.UtcNow;

                context.QuestionSets.Add(newQuestionSet);

                foreach (Guid questionId in request.QuestionIDs)
                {
                    E::QuestionSetQuestion newQuestionSetQuestion = new E.QuestionSetQuestion();
                    newQuestionSetQuestion.QuestionSetID = newQuestionSet.ID;
                    newQuestionSetQuestion.QuestionID = questionId;

                    context.QuestionSetQuestions.Add(newQuestionSetQuestion);
                }

                context.SaveChanges();

                response.QuestionSetID = newQuestionSet.ID;
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
        public void EditQuestionSet(EditQuestionSetRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Name, "Name");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Name, "Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.Assert(request.QuestionIDs.Any(), "No questions selected.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                if (string.IsNullOrEmpty(request.Name))
                {
                    throw new Common.Exceptions.ValidationException("Please fill in the name field.");
                }

                E::QuestionSet questionSet = context.QuestionSets.FirstOrDefault(q => q.ID == request.QuestionSetID);

                E::QuestionSetQuestion[] questionSetQuestions = context.QuestionSetQuestions.Where(iq => iq.QuestionSetID == request.QuestionSetID).ToArray();

                foreach (E::QuestionSetQuestion questionSetQuestion in questionSetQuestions)
                {
                    if (!request.QuestionIDs.Contains(questionSetQuestion.QuestionID))
                    {
                        context.QuestionSetQuestions.Remove(questionSetQuestion);
                    }
                }

                foreach (Guid questionId in request.QuestionIDs.Except(questionSetQuestions.Select(iq => iq.QuestionID)))
                {
                    E::QuestionSetQuestion newQuestionSetQuestion = new E.QuestionSetQuestion();
                    newQuestionSetQuestion.QuestionSetID = request.QuestionSetID;
                    newQuestionSetQuestion.QuestionID = questionId;

                    context.QuestionSetQuestions.Add(newQuestionSetQuestion);
                }

                questionSet.Name = request.Name;
                questionSet.LastUpdatedBy = authToken.Username;
                questionSet.LastUpdatedDate = DateTime.UtcNow;

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
        public void DeleteQuestionSet(DeleteQuestionSetRequest request)
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

                SqlConnection connection = DataController.CreateSqlConnection();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM [dbo].[QuestionSet] WHERE [ID] = @QuestionSetID";
                    command.Parameters.AddWithValue("QuestionSetID", request.QuestionSetID);

                    if (command.ExecuteNonQuery() == 0)
                    {
                        throw new Common.Exceptions.ValidationException("No rows affected.");
                    }
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
        }
    }
}
