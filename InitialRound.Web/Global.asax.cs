using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using InitialRound.BusinessLogic.Controllers;
using System.Web.Routing;
using System.ServiceModel.Activation;
using InitialRound.Web.Services;
using InitialRound.BusinessLogic.Helpers;

namespace InitialRound.Web
{
    public class Global : System.Web.HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            DataController.CreateTablesIfNecessary();

            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.Add(new ServiceRoute("services/applicants", new WebServiceHostFactory(), typeof(ApplicantService)));
            routes.Add(new ServiceRoute("services/questions", new WebServiceHostFactory(), typeof(QuestionService)));
            routes.Add(new ServiceRoute("services/interviews", new WebServiceHostFactory(), typeof(InterviewService)));
            routes.Add(new ServiceRoute("services/questionsets", new WebServiceHostFactory(), typeof(QuestionSetService)));
            routes.Add(new ServiceRoute("services/account", new WebServiceHostFactory(), typeof(AccountService)));
            routes.Add(new ServiceRoute("services/users", new WebServiceHostFactory(), typeof(UserService)));
            routes.Add(new ServiceRoute("services/reports", new WebServiceHostFactory(), typeof(ReportService)));

            routes.MapPageRoute("Home", "", "~/Views/Home.aspx");
            routes.MapPageRoute("Help", "Help", "~/Views/Help.aspx");

            routes.MapPageRoute("Login", "Login", "~/Views/Account/Login.aspx");
            routes.MapPageRoute("Logout", "Logout", "~/Views/Account/Logout.aspx");

            routes.MapPageRoute("Account", "Account", "~/Views/Account/Account.aspx");
            routes.MapPageRoute("ResetPassword", "Account/ResetPassword", "~/Views/Account/ResetPassword.aspx");
            routes.MapPageRoute("CompletePasswordReset", "Account/CompletePasswordReset", "~/Views/Account/CompletePasswordReset.aspx");
            routes.MapPageRoute("ChangePassword", "Account/ChangePassword", "~/Views/Account/ChangePassword.aspx");

            routes.MapPageRoute("ListUsers", "Users", "~/Views/Users/ListUsers.aspx");
            routes.MapPageRoute("CreateUser", "Users/Create", "~/Views/Users/CreateUser.aspx");
            routes.MapPageRoute("EditUser", "Users/Details/{username}", "~/Views/Users/EditUser.aspx");
            routes.MapPageRoute("DeleteUser", "Users/Delete/{username}", "~/Views/Users/DeleteUser.aspx");

            routes.MapPageRoute("ListApplicants", "Applicants", "~/Views/Applicant/ListApplicants.aspx");
            routes.MapPageRoute("CreateApplicant", "Applicants/Create", "~/Views/Applicant/Applicant.aspx");
            routes.MapPageRoute("BulkImport", "Applicants/BulkImport", "~/Views/Applicant/BulkImport.aspx");
            routes.MapPageRoute("Applicant", "Applicants/Details/{applicantId}", "~/Views/Applicant/Applicant.aspx");
            routes.MapPageRoute("DeleteApplicant", "Applicants/Delete/{applicantId}", "~/Views/Applicant/DeleteApplicant.aspx");

            routes.MapPageRoute("ListQuestions", "Questions", "~/Views/Question/ListQuestions.aspx");
            routes.MapPageRoute("CreateQuestion", "Questions/Create", "~/Views/Question/Question.aspx");
            routes.MapPageRoute("Question", "Questions/Details/{questionId}", "~/Views/Question/Question.aspx");
            routes.MapPageRoute("DeleteQuestion", "Questions/Delete/{questionId}", "~/Views/Question/DeleteQuestion.aspx");
            routes.MapPageRoute("DownloadQuestionInput", "Questions/DownloadInput", "~/Services/DownloadInput.ashx");

            routes.MapPageRoute("ListQuestionSets", "QuestionSets", "~/Views/QuestionSet/ListQuestionSets.aspx");
            routes.MapPageRoute("CreateQuestionSet", "QuestionSets/Create", "~/Views/QuestionSet/QuestionSet.aspx");
            routes.MapPageRoute("QuestionSet", "QuestionSets/Details/{questionSetId}", "~/Views/QuestionSet/QuestionSet.aspx");
            routes.MapPageRoute("DeleteQuestionSet", "QuestionSets/Delete/{questionSetId}", "~/Views/QuestionSet/DeleteQuestionSet.aspx");

            routes.MapPageRoute("ListInterviews", "Interviews", "~/Views/Interview/ListInterviews.aspx");
            routes.MapPageRoute("CreateInterview", "Interviews/Create", "~/Views/Interview/CreateInterview.aspx");
            routes.MapPageRoute("Interview", "Interviews/Details/{interviewId}", "~/Views/Interview/Interview.aspx");
            routes.MapPageRoute("DeleteInterview", "Interviews/Delete/{interviewId}", "~/Views/Interview/DeleteInterview.aspx");
            routes.MapPageRoute("PrintInterview", "Interviews/Print/{interviewId}", "~/Views/Interview/Print.aspx");
            routes.MapPageRoute("RunInterview", "Interviews/Interview", "~/Views/Interview/Run.aspx");
            routes.MapPageRoute("PracticeInterview", "Interviews/Practice", "~/Views/Interview/Practice.aspx");
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();

            if (!(error is HttpException && (error as HttpException).ErrorCode == 404))
            {
                ExceptionHelper.Log(error, null);
            }
        }
    }
}
