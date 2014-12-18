using System.Collections.Generic;

namespace ProjectDONE.Models.AppModels
{
    public interface IDemographics
    {
        IList<Address> Addresses { get; set; }
        IList<Email> EmailAddresses { get; set; }
        IList<PhoneNumber> PhoneNumbers { get; set; }
    }
}