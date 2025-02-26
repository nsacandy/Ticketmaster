using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    public class Manager
    {
        [Key]
        [ForeignKey("Employee")]
        public int ManagerId { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
