namespace kursah_5semestr.Abstractions
{
    public interface ISessionsRepository
    {
        public Task<Session> CreateSession(Session session);

        public Session FindByToken(string token);

    }
}
