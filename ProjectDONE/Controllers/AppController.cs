using ProjectDONE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectDONE.Models.AppModels;
using Microsoft.AspNet.Identity;
using ProjectDONE.Data.Repos;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json;
using ProjectDONE.Models;
using Microsoft.AspNet.Identity.EntityFramework;

//Services are defined by feature
//Rather than functional area
//Should make managmenet easier

namespace ProjectDONE.Controllers
{
   [Authorize]
   [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        const int default_skip = 0;
        const int default_take = 10;
        private JobRepo _IJobRepo;
        private BidRepo _IBidRepo;
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        protected ApplicationUser AppUser { get { return UserManager.FindById(User.Identity.GetUserId()); } }

        public AppController()  :this(new JobRepo(), new BidRepo()){}

        public AppController(JobRepo JobRepo, BidRepo BidRepo)
        {
            _IJobRepo = JobRepo;
            _IBidRepo = BidRepo;

            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        [HttpPost]
        [Route("Job")]
        public long AddJob(Job job)
        {
            
            job.CreatedOn = DateTime.Now;
            _IJobRepo.Add(job);
            _IJobRepo.Save();

            return job.ID;
        }

        [HttpGet]
        [Route("Owner/{id}/Jobs")]
        public IQueryable<Job> GetJobsByOwner(int id)
        {
            var query =
                from job in _IJobRepo.Get()
                where job.Owner.ID == id
                select job;

            return query;
        }

        [HttpGet]
        [Route("Jobs/{id}/")]
        public IQueryable<Job> GetJobById(long id)
        {
           
            var result =
                _IJobRepo
                .Get()
                .Where(j => j.ID == id);

            


            return result;
        }

        [HttpPost]
        [Route("Bids/")]
        public void CreateBid(Bid bid)
        {
            _IBidRepo.Add(bid);
            _IBidRepo.Save();
        }

        [HttpGet]
        [Route("Owner/{id}/Bids/")]
        public IQueryable<Bid> GetBidsByOwner(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                        where bid.Owner.ID == id
                        select bid;
                          

            
            return query;
        }

        [HttpDelete]
        [Route("Bids/")]
        public void WithdrawlBid(Bid bid)
        {
            _IBidRepo.Remove(bid);
            _IBidRepo.Save();

        }

        [HttpGet]
        [Route("Jobs/{id}/Bids/")]
        public IQueryable<Bid> GetBidsByJob(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                          where bid.ID == id
                          select bid;
           
            return query;
        }

        [HttpPost]
        [Route("Jobs/AcceptBid")]
        public void AcceptBid(Bid bid)
        {
            var bids = _IBidRepo.Get().Where(b => b.Job.ID == bid.Job.ID);
            var job = _IJobRepo.Get().Where(j=>j.ID == bid.Job.ID).FirstOrDefault();

            if (job == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("No job found with the ID of " + bid.Job.ID);
                return;
            }

            job.AcceptedBid = bid;
            job.AcceptedBid.Status = BidStatus.Accepted;

            _IBidRepo.Update(job.AcceptedBid);
            _IJobRepo.Update(job);

            foreach (var b in bids.Where(b => b.ID != job.AcceptedBid.ID))
            {
                b.Status = BidStatus.Declined;
                _IBidRepo.Update(b);
            }
            _IBidRepo.Save();
            _IJobRepo.Save();
        }

        [HttpPost]
        [Route("Bids/ConfirmBid")]
        public void ConfirmBid(Bid Bid)
        {
            var job = _IJobRepo.Get().Where(j=>j.ID == Bid.Job.ID).FirstOrDefault();
            if (job == null)
            {
                var Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                Response.Content = new StringContent("No job found with the ID of " + Bid.Job.ID);
                return;
            }
            job.Status = Jobstatus.Confirmed;
            _IJobRepo.Update(job);
            _IJobRepo.Save();
        }

      
    }
}