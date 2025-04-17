using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// A kanban-style board in the Ticketmaster system. 
    /// This class maps to the "Board" table in the database.
    /// Holds Stages and their tasks.
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    [Table("Board")]
    public class Board
    {
        /// <summary>
        /// Gets or sets the parent project.
        /// </summary>
        /// <value>
        /// The parent project.
        /// </value>
        [ForeignKey("ParentProjectId")]
        public Project ParentProject { get; set; }  // navigation property

        /// <summary>
        /// Gets or sets the parent project identifier.
        /// </summary>
        /// <value>
        /// The parent project identifier.
        /// </value>
        public int ParentProjectId { get; set; }

        /// <summary>
        /// Gets or sets the board identifier.
        /// </summary>
        /// <value>
        /// The board identifier.
        /// </value>
        [Key]
        public int BoardId { get; set; }
        

        /// <summary>
        /// The Stages on the board, see Stage.cs for details.
        /// </summary>
        public ICollection<Stage> Stages { get; set; }

    }
}
