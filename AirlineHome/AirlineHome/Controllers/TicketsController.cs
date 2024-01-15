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
    private readonly IWebHostEnvironment _hostingEnvironment;

    public TicketsController(FlightDbRepository flightRepository, TicketDBRepository ticketRepository, IWebHostEnvironment hostingEnvironment)
    {
        _flightRepository = flightRepository;
        _ticketRepository = ticketRepository;
        _hostingEnvironment = hostingEnvironment;
    }


    public IActionResult BookFlight(string flightIdstr)//genualy no idea why the parameter isnt getting passed
    {
        int flightId = Int32.Parse(flightIdstr);
        var flight = _flightRepository.GetFlight(flightId);

        if (flight == null || IsFlightFullyBooked(flight) || flight.DepartureDate <= DateTime.Now)
        {
            return RedirectToAction("ShowFlights");
        }

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

        ViewBag.flightDeets = ticketViewModel;
        return View("BookFlight");
    }

    [HttpPost]
    public IActionResult BookFlight(TicketViewModel ticketViewModel, IFormFile passportImg)
    {
        var flight = _flightRepository.GetFlight(ticketViewModel.Id);
        if (flight == null || IsFlightFullyBooked(flight) || flight.DepartureDate <= DateTime.Now)
        {
            return RedirectToAction("ShowFlights");
        }
        
        /*some work around the solution like this function are basicaly duplucates 
         Cuz i used Microsoft docs, cool ppl on yt and my main boi gpt and they all anser question with a slightly different aproach*/
        var bookedTickets = _ticketRepository.GetTickets(ticketViewModel.Id);
        if (bookedTickets.Any(t => t.Row == ticketViewModel.Row && t.Column == ticketViewModel.Column && !t.Cancelled))
        {
            ModelState.AddModelError("SeatUnavailable", "Selected seat is already booked.");
            return View(ticketViewModel);
        }

        if (passportImg != null)
        {
            var uniqueFileName = GetUniqueFileName(passportImg.FileName);
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            passportImg.CopyTo(new FileStream(filePath, FileMode.Create));
            ticketViewModel.Passport = Path.Combine("uploads", uniqueFileName);
        }

        if (flight.DepartureDate <= DateTime.Now)
        {
            ModelState.AddModelError("InvalidDepartureDate", "You cant time travel. Set the Departur for the future.");
            return View(ticketViewModel);
        }

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
    //but there is no login part so this dont really work
    public IActionResult ShowTicketsHistory()
    {
        var loggedInClientId = int.Parse(User.FindFirst("id")?.Value);

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

    public IActionResult GetTicketsFromFlight(int Id)
    {
        var ticketsHistory = _ticketRepository.GetTickets(Id)
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

    private string GetUniqueFileName(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName)
               + "_"
               + Guid.NewGuid().ToString("N")
               + Path.GetExtension(fileName);
    }

    private bool IsFlightFullyBooked(Flight flight)
    {
        var bookedTicketsCount = _ticketRepository.GetTickets(flight.Id)
            .Count(ticket => !ticket.Cancelled);

        return bookedTicketsCount > 150;//150 is the rough average on an a380
    }
}
