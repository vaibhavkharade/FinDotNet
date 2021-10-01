using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FMS.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Orders>();
        }

        public int CustomerId { get; set; }
        public string UserName { get; set; }
        public long CardNumber { get; set; }
        public DateTime ValidTill { get; set; }
        public decimal? Limit { get; set; }
        public decimal? Balance { get; set; }

        public virtual Registration UserNameNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
