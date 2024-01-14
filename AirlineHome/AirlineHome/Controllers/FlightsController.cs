// Presentation/Controllers/TicketsController.cs
using System;
using System.Collections.Generic;
using Data.Repository;
using Microsoft.AspNetCore.Mvc;
//using AirlineHome.Data.DataContext;
using AirlineHome.Models.ViewModels;

public class FlightsController : Controller
{
    private FlightDbRepository _flightsDBReposetory;
    private TicketDBRepository _ticketsDBRepository;
    public FlightsController(FlightDbRepository flightsDBReposetory
        , TicketDBRepository TicketDBRepository)
    {
        _flightsDBReposetory = flightsDBReposetory;
        _ticketsDBRepository = TicketDBRepository;
    }

    // Method and View to show a list of available flights with retail prices
    public IActionResult ShowAvailableFlights()
    {
        var now = DateTime.Now;

        var availableFlights = _flightsDBReposetory.GetFlights()
            .Where(f => f.DepartureDate > now)
            .Select(f => new FlightsViewModel
            {
                Id = f.Id,
                DepartureDate = f.DepartureDate,
                ArrivalDate = f.ArrivalDate,
                Row = f.Row,
                Column = f.Column,
                CountryForm = f.CountryForm,
                CountryTo = f.CountryTo
            });

        return View(availableFlights);
    }
}
