using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using System.Configuration;
using System.Diagnostics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Clients;

namespace CleanArchTemplate
{
    // Expose a way to send messages (i.e. email/sms)
    public class SmsService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        // Twilio
        public Task SendAsync2(IdentityMessage message)
        {
            // var Twilio = new TwilioRestClient(
            //    ConfigurationManager.AppSettings["TwilioSid"],
            //    ConfigurationManager.AppSettings["TwilioToken"]
            //);
            // var result = Twilio.SendMessage(
            //     ConfigurationManager.AppSettings["TwilioFromPhone"],
            //    message.Destination, message.Body);

            // Status is one of Queued, Sending, Sent, Failed or null if the 
            //  number is not valid
            //Trace.TraceInformation(result.Status);

           // // Twilio doesn't currently have an async API, so return success.

            return Task.FromResult(0);
        }

        // Scott Haselman Method to send SMS by twilio
        public Task SendAsync3(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            //message.Destination = Keys.ToPhone; //your number here
            //var twilio = new TwilioRestClient(Keys.TwilioSid, Keys.TwilioToken);
            //var result = twilio.SendMessage(Keys.FromPhone, message.Destination, message.Body);

            return Task.FromResult(0);
        }


        public Task SendAsync4(IdentityMessage message)
        {
            // Twilio Begin
            var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource result = MessageResource.Create(
            new PhoneNumber(message.Destination),
            from: new PhoneNumber(fromNumber),
            body: message.Body
            );

            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status.ToString());
            //Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);    
            // Twilio End

        }


        public Task SendAsync5(IdentityMessage message)
        {

            // ASPSMS Begin 
            // var soapSms = new MvcPWx.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            // soapSms.SendSimpleTextSMS(
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"],
            //   message.Destination,
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"],
            //   message.Body);
            // soapSms.Close();
             return Task.FromResult(0);
            // ASPSMS End
        }


    }


}
