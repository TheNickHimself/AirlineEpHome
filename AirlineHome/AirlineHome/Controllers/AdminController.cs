using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Data; // Ensure correct namespace for your data classes
using Domain.Models;
using Data.Repository; // Ensure correct namespace for your domain classes
using AirlineHome.Models.ViewModels;

namespace AirlineHome.Controllers
{
    /*
     Started working on the Admin thing 
    got stick on another part 
    never finished but there are some funky functions
     */
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly FlightDbRepository _flightDbRepository;
        private readonly TicketDBRepository _ticketDBRepository;


        public AdminController(FlightDbRepository flightDbRepository, TicketDBRepository ticketDBRepository)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDBRepository = ticketDBRepository;
        }

        public IActionResult SelectFlight()
        {
            var flights = _flightDbRepository.GetFlights();
            var flightViewModels = flights.Select(f => new FlightsViewModel
            {
                Id = f.Id,
                DepartureDate = f.DepartureDate,
                ArrivalDate = f.ArrivalDate,
                Row = f.Row,
                Column = f.Column,
                CountryForm = f.CountryForm,
                CountryTo = f.CountryTo
            });

            return View(flightViewModels);
        }

        public IActionResult GetTicketsForFlight(int flightId)
        {
            var tickets = _ticketDBRepository.GetTickets(flightId);
            var ticketViewModels = tickets.Select(t => new TicketViewModel// no filter needed as GetTIckets filters by FlightFK returning a list of only the aplicable tickets
            {
                Id = t.Id,
                Row = t.Row,
                Column = t.Column,
                FlightIdFK = t.FlightIdFK,
                Passport = t.Passport,
                PricePaid = t.PricePaid,
                Cancelled = t.Cancelled
            });

            return View(ticketViewModels);
        }
    }
}