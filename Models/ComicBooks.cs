using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace baithi.Models
{
    public class ComicBooks
    {
        [Key]
        public int ComicBookID{ get; set; }

        public string? Title { get; set; }

        public string? Author { get; set; }

        public decimal? PricePerDay { get; set; }

        public List<RentalDetails>? RentalDetails{ get; set; }
    }
}