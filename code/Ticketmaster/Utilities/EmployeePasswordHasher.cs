using Microsoft.AspNetCore.Identity;
using Ticketmaster.Models;

namespace Ticketmaster.Utilities
{
    public static class EmployeePasswordHasher
    {
        public static PasswordHasher<object> Hasher = new PasswordHasher<object>();

        public static bool VerifyPassword(string hashedPassword, string password)
        {
            return Hasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }

        public static String HashPassword(string password)
        {
            return Hasher.HashPassword(null, password);
        }

    }
}
