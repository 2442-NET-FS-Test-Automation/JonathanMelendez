using Serilog;

namespace Store.Domain;

public class History
{
    private readonly Stack<Transaction> Transactions = [];
    public void HistoryAdd(TransactionEnum action, Item item)
    {
        Transactions.Push(new Transaction(action, item));
        Log.Information("Added new item with {id}", item.Id);
    }
    public bool HistoryUndo()
    {
        Transaction lastTransaction = Transactions.Pop();
        switch (lastTransaction.Action)
        {
            case TransactionEnum.Add:
                break;
            case TransactionEnum.Remove:
                break;
            case TransactionEnum.Sell:
                break;
            case TransactionEnum.Restock:
                break;
        }
        throw new NotImplementedException();
    }
    public bool HistoryRedo()
    {
        throw new NotImplementedException();
    }
}