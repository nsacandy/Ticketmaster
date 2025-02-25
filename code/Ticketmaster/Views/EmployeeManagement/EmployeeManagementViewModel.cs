using Ticketmaster.Controllers;
using Ticketmaster.Models;

namespace Ticketmaster.Views.EmployeeManagement
{
    public class EmployeeManagementViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public List<EmployeeManagementController.EmployeeChange> StagedChanges { get; set; }
    }
}
