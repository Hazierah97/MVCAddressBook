using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCAddressBook
{
    public class AddressValidation
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please provide Name", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Please provide Surname", AllowEmptyStrings = false)]
        public string Surname { get; set; }

        [Display(Name = "Address 1")]
        [Required(ErrorMessage = "Please provide Address", AllowEmptyStrings = false)]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")] //optional to fill in
        public string Address2 { get; set; }

        [Display(Name = "Postcode")]
        [Required(ErrorMessage = "Please provide Postcode", AllowEmptyStrings = false)]
        public string Postcode { get; set; }

        [Display(Name = "Town")]
        [Required(ErrorMessage = "Please provide Town", AllowEmptyStrings = false)]
        public string Town { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please provide Country", AllowEmptyStrings = false)]
        public int CountryID { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Email not valid")]
        public string Email { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Please provide Mobile Number", AllowEmptyStrings = false)]
        public string MobileNumber { get; set; }
    }

    [MetadataType(typeof(AddressValidation))] //apply validation
    public partial class Address
    {

    }

}