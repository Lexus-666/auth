namespace kursah_5semestr.Models;

public class Session
{
    private Session(Guid id, User user, string token)
    {
        Id = id;
        User = user;
        Token = token;
    }

    public Guid Id { get; set; }

    public User User { get; set; }

    public string Token { get; set; }

    public static Session Create(Guid id, User user, string token)
    {
        var session = new Session(id, user, token);

        return session;
    }
}