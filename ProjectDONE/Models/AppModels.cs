using System;
using System.Collections.Generic;
//TODO: Setup Nav. Properties
namespace ProjectDONE.Models.AppModels
{
    public enum BidStatus
    {
        Pending,
        Accepted,
        Declined
    }

    public enum Jobstatus
    {
        Pending,
        Confirmed,
        Finished,
        Satisfied,
        Unsatisfied

    }
    public class BaseAppModel : IBaseAppModel
    {
        public DateTime CreatedOn { get; set; }
        public long CreatedByUserId { get; set; }
        public long ID { get; set; }
        public Guid TransactionID { get; set; }
    }

    public class Job : BaseAppModel, IJob
    {
        public IOwner Owner { get; set; }
        public string Title { get; set; }
        public string PublicDescription { get; set; }
        //TODO: Change to Geographical area type of some kind
        //More research is needed.
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public IDemographics Demographics { get; set; }
        public string PrivateDescription { get; set; }
        public virtual IBid AcceptedBid { get; set; }
        public virtual IList<IMedia> Media { get; set; }
        public virtual IList<IDialog> Dialog { get; set; }
        public virtual IList<IBid> Bids { get; set; }
        public virtual Jobstatus Status { get; set; }
    }

    public class Dialog
    {
        //TODO: Implement Question/ Answer
    }

    public class Demographics : IDemographics
    {
        public virtual IList<Address> Addresses { get; set; }
        public virtual IList<PhoneNumber> PhoneNumbers { get; set; }
        public virtual IList<Email> EmailAddresses { get; set; }
    }

    public class Address : BaseAppModel
    {
        //TODO: Implement as necessery
    }

    public class PhoneNumber : BaseAppModel
    {
        //TODO: Implment as necessery
    }

    public class Email : BaseAppModel
    {
        //TODO: Implement as necessery
    }

    public class Media : BaseAppModel, IMedia
    {
        public string MIME_TYPE { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Meta { get; set; }
    }

    public class Owner : BaseAppModel, IOwner
    {
        public string Name { get; set; }
        public bool IsCorporateEntity { get; set; }
        public virtual IList<Media> Media { get; set; }
        public virtual IList<Job> Jobs { get; set; }
        public virtual IList<Bid> Bids { get; set; }
    }

    // Add Media[] for bids
    public class Bid : BaseAppModel, IBid
    {
        public decimal Amount { get; set; }
        public virtual IOwner Owner { get; set; }
        public virtual IList<IDialog> Dialog { get; set; }
        public virtual IJob Job { get; set; }
        public virtual BidStatus Status { get; set; }
    }


    //Interfaces

    public interface IBaseAppModel
    {
        long CreatedByUserId { get; set; }
        DateTime CreatedOn { get; set; }
        long ID { get; set; }
        Guid TransactionID { get; set; }
    }

    public interface IDemographics
    {
        IList<Address> Addresses { get; set; }
        IList<Email> EmailAddresses { get; set; }
        IList<PhoneNumber> PhoneNumbers { get; set; }
    }

    public interface IJob : IBaseAppModel
    {
        IOwner Owner { get; set; }
        IDemographics Demographics { get; set; }
        long Latitude { get; set; }
        long Longitude { get; set; }
        IList<IMedia> Media { get; set; }
        string PrivateDescription { get; set; }
        string PublicDescription { get; set; }
        string Title { get; set; }
        IList<IDialog> Dialog { get; set; }
        IList<IBid> Bids { get; set; }
        IBid AcceptedBid { get; set; }
        Jobstatus Status { get; set; }

    }

    public interface IMedia : IBaseAppModel
    {
        string Meta { get; set; }
        string MIME_TYPE { get; set; }
        string Title { get; set; }
        string URL { get; set; }
    }

    public interface IOwner : IBaseAppModel
    {
        bool IsCorporateEntity { get; set; }
        IList<Media> Media { get; set; }
        string Name { get; set; }
        IList<Job> Jobs { get; set; }
        IList<Bid> Bids { get; set; }
    }

    public interface IBid : IBaseAppModel
    {
        decimal Amount { get; set; }
        IJob Job { get; set; }
        IOwner Owner { get; set; }
        BidStatus Status { get; set; }
    }

    public interface IDialog : IBaseAppModel
    {

    }
}