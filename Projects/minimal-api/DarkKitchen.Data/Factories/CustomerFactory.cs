using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data.Factories;

public class CustomerFactory
{
    public Customer CreateCustomer(string name, string email) => new Customer {Name = name, Email = email};
}