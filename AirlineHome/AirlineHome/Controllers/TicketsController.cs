// Presentation/Controllers/TicketsController.cs
using System;
using Data.Repository;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using AirlineHome.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

public class TicketsController : Controller
{
    private readonly FlightDbRepository _flightRepository;
    private readonly TicketDBRepository _ticketRepository;

    public TicketsController(FlightDbRepository flightRepository, TicketDBRepository ticketRepository)
    {
        _flightRepository = flightRepository;
        _ticketRepository = ticketRepository;
    }

    public IActionResult BookFlight(int flightId)
    {
        var flight = _flightRepository.GetFlight(flightId);

        // Check if the flight is not fully booked and departure date is in the future
        if (flight == null || IsFlightFullyBooked(flight) || flight.DepartureDate <= DateTime.Now)
        {
            return RedirectToAction("ShowFlights");
        }

        // Create a new TicketViewModel with default values
        var ticketViewModel = new TicketViewModel
        {
            Id = flight.Id,
            Row = flight.Row,
            Column = flight.Column,
            FlightIdFK = flight.Id,
            Passport = "",
            PricePaid = CalculateRetailPrice(flight.WholesalePrice, flight.CommissionRate),
            Cancelled = false
        };

        return View(ticketViewModel);
    }

    [HttpPost]
    public IActionResult BookFlight(TicketViewModel ticketViewModel)
    {
        // Check if the flight is not fully booked and departure date is in the future
        var flight = _flightRepository.GetFlight(ticketViewModel.Id);
        if (flight == null || IsFlightFullyBooked(flight) || flight.DepartureDate <= DateTime.Now)
        {
            return RedirectToAction("ShowFlights");
        }

        // Check if the selected seat is available
        var bookedTickets = _ticketRepository.GetTickets(ticketViewModel.Id);
        if (bookedTickets.Any(t => t.Row == ticketViewModel.Row && t.Column == ticketViewModel.Column && !t.Cancelled))
        {
            ModelState.AddModelError("SeatUnavailable", "Selected seat is already booked.");
            return View(ticketViewModel);
        }

        // Check if the selected ticket is already booked or canceled // MAYBE UNECESARY
        var existingTicket = bookedTickets.FirstOrDefault(t => t.Row == ticketViewModel.Row && t.Column == ticketViewModel.Column);
        if (existingTicket != null && !existingTicket.Cancelled)
        {
            ModelState.AddModelError("TicketUnavailable", "Selected ticket is already booked.");
            return View(ticketViewModel);
        }

        // Check if the departure date is in the future
        if (flight.DepartureDate <= DateTime.Now)
        {
            ModelState.AddModelError("InvalidDepartureDate", "You cant time travel. Set the Departur for the future.");
            return View(ticketViewModel);
        }

        // Create and save the new ticket
        var newTicket = new Ticket
        {
            Id = ticketViewModel.Id,
            Row = ticketViewModel.Row,
            Column = ticketViewModel.Column,
            FlightIdFK = ticketViewModel.FlightIdFK,
            Passport = ticketViewModel.Passport,
            PricePaid = ticketViewModel.PricePaid,
            Cancelled = false
        };
        _ticketRepository.Book(newTicket);

        return RedirectToAction("ShowTicketsHistory");
    }


    // Method and View to show the history of purchased tickets for the logged-in client
    [Authorize] // Add authorization attribute to ensure the user is logged in
    public IActionResult ShowTicketsHistory()
    {
        // Get the logged-in client's ID from the authentication context
        var loggedInClientId = int.Parse(User.FindFirst("id")?.Value);

        // Retrieve the tickets history for the logged-in client
        var ticketsHistory = _ticketRepository.GetTickets(loggedInClientId)
            .Select(t => new TicketViewModel
            {
                Id = t.Id,
                Row = t.Row,
                Column = t.Column,
                FlightIdFK = t.FlightIdFK,
                Passport = t.Passport,
                PricePaid = t.PricePaid,
                Cancelled = t.Cancelled
            });

        return View(ticketsHistory);
    }

    private decimal CalculateRetailPrice(decimal wholesalePrice, decimal commissionRate)
    {
        return wholesalePrice * (1 + commissionRate);
    }

    private bool IsFlightFullyBooked(Flight flight)
    {
        var bookedTicketsCount = _ticketRepository.GetTickets(flight.Id)
            .Count(ticket => !ticket.Cancelled);

        return bookedTicketsCount > 150;
    }
}
