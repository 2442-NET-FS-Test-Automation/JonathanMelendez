using Serilog;

namespace Store.Domain;

public class History(IStoreRepository repo)
{
    private readonly Stack<Transaction> Transactions = [];
    private IStoreRepository Repository = repo;
    public void Add(TransactionEnum action, Item item, int amount = 0)
    {
        Transactions.Push(new Transaction(action, item, amount));
        Log.Information("Added new transaction type {action} to the transaction stack", action);
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
                    Console.WriteLine($"Undid item add: {lastTransaction.Item_.Name}");
                    Log.Information("Undid item add: {name}", lastTransaction.Item_.Name);
                    break;
                case TransactionEnum.Remove:
                    Repository.AddItem(lastTransaction.Item_);
                    Console.WriteLine($"Undid item remove: {lastTransaction.Item_.Name}");
                    Log.Information("Undid item remove: {name}", lastTransaction.Item_.Name);
                    break;
                case TransactionEnum.Sell:
                    Repository.GetItemById(lastTransaction.Item_.Id).Restock(lastTransaction.Amount);
                    Console.WriteLine($"Undid item sell: {lastTransaction.Item_.Name}");
                    Log.Information("Undid item sell: {name}", lastTransaction.Item_.Name);
                    break;
                case TransactionEnum.Restock:
                    Repository.GetItemById(lastTransaction.Item_.Id).Sell(lastTransaction.Amount);
                    Console.WriteLine($"Undid item restock: {lastTransaction.Item_.Name}");
                    Log.Information("Undid item restock: {name}", lastTransaction.Item_.Name);
                    break;
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine("Nothing to undo! Go do something, and try again.");
            Log.Warning("Nothing to undo: {e}", e.Message);
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("Something went wrong. Sorry!");
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