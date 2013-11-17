///
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using InitialRound.BusinessLogic.Properties;
using System.Net;
using InitialRound.BusinessLogic.Helpers;
using System.Web;
using InitialRound.BusinessLogic.Classes;
using InitialRound.Models.Contexts;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure;

namespace InitialRound.BusinessLogic.Controllers
{
    public static class EmailController
    {
        private const string UserActivationTemplate = @"
            <p>{{FullName}},</p>
            <p>Your username '{{Username}}' is now active. Please click <a href='{{LoginURL}}'>here</a> to login.</p>
            <p>Thank you.</p>
        ";

        /// <summary>
        /// Note: Break up the URL using non-existent nolink tag to prevent jquery.form issue in IE 9.
        /// </summary>
        private const string InvitationTemplate = @"
            <p>You have been invited to complete an online interview by {{SenderName}}.</p>
            <p>Please copy the following URL into your web browser to begin. You will have an opportunity to review the instructions 
               before starting the interview.</p>
            <p><code>https<nolink>://</nolink>manage<nolink>.</nolink>initialround<nolink>.</nolink>com<nolink>/</nolink>Interviews<nolink>/</nolink>Interview<nolink>?</nolink>Token={{InterviewURI}}</code></p>
            <p>Thank you.</p>
        ";

        private const string InterviewCompletedTemplate = @"
            <p>{{RecipientName}},</p>
            <p>This is an automated email. The interview which you created for {{ApplicantName}} has now completed.</p>
            <p>You can now view the status of this interview and print a report by <a href='{{InterviewURI}}'>clicking here</a>.</p>
            <p>Thank you.</p>
        ";

        private const string ResetPasswordTemplate = @"
            <p>{{FullName}},</p>
            <p>You are receiving this email because a request to reset your password was recently received. If you did not initiate such a request, please disregard this email.</p>
            <p>If you would like to reset your password, please <a href='{{ResetURI}}'>click here</a>.</p>
            <p>Thank you.</p>
        ";

        public class EmailRecipient
        {
            public string EmailAddress { get; set; }

            public string FullName { get; set; }
        }

        public class EmailMessage
        {
            public string Subject { get; set; }

            public string Body { get; set; }

            public DateTime SendDate { get; set; }

            public bool UseHtml { get; set; }

            public EmailRecipient Recipient { get; set; }

            public EmailRecipient CC { get; set; }

            public static void Serialize(Stream stream, EmailMessage message)
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(message.Subject);
                    writer.Write(message.Body);
                    writer.Write(message.SendDate.Ticks);
                    writer.Write(message.UseHtml);

                    writer.Write(message.Recipient.EmailAddress);
                    writer.Write(message.Recipient.FullName);

                    if (message.CC == null)
                    {
                        writer.Write(false);
                    }
                    else
                    {
                        writer.Write(true);
                        writer.Write(message.CC.FullName);
                        writer.Write(message.CC.EmailAddress);
                    }
                }
            }

            public static EmailMessage Deserialize(Stream stream)
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    EmailMessage message = new EmailMessage();

                    message.Subject = reader.ReadString();
                    message.Body = reader.ReadString();
                    message.SendDate = new DateTime(reader.ReadInt64());
                    message.UseHtml = reader.ReadBoolean();

                    message.Recipient = new EmailRecipient();
                    message.Recipient.EmailAddress = reader.ReadString();
                    message.Recipient.FullName = reader.ReadString();

                    if (reader.ReadBoolean())
                    {
                        message.CC = new EmailRecipient();
                        message.CC.FullName = reader.ReadString();
                        message.CC.EmailAddress = reader.ReadString();
                    }

