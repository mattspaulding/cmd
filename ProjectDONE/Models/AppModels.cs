using System;
using System.Collections.Generic;

namespace ProjectDONE.Models.AppModels
{
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
        public IMedia[] Media { get; set; }
        //TODO: Change to Geographical area type of some kind
        //More research is needed.
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public IDemographics Demographics { get; set; }
        public string PrivateDescription { get; set; }
        public IDialog Dialog { get; set; }
    }

    public class Dialog
    {
        //TODO: Implemnte Question/ Answer
    }

    public class Demographics : IDemographics
    {
        public Address[] Addresses { get; set; }
        public PhoneNumber[] PhoneNumbers { get; set; }
        public Email[] EmailAddresses { get; set; }
    }

    public class Address : BaseAppModel
    {
        //TODO: Implement as necessery
    }

    public class PhoneNumber : BaseAppModel
    {
        //TODO: Implment as necessery
    }

    public class Email :BaseAppModel
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
        public Media[] Media { get; set; }
       
    }

    public class Bid : BaseAppModel, IBid
    {
        public decimal Amount { get; set; }
        public Job Job { get; set; }
        public Owner Owner { get; set; }
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
        Address[] Addresses { get; set; }
        Email[] EmailAddresses { get; set; }
        PhoneNumber[] PhoneNumbers { get; set; }
    }

    public interface IJob : IBaseAppModel
    {
        IOwner Owner { get; set; }
        IDemographics Demographics { get; set; }
        long Latitude { get; set; }
        long Longitude { get; set; }
        IMedia[] Media { get; set; }
        string PrivateDescription { get; set; }
        string PublicDescription { get; set; }
        string Title { get; set; }
        IDialog Dialog { get; set; }
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
        Media[] Media { get; set; }
        string Name { get; set; }
    }

    public interface IBid : IBaseAppModel
    {
        decimal Amount { get; set; }
        Job Job { get; set; }
        Owner Owner { get; set; }
    }

    public interface IDialog : IBaseAppModel
    {

    }
}