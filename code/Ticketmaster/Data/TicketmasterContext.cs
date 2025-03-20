using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Models;
using Ticketmaster.Utilities;

namespace Ticketmaster.Data
{
    public class TicketmasterContext : DbContext
    {
        public TicketmasterContext(DbContextOptions<TicketmasterContext> options)
            : base(options)
        {
        }

        public DbSet<Ticketmaster.Models.Employee> Employee { get; set; } = default!;
        public DbSet<Ticketmaster.Models.Group> Groups { get; set; } = default!;
        public DbSet<Ticketmaster.Models.Project> Project { get; set; } = default!;
        public DbSet<Manager> Manager { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<Project>().ToTable("Project");


            var admin = new Employee
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@ticketmaster.com",
                Pword = EmployeePasswordHasher.HashPassword("AdminPass123"),
                PhoneNum = "123-456-7890",
                ERole = "admin"
            };
            modelBuilder.Entity<Employee>().HasData(admin);


        }


    }
}
