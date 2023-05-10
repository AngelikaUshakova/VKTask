using System.Security.Cryptography;
using System.Text;

namespace UserManager.Services;

public class PasswordService : IPasswordService
{
    public void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hasher = new HMACSHA512();

        salt = hasher.Key;
        hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}