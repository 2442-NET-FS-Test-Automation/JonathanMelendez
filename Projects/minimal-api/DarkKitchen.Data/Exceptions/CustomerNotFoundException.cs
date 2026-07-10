namespace DarkKitchen.Data.Exceptions;

public class CustomerNotFoundException(int id) : Exception($"Customer with id {id} not found")
{
    public int customerId = id;
}