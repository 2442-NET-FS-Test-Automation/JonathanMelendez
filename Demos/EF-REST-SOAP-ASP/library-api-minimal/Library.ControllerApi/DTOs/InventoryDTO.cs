using System.ComponentModel.DataAnnotations;

namespace Library.ControllerApi.DTOs;

public record InventoryDTO(
    [Required,  MaxLength(20)] string Sku, 
    [Required, MaxLength(200)] string Name, 
    [Required, Range(0, int.MaxValue)] int CurrentStock, 
    [Range(0.01, 100000)] decimal Price
);