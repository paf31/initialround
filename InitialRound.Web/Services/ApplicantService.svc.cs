using System;
using System.Collections.Generic;
using System.Data.Common;
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
using InitialRound.Web.Classes.ApplicantService;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ApplicantService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public GetApplicantsResponse GetApplicants(GetApplicantsRequest request)
        {
            GetApplicantsResponse response = new GetApplicantsResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                     
                Common.Helpers.ValidationHelper.ValidateStringLength(request.EmailAddress, "Email Address", Constants.MaxEmailAddressLength);
                Common.Helpers.ValidationHelper.Assert(request.StartAt < 1E4, "StartAt is too large.");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                IQueryable<E::Applicant> applicants = context.Applicants;

                if (!string.IsNullOrEmpty(request.Name))
                {
                    applicants = applicants.Where(a => (a.FirstName + " " + a.LastName).Contains(request.Name));
                }

                if (!string.IsNullOrEmpty(request.EmailAddress))
                {
                    applicants = applicants.Where(a => a.EmailAddress.Contains(request.EmailAddress));
                }

                response.TotalCount = applicants.Count();

                applicants = applicants.OrderByDescending(a => a.CreatedDate)
                    .Skip(request.StartAt)
                    .Take(InitialRound.Web.Properties.Settings.Default.PageSize);

                response.Results = applicants.AsEnumerable().Select(a => new GetApplicantsResponseItem
                {
                    ID = a.ID,
                    Name = string.IsNullOrEmpty(a.FirstName) && string.IsNullOrEmpty(a.LastName) ? "No Name" : a.FirstName + " " + a.LastName,
                    EmailAddress = a.EmailAddress,
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
        public GetApplicantResponse GetApplicant(GetApplicantRequest request)
        {
            GetApplicantResponse response = new GetApplicantResponse();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var applicant = context.Applicants
                    .Where(q => q.ID == request.ApplicantID)
                    .Select(a => new
                    {
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        EmailAddress = a.EmailAddress,
                        LastUpdatedBy = a.LastUpdatedBy,
                        LastUpdatedDate = a.LastUpdatedDate,
                        CreatedBy = a.CreatedBy,
                        CreatedDate = a.CreatedDate
                    })
                    .FirstOrDefault();

                response.FirstName = applicant.FirstName;
                response.LastName = applicant.LastName;
                response.EmailAddress = applicant.EmailAddress;
                response.LastUpdatedBy = applicant.LastUpdatedBy;
                response.LastUpdatedDate = applicant.LastUpdatedDate.ToShortDateString();
                response.CreatedBy = applicant.CreatedBy;
                response.CreatedDate = applicant.CreatedDate.ToShortDateString();
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
        public CreateApplicantResponse CreateApplicant(CreateApplicantRequest request)
        {
            CreateApplicantResponse response = new CreateApplicantResponse();

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

                E::Applicant newApplicant = CreateApplicant(context, request.FirstName, request.LastName, request.EmailAddress, authToken.Username);

                context.SaveChanges();

                response.ApplicantID = newApplicant.ID;
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

        private static E.Applicant CreateApplicant(DbContext context, string firstName, string lastName, string emailAddress, string username)
        {
            Common.Helpers.ValidationHelper.ValidateRequiredField(firstName, "First Name");
            Common.Helpers.ValidationHelper.ValidateRequiredField(lastName, "Last Name");
            Common.Helpers.ValidationHelper.ValidateRequiredField(emailAddress, "Email Address");

            Common.Helpers.ValidationHelper.ValidateStringLength(firstName, "First Name", Constants.MaxNameLength);
            Common.Helpers.ValidationHelper.ValidateStringLength(lastName, "Last Name", Constants.MaxNameLength);
            Common.Helpers.ValidationHelper.ValidateStringLength(emailAddress, "Email Address", Constants.MaxEmailAddressLength);

            Common.Helpers.ValidationHelper.ValidateEmailAddress(emailAddress);

            E::Applicant newApplicant = new E::Applicant();

            newApplicant.ID = Guid.NewGuid();
            newApplicant.FirstName = firstName;
            newApplicant.LastName = lastName;
            newApplicant.EmailAddress = emailAddress;
            newApplicant.CreatedBy = username;
            newApplicant.CreatedDate = DateTime.UtcNow;
            newApplicant.LastUpdatedBy = username;
            newApplicant.LastUpdatedDate = DateTime.UtcNow;

            context.Applicants.Add(newApplicant);

            return newApplicant;
        }

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public void EditApplicant(EditApplicantRequest request)
        {
            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AntiForgeryToken, "Anti Forgery Token");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.FirstName, "First Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.LastName, "Last Name");
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.EmailAddress, "Email Address");

                Common.Helpers.ValidationHelper.ValidateStringLength(request.FirstName, "First Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.LastName, "Last Name", Constants.MaxNameLength);
                Common.Helpers.ValidationHelper.ValidateStringLength(request.EmailAddress, "Email Address", Constants.MaxEmailAddressLength);

                Common.Helpers.ValidationHelper.ValidateEmailAddress(request.EmailAddress);

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                UserController.ValidateAntiForgeryToken(request.AntiForgeryToken, authToken);

                DbContext context =  DataController.CreateDbContext();

                E::Applicant applicant = context.Applicants.FirstOrDefault(q => q.ID == request.ApplicantID);

                applicant.FirstName = request.FirstName;
                applicant.LastName = request.LastName;
                applicant.EmailAddress = request.EmailAddress;
                applicant.LastUpdatedBy = authToken.Username;
                applicant.LastUpdatedDate = DateTime.UtcNow;

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
        public void DeleteApplicant(DeleteApplicantRequest request)
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

                E::Applicant applicant = context.Applicants
                    .Where(q => q.ID == request.ApplicantID)
                    .FirstOrDefault();

                var attempts = context.Attempts
                    .Where(a => a.InterviewQuestion.Interview.ApplicantID == request.ApplicantID)
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

                context.Applicants.Remove(applicant);

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
        public ParseApplicantsCSVResponse ParseApplicantsCSV(ParseApplicantsCSVRequest request)
        {
            ParseApplicantsCSVResponse response = new ParseApplicantsCSVResponse();

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

                IList<CSVApplicant> applicants = new List<CSVApplicant>();

                foreach (var row in Common.Helpers.CSVReader.Read(request.CSV))
                {
                    if (row.Length != 3)
                    {
                        throw new Common.Exceptions.ValidationException("Invalid number of columns in row " + rowNumber);
                    }

                    Common.Helpers.ValidationHelper.ValidateRequiredField(row[0], "First Name");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(row[1], "Last Name");
                    Common.Helpers.ValidationHelper.ValidateRequiredField(row[2], "Email Address");

                    Common.Helpers.ValidationHelper.ValidateStringLength(row[0], "First Name", Constants.MaxNameLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(row[1], "Last Name", Constants.MaxNameLength);
                    Common.Helpers.ValidationHelper.ValidateStringLength(row[2], "Email Address", Constants.MaxEmailAddressLength);

                    Common.Helpers.ValidationHelper.ValidateEmailAddress(row[2]);

                    applicants.Add(new CSVApplicant
                    {
                        FirstName = row[0],
                        LastName = row[1],
                        EmailAddress = row[2]
                    });

                    rowNumber++;
                }

                response.Applicants = applicants.ToArray();
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
        public BulkCreateApplicantsResponse BulkCreateApplicants(BulkCreateApplicantsRequest request)
        {
            BulkCreateApplicantsResponse response = new BulkCreateApplicantsResponse();

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

                foreach (var applicant in request.Applicants)
                {
                    CreateApplicant(context, applicant.FirstName, applicant.LastName, applicant.EmailAddress, authToken.Username);
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
    }
}
