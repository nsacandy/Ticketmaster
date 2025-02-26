using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    [Table("Employee")]
    public class Employee
    {
        [Column("Id")]
        public int Id { get; set; }
        [Column("FirstName")]
        public String  FirstName   { get; set; }
        [Column("LastName")]
        public String  LastName    { get; set; }
        [Column("Email")]
        public String  Email       { get; set; }
        [Column("Pword")]
        public String Pword { get; set; }
        [Column("PhoneNum")]
        public String PhoneNum { get; set; }
        [Column("ERole")]
        public String ERole { get; set; }
    }
}
