using System;
using System.Collections.Generic;
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
using InitialRound.BusinessLogic.Helpers;
using InitialRound.BusinessLogic.Properties;
using InitialRound.Models.Contexts;
using InitialRound.Web.Classes.AccountService;
using InitialRound.Web.Classes.ReportService;
using E = InitialRound.Models.Schema.dbo;
using InitialRound.BusinessLogic;
using System.Data.Entity.Core.Objects;

namespace InitialRound.Web.Services
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReportService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public HomePageReport HomePageReport(HomePageReportRequest request)
        {
            HomePageReport report = new HomePageReport();

            AuthToken authToken = null;

            try
            {
                Common.Helpers.ValidationHelper.ValidateRequiredField(request.AuthToken, "Auth Token");

                if (!UserController.ValidateSession(request.AuthToken, out authToken))
                {
                    throw new AuthenticationException("Authentication failed.");
                }

                DbContext context =  DataController.CreateDbContext();

                var interviews =
                    context.Interviews
                     .GroupBy(i => new
                     {
                         Sent = i.SentDate.HasValue,
                         Started = i.StartedDate.HasValue,
                         Elapsed = i.StartedDate.HasValue && System.Data.Entity.DbFunctions.AddMinutes(i.StartedDate.Value, i.MinutesAllowed) < DateTime.UtcNow
                     })
                     .ToDictionary(g => g.Key, g => g.Count());

                if (interviews.ContainsKey(new { Sent = false, Started = false, Elapsed = false }))
                {
                    report.NewInterviews = interviews[new { Sent = false, Started = false, Elapsed = false }];
                }

                if (interviews.ContainsKey(new { Sent = true, Started = false, Elapsed = false }))
                {
                    report.PendingInterviews = interviews[new { Sent = true, Started = false, Elapsed = false }];
                }

                if (interviews.ContainsKey(new { Sent = true, Started = true, Elapsed = false }))
                {
                    report.InterviewsInProgress = interviews[new { Sent = true, Started = true, Elapsed = false }];
                }

                if (interviews.ContainsKey(new { Sent = true, Started = true, Elapsed = true }))
                {
                    report.InterviewsRequiringReview = interviews[new { Sent = true, Started = true, Elapsed = true }];
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

            return report;
        }
    }
}
