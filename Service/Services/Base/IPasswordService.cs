namespace Service.Services.Base;

public interface IPasswordService
{
    (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password);
    bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
}
