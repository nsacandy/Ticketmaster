using System.Collections.Generic;
using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{
    public class ProjectManagementViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}
