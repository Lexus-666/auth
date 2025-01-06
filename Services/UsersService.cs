using kursah_5semestr.Abstractions;
using kursah_5semestr.Contracts;
using Org.BouncyCastle.Crypto.Generators;

namespace kursah_5semestr.Services
{
    public class UsersService : IUsersService
    {
        private IUsersRepository _usersRepository;
        private ISessionsService _sessions;
        private IBrokerService _brokerService;
        private ILogger _logger;

        public UsersService(IUsersRepository usersRepository, ISessionsService sessions, IBrokerService brokerService, ILogger<UsersService> logger)
        {
            _usersRepository = usersRepository;
            _sessions = sessions;
            _brokerService = brokerService;
            _logger = logger;
        }

        public async Task<User> CreateUser(User user)
        {
            user = await _usersRepository.CreateUser(user);
            _logger.LogInformation($"Created user {user.Login}");
            var dto = new UserOutDto(Id: user.Id, Login: user.Login);
            var message = new InstanceChanged(Action: "create", Entity: "user", Data: dto);
            await _brokerService.SendMessage("changes", message);
            return user;
        }

        public User? FindById(Guid id)
        {
            return _usersRepository.FindById(id);
        }

        public User FindByLogin(string login)
        {
            return _usersRepository.FindByLogin(login);
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
                    _logger.LogWarning($"Password check failed for user '{login}'");
                    throw new Exception("Invalid password");
                }
                var session = await _sessions.CreateSession(user);
                _logger.LogWarning($"Logged in user '{login}'");
                return session;
            }
            else
            {
                _logger.LogWarning($"User '{login}' not found");
                throw new Exception("User not found");
            }
        }
    }
}
