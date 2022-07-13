namespace Testapi.Models;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string[] Adresses { get; set; } = Array.Empty<string>();
}