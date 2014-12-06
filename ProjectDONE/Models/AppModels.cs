﻿using System;
using System.Collections.Generic;
//TODO: Come back around for testing
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
        public DateTime CreatedOn { get; set; }
        public long CreatedByUserId { get; set; }

        public long ID { get; set; }
        public Guid TransactionID { get; set; }
    }

    public class Job : BaseAppModel
    {
        public Owner Owner { get; set; }
        public string Title { get; set; }
        public string PublicDescription { get; set; }
        //TODO: Change to Geographical area type of some kind
        //More research is needed.
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public Demographics Demographics { get; set; }
        public string PrivateDescription { get; set; }
        public virtual Bid AcceptedBid { get; set; }
        public virtual List<Media> Media { get; set; }
        public virtual List<Dialog> Dialog { get; set; }
        public virtual List<Bid> Bids { get; set; }
        public virtual Jobstatus Status { get; set; }
    }

    public class Dialog : BaseAppModel
    {
        //TODO: Implement Question/ Answer
    }

    public class Demographics : BaseAppModel
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

    // Add Media[] for bids
    public class Bid : BaseAppModel
    {
        public decimal Amount { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual IList<Dialog> Dialog { get; set; }
        public virtual Job Job { get; set; }
        public virtual BidStatus Status { get; set; }
    }




  
}