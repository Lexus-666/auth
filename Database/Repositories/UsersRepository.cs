using kursah_5semestr.Abstractions;

namespace kursah_5semestr.Database.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private AppDbContext _context;

        public UsersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public User? FindById(Guid id)
        {
            return _context.Users.Find(id);
        }

        public User FindByLogin(string login)
        {
            var user = _context.Users.Where(u => u.Login == login).First();
            return user;
        }
    }
}
