using Library.Data;
using Library.Data.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class InventoryController(IInventoryRepository repo)
    : ControllerBase
{
    private readonly IInventoryRepository _repo = repo;

    [HttpGet]
    public async Task<ActionResult<EntireInventoryDTO>> Get()
    {
        // Infinite loop when serializing to JSON
        // return Ok(await _repo.GetAllAsync());

        var items = await _repo.GetAllAsync();
        EntireInventoryDTO response = new();

        foreach (var item in items)
        {
            response.EntireInventory.Add(new InventoryReturnDTO
            {
               Name = item.Product.Name,
               Sku = item.Product.Sku,
               CurrentStock = item.CurrentStock
            });
        }

        return Ok(response);
    }
    [HttpGet("[sku]")]
    public async Task<ActionResult<InventoryReturnDTO>> GetBySku(string sku)
    {
        var item = await _repo.GetInventoryItemBySkuAsync(sku);
        
        if (item is null) return NotFound();
        
        var response = new InventoryReturnDTO
        {
            Name = item.Product.Name,
            Sku = item.Product.Sku,
            CurrentStock = item.CurrentStock
        };

        return Ok(response);
    }
}