using CoreLedger.Application.Common.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace CoreLedger.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BC.HashPassword(password, workFactor: 11);

    public bool Verify(string password, string hash) => BC.Verify(password, hash);
}
