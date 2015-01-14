using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
    public class BaseAppModel
    {
        public DateTime? CreatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public long ID { get; set; }
        public Guid TransactionID { get; set; }
        public bool Deleted { get; set; }
    }

    //TODO Extract interace for view models
    public class Job : BaseAppModel
    {
        //TODO: Add Tagging
        public long Owner_ID { get; set; }
        [ForeignKey("Owner_ID")]
        public virtual Owner Owner { get; set; }
        public string Title { get; set; }
        public string PublicDescription { get; set; }
        //TODO: Change to Geographical area type of some kind
        //More research is needed.
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public string PrivateDescription { get; set; }
        [ForeignKey("AcceptedBid_ID")]
        public virtual Bid AcceptedBid { get; set; }
        public virtual long? AcceptedBid_ID { get; set; }
        public long Media_ID { get; set; }
        [ForeignKey("Media_ID")]
        public virtual Media Media { get; set; }
        public virtual List<Dialog> Dialog { get; set; }
        public virtual List<Bid> Bids { get; set; }
        public virtual Address Address { get; set; }
        public string AddressNotes { get; set; }
        public DateTime? Earliest { get; set; }
        public DateTime? DoneBy { get; set; }
        public decimal MaxPay { get; set; }
        public virtual Jobstatus Status { get; set; }
    }

    public class Dialog : BaseAppModel
    {
        //TODO: Implement Question/ Answer
    }

   

   public class Address : BaseAppModel
    {
        public string Name { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

    }

    public class PhoneNumber : BaseAppModel
    {
        //TODO: Implment as necessery
    }

    public class Email : BaseAppModel
    {
        //TODO: Implement as necessery
    }

    public class Media : BaseAppModel
    {

        public string MIME_TYPE { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Meta { get; set; }
    }

    public class Owner : BaseAppModel
    {
        public string Name { get; set; }
        public bool IsCorporateEntity { get; set; }
        public virtual IList<Media> Media { get; set; }
        public virtual IList<Job> Jobs { get; set; }
        public virtual IList<Bid> Bids { get; set; }
    }

    
    public class Bid : BaseAppModel
    {
        public decimal Amount { get; set; }

        public long Owner_ID{ get; set; }
        [ForeignKey("Owner_ID")]
        public virtual Owner Owner { get; set; }

        [InverseProperty("Bids")]
        [ForeignKey("Job_ID")]
        public virtual Job Job { get; set; }
        public long Job_ID { get; set; }

        public virtual BidStatus Status { get; set; }

        public virtual IList<Dialog> Dialog { get; set; }
    }


    
}