using Microsoft.EntityFrameworkCore;
using Testapi.DataTranserObjects;
using Testapi.Mappings;
using Testapi.Models;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
await using var context = new PeopleContext(); // Can I access to an already created instance?

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<PeopleContext>(opt => 
    opt.UseNpgsql(@"Host=localhost;Username=postgres;Password=password;Database=yes"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
