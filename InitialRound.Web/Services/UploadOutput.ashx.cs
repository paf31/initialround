using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Web;
using InitialRound.Web.Classes.InterviewService;
using InitialRound.Web.Classes.QuestionService;

namespace InitialRound.Web.Services
{
    public class UploadOutput : IHttpHandler
    {
        public class Message
        {
            public string ErrorMessage { get; set; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";

                try
                {
                    string practice = context.Request["Practice"];

                    if (practice != null)
                    {
                        string code = ReadFile(context.Request.Files["Code"]);
                        string output = ReadFile(context.Request.Files["Output"]);

                        var response = new InterviewService().Practice(new PracticeRequest
                        {
                            Code = code,
                            Output = output
                        });

                        var serializer = new DataContractJsonSerializer(typeof(PracticeResponse));
                        serializer.WriteObject(context.Response.OutputStream, response);
                    }
                    else
                    {
                        string token = context.Request["Token"];
                        string authToken = context.Request["AuthToken"];
                        string antiForgeryToken = context.Request["AntiForgeryToken"];
                        string attemptToken = context.Request["AttemptToken"];
                        string questionIdString = context.Request["QuestionID"];

                        Guid questionId;

                        if (!Guid.TryParse(questionIdString, out questionId))
                        {
                            throw new Exception("Invalid Question ID");
                        }

                        if (token != null)
                        {
                            string code = ReadFile(context.Request.Files["Code"]);
                            string output = ReadFile(context.Request.Files["Output"]);

                            var response = new InterviewService().SubmitAnswer(new SubmitAnswerRequest
                            {
                                Token = token,
                                ID = questionId,
                                Code = code,
                                Output = output,
                                AttemptToken = attemptToken
                            });

                            var serializer = new DataContractJsonSerializer(typeof(SubmitAnswerResponse));
                            serializer.WriteObject(context.Response.OutputStream, response);
                        }
                        else if (authToken != null)
                        {
                            string output = ReadFile(context.Request.Files["Output"]);

                            var response = new QuestionService().RunTests(new RunTestsRequest
                            {
                                AuthToken = authToken,
                                AntiForgeryToken = antiForgeryToken,
                                QuestionID = questionId,
                                Output = output
                            });

                            var serializer = new DataContractJsonSerializer(typeof(RunTestsResponse));
                            serializer.WriteObject(context.Response.OutputStream, response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var serializer = new DataContractJsonSerializer(typeof(Message));

                    string errorMessage;

                    if (ex is WebFaultException<string>)
                    {
                        errorMessage = (ex as WebFaultException<string>).Detail;
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }

                    serializer.WriteObject(context.Response.OutputStream, new Message { ErrorMessage = errorMessage });
                }
                finally
                {
                    context.Response.End();
                }
            }
        }

        private static string ReadFile(HttpPostedFile outputFile)
        {
            if (outputFile.ContentLength > 1024 * 1024)
            {
                throw new Exception("File too large.");
            }

            string output;

            using (StreamReader reader = new StreamReader(outputFile.InputStream))
            {
                output = reader.ReadToEnd();
            }
            return output;
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