using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents a project within the work order management system.
    /// Each project is associated with one or more groups and has a designated project lead.
    /// </summary>
    [Table("Project")]
    public class Project
    {
        /// <summary>
        /// The unique identifier for the project.
        /// </summary>
        [Key]
        public int ProjectId { get; set; }

        /// <summary>
        /// The name of the project.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        /// <summary>
        /// A detailed description of the project.
        /// </summary>
        public string ProjectDescription { get; set; }

        /// <summary>
        /// A comma-separated list of group identifiers involved in this project.
        /// </summary>
        public string InvolvedGroups { get; set; }

        /// <summary>
        /// The user ID of the employee leading the project.
        /// </summary>
        public int ProjectLeadId { get; set; }
    }
}