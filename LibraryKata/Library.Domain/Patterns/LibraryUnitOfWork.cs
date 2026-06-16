using Serilog;

namespace LibraryKata.Domain;

public class LibraryUnitOfWork(ILibraryRepo items) : IUnitOfWork
{
    public ILibraryRepo Items { get; } = items;

    private readonly List<string> _staged = [];

    public int Commit()
    {
        int count = _staged.Count;
        Log.Information($"LibraryUnitOfWork commited {count} staged changes");

        _staged.Clear();

        return count;
    }

    public void Stage(string change)
    {
        _staged.Add(change);
    }
}