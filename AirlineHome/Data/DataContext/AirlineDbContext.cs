using Domain.Models;
//using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data.DataContext
{
    public class AirlineDbContext : DbContext
    {
        public AirlineDbContext(DbContextOptions<AirlineDbContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        //public DbSet<> FlightsViewModel { get; set; } = default!;
    }

}
