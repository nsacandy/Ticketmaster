using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// A kanban-style board in the Ticketmaster system. 
    /// This class maps to the "Board" table in the database.
    /// Holds Tasks and their details.
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    public class Board
    {
        /// <summary>
        /// Title of the board, eg("To Do", "In Progress", "Done").
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Project the board is associated with.
        /// </summary>
        public Project ParentProject { get; set; }  // navigation property

        [Required]
        public int ParentProjectId { get; set; }

        /// <summary>
        /// The tasks on the board, see BoardTask.cs for details.
        /// </summary>
        public ICollection<BoardTask> Tasks { get; set; }
        
        /// <summary>
        /// The position the board is in the kanban-style layout.
        /// </summary>
        [Required]
        public int Position { get; set; }
    }
}
