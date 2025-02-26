using System.ComponentModel.DataAnnotations.Schema;


namespace Ticketmaster.Models
{
    [Table("Employee")]
    public class Employee
    {
        public int employee_id { get; set; }
        public string username { get; set; }
        public string password_hash { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string role { get; set; }
    }
}
