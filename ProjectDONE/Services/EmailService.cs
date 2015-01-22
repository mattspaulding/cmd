using System;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Configuration;
using ProjectDONE.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ProjectDONE.Data;
using System.Linq;
using System.Diagnostics;

namespace ProjectDONE.Services
{
    //TODO: This needs to be wrapped in a test.
    public class EmailService
    {
        public static string USERNAME = ConfigurationManager.AppSettings["SendGrid_User"];
        public static string PASSWORD = ConfigurationManager.AppSettings["SendGrid_Pass"];

        //New Bid
        public async void Send_JobReceiveBid(BidViewModel bid, JobViewModel job)
        {
            var usercontext = new ApplicationDbContext();
            var job_creator = usercontext.Users.Where(u => u.Id == job.Owner.CreatedByUserId).FirstOrDefault();
            var bid_creator = usercontext.Users.Where(u => u.Id == bid.Owner.CreatedByUserId).FirstOrDefault();

            var mail = new SendGridMessage();
            mail.From = new MailAddress("JobUpdate@ProjectDone.co", "Project Done");
            mail.To = new MailAddress[] { new MailAddress(job_creator.Email, job_creator.UserName)};

            mail.Subject = string.Format("New Bid! - {0}",job.Title);
            mail.Text = string.Format("User: {0} has bid ${1} on your job titled: {2}",bid_creator.UserName,bid.Amount,job.Title);
            Trace.Write(mail);
            await SendMessage(mail);  
        }
        //If My Bid is accepted
        public async void Send_JobFinished(BidViewModel bid, JobViewModel job)
        {
            var usercontext = new ApplicationDbContext();
            var job_creator = usercontext.Users.Where(u => u.Id == job.Owner.CreatedByUserId).FirstOrDefault();
            var bid_creator = usercontext.Users.Where(u => u.Id == bid.Owner.CreatedByUserId).FirstOrDefault();

            var mail = new SendGridMessage();
            mail.From = new MailAddress("JobUpdate@ProjectDone.co", "Project Done");
            mail.To = new MailAddress[] { new MailAddress(job_creator.Email, job_creator.UserName) };

            mail.Subject = string.Format("Your project is Done! - {0}", job.Title);
            mail.Text = string.Format("User: {0} has finished your project {1} and is awaiting payment.", bid_creator.UserName, job.Title);
            Trace.Write(mail);
            await SendMessage(mail);
        }

        //If my job is finished
        public async void Send_BidAccepted(BidViewModel bid, JobViewModel job)
        {
            var usercontext = new ApplicationDbContext();
            var job_creator = usercontext.Users.Where(u => u.Id == job.Owner.CreatedByUserId).FirstOrDefault();
            var bid_creator = usercontext.Users.Where(u => u.Id == bid.Owner.CreatedByUserId).FirstOrDefault();

            var mail = new SendGridMessage();
            mail.From = new MailAddress("JobUpdate@ProjectDone.co", "Project Done");
            mail.To = new MailAddress[] { new MailAddress(bid_creator.Email, bid_creator.UserName) };

            mail.Subject = string.Format("Your bid was accepted! - {0}", job.Title);
            mail.Text = string.Format("User: {0} has accepted your bid of ${1} for job: {2}", job_creator.UserName, bid.Amount, job.Title);
            Trace.Write(mail);
            await SendMessage(mail);
        }
      
        //If my job is paid
        public async void Send_JobPaid(BidViewModel bid, JobViewModel job)
        {
             var usercontext = new ApplicationDbContext();
            var job_creator = usercontext.Users.Where(u => u.Id == job.Owner.CreatedByUserId).FirstOrDefault();
            var bid_creator = usercontext.Users.Where(u => u.Id == bid.Owner.CreatedByUserId).FirstOrDefault();

            var mail = new SendGridMessage();
            mail.From = new MailAddress("JobUpdate@ProjectDone.co", "Project Done");
            mail.To = new MailAddress[] { new MailAddress(bid_creator.Email, bid_creator.UserName) };

            mail.Subject = string.Format("You were paid for - {0}", job.Title);
            mail.Text = string.Format("User: {0} has completed payment of ${1} for {2}. Please allow 7 days for processing.", job_creator.UserName, bid.Amount, job.Title);
            Trace.Write(mail);
            await SendMessage(mail);
        }
        private Task SendMessage(SendGridMessage message)
        {
            ////Quick and dirty; i really should create a factory for this.
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    return new Task(() => { Console.Write(message.ToString()); }); //Worst line of code EVER!
            //}


                var credentials = new NetworkCredential(USERNAME, PASSWORD);
                var transportWeb = new Web(credentials);
                return transportWeb.DeliverAsync(message);
        }
    }

}