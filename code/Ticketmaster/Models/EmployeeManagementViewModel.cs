using Ticketmaster.Controllers;

namespace Ticketmaster.Models
{
    public class EmployeeManagementViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public List<EmployeeManagementController.EmployeeChange> StagedChanges { get; set; }
    }
}
