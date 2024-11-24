namespace PulseService.Domain.Adapters;

public interface IPasswordHasher
{
    string Hash(string password);
}