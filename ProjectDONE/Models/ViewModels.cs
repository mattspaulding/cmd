using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDONE.Models
{
    public class BaseViewModel
    {
        public DateTime? CreatedOn { get; set; }
        public string CreatedByUserId { get; set; }
        public long ID { get; set; }
        public bool Deleted { get; set; }
        public Guid TransactionID { get; set; }
    }

    public class JobViewModel : BaseViewModel
    {
        public JobViewModel() { }
        public BidViewModel AcceptedBid { get; internal set; }
        public long? AcceptedBid_ID { get; set; }
        public AddressViewModel Address { get; set; }
        public string AddressNotes { get; set; }
        public List<BidViewModel> Bids { get; set; }
        public object Demographics { get; set; }
        public List<Dialog> Dialog{ get; set; }
        public DateTime? DoneBy{ get; set; }
        public DateTime? Earliest{ get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public decimal MaxPay { get; set; }
        public List<Media> Media{ get; set; }
        public OwnerViewModel Owner { get; set; }
        public long Owner_ID { get; set; }
        public string PrivateDescription { get; set; }
        public string PublicDescription { get; set; }
        public Jobstatus Status { get; set; }
        public string Title { get; set; }

    }

    public class BidViewModel : BaseViewModel
    {
        public BidViewModel()
        {

        }

        public decimal Amount { get; set; }


        public JobViewModel Job { get; set; }
        public long Job_ID { get; set; }
        public OwnerViewModel Owner { get; set; }

        public long Owner_ID { get; set; }

        public BidStatus Status { get; set; }       
    }

    public class OwnerViewModel : BaseViewModel
    {
        public OwnerViewModel()
        {

        }


        public List<BidViewModel> Bids { get; set; }
        public bool IsCorporateEntity { get; set; }
        public List<JobViewModel> Jobs { get; set; }

        public string Name { get; set; }

    }

   public class AddressViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public static implicit operator AddressViewModel(Address model)
        {
            var retval = new AddressViewModel();
            retval.Name = model.Name;
            retval.Line1 = model.Line1;
            retval.Line2 = model.Line2;
            retval.City = model.City;
            retval.State = model.State;
            retval.Zip = model.Zip;
            retval.ID = model.ID;
            retval.Deleted = model.Deleted;
            return retval;
        }
    }

    public class MediaViewModel : BaseViewModel
    {
        public string MIME_TYPE { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Meta { get; set; }

        public static implicit operator MediaViewModel(Media model)
        {
            var retval = new MediaViewModel();
            retval.MIME_TYPE = model.MIME_TYPE;
            retval.URL = model.URL;
            retval.Title = model.Title;
            retval.Meta = model.Meta;
            return retval;
        }
    }
}