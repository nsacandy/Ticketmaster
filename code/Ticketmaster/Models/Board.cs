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
        [ForeignKey("ParentProjectId")]
        public Project ParentProject { get; set; }  // navigation property

        public int ParentProjectId { get; set; }

        [Key]
        public int BoardId { get; set; }
        

        /// <summary>
        /// The Stages on the board, see Stage.cs for details.
        /// </summary>
        public ICollection<Stage> Stages { get; set; }

    }
}
