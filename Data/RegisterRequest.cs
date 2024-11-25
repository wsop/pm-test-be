using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Data;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public bool Agree { get; set; }

    [Required]
    public int Country { get; set; }

    [Required]
    public int City { get; set; }
}

