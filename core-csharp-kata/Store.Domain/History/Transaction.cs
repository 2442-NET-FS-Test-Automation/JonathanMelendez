namespace Store.Domain;

public class Transaction(TransactionEnum action, Item item)
{
    public DateTime Time { get; } = DateTime.Now;
    public Item Item_ { get; } = item;
    public TransactionEnum Action { get; } = action;
}