using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCAddressBook.ViewModel
{
    public class AddressModel
    {
        public int AddressID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public int CountryID { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ImagePath { get; set; }
    }
}