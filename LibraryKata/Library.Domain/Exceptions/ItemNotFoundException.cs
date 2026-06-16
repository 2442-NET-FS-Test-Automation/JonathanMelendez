namespace LibraryKata.Domain;
public class ItemNotFoundException(int id) : LibraryException($"No library item with id {id}")
{
    public DateTime Time = DateTime.Now;
    public int Id { get; } = id;
}