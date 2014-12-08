using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDONE.Models
{
    public class BaseViewModel
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public long ID { get; set; }
    }
    //TODO: Automapper would be nice in here
    public class JobViewModel : BaseViewModel
    {
        public JobViewModel() { }
        public JobViewModel(Job source, long ownerID_privateInclusion)
        {
            var excludePrivate =
                ownerID_privateInclusion == source.Owner_ID ||
                (source.AcceptedBid != null && source.AcceptedBid.Owner_ID == ownerID_privateInclusion);

            base.ID = source.ID;
            base.CreatedByUserId = source.CreatedByUserId;
            base.CreatedOn = source.CreatedOn;

            this.Owner_Id = source.Owner_ID??0;
            this.Title = source.Title;
            this.PublicDescription = source.PublicDescription;
            this.Latitude = source.Latitude;
            this.Longitude = source.Longitude;
            this.PrivateDescription =excludePrivate ? string.Empty : source.PrivateDescription;
            this.Status = source.Status;
            this.AcceptedBid = !excludePrivate && source.AcceptedBid != null ? new BidViewModel(source.AcceptedBid) : null;
            this.Bids = source.Bids != null ? BidViewModel.CreateFrom(source.Bids) : null;
            this.Demographics = source.Demographics == null ? new DemographicsViewModel(source.Demographics) : null;
            this.Owner = source.Owner != null ? new OwnerViewModel(source.Owner) : null;
        }

        public BidViewModel AcceptedBid { get;  set; }
        public IQueryable<BidViewModel> Bids { get;  set; }
        public object Demographics { get;  set; }
        public long Latitude { get;  set; }
        public long Longitude { get;  set; }
        public OwnerViewModel Owner { get;  set; }
        public long Owner_Id { get;  set; }
        public string PrivateDescription { get;  set; }
        public string PublicDescription { get;  set; }
        public Jobstatus Status { get;  set; }
        public string Title { get;  set; }

        internal static IQueryable<JobViewModel> CreateFrom(List<Job> jobs, long ownerID_privateInclusion)
        {
            var retval = new List<JobViewModel>();
            jobs.ForEach(j => retval.Add(new JobViewModel(j, ownerID_privateInclusion)));
            return retval as IQueryable<JobViewModel>;
        }
    }

    public class BidViewModel : BaseViewModel
    {
        public BidViewModel(Bid source, bool includeChildren = false)
        {
            base.ID = source.ID;
            base.CreatedByUserId = source.CreatedByUserId;
            base.CreatedOn = source.CreatedOn;
            this.Amount = source.Amount;
            this.Owner = new OwnerViewModel(source.Owner);
            this.Job_ID = source.Job_ID;
            this.Status = source.Status;
            if(includeChildren)
            {
                this.Job = new JobViewModel(source.Job, source.Owner_ID);
            }
        }
        public decimal Amount { get; private set; }
        public JobViewModel Job { get; private set; }
        public long Job_ID { get; private set; }
        public OwnerViewModel Owner { get; private set; }
        public BidStatus Status { get; private set; }

        internal static IQueryable<BidViewModel> CreateFrom(List<Bid> bids, bool excludePrivate = true, bool includeChildren = false)
        {
            var retval = new List<BidViewModel>();
            bids.ForEach(b => retval.Add(new BidViewModel(b, includeChildren)));
            return retval as IQueryable<BidViewModel>;
        }
    }

    public class OwnerViewModel : BaseViewModel
    {
        public OwnerViewModel(Owner source,bool excludePrivate = true, bool includeChildren = false)
        {
            
            base.ID = source.ID;
            base.CreatedOn = source.CreatedOn;
            base.CreatedByUserId = source.CreatedByUserId;
            this.Name = source.Name;
            this.IsCorporateEntity = source.IsCorporateEntity;
            //media
            if(includeChildren)
            {
                this.Jobs = JobViewModel.CreateFrom(source.Jobs.ToList(), source.ID);
                this.Bids = BidViewModel.CreateFrom(source.Bids.ToList());
            }
        }

        public IQueryable<BidViewModel> Bids { get; private set; }
        public bool IsCorporateEntity { get; private set; }
        public IQueryable<JobViewModel> Jobs { get; private set; }
        public string Name { get; private set; }
    }

    public class DemographicsViewModel : BaseViewModel
    {
        public DemographicsViewModel(Demographics source)
        {

        }
    }
}