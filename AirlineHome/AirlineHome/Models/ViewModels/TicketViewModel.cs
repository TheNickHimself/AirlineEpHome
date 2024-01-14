using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineHome.Models.ViewModels
{
    public class TicketViewModel
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int FlightIdFK { get; set; }// need to figure out how to use FK
        public string Passport { get; set; }// path to img
        public decimal PricePaid { get; set; }
        public bool Cancelled { get; set; }
    }
}
