using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using InitialRound.BusinessLogic.Controllers;
using InitialRound.BusinessLogic.Helpers;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Interview.EmailService
{
    public class WorkerRole : RoleEntryPoint
    {
        private int failureCount = 0;

        public override void Run()
        {
            DataController.CreateQueuesIfNecessary();

            while (true)
            {
                try
                {
                    DequeueAndSendEmails();
                }
                catch (Exception ex)
                {
                    if (failureCount++ < 100)
                    {
                        ExceptionHelper.Log(ex, string.Empty);
                    }
                }

                try
                {
                    Thread.Sleep(60000);
                }
                catch (Exception ex)
                {
                    if (failureCount++ < 100)
                    {
                        ExceptionHelper.Log(ex, string.Empty);
                    }
                }
            }
        }

        private void DequeueAndSendEmails()
        {
            var messages = EmailController.Dequeue(20);

            foreach (var message in messages)
            {
                bool requeueMessage = true;

                try
                {
                    if (message.EmailMessage.SendDate <= DateTime.UtcNow)
                    {
                        EmailController.SendEmail(message.EmailMessage);
                        message.Delete();
                        requeueMessage = false;
                    }
                }
                catch (Exception ex)
                {
                    if (failureCount++ < 100)
                    {
                        ExceptionHelper.Log(ex, string.Empty);
                    }
                }
                finally
                {
                    if (requeueMessage)
                    {
                        message.Requeue();
                    }
                }
            }
        }
    }
}
