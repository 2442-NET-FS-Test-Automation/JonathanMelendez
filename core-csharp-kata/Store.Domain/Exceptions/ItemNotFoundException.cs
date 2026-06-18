namespace Store.Domain;

public class ItemNotFoundException(int id) : ItemException($"No library item with id {id}")
{
    public int Id { get; } = id;
}