using Data.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Data.DataContext;

namespace Data.Repository
{
    public class TicketDBRepository
    {
        private readonly AirlineDbContext _context;

        public TicketDBRepository(AirlineDbContext context)
        {
            _context = context;
        }

        public void Book(Ticket ticket)
        {
            // Check if the seat is already booked
            if (_context.Tickets.Any(t => t.FlightIdFK == ticket.FlightIdFK && t.Row == ticket.Row && t.Column == ticket.Column && !t.Cancelled))
            {
                throw new InvalidOperationException("Seat is already booked.");
            }

            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public void Cancel(int ticketId)
        {
            var ticket = _context.Tickets.Find(ticketId);
            if (ticket != null)
            {
                ticket.Cancelled = true;
                _context.SaveChanges();
            }
        }

        public IEnumerable<Ticket> GetTickets(int flightId)
        {
            return _context.Tickets.Where(t => t.FlightIdFK == flightId).ToList();
        }
    }

}
