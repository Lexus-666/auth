namespace kursah_5semestr.Models;

public class User
{
    public const int loginLenMax = 128;
    public const int loginLenMin = 8;
    public const int passwordLen = 128;
    private User(Guid id, string login, string passwordHash)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; set; }

    public string Login { get; set; }

    public string PasswordHash { get; set; }

    private static string BasicCheck(string login, string passwordHash)
    {
        var error = string.Empty;

        if (string.IsNullOrEmpty(login) || login.Length > loginLenMax || login.Length < loginLenMin)
        {
            error = "Login can't be empty and len must be 8-128.";
        }
        else if (string.IsNullOrEmpty(passwordHash) || passwordHash.Length > passwordLen)
        {
            error = "Password cannot be empty. Password Hash 128 symbols max";
        }

        return error;
    }

    public static (User User, string Error) Create(Guid id, string login, string passwordHash)
    {
        var error = BasicCheck(login, passwordHash);

        if (!string.IsNullOrEmpty(error))
        {
            return (null!, error);
        }

        var user = new User(id, login, passwordHash);

        return(user, error);
    }
}