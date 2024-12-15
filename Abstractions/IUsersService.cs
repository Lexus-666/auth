namespace kursah_5semestr.Abstractions
{
    public interface IUsersService
    {
        public Task<User> CreateUser(User user);
        public bool CheckPassword(string login, string password);
        public Task<Session> Login(string login, string password);
        public User? FindById(Guid id);
        public User FindByLogin(string login);
    }
}
