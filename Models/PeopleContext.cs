using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Testapi.Models;

public class PeopleContext : DbContext
{
    public PeopleContext(DbContextOptions options) : base(options) { }
    public PeopleContext( /*DbContextOptions options */ ) /* : base(options) */ { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=password;Database=yes");

    public DbSet<Person> People { get; set; } = null!;
}