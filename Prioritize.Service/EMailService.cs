
using Prioritize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Prioritize.Service
{
        public class EmailService
    {
            private SmtpClient _smtpClient;
            private AppConfig _appConfig;
            public EmailService(SmtpClient smtpClient, AppConfigService appConfigService)
            {
                _appConfig = appConfigService.AppConfig;
                _smtpClient = smtpClient;

                smtpClient.SendCompleted += (s, e) => smtpClient.Dispose();

            }
            public void SendEmail(List<string> toAddresses, string subject, string body)
            {
                _smtpClient.Send(CreateMessage(toAddresses, subject, body));
            }
            public void SendEmail(string toAddresses, string subject, string body)
            {
                SendEmail(new List<string>() { toAddresses }, subject, body);
            }

            private MailMessage CreateMessage(List<string> toAddresses, string subject, string body)
            {
                var message = new MailMessage();

                ValidateMail(toAddresses, subject, body);

                if (_appConfig.DebugEmail)
                {
                    foreach (var toAddress in _appConfig.DebugEmailTo)
                    {
                        message.To.Add(toAddress);
                    }
                    subject = $"Dev: {subject}";
                    body = $"<strong>This Email was generated in the Development Environment</strong><p>{body}<p>Original Recipients:<p>{string.Join("</br>", toAddresses)}";
                }
                else
                {
                    foreach (var address in toAddresses)
                    {
                        message.To.Add(new MailAddress(address));
                    }
                }

                message.From = new MailAddress(_appConfig.MailFromAddress, _appConfig.MailFromName);
                message.Body = body;
                message.Subject = subject;
                message.IsBodyHtml = true;

                return message;

            }

            private bool ValidateMail(List<string> toAddresses, string subject, string body)
            {
                if (!toAddresses.Any())
                    throw new ArgumentOutOfRangeException("toAddressses");

                if (string.IsNullOrEmpty(subject))
                    throw new ArgumentNullException("subject");

                if (string.IsNullOrEmpty(body))
                    throw new ArgumentNullException("body");

                if (string.IsNullOrEmpty(_appConfig.MailFromAddress))
                    throw new ArgumentNullException("The From Email Address has not been set in AppSettings.");

                return true;
            }

        }

}
