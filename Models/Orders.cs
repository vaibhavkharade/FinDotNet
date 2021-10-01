using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FMS.Models
{
    public partial class Orders
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? PurchasedDate { get; set; }
        public int? EmiTenure { get; set; }
        public decimal? MonthlyEmi { get; set; }
        public int? EmiPaid { get; set; }
        public double? ProcessingFee { get; set; }
        public double? AmountPaid { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Products Product { get; set; }
    }
}
