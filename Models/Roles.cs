using AllulExpressApi.Models;

public class Role
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public string? Description { get; set; }

    public ICollection<Employees>? Employees { get; set; }
}
