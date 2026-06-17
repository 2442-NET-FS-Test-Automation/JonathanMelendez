using Serilog;

namespace Store.Domain;

public class History
{
    private readonly Stack<Transaction> Transactions = [];
    public void HistoryAdd()
    {
        throw new NotImplementedException();
    }
    public bool HistoryUndo()
    {
        throw new NotImplementedException();
    }
    public bool HistoryRedo()
    {
        throw new NotImplementedException();
    }
}