namespace Store.Domain;

public abstract class Transaction(Item item)
{
    public DateTime Time { get; } = DateTime.Now;
    public Item Item_ { get; } = item;
    public TransactionEnum Action { get; }
}