using System;
using System.Collections.Generic;
using Data.Repository;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult ShowFlights()
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
