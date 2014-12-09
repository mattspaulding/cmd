﻿using ProjectDONE.Data;
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
using System.Net.Http.Formatting;

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
        private OwnerRepo _IOwnerRepo;
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser AppUser { get { return UserManager.FindById(User.Identity.GetUserId()); } }

        public AppController() : this(new JobRepo(), new BidRepo(), new OwnerRepo()) { }

        public AppController(JobRepo JobRepo, BidRepo BidRepo, OwnerRepo OwnerRepo)
        {
            _IJobRepo = JobRepo;
            _IBidRepo = BidRepo;
            _IOwnerRepo = OwnerRepo;

            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }


        [HttpPost]
        [Route("Job")]
        public JobViewModel AddJob(Job job)
        {
            
            job.CreatedOn = DateTime.Now;
            job.CreatedByUserId = User.Identity.GetUserId();
            job.Owner_ID = AppUser.Owner.ID;
            job.ID = 0;
            _IJobRepo.Add(job);
            _IJobRepo.Save();
            
            
            
            return new JobViewModel
            {
                ID = job.ID,
                CreatedByUserId = job.CreatedByUserId,
                CreatedOn = job.CreatedOn,
                Owner_Id = job.Owner_ID,
                Title = job.Title,
                PublicDescription = job.PublicDescription,
                Latitude = job.Latitude,
                Longitude = job.Longitude,
                PrivateDescription = job.PrivateDescription,
                Status = job.Status,
                Bids = new List<BidViewModel>(),
                AcceptedBid_ID = null,
                Owner = new OwnerViewModel {ID = AppUser.Owner.ID, Name = AppUser.Owner.Name }
            };
        }

        [HttpGet]
        [Route("Job/{id}/")]
        [EnableQuery]
        public JobViewModel GetJobById(long id)
        { 
            var ownerId = AppUser.Owner.ID;
            var result =
                from job in _IJobRepo.Get()
                where job.ID == id
                select new JobViewModel
                {
                    ID = job.ID,
                    CreatedByUserId = job.CreatedByUserId,
                    CreatedOn = job.CreatedOn,
                    Owner_Id = job.Owner.ID,
                    Title = job.Title,
                    PublicDescription = job.PublicDescription,
                    Latitude = job.Latitude,
                    Longitude = job.Longitude,
                    Bids = (from b in job.Bids select new BidViewModel
                    {
                        Amount = b.Amount,
                        CreatedByUserId = b.CreatedByUserId,
                        CreatedOn = b.CreatedOn,
                        ID = b.ID,
                        Job_ID = b.Job_ID,
                        Status = b.Status,
                        Owner = new OwnerViewModel
                        {
                            ID = b.Owner.ID,
                            CreatedOn = b.Owner.CreatedOn,
                            CreatedByUserId = b.Owner.CreatedByUserId,
                            Name = b.Owner.Name,
                            IsCorporateEntity = b.Owner.IsCorporateEntity
                        }

                    }).ToList() ,
                    //PrivateDescription = excludePrivate ? string.Empty : job.PrivateDescription,
                    Status = job.Status
                    //AcceptedBid = !excludePrivate && source.AcceptedBid != null ? new BidViewModel(source.AcceptedBid) : null,
                };

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get a list of all jobs that belong to
        /// the ownermaking the request
        /// </summary>
        [HttpGet]
        [Route("Job")]
        [EnableQuery]
        public IQueryable<JobViewModel> GetJobs()
        {
            var oid = AppUser.Owner.ID;
            //Future self: obviously...i hate you.
            //TODO: Can we use a casting overlad here?
            var query =
                from job in _IJobRepo.Get()
                let ownerId = oid
                select new JobViewModel
                {
                    ID = job.ID,
                    CreatedByUserId = job.CreatedByUserId,
                    CreatedOn = job.CreatedOn,
                    Owner_Id = job.Owner.ID,
                    Title = job.Title,
                    PublicDescription = job.PublicDescription,
                    Latitude = job.Latitude,
                    Longitude = job.Longitude,
                    PrivateDescription = ownerId!=job.Owner_ID ? 
                                    string.Empty : job.PrivateDescription,
                    Status = job.Status,
                    AcceptedBid_ID = 
                            (ownerId != job.Owner_ID) && 
                            job.AcceptedBid_Id != null ?job.AcceptedBid_Id : null,
                };


            return query;
        }


        /// <summary>
        /// Bid on a job, requires the JobID and the bid object
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="bid"></param>
        [HttpPost]
        [Route("Job/{JobId}/Bid")]
        public BidViewModel CreateBid(long JobId, Bid bid)
        {
            bid.Job = null;
            bid.CreatedOn = DateTime.Now;
            bid.CreatedByUserId = User.Identity.GetUserId();
            bid.Owner_ID = AppUser.Owner.ID;
            bid.Job_ID = JobId;
            _IBidRepo.Add(bid);
            _IBidRepo.Save();

            return new BidViewModel
            {
                Amount = bid.Amount,
                CreatedByUserId = bid.CreatedByUserId,
                CreatedOn = bid.CreatedOn,
                ID = bid.ID,
                Job_ID = bid.Job_ID,
                Status = bid.Status,
                Owner = new OwnerViewModel
                {
                    ID = AppUser.Owner.ID,
                    CreatedOn = AppUser.Owner.CreatedOn,
                    CreatedByUserId = AppUser.Owner.CreatedByUserId,
                    Name = AppUser.Owner.Name,
                    IsCorporateEntity = AppUser.Owner.IsCorporateEntity
                }
            };
        }

        [HttpGet]
        [Route("Bids")]
        [EnableQuery]
        public IQueryable<BidViewModel> GetBidsByOwner(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                        where bid.Owner.ID == id
                        select new BidViewModel
                        {
                            Amount = bid.Amount,
                            CreatedByUserId = bid.CreatedByUserId,
                            CreatedOn = bid.CreatedOn,
                            ID = bid.ID,
                            Job_ID = bid.Job_ID,
                            Status = bid.Status,
                            Owner = new OwnerViewModel
                            {
                                ID = AppUser.Owner.ID,
                                CreatedOn = AppUser.Owner.CreatedOn,
                                CreatedByUserId = AppUser.Owner.CreatedByUserId,
                                Name = AppUser.Owner.Name,
                                IsCorporateEntity = AppUser.Owner.IsCorporateEntity
                            },
                           Job = new JobViewModel
                           {
                               ID = bid.Job.ID,
                               CreatedByUserId = bid.Job.CreatedByUserId,
                               CreatedOn = bid.Job.CreatedOn,
                               Owner_Id = bid.Job.Owner.ID,
                               Title = bid.Job.Title,
                               PublicDescription = bid.Job.PublicDescription,
                               Latitude = bid.Job.Latitude,
                               Longitude = bid.Job.Longitude,
                               //Bids - Up in the air on this one
                               Status = bid.Job.Status
                               //AcceptedBid = !excludePrivate && source.AcceptedBid != null ? new BidViewModel(source.AcceptedBid) : null,
                           }
                        };

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
        [Route("Job/{id}/Bids/")]
        public IQueryable<Bid> GetBidsByJob(long id, int? skip, int? take)
        {
            var query = from bid in _IBidRepo.Get()
                        where bid.ID == id
                        select bid;

            return query;
        }

        [HttpPost]
        [Route("Bid/Accept/{BidId}")]
        public HttpResponseMessage AcceptBid(long BidId)
        {
            var bid = _IBidRepo.Get().Where(b => b.ID == BidId).FirstOrDefault();
            var bids = _IBidRepo.Get().Where(b => b.Job.ID == bid.Job.ID && b.ID!= BidId);
            var job = _IJobRepo.Get().Where(j => j.ID == bid.Job.ID).FirstOrDefault();
            HttpResponseMessage response;
            if (job == null)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("No job found with the ID of " + bid.Job.ID);
                return response;
            }

            if(job.Owner_ID != AppUser.Owner.ID)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Content = new StringContent("Authenticated user is not indicated as the owner of this job.");
                return response;
            }

            job.AcceptedBid_Id = bid.ID;
            bid.Status = BidStatus.Accepted;

            _IBidRepo.Update(bid);
            _IJobRepo.Update(job);

            foreach (var b in bids.Where(b => b.ID != bid.ID))
            {
                b.Status = BidStatus.Declined;
                _IBidRepo.Update(b);
            }
            _IBidRepo.Save();
            _IJobRepo.Save();

            var result = new BidViewModel
            {
                Amount = bid.Amount,
                CreatedByUserId = bid.CreatedByUserId,
                CreatedOn = bid.CreatedOn,
                ID = bid.ID,
                Job_ID = bid.Job_ID,
                Status = bid.Status,
                Owner = new OwnerViewModel
                {
                    ID = bid.Owner.ID,
                    CreatedOn = bid.Owner.CreatedOn,
                    CreatedByUserId = bid.Owner.CreatedByUserId,
                    Name = bid.Owner.Name,
                    IsCorporateEntity = bid.Owner.IsCorporateEntity
                }
            };

            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ObjectContent<BidViewModel>(result, new JsonMediaTypeFormatter(), "application/json");
            return response;
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