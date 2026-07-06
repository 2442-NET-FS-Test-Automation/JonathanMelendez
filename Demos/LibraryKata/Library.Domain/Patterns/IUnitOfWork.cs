namespace LibraryKata.Domain;
public interface IUnitOfWork
{
    ILibraryRepo Items { get; }
    void Stage(string change);

    int Commit();
}