using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{

    public class Flight
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string CountryForm { get; set; }
        public string CountryTo { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal CommissionRate { get; set; }
    }
}