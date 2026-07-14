using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[ApiController]
[Route("api/[controller]")]

public class InventoryController(
        IInventoryService service, 
        IMapper mapper, 
        IMemoryCache cache,
        ISupplierClient client
    ) : ControllerBase
{
    private readonly IInventoryService _service = service;
    private readonly IMapper _mapper = mapper;
    private readonly IMemoryCache _cache = cache;
    private readonly ISupplierClient _client = client;

    [HttpGet]
    [ResponseCache(Duration = 30)] // Adding response caching (Is not mandatory for the front to obey)
    public async Task<ActionResult<IEnumerable<InventoryDTO>>> Get()
    {
        // Infinite loop when serializing to JSON
        // return Ok(await _repo.GetAllAsync());

        // Without caching
        // var mappedItems = _mapper.Map<List<InventoryDTO>>(await _service.GetAllAsync());
        // return Ok(mappedItems);

        var itemDtos = await _cache.GetOrCreateAsync("inventory-all", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
            return _mapper.Map<List<InventoryDTO>>(await _service.GetAllAsync());
        });
        return Ok(itemDtos);
    }
    [HttpGet("{sku}")]
    public async Task<ActionResult<InventoryDTO>> GetBySku(string sku)
    {
        var item = await _service.GetBySkuAsync(sku);
        
        if (item is null) return NotFound();
        
        var response = _mapper.Map<InventoryDTO>(item);
        return Ok(response);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InventoryDTO>> Create(InventoryDTO newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDTO>(created);

        // inventory cache invalidation
        _cache.Remove("inventory-all");

        return CreatedAtAction(nameof(GetBySku), new {sku = response.Sku}, response);
    }
    [HttpDelete("{sku}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(string sku)
    {
        if (await _service.RemoveAsync(sku))
        {
            // inventory cache invalidation
            _cache.Remove("inventory-all");
            return NoContent();
        }

        return NotFound();
    }
    [HttpGet("{sku}/supplier-price")]
    [Authorize]
    public async Task<IActionResult> GetSupplierPrice(string sku)
    {
        var price = await _client.GetListPriceAsync(sku);
        if (price is null) return NotFound();
        return Ok(new { sku, supplierPrice = price });
    }
}