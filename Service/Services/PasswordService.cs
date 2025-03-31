using Service.Services.Base;
using System.Security.Cryptography;
using System.Text;

namespace Service.Services;

public class PasswordService : IPasswordService
{
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    public (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
    {
        using (var hmac = new HMACSHA512())
        {
           var passwordSalt = hmac.Key;
           var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return (passwordHash, passwordSalt);
        }
    }
}
