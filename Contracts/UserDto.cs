using System.Security.Cryptography.X509Certificates;

namespace kursah_5semestr.Contracts
{
    public record UserDto(
        string Login, 
        string Password
        );
}
