using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectDONE.Controllers.Api;
using Moq;
using ProjectDONE.Data;
using ProjectDONE.Models.AppModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;

namespace ProjectDONE.Tests.Controllers
{
    [TestClass]
    public class AppControllerTests
    {
        const int default_take = 10;
        const int default_skip = 0;
        //As a User I expect to be able to post jobs.
        [TestMethod]
        public void CreateJob()
        {
            var mock_job = new Job { ID = 5 };

            var mock_IJobRepo = new Mock<IFactory_IJobRepo>();
            mock_IJobRepo.Setup(jr => jr.Add(mock_job));

            var job_repo = mock_IJobRepo.Object;

            AppController controller = new AppController(job_repo, null);
            controller.AddJob(mock_job);

            mock_IJobRepo.Verify(jr => jr.Add(mock_job), Times.Once());
        }

        //As a User, I expect to see a list of all my posted jobs
        [TestMethod]
        public void GetJobsForUser()
        {
            //TODO: verify routes as well
            //This should be a HTTP GET at /api/App/Owner/5/Jobs

           
            var mock_return_jobs = new List<IJob> { new Job { ID = 1 }, new Job { ID = 2 }, new Job { ID = 3 } };
            var mock_job_owner = new Owner { ID = 5 };
            var mock_IJobRepo = Mock.Of<IFactory_IJobRepo>(
                jb =>
                jb.GetByOwner(mock_job_owner.ID, default_skip, default_take)
                == mock_return_jobs
            );

            AppController controller = new AppController(mock_IJobRepo, null);
            var result = controller.GetJobsByOwner(mock_job_owner.ID);

            Mock.Get<IFactory_IJobRepo>(mock_IJobRepo)
                .Verify(jr => jr.GetByOwner(mock_job_owner.ID, default_skip, default_take),
                Times.Once);

            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.JsonResult), "Return value is not a JsonResult");
            var result_json = ((System.Web.Mvc.JsonResult)result).Data as List<IJob>;
            Assert.IsNotNull(result_json);
            Assert.AreEqual(result_json, mock_return_jobs);

        }

        //As a User, I am able to view a single job
        [TestMethod]
        public void GetJobById()
        {
            IJob mock_job = new Job { ID = 1 };

            var mock_IJobRepo = Mock.Of<IFactory_IJobRepo>(
                jr => jr.GetSingle(mock_job.ID) == mock_job
                );

            var controller = new AppController(mock_IJobRepo, null);
            var result = controller.GetJobById(mock_job.ID);

            Mock.Get<IFactory_IJobRepo>(mock_IJobRepo)
               .Verify(jr => jr.GetSingle(mock_job.ID),
               Times.Once);

            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.JsonResult), "Return value is not a JsonResult");
            var result_json = ((System.Web.Mvc.JsonResult)result).Data as IJob;
            Assert.IsNotNull(result_json);
            Assert.AreEqual(result_json, mock_job);

        }

        //As a User, I am able to place bids on a job
        [TestMethod]
        public void CreateBid()
        {
            //I think creating a bid should also take the 
            //Job and do some validation before adding the bid
            //to protect both the job and the bid from to much
            //meddling.

            //TODO: Prevent bidding more than once

            var mock_Job = new Job { ID = 1 };
            var mock_Bid = new Bid { ID = 1, Job = mock_Job };
            var mock_IBidRepo = new Mock<IFactory_IBidRepo>();
            mock_IBidRepo.Setup(br => br.Add(mock_Bid));
            var bidRepo = mock_IBidRepo.Object;
            AppController controller = new AppController(null, bidRepo);
            controller.CreateBid(mock_Bid);

            mock_IBidRepo.Verify(jr => jr.Add(mock_Bid), Times.Once());
        }

        //As a User, I am able to get a list of all jobs I have bids on along with my bid
        [TestMethod]
        public void GetMy_BidOn_Jobs()
        {
            var mock_Owner = new Owner { ID = 5 };
            var mock_Job = new Job { ID = 1 };
            var mock_Bid = new Bid { ID = 1, Job = mock_Job, Owner = mock_Owner };
            
            var bids = new List<IBid> { mock_Bid };
            var mock_IBidRepo = Mock
                .Of<IFactory_IBidRepo>(
                    br => br.GetByOwner(mock_Owner.ID, default_skip, default_take) == bids
                );
            var controller = new AppController(null, mock_IBidRepo);
            var result = controller.GetBidsByOwner(mock_Owner.ID);

            Mock.Get<IFactory_IBidRepo>(mock_IBidRepo)
              .Verify(jr => jr.GetByOwner(mock_Owner.ID,default_skip,default_take),
              Times.Once);

            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.JsonResult), "Return value is not a JsonResult");
            var result_json = ((System.Web.Mvc.JsonResult)result).Data as List<IBid>;
            Assert.IsNotNull(result_json);
            Assert.AreEqual(result_json, bids);
        }

        //As a User, I am able to see all the bids on my posted job
        [TestMethod]
        public void GetAllBidsByJob()
        {
            var mock_job = new Job { ID = 1 };
            var mock_bid_1 = new Bid { ID = 1, Job = mock_job };
            var mock_bid_2 = new Bid { ID = 2, Job = mock_job };
            var mock_bid_3 = new Bid { ID = 3, Job = mock_job };
            var bids = new List<IBid> { mock_bid_1, mock_bid_2, mock_bid_3 };

            var mock_IBidRepo = Mock.Of<IFactory_IBidRepo>(
                    br => br.GetByJob(mock_job.ID, default_skip,default_take) == bids
                );

            var controller = new AppController(null, mock_IBidRepo);
            var result = controller.GetBidsByJob(mock_job.ID);

            Mock.Get<IFactory_IBidRepo>(mock_IBidRepo)
            .Verify(br => br.GetByJob(mock_job.ID, default_skip, default_take),
            Times.Once);

            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.JsonResult), "Return value is not a JsonResult");
            var result_json = ((System.Web.Mvc.JsonResult)result).Data as List<IBid>;
            Assert.IsNotNull(result_json);
            Assert.AreEqual(result_json, bids);

        }

        //As a User, I am able to withdrawl my unaccepted bid
        [TestMethod]
        public void WithdrawlBid()
        {
            var mock_owner = new Owner { ID = 5 };
            var mock_Bid = new Bid { ID = 1, Owner = mock_owner };
            var mock_IBidRepo = new Mock<IFactory_IBidRepo>();
            mock_IBidRepo.Setup(br => br.Remove(mock_Bid));
            var bidRepo = mock_IBidRepo.Object;

            var controller = new AppController(null, bidRepo);
            controller.WithdrawlBid(mock_Bid);

            mock_IBidRepo.Verify(br => br.Remove(mock_Bid), Times.Once());

        }

        //As a User, I am able to accept a bid
        [TestMethod]
        public void AcceptBid()
        {
            //var mock_job = new Job { ID = 1 , AcceptedBid = null};
            //var mock_bid = new Bid { ID = 1 , Job = mock_job, Status = BidStatus.Pending};

            //var mock_IJobRepo = new Mock<IFactory_IJobRepo>();
            //var mock_IBidRepo = new Mock<IFactory_IBidRepo>();

            //var bidRepo = mock_IBidRepo.Object;
            //var jobRepo = mock_IJobRepo.Object;

            //var controller = new AppController(jobRepo, bidRepo);

            //controller.AcceptBid(mock_job.ID, mock_bid.ID);

            //mock_IJobRepo.Verify(
            //    jr => jr.UpdateJob(new Job { ID=1, AcceptedBid = mock_job})
            //    ,Times.Once);

            //mock_IBidRepo.Verify(
            //    br => br.UpdateBid(
            //        new Bid { ID= 1, Job = mock_job, Status = BidStatus.Accepted})
            //    ,Times.Once);

        }

        //As a User, I am able to confirm a Job (happens after a bid has been accepted)
        [TestMethod]
        public void ConfirmJob()
        {
            Assert.IsTrue(false);
        }

        //As a User, I am able to get a jobs private details of Jobs I have confirmed
        [TestMethod]
        public void JobDetails()
        {
            Assert.IsTrue(false);
        }

        //As a User who confirmed a job, I am able to claim that it is finished
        [TestMethod]
        public void ClaimFinishJob()
        {
            Assert.IsTrue(false);
        }

        //As a User who posted a job that another user has claimed to be finished, I am able to Verify the job was done satisfactorily
        [TestMethod]
        public void FinishedJob_Satisfied()
        {
            Assert.IsTrue(false);
        }

        //As a User who posted a job that another user has claimed to be finished, I am able to Verify the job was done un-satisfactorily
        [TestMethod]
        public void FinishedJob_UnSatisifed()
        {
            Assert.IsTrue(false);
        }

        //As a User who posted a job that is now finished, I must pay the user who finished it via the app
        [TestMethod]
        public void FinalizeJob()
        {
            Assert.IsTrue(false);
        }

        //As a User who finished a Job, I can review the interaction
        [TestMethod]
        public void Review_FinshedJob()
        {
            Assert.IsTrue(false);
        }

        //As a User who finished a Job, I can review the user who posted the job
        [TestMethod]
        public void Review_JobPostingUser()
        {
            Assert.IsTrue(false);
        }

        //As a User who posted a now Finalized job, i can review the user who finished the job
        [TestMethod]
        public void Review_JobFinishingUser()
        {
            Assert.IsTrue(false);
        }
    }
}
