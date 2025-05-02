using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;
using Ticketmaster.Data;

namespace TicketmasterDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TicketmasterContext DbContext { get; private set; }
        public static DbContextOptions<TicketmasterContext> DbOptions { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DbOptions = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TicketmasterContext-d12841e7-3bb4-494b-afde-d73f97b2c023;Trusted_Connection=True;TrustServerCertificate=True;")

                .Options;

            DbContext = new TicketmasterContext(DbOptions);

        }
    }

}
