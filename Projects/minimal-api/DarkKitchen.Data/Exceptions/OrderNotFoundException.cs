namespace DarkKitchen.Data.Exceptions;

public class OrderNotFoundException(int id) : Exception($"Order with id {id} not found")
{
    public int orderId = id;
}