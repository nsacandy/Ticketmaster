using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        public int? ManagerId { get; set; }

        public string? EmployeeIds { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Manager? Manager { get; set; }
    }
}
