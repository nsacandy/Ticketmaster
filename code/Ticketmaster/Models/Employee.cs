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
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [Column("FirstName")]
        public String  FirstName   { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [Column("LastName")]
        public String  LastName    { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Column("Email")]
        public String  Email       { get; set; }
        
        /// <summary>
        /// Gets or sets the pword.
        /// </summary>
        /// <value>
        /// The pword.
        /// </value>
        [Column("Pword")]
        public String Pword { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [Column("PhoneNum")]
        public String PhoneNum { get; set; }

        /// <summary>
        /// Gets or sets the e role.
        /// </summary>
        /// <value>
        /// The e role.
        /// </value>
        [Column("ERole")]
        public String ERole { get; set; }
    }
}
