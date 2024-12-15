namespace kursah_5semestr.Abstractions
{
    public interface ISessionsService
    {
        public Task<Session> CreateSession(User user);

        public Session FindByToken(string token);

    }
}
