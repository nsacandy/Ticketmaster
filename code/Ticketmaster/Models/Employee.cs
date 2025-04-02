using System.ComponentModel.DataAnnotations.Schema;

namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents an employee in the Ticketmaster system. 
    /// This class maps to the "Employee" table in the database.
    /// Used for storing core user details such as name, email, password, phone number, and role.
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
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
