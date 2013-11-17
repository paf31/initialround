using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Web;
using InitialRound.BusinessLogic.Exceptions;
using InitialRound.Web.Classes.ApplicantService;

namespace InitialRound.Web.Services
{
    public class UploadApplicantsCSV : IHttpHandler
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
                    string authToken = context.Request["AuthToken"];
                    string antiForgeryToken = context.Request["AntiForgeryToken"];

                    string csv = ReadFile(context.Request.Files["CSV"]);

                    var response = new ApplicantService().ParseApplicantsCSV(new ParseApplicantsCSVRequest
                    {
                        AuthToken = authToken,
                        AntiForgeryToken = antiForgeryToken,
                        CSV = csv,
                    });

                    var serializer = new DataContractJsonSerializer(typeof(ParseApplicantsCSVResponse));
                    serializer.WriteObject(context.Response.OutputStream, response);
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
                throw new Common.Exceptions.ValidationException("File too large.");
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