using Atlas.Models;
using System.Security.Claims;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace Atlas.Utilities
{
    public class Utils
    {

        public static string GenerateJwt(User user, string key, string issuer, string audience)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // Custom "userId" claim used by all controllers to resolve the acting user.
                new Claim("userId", user.Id.ToString())
            };
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds,
                notBefore: DateTime.UtcNow // Prevents use before issuance (clock-skew attacks)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            // Regex breakdown:
            //   (?=.*[a-z])        — at least one lowercase letter
            //   (?=.*[A-Z])        — at least one uppercase letter
            //   (?=.*\d)           — at least one digit
            //   (?=.*[^\da-zA-Z])  — at least one non-alphanumeric (special) character
            //   .{8,}              — minimum 8 characters total
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        public static bool CorrectValues(string name, decimal amount, int recurrent)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            // Reject zero or negative amounts, and amounts above the UI display cap.
            if (amount <= 0 || amount > 999_999_999) return false;
            // Recurrent codes outside 0–3 don't map to any known Recurrent enum value.
            if (recurrent < 0 || recurrent > 3) return false;
            return true;
        }

        public static bool IsInputValid(string name, string username, string email, string password)
        {
            if (!IsValidEmail(email)) return false;
            if (!IsValidPassword(password)) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            if (string.IsNullOrWhiteSpace(username)) return false;

            return true;
        }

        public static string GenerateNumericCode(int digits = 6)
        {
            if (digits is < 1 or > 9)
                throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 1 and 9.");
            // Use RandomNumberGenerator (CSPRNG) instead of Random to ensure the
            // code is unpredictable even if the process start time is known.
            var max = (int)Math.Pow(10, digits);
            var number = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, max);
            return number.ToString(new string('0', digits));
        }

        public static string HashCode(string code)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(bytes);
        }

        public static string HashRefreshToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }

        public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
