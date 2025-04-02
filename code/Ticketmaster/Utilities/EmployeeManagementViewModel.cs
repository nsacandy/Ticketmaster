using Ticketmaster.Controllers;
using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{
    /// <summary>
    /// Used for staging changes in the EmployeeManagementController.
    /// Needed because only one model can be provided to a view, but both
    /// employees and changes need to be displayed.
    /// </summary>
    public class EmployeeManagementViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public List<EmployeeManagementController.EmployeeChange> StagedChanges { get; set; }
    }
}
