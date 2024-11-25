namespace WebApplication1.Data;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool AgreedToTerms { get; set; }
    public int CountryId { get; set; }
    public int CityId { get; set; }
}
