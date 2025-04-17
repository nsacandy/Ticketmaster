using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Creates a manager-attribute pairing
    /// for an employee
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Gets or sets the manager identifier.
        /// </summary>
        /// <value>
        /// The manager identifier.
        /// </value>
        [Key]
        [ForeignKey("Employee")]
        public int ManagerId { get; set; }


        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        /// <value>
        /// The employee.
        /// </value>
        public virtual Employee Employee { get; set; }
    }
}
