using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HanYu.Infrastructure.Identity;

public sealed class PasswordService
    : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher =
        new();

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(
            new object(),
            password);
    }

    public bool VerifyPassword(
        string passwordHash,
        string password)
    {
        if (string.IsNullOrWhiteSpace(passwordHash) ||
            string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        var result =
            _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

        return result is
            PasswordVerificationResult.Success or
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}