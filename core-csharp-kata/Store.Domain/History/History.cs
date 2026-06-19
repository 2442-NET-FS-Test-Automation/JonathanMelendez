using Serilog;

namespace Store.Domain;

public class History(IStoreRepository repo)
{
    private readonly Stack<Transaction> Transactions = [];
    private IStoreRepository Repository = repo;
    public void Add(TransactionEnum action, Item item, int amount = 0)
    {
        Transactions.Push(new Transaction(action, item, amount));
        Log.Information("Added new item with {id}", item.Id);
    }
    public bool Undo()
    {
        try
        {
            Transaction lastTransaction = Transactions.Pop();
            switch (lastTransaction.Action)
            {
                case TransactionEnum.Add:
                    Repository.RemoveById(lastTransaction.Item_.Id);
                    break;
                case TransactionEnum.Remove:
                    Repository.AddItem(lastTransaction.Item_);
                    break;
                case TransactionEnum.Sell:
                    Repository.GetItemById(lastTransaction.Item_.Id).Restock(lastTransaction.Amount);
                    break;
                case TransactionEnum.Restock:
                    Repository.GetItemById(lastTransaction.Item_.Id).Sell(lastTransaction.Amount);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error("Undo Error: {e}", e.Message);
            return false;
        }
        return true;        
    }
    public bool Redo()
    {
        throw new NotImplementedException();
    }
}