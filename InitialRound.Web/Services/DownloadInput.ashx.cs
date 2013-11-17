using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.Web.Classes.QuestionService;

namespace InitialRound.Web.Services
{
    public class DownloadInput : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string practice = context.Request["Practice"];

                    string input;

                    if (practice != null)
                    {
                        input = PracticeController.InputString;
                    }
                    else
                    {
                        string token = context.Request["Token"];
                        string authToken = context.Request["AuthToken"];
                        string attemptToken = context.Request["AttemptToken"];
                        string questionIdString = context.Request["QuestionID"];

                        Guid questionId;

                        if (!Guid.TryParse(questionIdString, out questionId))
                        {
                            throw new Exception("Invalid Question ID");
                        }

                        if (token != null)
                        {
                            var response = new InterviewService().DownloadInput(new Classes.InterviewService.DownloadInputRequest
                            {
                                Token = token,
                                QuestionID = questionId,
                                AttemptToken = attemptToken
                            });

                            input = response.Input;
                        }
                        else
                        {
                            var response = new QuestionService().DownloadInput(new DownloadInputRequest
                            {
                                AuthToken = authToken,
                                QuestionID = questionId
                            });

                            input = response.Input;
                        }
                    }

                    context.Response.Clear();
                    context.Response.AddHeader("content-disposition", "attachment;filename=input.txt");
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(input);
                }
                catch (Exception)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.Response.End();
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}