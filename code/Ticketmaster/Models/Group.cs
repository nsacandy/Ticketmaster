using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents a group within the Ticketmaster system.
    /// This class is mapped to the "Groups" table in the database.
    /// </summary>
    [Table("Groups")]
    public class Group
    {
        /// <summary>
        /// The unique identifier for the group.
        /// This is the primary key for the "Groups" table.
        /// </summary>
        [Key]
        public int GroupId { get; set; }

        /// <summary>
        /// The name of the group.
        /// This field is required and cannot exceed 100 characters.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        /// <summary>
        /// The identifier of the manager responsible for this group.
        /// This is a nullable foreign key referencing the <see cref="Manager"/> entity.
        /// If null, the group does not have a designated manager.
        /// </summary>
        public int? ManagerId { get; set; }

        /// <summary>
        /// A string containing the identifiers of the employees belonging to this group.
        /// The format of this string is not explicitly defined within this class.
        /// It is likely a comma-separated list or another delimited format of employee IDs.
        /// This property might be used for simpler data storage or integration purposes.
        /// Consider creating a separate many-to-many relationship for a more normalized approach.
        /// </summary>
        public string? EmployeeIds { get; set; }

        /// <summary>
        /// Navigation property to the <see cref="Manager"/> entity.
        /// This represents the manager assigned to this group, if any.
        /// It is based on the <see cref="ManagerId"/> foreign key.
        /// </summary>
        [ForeignKey("ManagerId")]
        public virtual Manager? Manager { get; set; }
    }
}
