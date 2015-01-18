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
using System.Net.Http.Formatting;
using Stripe;

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
        private JobRepo JobRepo;
        private BidRepo BidRepo;
        private OwnerRepo OwnerRepo;
        private StripeTransactionRepo StripeTransactionRepo;
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected ApplicationUser AppUser { get { return UserManager.FindById(User.Identity.GetUserId()); } }
        public AppController() : this(new JobRepo(), new BidRepo(), new OwnerRepo(), new StripeTransactionRepo()) { }
        public AppController(JobRepo JobRepo, BidRepo BidRepo, OwnerRepo OwnerRepo, StripeTransactionRepo StripeTransactionRepo)
        {
            this.JobRepo = JobRepo;
            this.BidRepo = BidRepo;
            this.OwnerRepo = OwnerRepo;
            this.StripeTransactionRepo = StripeTransactionRepo;
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }


        [HttpPost]
        [Route("Job")]
        public JobViewModel CreateJob(Job job)
        {
            
            job.CreatedOn = DateTime.Now;
            job.CreatedByUserId = User.Identity.GetUserId();
            job.Owner_ID = AppUser.Owner.ID;
            job.ID = 0;
            job.Bids = null;
            job.AcceptedBid = null;
            job.AcceptedBid_ID = null;
            job.Owner = null;
            job.Media_ID = job.Media.ID;
            job.Media = null;

            if(job.Address!=null && job.Address.ID == 0)
                job.Address.CreatedByUserId = User.Identity.GetUserId();

            JobRepo.Add(job);
            JobRepo.Save();



            return new JobViewModel
            {
                ID = job.ID,
                CreatedByUserId = job.CreatedByUserId,
                CreatedOn = job.CreatedOn,
                Owner_ID = job.Owner_ID,
                Title = job.Title,
                PublicDescription = job.PublicDescription,
                Latitude = job.Latitude,
                Longitude = job.Longitude,
                PrivateDescription = job.PrivateDescription,
                Status = job.Status,
                Bids = new List<BidViewModel>(),
                AcceptedBid_ID = null,
                Owner = new OwnerViewModel { ID = AppUser.Owner.ID, Name = AppUser.Owner.Name },
                //we can use casting here because we only care 
                //if the entire address is visible, not particular attributes
                Address = (AddressViewModel)job.Address 
            };
        }

        [HttpGet]
        [Route("Job/{id}/")]
        [EnableQuery]
        public JobViewModel GetJobById(long id)
        { 
            var ownerId = AppUser.Owner.ID;
            var result =
                from job in JobRepo.Get()
                where job.ID == id
                select new JobViewModel
                {
                    ID = job.ID,
                    CreatedByUserId = job.CreatedByUserId,
                    CreatedOn = job.CreatedOn,
                    Owner_ID = job.Owner.ID,
                    Owner = new OwnerViewModel {
                        ID = job.Owner.ID,
                        CreatedOn = job.Owner.CreatedOn,
                        CreatedByUserId = job.Owner.CreatedByUserId,
                        Name = job.Owner.Name,
                        IsCorporateEntity = job.Owner.IsCorporateEntity
                    },
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
                    Address = new AddressViewModel
                    {
                        ID = job.Address.ID,
                        City = job.Address.City,
                        State = job.Address.State
                        
                    },
                    //PrivateDescription = excludePrivate ? string.Empty : job.PrivateDescription,
                    Status = job.Status,
                    AcceptedBid_ID = job.AcceptedBid_ID,
                    AcceptedBid = job.AcceptedBid ==null?null :
                    new BidViewModel
                    {
                        Amount = job.AcceptedBid.Amount,
                        CreatedByUserId = job.AcceptedBid.CreatedByUserId,
                        CreatedOn = job.AcceptedBid.CreatedOn,
                        ID = job.AcceptedBid.ID,
                        Job_ID = job.AcceptedBid.Job_ID,
                        Status = job.AcceptedBid.Status,
                        Owner = new OwnerViewModel
                        {
                            ID = job.AcceptedBid.Owner.ID,
                            CreatedOn = job.AcceptedBid.Owner.CreatedOn,
                            CreatedByUserId = job.AcceptedBid.Owner.CreatedByUserId,
                            Name = job.AcceptedBid.Owner.Name,
                            IsCorporateEntity = job.AcceptedBid.Owner.IsCorporateEntity
                        }
                    },
                    Media_ID = job.Media_ID,
                    Media = job.Media
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
            var query =
                from job in JobRepo.Get()
                let ownerId = oid
                select new JobViewModel
                {
                    ID = job.ID,
                    CreatedByUserId = job.CreatedByUserId,
                    CreatedOn = job.CreatedOn,
                    Owner_ID = job.Owner.ID,
                    Owner = new OwnerViewModel
                    {
                        Name = job.Owner.Name,
                        ID = job.Owner.ID
                    },
                    Title = job.Title,
                    PublicDescription = job.PublicDescription,
                    Latitude = job.Latitude,
                    Longitude = job.Longitude,
                    PrivateDescription = ownerId != job.Owner_ID ?
                                    string.Empty : job.PrivateDescription,
                    Status = job.Status,
                    AcceptedBid_ID =
                            (ownerId != job.Owner_ID) &&
                            job.AcceptedBid_ID != null ? job.AcceptedBid_ID : null,
                    Address = new AddressViewModel
                    {
                        ID = job.Address.ID,
                        City = job.Address.City,
                        State = job.Address.State,
                        Zip = job.Address.Zip,
                        Line1 = (ownerId == job.Owner_ID) ? job.Address.Line1 : null,
                        Line2 = (ownerId == job.Owner_ID) ? job.Address.Line2 : null,
                        
                    },
                    Media_ID = job.Media_ID,
                    Media = job.Media
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
            BidRepo.Add(bid);
            BidRepo.Save();

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

        [HttpPost]
        [Route("Job/{jobId}/MakePayment")]
        public HttpResponseMessage MakePayment([FromBody]StripeToken token,long jobId)
        {
            HttpResponseMessage response;
            var job = JobRepo.Get().Where(j => j.ID == jobId).FirstOrDefault();
            if(job==null)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("The Job could not be found in the database");
                return response;
            }

            var myCharge = new StripeChargeCreateOptions();
            myCharge.Amount = (int)(job.AcceptedBid.Amount*100);
            myCharge.Currency = "usd";
            myCharge.Description = job.Title;
            myCharge.TokenId = token.Id;

            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(myCharge);

            var raw_stripeTransaction = JsonConvert.SerializeObject(stripeCharge);
            var raw_meta = JsonConvert.SerializeObject(stripeCharge.Metadata);

            var transaction = new Models.AppModels.StripeTransaction {
                Amount = stripeCharge.Amount,
                Paid = stripeCharge.Paid,
                ReciptEmail = stripeCharge.ReceiptEmail,
                CustomerId = stripeCharge.CustomerId,
                FailureCode = stripeCharge.FailureCode,
                FailureMessage = stripeCharge.FailureMessage,
                RawStripeTransaction = raw_stripeTransaction,
                MetaData = raw_meta,
                CreatedByUserId = AppUser.Owner.CreatedByUserId,
                ID = job.AcceptedBid.ID
                
            };
            
                StripeTransactionRepo.Add(transaction);
                StripeTransactionRepo.Save();

            
           


            job.Status = Jobstatus.Satisfied;
            JobRepo.Save();

            

            response = new HttpResponseMessage(HttpStatusCode.OK);
            //TODO: Should respond with what the total was of the payment
            //sort of like a recipt
            response.Content = new StringContent("Payment successfully made. Quack Quack.");
            return response;
        }

        [HttpGet]
        [Route("Owner/Bid")]
        [EnableQuery]
        public IQueryable<BidViewModel> GetBidsByOwner(long id, int? skip, int? take)
        {

            var query = from bid in BidRepo.Get()
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
                               Owner_ID = bid.Job.Owner.ID,
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
        [Route("Bid/{bidId}/Withdrawl")]
        public HttpResponseMessage WithdrawlBid(long bidId)
        {
            var  bid = BidRepo.Get().Where(b => b.ID == bidId).FirstOrDefault();
            HttpResponseMessage response;
            if(bid==null)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("The bid could not be found in the database");
                return response;
            }
            if(AppUser.Owner.ID != bid.Owner_ID)
            {
                response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Content = new StringContent("The logged in User is not the owner of this bid.");
                return response;
            }
            
            BidRepo.Remove(bid);
            BidRepo.Save();
            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("Bid successfully withdrawn.");
            return response;
        }

        [HttpGet]
        [Route("Job/{id}/Bids/")]
        public IQueryable<Bid> GetBidsByJob(long id, int? skip, int? take)
        {
            var query = from bid in BidRepo.Get()
                        where bid.ID == id
                        select bid;

            return query;
        }

        [HttpPost]
        [Route("Job/{id}/Finish/")]
        public HttpResponseMessage FinishJob(long id)
        {
            HttpResponseMessage response;
            //Get the job
            var job = JobRepo.Get().Where(j => j.ID == id).FirstOrDefault();
            if(job== null)
            {
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("No job found with the ID of " + id);
                return response;
            }

            //Current user must be the winner of the accepted bid
            
            if(!(job.AcceptedBid != null ? job.AcceptedBid.Owner_ID == AppUser.Owner.ID : false))
            {
                var x = AppUser.Owner;
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.Content = new StringContent("User is not the owner of the accepted bid and is therefore unauthorized to finalize job");
                return response;
            }

            job.Status = Jobstatus.Finished;
            JobRepo.Save();
            response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("Job is now marked as finished.");
            return response;
        }

        //The job poster accepts a bid
        [HttpPost]
        [Route("Bid/{BidId}/Accept")]
        public HttpResponseMessage AcceptBid(long BidId)
        {
            var bid = BidRepo.Get().Where(b => b.ID == BidId).FirstOrDefault();
            var bids = BidRepo.Get().Where(b => b.Job.ID == bid.Job.ID && b.ID!= BidId);
            var job = JobRepo.Get().Where(j => j.ID == bid.Job.ID).FirstOrDefault();
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

            job.AcceptedBid_ID = bid.ID;
            bid.Status = BidStatus.Accepted;

            BidRepo.Update(bid);
            JobRepo.Update(job);

            foreach (var b in bids.Where(b => b.ID != bid.ID))
            {
                b.Status = BidStatus.Declined;
                BidRepo.Update(b);
            }
            BidRepo.Save();
            JobRepo.Save();

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

        //The bid poster confirms the bid, effectivly saying they will do the work
        //this is the last step before the actual work is done
        [HttpPost]
        [Route("Bid/{BidId}/Confirm")]
        public HttpResponseMessage ConfirmBid(long BidId)
        {
            var bid = BidRepo.Get().Where(b => b.ID == BidId).FirstOrDefault();
            var job = JobRepo.Get().Where(j => j.ID == bid.Job.ID).FirstOrDefault();

            job.Status = Jobstatus.Confirmed;
            JobRepo.Update(job);
            JobRepo.Save();
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StringContent("The job and bid have been confirmed.");
            return result;
        }

        [HttpGet]
        [Route("Owner")]
        public OwnerViewModel GetCurrentOwner()
        {
            return new OwnerViewModel
            {
                ID = AppUser.Owner.ID,
                Name = AppUser.Owner.Name
            };
        }
    }
}