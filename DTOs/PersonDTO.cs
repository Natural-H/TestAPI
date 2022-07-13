namespace Testapi.DataTranserObjects;

public class PersonDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string[] Adresses { get; set; } = Array.Empty<string>();
}