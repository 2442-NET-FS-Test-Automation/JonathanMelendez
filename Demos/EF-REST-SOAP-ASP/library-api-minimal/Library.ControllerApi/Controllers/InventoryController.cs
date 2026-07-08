using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.ControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class InventoryController(IInventoryService service, IMapper mapper)
    : ControllerBase
{
    private readonly IInventoryService _service = service;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryDTO>>> Get()
    {
        // Infinite loop when serializing to JSON
        // return Ok(await _repo.GetAllAsync());

        var mappedItems = _mapper.Map<List<InventoryDTO>>(await _service.GetAllAsync());
        return Ok(mappedItems);
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
    public async Task<ActionResult<InventoryDTO>> Create(InventoryDTO newInv)
    {
        var created = await _service.AddAsync(newInv);
        var response = _mapper.Map<InventoryDTO>(created);
        return CreatedAtAction(nameof(GetBySku), new {sku = response.Sku}, response);
    }
    [HttpDelete("{sku}")]
    public async Task<ActionResult> Delete(string sku)
    {
        if (await _service.RemoveAsync(sku))
        {
            return NoContent();
        }

        return NotFound();
    }
}