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
        private string _taskTitle { get; set; }
        private string _taskDescription { get; set; }
        private Board _parentBoard { get; set; }

        public string TaskTitle
        {
            get => _taskTitle;
            set => _taskTitle = value;
        }

        public string TaskDescription
        {
            get => _taskDescription;
            set => _taskDescription = value;
        }

        public Board ParentBoard
        {
            get => _parentBoard;
            set => _parentBoard = value;
        }
    }
}
