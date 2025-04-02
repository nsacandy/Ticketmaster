using Microsoft.AspNetCore.Identity;
using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{

        /// <summary>
        /// Utility class for hashing and verifying employee passwords using ASP.NET Core's built-in password hasher.
        /// </summary>
        public static class EmployeePasswordHasher
        {
            /// <summary>
            /// An instance of the password hasher used for hashing and verifying passwords.
            /// </summary>
            public static PasswordHasher<object> Hasher = new PasswordHasher<object>();

            /// <summary>
            /// Verifies a plain text password against a previously hashed password.
            /// </summary>
            /// <param name="hashedPassword">The hashed password stored in the database.</param>
            /// <param name="password">The plain text password to verify.</param>
            /// <returns>True if the password is valid; otherwise, false.</returns>
            public static bool VerifyPassword(string hashedPassword, string password)
            {
                return Hasher.VerifyHashedPassword(null, hashedPassword, password) ==
                       PasswordVerificationResult.Success;
            }

            /// <summary>
            /// Hashes a plain text password using a secure algorithm.
            /// </summary>
            /// <param name="password">The plain text password to hash.</param>
            /// <returns>A securely hashed representation of the password.</returns>
            public static string HashPassword(string password)
            {
                return Hasher.HashPassword(null, password);
            }
        }
}