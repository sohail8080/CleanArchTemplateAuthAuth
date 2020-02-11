using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendGrid;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace CleanArchTemplate
{
    public class EmailService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            // always return email send success result
            return Task.FromResult(0);
        }

        // this interface method is called by framework
        public async Task SendAsync2(IdentityMessage message)
        {

            // Destination, Subject, Body is contained by message
            //var response = await configSendGridasync(message);
            //var response = await configSendGridasync2(message);
            await configSendGridasync3(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task<Response> configSendGridasync(IdentityMessage message)
        {

            // A HTTP client wrapper for interacting with Twilio SendGrid's API
            var client = new SendGridClient(AuthMessageSenderOptions.SendGridKey);

            //     Class SendGridMessage builds an object 
            // that sends an email through Twilio SendGrid.
            var myMessage = new SendGridMessage();

            //Add a recipient email.
            myMessage.AddTo(message.Destination);

            //     Gets or sets an email object containing the email address 
            // and name of the sender.
            //     Unicode encoding is not supported for the from field.
            //   email:
            //     The email address of the sender or recipient.
            //   name:
            //     The name of the sender or recipient.
            myMessage.From = new EmailAddress("sohail8080@gmail.com", AuthMessageSenderOptions.SendGridUser);
            myMessage.Subject = message.Subject;
            myMessage.PlainTextContent = message.Body;
            myMessage.HtmlContent = message.Body;

            myMessage.SetClickTracking(false, false);

            return await client.SendEmailAsync(myMessage);

            //var credentials = new NetworkCredential(
            //           ConfigurationManager.AppSettings["mailAccount"],
            //           ConfigurationManager.AppSettings["mailPassword"]
            //           );

            // Create a Web transport for sending email.
            //var transportWeb = new Web(credentials);

            //// Send the email.
            //if (transportWeb != null)
            //{
            //    await transportWeb.DeliverAsync(myMessage);
            //}
            //else
            //{
            //    Trace.TraceError("Failed to create Web transport.");
            //    await Task.FromResult(0);
            //}
        }


        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task<Response> configSendGridasync2(IdentityMessage message)
        {

            var client = new SendGridClient(AuthMessageSenderOptions.SendGridKey);
            var from = new EmailAddress("sohail8080@abc.com", AuthMessageSenderOptions.SendGridUser);
            var subject = message.Subject;
            var to = new EmailAddress(message.Destination, "Example User");
            var plainTextContent = message.Body;
            var htmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            return await client.SendEmailAsync(msg);

        }


        // Not working, but Alert Email to my accound
        // Finall Work, I need to allow Less Secure Applications ON on my
        // Gmail account by going on this email https://myaccount.google.com/lesssecureapps
        // Got Email, Link Cliked, Email Confirmed, Log in the app.
        private async Task configSendGridasync3(IdentityMessage message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("sohail8080@gmail.com");
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("xxxxxxx@gmail.com", "xxxxxxxx");
            SmtpServer.EnableSsl = true;

            await SmtpServer.SendMailAsync(mail);

        }


        // Old SendGridMethod to Send Email.
        //private async Task configSendGridasync4(IdentityMessage message)
        //{
        //    var myMessage = new SendGridMessage();
        //    myMessage.AddTo(message.Destination);
        //    myMessage.From = new System.Net.Mail.MailAddress(
        //                        "Joe@contoso.com", "Joe S.");
        //    myMessage.Subject = message.Subject;
        //    myMessage.Text = message.Body;
        //    myMessage.Html = message.Body;

        //    var credentials = new NetworkCredential(
        //               ConfigurationManager.AppSettings["mailAccount"],
        //               ConfigurationManager.AppSettings["mailPassword"]
        //               );

        //    // Create a Web transport for sending email.
        //    var transportWeb = new Web(credentials);

        //    try
        //    {
        //        // Send the email.
        //        if (transportWeb != null)
        //        {
        //            await transportWeb.DeliverAsync(myMessage);
        //        }
        //        else
        //        {
        //            Trace.TraceError("Failed to create Web transport.");
        //            await Task.FromResult(0);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceError(ex.Message + " SendGrid probably not configured correctly.");
        //    }
        //}


        // Venkat Code to send Email
        public static void SendEmail(string emailbody)
        {
            // Specify the from and to email address
            MailMessage mailMessage = new MailMessage("from_email@gmail.com", "To_Email@gmail.com");
            // Specify the email body
            mailMessage.Body = emailbody;
            // Specify the email Subject
            mailMessage.Subject = "Exception";

            // Specify the SMTP server name and post number
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            // Specify your gmail address and password
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "from_email@gmail.com",
                Password = "your_password"
            };
            // Gmail works on SSL, so set this property to true
            smtpClient.EnableSsl = true;
            // Finall send the email message using Send() method
            smtpClient.Send(mailMessage);
        }


        // https://docs.aws.amazon.com/ses/latest/DeveloperGuide/send-using-smtp-net.html
        public static void SendEmailAmazonSES(string emailbody)
        {
            // Replace sender@example.com with your "From" address. 
            // This address must be verified with Amazon SES.
            String FROM = "sender@example.com";
            String FROMNAME = "Sender Name";

            // Replace recipient@example.com with a "To" address. If your account 
            // is still in the sandbox, this address must be verified.
            String TO = "recipient@amazon.com";

            // Replace smtp_username with your Amazon SES SMTP user name.
            String SMTP_USERNAME = "smtp_username";

            // Replace smtp_password with your Amazon SES SMTP user name.
            String SMTP_PASSWORD = "smtp_password";

            // (Optional) the name of a configuration set to use for this message.
            // If you comment out this line, you also need to remove or comment out
            // the "X-SES-CONFIGURATION-SET" header below.
            String CONFIGSET = "ConfigSet";

            // If you're using Amazon SES in a region other than US West (Oregon), 
            // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
            // endpoint in the appropriate AWS Region.
            String HOST = "email-smtp.us-west-2.amazonaws.com";

            // The port you will connect to on the Amazon SES SMTP endpoint. We
            // are choosing port 587 because we will use STARTTLS to encrypt
            // the connection.
            int PORT = 587;

            // The subject line of the email
            String SUBJECT =
                "Amazon SES test (SMTP interface accessed using C#)";

            // The body of the email
            String BODY =
                "<h1>Amazon SES Test</h1>" +
                "<p>This email was sent through the " +
                "<a href='https://aws.amazon.com/ses'>Amazon SES</a> SMTP interface " +
                "using the .NET System.Net.Mail library.</p>";

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            message.To.Add(new MailAddress(TO));
            message.Subject = SUBJECT;
            message.Body = BODY;
            // Comment or delete the next line if you are not using a configuration set
            message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);

            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                // Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    Console.WriteLine("Email sent!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                }
            }
        }
    }
}