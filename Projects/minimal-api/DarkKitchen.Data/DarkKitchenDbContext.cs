using Microsoft.EntityFrameworkCore;
using DarkKitchen.Data.Entities;

namespace DarkKitchen.Data;

public class DarkKitchenDbContext : DbContext
{
    public DarkKitchenDbContext(DbContextOptions<DarkKitchenDbContext> options) : base(options) { }   
}