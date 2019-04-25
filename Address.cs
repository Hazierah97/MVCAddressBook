//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCAddressBook
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class Address
    {
        [Key]
        public int AddressID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [StringLength(Int32.MaxValue)]
        public string Postcode { get; set; }

        public string Town { get; set; }
        public int CountryID { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ImagePath { get; set; }
    
        public virtual Country Country { get; set; }
    }
}
