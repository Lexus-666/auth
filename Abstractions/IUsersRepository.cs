namespace kursah_5semestr.Abstractions
{
    public interface IUsersRepository
    {
        public Task<User> CreateUser(User user);
        public User? FindById(Guid id);
        public User FindByLogin(string login);
    }
}
