using kursah_5semestr.Abstractions;
using kursah_5semestr.Models;
using Microsoft.EntityFrameworkCore;

namespace kursah_5semestr.Database.Repositories
{
    public class SessionsRepository : ISessionsRepository
    {
        private AppDbContext _context;

        public SessionsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Session> CreateSession(Session session)
        {
            session.Id = Guid.NewGuid();
            _context.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public Session FindByToken(string token)
        {
            var session = _context.Sessions.Include(s => s.User).Where(s => s.Token == token).First();
            return session;
        }
    }
}
