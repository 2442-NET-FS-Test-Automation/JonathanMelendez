namespace DarkKitchen.Data.Exceptions;

public class DishNotFoundException(int id) : Exception ($"Dish with id {id} not found")
{
    public int dishId = id;
}