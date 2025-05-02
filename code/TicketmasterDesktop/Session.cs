using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketmaster.Models;

namespace TicketmasterDesktop
{
    public static class Session
    {
        public static Employee? CurrentUser { get; set; }
    }
}
