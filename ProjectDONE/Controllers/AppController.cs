using ProjectDONE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectDONE.Models.AppModels;
using Microsoft.AspNet.Identity;
using ProjectDONE.Data.Repos;
using System.Net;

//Services are defined by feature
//Rather than functional area
//Should make managmenet easier

namespace ProjectDONE.Controllers.Api
{
    [Route("Api/App")]
    public class AppController : Controller
    {
        const int default_skip = 0;
        const int default_take = 10;
        private IJobRepo _IJobRepo;
        private IBidRepo _IBidRepo;
        

        public AppController()  :this(new JobRepo(), new BidRepo()){}

        public AppController(IJobRepo JobRepo, IBidRepo BidRepo)
        {
            _IJobRepo = JobRepo;
            _IBidRepo = BidRepo;
        }

        [HttpPost]
        [Route("Job/")]
        public void AddJob(IJob job)
        {
            _IJobRepo.Add(job);
            _IJobRepo.Save();
        }

        //TODO: pass in paging data
        [HttpGet]
        [Route("Owner/{id}/Jobs/")]
        public ActionResult GetJobsByOwner(long id, int? skip, int? take)
        {
            var query =
                from job in _IJobRepo.Get()
                where job.Owner.ID == id
                select job;

            var results = query
                            .Skip(skip ?? default_skip)
                            .Take(take ?? default_take)
                            .ToList();
             
            return Json(results);
        }

        [HttpGet]
        [Route("/Jobs/{id}/")]
        public ActionResult GetJobById(long id)
        {
            var result =
                _IJobRepo
                .Get()
                .Where(j => j.ID == id);
                
            return Json(result);
        }

        [HttpPost]
        [Route("Bids/")]
        public void CreateBid(IBid bid)
        {
            _IBidRepo.Add(bid);
            _IBidRepo.Save();
        }

        [HttpGet]
        [Route("Owner/{id}/Bids/")]
        public ActionResult GetBidsByOwner(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                        where bid.Owner.ID == id
                        select bid;
                          

            var results = query.Skip(skip??default_skip).Take(take??default_take).ToList();
            return Json(results);
        }

        [HttpDelete]
        [Route("Bids/")]
        public void WithdrawlBid(IBid bid)
        {
            _IBidRepo.Remove(bid);
            _IBidRepo.Save();

        }

        [HttpGet]
        [Route("Jobs/{id}/Bids/")]
        public ActionResult GetBidsByJob(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                          where bid.ID == id
                          select bid;
            var results = query.Skip(skip ?? default_skip)
                                .Take(take ?? default_take)
                                .ToList();
            return Json(results);
        }

        [HttpPost]
        [Route("Jobs/AcceptBid")]
        public void AcceptBid(IBid bid)
        {
            var bids = _IBidRepo.Get().Where(b => b.Job.ID == bid.Job.ID);
            var job = _IJobRepo.Get().Where(j=>j.ID == bid.Job.ID).FirstOrDefault();

            if (job == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "No job found with the ID of " + bid.Job.ID;
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
        public void ConfirmBid(IBid Bid)
        {
            var job = _IJobRepo.Get().Where(j=>j.ID == Bid.Job.ID).FirstOrDefault();
            if (job == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.StatusDescription = "No job found with the ID of " + Bid.Job.ID;
                return;
            }
            job.Status = Jobstatus.Confirmed;
            _IJobRepo.Update(job);
            _IJobRepo.Save();
        }

      
    }
}