namespace HanYu.Application.Interfaces.Authentication;

public interface IPasswordService
{
    string HashPassword(string password);

    bool VerifyPassword(
        string passwordHash,
        string password);
}