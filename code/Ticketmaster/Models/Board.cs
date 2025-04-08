using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

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
        [ForeignKey("ProjectId")]
        private int _parentProjectId;

        [Required]
        private string _title { get; set; }

        private BoardTask[] _tasks;
        private int _position;
        public int Position
        {
            get => _position;
            set => _position = value;
        }
        public BoardTask[] Tasks
        {
            get => _tasks;
            set => _tasks = value;
        }
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public int ParentProjectId
        {
            get => _parentProjectId;
        }
    }
}
