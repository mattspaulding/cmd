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
using System.Web.Http.OData;

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

        public AppController() : this(new JobRepo(), new BidRepo()) { }

        public AppController(JobRepo JobRepo, BidRepo BidRepo)
        {
            _IJobRepo = JobRepo;
            _IBidRepo = BidRepo;

            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }


        [HttpPost]
        [Route("Job")]
        public Job AddJob(Job job)
        {
            job.CreatedOn = DateTime.Now;
            job.CreatedByUserId = User.Identity.GetUserId();
            job.Owner_ID = AppUser.Owner.ID;
            _IJobRepo.Add(job);
            _IJobRepo.Save();

            return job;
        }

        [HttpGet]
        [Route("Job/{id}/")]
        [EnableQuery]
        public IQueryable<Job> GetJobById(long id)
        {
            
            //TODO: refactor to conditionally exclude private details
            
            var oid = AppUser.Owner.ID;
            var result =
                from job in _IJobRepo.Get()
                let ownerId = oid
                where job.ID == id
                select job;




            return result;
        }

        /// <summary>
        /// Get a list of all jobs that belong to
        /// the ownermaking the request
        /// </summary>
        [HttpGet]
        [Route("Job")]
        [EnableQuery]
        public IQueryable<Job> GetJobs()
        {
            //TODO: refactor to conditionally exclude private details
            var query =
                from job in _IJobRepo.Get()
                select job;


            return query;
        }


        /// <summary>
        /// Bid on a job, requires the JobID and the bid object
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="bid"></param>
        [HttpPost]
        [Route("Job/{JobId}/Bid")]
        public Bid CreateBid(long JobId, Bid bid)
        {
            bid.Job = null;
            bid.CreatedOn = DateTime.Now;
            bid.CreatedByUserId = User.Identity.GetUserId();
            bid.Owner_ID = AppUser.Owner.ID;
            bid.Job_ID = JobId;
            _IBidRepo.Add(bid);
            _IBidRepo.Save();

            return bid;
        }

        [HttpGet]
        [Route("Bids")]
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
            var job = _IJobRepo.Get().Where(j => j.ID == bid.Job.ID).FirstOrDefault();

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
            var job = _IJobRepo.Get().Where(j => j.ID == Bid.Job.ID).FirstOrDefault();
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