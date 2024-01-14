using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{

    public class Ticket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        [ForeignKey("Flight")]
        public int FlightIdFK { get; set; }
        public string Passport { get; set; }// path to img
        public decimal PricePaid { get; set; }
        public bool Cancelled { get; set; }
    }

}