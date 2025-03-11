using Ticketmaster.Controllers;
using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{
    public class EmployeeManagementViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public List<EmployeeManagementController.EmployeeChange> StagedChanges { get; set; }
    }
}
