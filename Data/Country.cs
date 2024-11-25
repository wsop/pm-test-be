namespace WebApplication1.Data;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Province> Provinces { get; set; } = new();
}