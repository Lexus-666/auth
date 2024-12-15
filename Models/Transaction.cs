namespace kursah_5semestr.Models;

public class Transaction
{
    public const double MinTotalPrice = 0.01;

    private Transaction(Guid id, string transactionDetails, User user, double totalPrice)
    {
        Id = id;
        TransactionDetails = transactionDetails;
        User = user;
        TotalPrice = totalPrice;
    }

    public Guid Id { get; set; }

    public string TransactionDetails { get; set; }

    public User User { get; set; }

    public double TotalPrice { get; set; }

    public static string BasicCheck(string transactionDetails, double totalPrice)
    {
        var error = string.Empty;

        if (string.IsNullOrWhiteSpace(transactionDetails))
        {
            error = "Transaction details cannot be empty.";
        }
        else if (totalPrice < MinTotalPrice)
        {
            error = $"Total price must be at least {MinTotalPrice}";
        }

        return error;
    }

    public static (Transaction Transactionc, string Error) Create(Guid id, string transactionDetails, User user, double totalPrice)
    {
        var error = BasicCheck(transactionDetails, totalPrice);

        if (!string.IsNullOrEmpty(error))
        {
            return (null!, error);
        }

        var transaction = new Transaction(id, transactionDetails, user, totalPrice);

        return (transaction, error);
    }
}