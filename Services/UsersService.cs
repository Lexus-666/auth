using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Org.BouncyCastle.Crypto.Generators;

namespace kursah_5semestr.Services
{
    public class UsersService : IUsersService
    {
        private AppDbContext _context;
        private ISessionsService _sessions;
        private IBrokerService _brokerService;

        public UsersService(AppDbContext context, ISessionsService sessions, IBrokerService brokerService)
        {
            _context = context;
            _sessions = sessions;
            _brokerService = brokerService;
        }

        public async Task<User> CreateUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var dto = new UserOutDto(Id: user.Id, Login: user.Login);
            var message = new InstanceChanged(Action: "create", Entity: "user", Data: dto);
            await _brokerService.SendMessage("changes", message);
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

        public bool CheckPassword(string login, string password)
        {
            var user = FindByLogin(login);
            if (user != null)
            {
                var passwordHash = Convert.ToBase64String(BCrypt.PasswordToByteArray(password.ToCharArray()));
                if (passwordHash == user.PasswordHash)
                {
                    return true;
                }
            }
            return false;
        }


        public async Task<Session> Login(string login, string password)
        {
            var user = FindByLogin(login);
            if (user != null)
            {
                var passwordHash = Convert.ToBase64String(BCrypt.PasswordToByteArray(password.ToCharArray()));
                if (passwordHash != user.PasswordHash)
                {
                    throw new Exception("Invalid password");
                }
                var session = await _sessions.CreateSession(user);
                return session;
            }
            else
            {
                throw new Exception("User not found");
            }
        }
    }
}