                    return message;
                }
            }
        }

        private static string Interpolate(string template, params KeyValue[] values)
        {
            string result = template;

            foreach (KeyValue pair in values)
            {
                string placeholder = "{{" + pair.Key + "}}";
                result = result.Replace(placeholder, HttpUtility.HtmlEncode(pair.Value));
            }

            return result;
        }

        public class DequeuedMessage
        {
            private readonly CloudQueueMessage message;
            private readonly CloudQueue emailQueue;
            private readonly EmailMessage emailMessage;

            public EmailMessage EmailMessage
            {
                get { return emailMessage; }
            }

            public DequeuedMessage(CloudQueueMessage message, CloudQueue emailQueue, EmailMessage emailMessage)
            {
                this.message = message;
                this.emailQueue = emailQueue;
                this.emailMessage = emailMessage;
            }

            public void Delete()
            {
                emailQueue.DeleteMessage(message);
            }

            public void Requeue()
            {
                emailQueue.UpdateMessage(message, TimeSpan.Zero, MessageUpdateFields.Visibility);
            }
        }

        public static IEnumerable<DequeuedMessage> Dequeue(int count)
        {
            var emailQueue = DataController.GetEmailQueue();

            var messages = emailQueue.GetMessages(count);

            foreach (var message in messages)
            {
                using (var memoryStream = new MemoryStream(message.AsBytes))
                {
                    var emailMessage = EmailMessage.Deserialize(memoryStream);
                    yield return new DequeuedMessage(message, emailQueue, emailMessage);
                }
            }
        }

        private static void Enqueue(EmailMessage message)
        {
            var emailQueue = DataController.GetEmailQueue();

            using (var memoryStream = new MemoryStream())
            {
                EmailMessage.Serialize(memoryStream, message);

                emailQueue.AddMessage(new CloudQueueMessage(memoryStream.ToArray()));
            }
        }

        public static void SendEmail(EmailMessage emailMessage)
        {
            using (SmtpClient smtpClient = new SmtpClient(CloudConfigurationManager.GetSetting("SmtpHost"),
                int.Parse(CloudConfigurationManager.GetSetting("SmtpPort"))))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(CloudConfigurationManager.GetSetting("SmtpUsername"),
                    CloudConfigurationManager.GetSetting("SmtpPassword"));
                smtpClient.Timeout = int.Parse(CloudConfigurationManager.GetSetting("SmtpTimeout"));
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();

                message.Subject = emailMessage.Subject;
                message.Body = emailMessage.Body;
                message.IsBodyHtml = emailMessage.UseHtml;

                message.To.Add(new MailAddress(emailMessage.Recipient.EmailAddress, emailMessage.Recipient.FullName));
                message.From = message.Sender = new MailAddress(Settings.Default.EmailSender, Settings.Default.EmailSenderName);

                if (emailMessage.CC != null)
                {
                    message.CC.Add(new MailAddress(emailMessage.CC.EmailAddress, emailMessage.CC.FullName));
                }

                message.ReplyToList.Clear();

                smtpClient.Send(message);
            }
        }

        public static void SendNewUserEmail(string firstName, string lastName, string username, string emailAddress)
        {
            string subject = "Account Activation";

            string fullName = string.Format("{0} {1}", firstName, lastName);

            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = "";
            }

            string loginUrl = CloudConfigurationManager.GetSetting("RootURI") + "Login";

            string body = Interpolate(UserActivationTemplate,
                KeyValue.Of("FullName", fullName),
                KeyValue.Of("Username", username),
                KeyValue.Of("LoginURL", loginUrl));

            Enqueue(new EmailMessage
            {
                Subject = subject,
                Body = body,
                Recipient = new EmailRecipient
                {
                    EmailAddress = emailAddress,
                    FullName = fullName
                },
                SendDate = DateTime.UtcNow,
                UseHtml = true
            });
        }

        public static void SendInvitationEmail(string firstName, string lastName, string emailAddress,
            string senderFirstName, string senderLastName, string senderEmail,
            Guid interviewId)
        {
            string subject = "Interview Invitation";

            string recipientName = string.Format("{0} {1}", firstName, lastName);
            string senderFullName = string.Format("{0} {1}", senderFirstName, senderLastName);

            if (string.IsNullOrWhiteSpace(recipientName))
            {
                recipientName = emailAddress;
            }

            InterviewToken token = new InterviewToken(interviewId, Common.Helpers.RandomHelper.RandomLong());
            string encryptedToken = Convert.ToBase64String(EncryptionHelper.EncryptURL(token.AsBytes()));

            string body = Interpolate(InvitationTemplate,
                KeyValue.Of("SenderName", senderFullName),
                KeyValue.Of("InterviewURI", HttpUtility.UrlEncode(encryptedToken)));

            Enqueue(new EmailMessage
            {
                Subject = subject,
                Body = body,
                Recipient = new EmailRecipient
                {
                    EmailAddress = emailAddress,
                    FullName = recipientName
                },
                SendDate = DateTime.UtcNow,
                UseHtml = true,
                CC = new EmailRecipient
                {
                    FullName = senderFullName,
                    EmailAddress = senderEmail
                }
            });
        }

        public static void SendInvitationEmail(Guid interviewId, string username)
        {
            DbContext context = DataController.CreateDbContext();

            var interviewInfo = context.Interviews
                .Where(i => i.ID == interviewId)
                .Select(interview => new
                {
                    Interview = interview,
                    Applicant = new
                    {
                        FirstName = interview.Applicant.FirstName,
                        LastName = interview.Applicant.LastName,
                        EmailAddress = interview.Applicant.EmailAddress
                    }
                })
                .FirstOrDefault();

            var userInfo = context.Users
                .Where(u => u.ID == username)
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailAddress = u.EmailAddress
                }).FirstOrDefault();

            EmailController.SendInvitationEmail(
                interviewInfo.Applicant.FirstName, interviewInfo.Applicant.LastName, interviewInfo.Applicant.EmailAddress,
                userInfo.FirstName, userInfo.LastName, userInfo.EmailAddress,
                interviewId);
        }

        public static void SendInterviewCompletedEmail(string recipientName,
            string applicantName, Guid interviewId, string emailAddress,
            TimeSpan duration)
        {
            string interviewUri = string.Format("{0}Interviews/Details/{1}",
                CloudConfigurationManager.GetSetting("RootURI"),
                interviewId.ToString());

            string body = Interpolate(InterviewCompletedTemplate,
                KeyValue.Of("RecipientName", recipientName),
                KeyValue.Of("ApplicantName", applicantName),
                KeyValue.Of("InterviewURI", interviewUri));

            Enqueue(new EmailMessage
            {
                Subject = "Initial Round Interview Completed",
                Body = body,
                Recipient = new EmailRecipient
                {
                    EmailAddress = emailAddress,
                    FullName = recipientName
                },
                SendDate = DateTime.UtcNow.Add(duration),
                UseHtml = true
            });
        }

        public static void SendPasswordResetEmail(string firstName, string lastName, string emailAddress, string username)
        {
            DateTime expiresOn = DateTime.UtcNow + Settings.Default.ResetPasswordTokenExpiryInterval;

            long random = Common.Helpers.RandomHelper.RandomLong();

            ResetPasswordToken token = new ResetPasswordToken(username, expiresOn, random);

            string encryptedToken = Convert.ToBase64String(EncryptionHelper.EncryptURL(token.AsBytes()));

            string subject = "Initial Round Password Reset";

            string recipientName = string.Format("{0} {1}", firstName, lastName);

            if (string.IsNullOrWhiteSpace(recipientName))
            {
                recipientName = username;
            }

            string resetUri = string.Format("{0}Account/CompletePasswordReset?Token={1}",
                CloudConfigurationManager.GetSetting("RootURI"),
                HttpUtility.UrlEncode(encryptedToken));

            string body = Interpolate(ResetPasswordTemplate,
                KeyValue.Of("FullName", recipientName),
                KeyValue.Of("ResetURI", resetUri));

            Enqueue(new EmailMessage
            {
                Subject = subject,
                Body = body,
                Recipient = new EmailRecipient
                {
                    EmailAddress = emailAddress,
                    FullName = recipientName
                },
                SendDate = DateTime.UtcNow,
                UseHtml = true
            });
        }

        private class KeyValue
        {
            public string Key { get; set; }

            public string Value { get; set; }

            public static KeyValue Of(string key, string value)
            {
                return new KeyValue
                {
                    Key = key,
                    Value = value
                };
            }
        }
    }
}
