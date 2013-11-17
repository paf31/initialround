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
using InitialRound.BusinessLogic.Classes.Services;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.Common.Extensions;
using InitialRound.BusinessLogic.Helpers;
using InitialRound.Models.Contexts;
using InitialRound.Web.Classes.QuestionService;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.Web.Classes.AccountService;
using InitialRound.BusinessLogic;
using InitialRound.BusinessLogic.Enums;
using Microsoft.WindowsAzure.ServiceRuntime;
using InitialRound.BusinessLogic.Properties;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QuestionService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetQuestionsResponse GetQuestions(GetQuestionsRequest request)
        {
            GetQuestionsResponse response = new GetQuestionsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                IQueryable<E::Question> questions = context.Questions;

                if (!string.IsNullOrEmpty(request.Name))
                {
                    questions = questions.Where(q => q.Name.Contains(request.Name));
                }

                response.TotalCount = questions.Count();

                questions = questions.OrderByDescending(a => a.CreatedDate)
                    .Skip(request.StartAt)
                    .Take(InitialRound.Web.Properties.Settings.Default.PageSize);

                response.Results = questions.AsEnumerable().Select(a => new GetQuestionsResponseItem
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
        public GetQuestionResponse GetQuestion(GetQuestionRequest request)
        {
            GetQuestionResponse response = new GetQuestionResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                E::Question question = context.Questions
                    .Where(q => q.ID == request.QuestionID)
                    .FirstOrDefault();

                response.QuestionBody = DataController.DownloadBlob(question.QuestionBody.ToString());

                response.Name = question.Name;
                response.LastUpdatedBy = question.LastUpdatedBy;
                response.LastUpdatedDate = question.LastUpdatedDate.ToShortDateString();
                response.CreatedBy = question.CreatedBy;
                response.CreatedDate = question.CreatedDate.ToShortDateString();

                response.CanEdit = !context.InterviewQuestions
                    .Where(q => q.QuestionID == request.QuestionID)
                    .Where(q => q.Interview.StartedDate.HasValue)
                    .Any();

                response.IsCodedTest = question.QuestionTypeID == (short)QuestionType.Coded;

                if (!response.IsCodedTest)
                {
                    var tests = TestsController.FromJson(DataController.DownloadBlob(question.Tests.Value.ToString()));

                    IList<QuestionTest> questionTests = new List<QuestionTest>();

                    foreach (var test in tests.AsEnumerable())
                    {
                        QuestionTest questionTest = new QuestionTest();

                        questionTest.ID = test.ID;
                        questionTest.Name = test.Name;
                        questionTest.Input = test.Input;
                        questionTest.ExpectedOutput = test.ExpectedOutput;

                        questionTests.Add(questionTest);
                    }

                    response.Tests = questionTests.ToArray();
                }
                else
                {
                    response.Tests = new QuestionTest[0];
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
        public CreateQuestionResponse CreateQuestion(CreateQuestionRequest request)
        {
            CreateQuestionResponse response = new CreateQuestionResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Name, "Name");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Name, "Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.QuestionBody, "Hidden Code", Constants.MaxBlobLength);

                foreach (QuestionTest test in request.Tests)
                {
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.Name, "Test Name");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.Input, "Test Input");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.ExpectedOutput, "Test Output");

                    Common.Helpers.ValidationHelper.ValidateStringLength(test.Name, "Test Name", Constants.MaxNameLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(test.Input, "Test Input", Constants.MaxBlobLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(test.ExpectedOutput, "Test Expected Output", Constants.MaxBlobLength);
                }

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Question newQuestion = new E::Question();

                newQuestion.ID = Guid.NewGuid();
                newQuestion.Name = request.Name;
                newQuestion.QuestionTypeID = (short)QuestionType.Standard;
                newQuestion.QuestionBody = Guid.NewGuid();
                newQuestion.Tests = Guid.NewGuid();
                newQuestion.CreatedBy = authToken.Username;
                newQuestion.CreatedDate = DateTime.UtcNow;
                newQuestion.LastUpdatedBy = authToken.Username;
                newQuestion.LastUpdatedDate = DateTime.UtcNow;

                DataController.UploadBlob(newQuestion.QuestionBody.ToString(), request.QuestionBody);

                IList<TestsController.Test> tests = new List<TestsController.Test>();
                
                foreach (QuestionTest test in request.Tests)
                {
                    TestsController.Test newTest = new TestsController.Test();

                    newTest.ID = Guid.NewGuid();
                    newTest.Name = test.Name;
                    newTest.Input = test.Input;
                    newTest.ExpectedOutput =  test.ExpectedOutput;

                    newTest.ID = Guid.NewGuid();

                    tests.Add(newTest);
                }

                response.TestIDs = tests.Select(t => t.ID).ToArray();

                DataController.UploadBlob(newQuestion.Tests.Value.ToString(), TestsController.ToJson(tests.ToArray()));

                context.Questions.Add(newQuestion);

                context.SaveChanges();

                response.QuestionID = newQuestion.ID;
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
        public EditQuestionResponse EditQuestion(EditQuestionRequest request)
        {
            EditQuestionResponse response = new EditQuestionResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Name, "Name");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Name, "Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.QuestionBody, "Problem Description", Constants.MaxBlobLength);

                foreach (QuestionTest test in request.Tests)
                {
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.Name, "Test Name");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.Input, "Test Input");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(test.ExpectedOutput, "Test Output");

                    Common.Helpers.ValidationHelper.ValidateStringLength(test.Name, "Test Name", Constants.MaxNameLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(test.Input, "Test Input", Constants.MaxBlobLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(test.ExpectedOutput, "Test Output", Constants.MaxBlobLength);
                }

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                Common.Helpers.ValidationHelper.AssertFalse(context.InterviewQuestions
                    .Where(iq => iq.QuestionID == request.QuestionID)
                    .Where(iq => iq.Interview.StartedDate.HasValue)
                    .Any(),
                    "This question cannot be edited as it has been used in existing interviews.");

                if (string.IsNullOrEmpty(request.Name))
                {
                    throw new Common.Exceptions.ValidationException("Please fill in the name field.");
                }

                E::Question question = context.Questions
                    .Where(q => q.ID == request.QuestionID)
                    .FirstOrDefault();

                question.Name = request.Name;
                question.LastUpdatedBy = authToken.Username;
                question.LastUpdatedDate = DateTime.UtcNow;

                DataController.UploadBlob(question.QuestionBody.ToString(), request.QuestionBody);
               
                if (question.QuestionTypeID == (short)QuestionType.Standard)
                {
                    var tests = TestsController.FromJson(DataController.DownloadBlob(question.Tests.Value.ToString()));

                    IList<TestsController.Test> newTests = new List<TestsController.Test>();

                    foreach (QuestionTest test in request.Tests)
                    {
                        TestsController.Test newTest = new TestsController.Test();

                        newTest.ID = Guid.NewGuid();
                        newTest.Name = test.Name;
                        newTest.Input = test.Input;
                        newTest.ExpectedOutput = test.ExpectedOutput;

                        newTest.ID = Guid.NewGuid();

                        newTests.Add(newTest);
                    }

                    response.TestIDs = tests.Select(t => t.ID).ToArray();
                    
                    DataController.UploadBlob(question.Tests.Value.ToString(), TestsController.ToJson(newTests.ToArray()));
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

            return response;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void DeleteQuestion(DeleteQuestionRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                Common.Helpers.ValidationHelper.AssertFalse(context.InterviewQuestions
                    .Where(iq => iq.QuestionID == request.QuestionID)
                    .Any(),
                    "This question cannot be deleted as it has been used in existing interviews. Please delete those interviews first and then attempt to delete the question again.");

                E::Question question = context.Questions
                    .Where(q => q.ID == request.QuestionID)
                    .FirstOrDefault();

                if (question.Tests.HasValue)
                {
                    DataController.DeleteBlob(question.Tests.Value.ToString());
                }

                context.Questions.Remove(question);

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
        public DownloadInputResponse DownloadInput(DownloadInputRequest request)
        {
            DownloadInputResponse response = new DownloadInputResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (string.IsNullOrEmpty(request.AuthToken) ||
                    !UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var question = context.Questions
                    .Where(q => q.ID == request.QuestionID)
                    .Select(q => new
                    {
                        QuestionTypeID = q.QuestionTypeID,
                        CodedTestID = q.CodedTestID,
                        TestsID = q.Tests
                    })
                    .FirstOrDefault();

                if (question.QuestionTypeID == (short)QuestionType.Standard)
                {
                    var testsJson = DataController.DownloadBlob(question.TestsID.Value.ToString());
                    var tests = TestsController.FromJson(testsJson).OrderBy(t => t.ID);

                    int numberOfTests = Settings.Default.TestsExecutedPerQuestion;
                    var randomTests = tests.Shuffle(0).Take(numberOfTests).ToList();

                    var selectedTests =
                        from test in randomTests
                        orderby test.Name
                        select test.Input;

                    response.Input = string.Join("\n", selectedTests);
                }
                else
                {
                    response.Input = CodedQuestionController.GetInput(question.CodedTestID.Value, 0);
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
        public RunTestsResponse RunTests(RunTestsRequest request)
        {
            RunTestsResponse response = new RunTestsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.Output, "Output");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.Output, "Output", Constants.MaxBlobLength);

                if (string.IsNullOrEmpty(request.AuthToken) ||
                    !UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Question question = context.Questions
                    .Where(q => q.ID == request.QuestionID)
                    .FirstOrDefault();

                if (question.LastTestRunOn.HasValue && DateTime.UtcNow - question.LastTestRunOn.Value < TimeSpan.FromSeconds(5))
                {
                    throw new Common.Exceptions.ValidationException("Please wait at least 5 seconds between test runs.");
                }

                question.LastTestRunOn = DateTime.UtcNow;

                if (question.QuestionTypeID == (short)QuestionType.Standard)
                {
                    var testsJson = DataController.DownloadBlob(question.Tests.Value.ToString());
                    var tests = TestsController.FromJson(testsJson).OrderBy(t => t.ID);

                    int numberOfTests = Settings.Default.TestsExecutedPerQuestion;
                    var randomTests = tests.Shuffle(0).Take(numberOfTests).ToList();

                    var parsedOutput = request.Output.Lines();

                    if (randomTests.Count > parsedOutput.Length)
                    {
                        throw new Common.Exceptions.ValidationException(string.Format("Invalid output. Expected {0} lines, found {1}.", randomTests.Count, parsedOutput.Length));
                    }
                    else
                    {
                        response.Tests =
                            (from pair in randomTests.OrderBy(t => t.Name).Zip(parsedOutput, Tuple.Create)
                             select new RunTestsResponseItem
                             {
                                 TestName = pair.Item1.Name,
                                 TestID = pair.Item1.ID,
                                 Success = pair.Item1.ExpectedOutput.EqualsLenient(pair.Item2),
                                 Input = pair.Item1.Input.Truncate(100),
                                 ExpectedOutput = pair.Item1.ExpectedOutput.Truncate(100),
                                 Output = pair.Item2.Truncate(100)
                             })
                            .ToArray();
                    }
                }
                else
                {
                    response.Tests = CodedQuestionController.RunTests(question.CodedTestID.Value, request.Output, 0)
                        .Select(test => new RunTestsResponseItem
                        {
                            TestName = test.TestName,
                            Success = test.Success,
                            Input = test.Input.Truncate(100),
                            ExpectedOutput = test.ExpectedOutput.Truncate(100),
                            Output = test.Output.Truncate(100)
                        })
                        .OrderBy(test => test.TestName)
                        .ToArray();
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
        public ParseTestsCSVResponse ParseTestsCSV(ParseTestsCSVRequest request)
        {
            ParseTestsCSVResponse response = new ParseTestsCSVResponse();

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

                int rowNumber = 1;

                IList<CSVTest> tests = new List<CSVTest>();

                foreach (var row in Common.Helpers.CSVReader.Read(request.CSV))
                {
                    if (row.Length != 3)
                    {
                        throw new Common.Exceptions.ValidationException("Invalid number of columns in row " + rowNumber);
                    }

                    Common.Helpers.ValidationHelper.ValidateStringLength(row[0], "Name", Constants.MaxNameLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(row[1], "Input", Constants.MaxBlobLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(row[2], "Expected Output", Constants.MaxBlobLength);

                    tests.Add(new CSVTest
                    {
                        Name = row[0],
                        Input = row[1],
                        ExpectedOutput = row[2]
                    });

                    rowNumber++;
                }

                response.Tests = tests.ToArray();
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
    }
}
