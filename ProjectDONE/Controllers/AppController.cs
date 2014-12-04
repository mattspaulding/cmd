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
        private IFactory_IJobRepo _IJobRepo;
        private IFactory_IBidRepo _IBidRepo;
        private int default_skip = 0;
        private int default_take = 10;

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
        [Route("Job")]
        public void AddJob(Job job)
        {
            _IJobRepo.Add(job);
        }

        [HttpGet]
        [Route("Owner/{id}/Jobs")]
        public ActionResult GetJobsByOwner(long id)
        {
            var results =
                _IJobRepo
                .GetByOwner(id, this.default_skip, this.default_take);
                
            return Json(results);
        }
        [HttpGet]
        [Route("/Jobs/{id}")]
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
    }
}