using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Models;

namespace Ticketmaster.Data
{
    public class TicketmasterContext : DbContext
    {
        public TicketmasterContext (DbContextOptions<TicketmasterContext> options)
            : base(options)
        {
        }

        public DbSet<Ticketmaster.Models.Employee> Employee { get; set; } = default!;
        public DbSet<Ticketmaster.Models.Group> Groups { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Group>().ToTable("Groups");
        }


    }
}
