using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{
    public class GroupManagementViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public List<Group> Groups { get; set; }
    }
}
