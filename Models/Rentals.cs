using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace baithi.Models
{
    public class Rentals
    {
        [Key]
        public int RentalID { get; set; }
        public int CustomerID { get; set; }
        public Customers? Customers { get; set; }

        public DateTime? RentalDate{ get; set; }
        public DateTime? ReturnDate{ get; set; }
        public string? Status{ get; set; }

        public List<RentalDetails>? RentalDetails { get; set;}

    }
}