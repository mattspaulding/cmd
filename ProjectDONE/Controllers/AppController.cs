using ProjectDONE.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectDONE.Models.AppModels;

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
        private IFactory_IJobRepo _IJobRepo;
        private IFactory_IBidRepo _IBidRepo;
        

        public AppController() // :this(new JobRepo())
        {
            //TODO: New up a concrete factory for _IJobRepo
            //TODO: New up a concrete factory for _IBidRepo
        }

        public AppController(IFactory_IJobRepo JobRepo, IFactory_IBidRepo BidRepo)
        {
            _IJobRepo = JobRepo;
            _IBidRepo = BidRepo;
        }
        [HttpPost]
        [Route("Job/")]
        public void AddJob(Job job)
        {
            _IJobRepo.Add(job);
        }

        [HttpGet]
        [Route("Owner/{id}/Jobs/")]
        public ActionResult GetJobsByOwner(long id)
        {
            var results =
                _IJobRepo
                .GetByOwner(id, default_skip, default_take);
                
            return Json(results);
        }
        [HttpGet]
        [Route("/Jobs/{id}/")]
        public ActionResult GetJobById(long id)
        {
            var result =
                _IJobRepo
                .GetSingle(id);
            return Json(result);
        }

        [HttpPost]
        [Route("Bids/")]
        public void CreateBid(Bid bid)
        {
            _IBidRepo.Add(bid);
        }

        [HttpGet]
        [Route("Owner/{id}/Bids/")]
        public ActionResult GetBidsByOwner(long id)
        {
            var results = _IBidRepo.GetByOwner(id,default_skip,default_take);
            return Json(results);
        }
        [HttpDelete]
        [Route("Bids/")]
        public void WithdrawlBid(Bid bid)
        {
            _IBidRepo.Remove(bid);
        }
        [HttpGet]
        [Route("Jobs/{id}/Bids/")]
        public ActionResult GetBidsByJob(long id)
        {
            var results = _IBidRepo.GetByJob(id,default_skip,default_take);
            return Json(results);
        }
        [HttpPost]
        [Route("Jobs/{jobId}/Bids/{bidId}/Accept")]
        public void AcceptBid(long jobId, long bidId)
        {
            var job = _IJobRepo.GetSingle(jobId);
            var bids = (List<IBid>)_IBidRepo.GetByJob(jobId,default_skip,int.MaxValue);

            job.AcceptedBid = bids.FirstOrDefault(b => b.ID == bidId);
            job.AcceptedBid.Status = BidStatus.Accepted;

            _IBidRepo.Update(job.AcceptedBid);
            _IJobRepo.Update(job);

            foreach (var bid in bids.Where(b => b.ID != bidId))
            {
                bid.Status = BidStatus.Declined;
                _IBidRepo.Update(bid);
            }
            
        }
    }
}