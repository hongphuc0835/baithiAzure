using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace baithi.Models
{
    public class RentalDetails
    {
        [Key]
        public int RentalDetailID{ get; set; }

        public int RentalID{ get; set; }

        public Rentals? Rentals { get; set; }

        public int ComicBookID{ get; set; }
        public ComicBooks? ComicBooks { get; set; }

        public int? Quantity{ get; set; }

        public decimal? PricePerDay{ get; set; }

    }
}