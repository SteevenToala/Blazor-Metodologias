// filepath: MyCleanApp.Application/Interfaces/IPasswordHasher.cs
namespace MyCleanApp.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}