using WebApplication1.Data;

namespace Demo.Shop.Back.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("/api/register")]
public class RegisterController : ControllerBase
{
    private readonly AppDbContext _context;

    public RegisterController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.Password != request.ConfirmPassword)
            return BadRequest(new { Message = "Passwords do not match." });

        if (!request.Agree)
            return BadRequest(new { Message = "You must agree to the terms." });

        if (!await _context.Countries.AnyAsync(c => c.Id == request.Country))
            return BadRequest(new { Message = "Invalid country." });

        if (!await _context.Provinces.AnyAsync(p => p.Id == request.City && p.CountryId == request.Country))
            return BadRequest(new { Message = "Invalid city for the selected country." });

        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return Conflict(new { Message = "Username is already taken." });
        
        using var sha256 = SHA256.Create();
        var passwordHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));
        
        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            AgreedToTerms = request.Agree,
            CountryId = request.Country,
            CityId = request.City
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { Id = user.Id }, new { Message = "User registered successfully." });
    }
}
