﻿using System;
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

namespace CleanArchTemplate
{
    // Expose a way to send messages (i.e. email/sms)
    public class SmsService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
           // var Twilio = new TwilioRestClient(
           //    ConfigurationManager.AppSettings["TwilioSid"],
           //    ConfigurationManager.AppSettings["TwilioToken"]
           //);
           // var result = Twilio.SendMessage(
           //     ConfigurationManager.AppSettings["TwilioFromPhone"],
           //    message.Destination, message.Body);

           // // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
           // Trace.TraceInformation(result.Status);

           // // Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
        }


        public Task SendAsync2(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}