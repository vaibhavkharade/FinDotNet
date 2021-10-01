using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FMS.Models
{
    public partial class Registration
    {
        public Registration()
        {
            Customer = new HashSet<Customer>();
        }

        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public long Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string CardType { get; set; }
        public string Bank { get; set; }
        public long Account { get; set; }
        public string Ifsc { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
    }
}
