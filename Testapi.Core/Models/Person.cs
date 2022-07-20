namespace Testapi.Models;

public class Person
{
    public Person()
    {
        Addresses = new HashSet<Address>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    // public string[] Adresses { get; set; } = Array.Empty<string>();
    public ICollection<Address> Addresses { get; set; } = null!;
}