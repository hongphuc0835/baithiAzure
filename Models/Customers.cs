using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace baithi.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID{ get; set; }
        public string? FullName{ get; set; }

        public string? PhoneNumber { get; set; }
        public string? Password{ get; set; }
        public DateTime? Registration{ get; set; }
        public List<Rentals>? Rentals { get; set; }


    }
}