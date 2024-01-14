using Data.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Data.Repository
{
    public class FlightDbRepository
    {
        private readonly AirlineDbContext _context;

        public FlightDbRepository(AirlineDbContext context)
        {
            _context = context;
        }

        public Flight GetFlight(int flightId)
        {
            return _context.Flights.Find(flightId);
        }

        public IEnumerable<Flight> GetFlights()
        {
            return _context.Flights.ToList();
        }
    }
}
