namespace kursah_5semestr.Abstractions
{
    public interface IBrokerService
    {
        public Task SendMessage(string exchange, object message);
    }
}
