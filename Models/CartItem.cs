namespace kursah_5semestr.Models;

public class CartItem
{
    public const int MinQuantity = 1;
    private CartItem(Guid id, Product product, User user, int quantity)
    {
        Id = id;
        Product = product;
        User = user;
        Quantity = quantity;
    }
    
    public Guid Id { get; set; }

    public Product Product { get; set; }

    public User User { get; set; }

    public int Quantity { get; set; }

    private static string BasicCheck(int quantity)
    {
        var error = string.Empty;

        if (quantity < MinQuantity)
        {
            error = $"Quantity must be greater than {MinQuantity - 1}";
        }

        return error;
    }

    public static (CartItem CartItem, string Error) Create(Guid id, Product product, User user, int quantity)
    {
        var error = BasicCheck(quantity);

        var cartItem = new CartItem(id, product, user, quantity);

        return (cartItem, error);
    }
}