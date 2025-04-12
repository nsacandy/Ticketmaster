using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents stages to be placed on Board(s) in the Ticketmaster system. 
    /// This class maps to the "Boardstage" table in the database
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    public class Stage
    {
        /// <summary>
        /// Gets or sets the stage title.
        /// </summary>
        /// <precondition>
        /// Every stage on a board must have a unique title
        /// </precondition>
        /// <value>
        /// The stage title.
        /// </value>
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(100)]
        public string StageTitle { get; set; }


        /// <summary>
        /// The Board the stage is associated with.
        /// </summary>
        public Board ParentBoard { get; set; }  // navigation property

        /// <summary>
        /// Gets or sets the parent board identifier.
        /// </summary>
        /// <value>
        /// The parent board identifier.
        /// </value>
            [System.ComponentModel.DataAnnotations.Required]
        public int ParentBoardId { get; set; }  // foreign key

        /// <summary>
        /// Gets or sets the stage identifier.
        /// </summary>
        /// <value>
        /// The stage identifier.
        /// </value>
            [Key]
        public int StageId { get; set; }  // primary key

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position { get; set; } // Position of the stage on the board}

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

    }
}
