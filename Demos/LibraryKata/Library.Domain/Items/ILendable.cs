namespace LibraryKata.Domain;

public interface ILendable
{
    bool Checkout();
    void Return();
}