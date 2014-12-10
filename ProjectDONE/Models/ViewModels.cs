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
    
    public class JobViewModel : BaseViewModel
    {
        public JobViewModel() { }

        public BidViewModel AcceptedBid { get; internal set; }

        public long? AcceptedBid_ID { get;  set; }
        public List<BidViewModel> Bids { get;  set; }
        public object Demographics { get;  set; }
        public long Latitude { get;  set; }
        public long Longitude { get;  set; }
        public OwnerViewModel Owner { get;  set; }
        public long Owner_Id { get;  set; }
        public string PrivateDescription { get;  set; }
        public string PublicDescription { get;  set; }
        public Jobstatus Status { get;  set; }
        public string Title { get;  set; }

      
    }

    public class BidViewModel : BaseViewModel
    {
        public BidViewModel()
        {

        }
      
        public decimal Amount { get;  set; }
        public JobViewModel Job { get;  set; }
        public long Job_ID { get;  set; }
        public OwnerViewModel Owner { get;  set; }
        public BidStatus Status { get;  set; }

       
    }

    public class OwnerViewModel : BaseViewModel
    {
        public OwnerViewModel()
        {

        }
       

        public List<BidViewModel> Bids { get;  set; }
        public bool IsCorporateEntity { get;  set; }
        public List<JobViewModel> Jobs { get;  set; }
        public string Name { get;  set; }
    }

    public class DemographicsViewModel : BaseViewModel
    {
        public DemographicsViewModel(Demographics source)
        {

        }
    }
}