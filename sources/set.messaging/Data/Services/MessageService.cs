using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

using set.messaging.Helpers;

namespace set.messaging.Data.Services
{

    public class MessageService : IMessageService
    {
        private const string FROM_EMAIL = "info@setcrm.com";

        public Task<SendEmailResponse> SendEmail(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(htmlBody) || !to.IsEmail()) throw new Exception("not valid");

            var destination = new Destination { ToAddresses = new List<string> { to } };

            var contentSubject = new Content { Charset = Encoding.UTF8.EncodingName, Data = subject };
            var contentBody = new Content { Charset = Encoding.UTF8.EncodingName, Data = htmlBody };
            var body = new Body { Html = contentBody };

            var message = new Message { Body = body, Subject = contentSubject };

            var request = new SendEmailRequest
            {
                Source = FROM_EMAIL,
                Destination = destination,
                Message = message
            };

            var client = new AmazonSimpleEmailServiceClient();

            var response = client.SendEmail(request);

            return Task.FromResult(response);
        }
    }

    public interface IMessageService
    {
        Task<SendEmailResponse> SendEmail(string to, string subject, string htmlBody);
    }
}