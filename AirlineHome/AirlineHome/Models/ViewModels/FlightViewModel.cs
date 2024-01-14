using System.ComponentModel.DataAnnotations.Schema;

namespace AirlineHome.Models.ViewModels
{
    public class FlightsViewModel
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string CountryForm { get; set; }
        public string CountryTo { get; set; }
    }
}
