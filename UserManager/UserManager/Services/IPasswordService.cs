namespace UserManager.Services;

public interface IPasswordService
{
    void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);
}