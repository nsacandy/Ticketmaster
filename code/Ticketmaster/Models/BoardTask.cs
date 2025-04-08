using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents tasks to be placed on Board(s) in the Ticketmaster system. 
    /// This class maps to the "BoardTask" table in the database
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    public class BoardTask
    {
        /// <summary>
        /// Gets or sets the task title.
        /// </summary>
        /// <precondition>
        /// Every task on a board must have a unique title
        /// </precondition>
        /// <value>
        /// The task title.
        /// </value>
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(100)]
        public string TaskTitle { get; set; }

        /// <summary>
        /// What the task entails
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// The Board the task is associated with.
        /// </summary>
        public Board ParentBoard { get; set; }  // navigation property
        
        [System.ComponentModel.DataAnnotations.Required]
        public int ParentBoardId { get; set; }  // foreign key
    }
}
