using System.ComponentModel.DataAnnotations;

namespace Library.ControllerApi.DTOs;

public record RegisterDto(
    [Required, MaxLength(64)] string UserName,
    [Required, MinLength(8)] string Password    
);

public record LoginDto(
    [Required, MaxLength(64)] string UserName,
    [Required, MinLength(8)] string Password    
);