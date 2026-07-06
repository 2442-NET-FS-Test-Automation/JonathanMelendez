namespace LibraryKata.Domain;
public class ItemNotAvailableException(string title) 
    : LibraryException($"{title} has no copies available.") {}