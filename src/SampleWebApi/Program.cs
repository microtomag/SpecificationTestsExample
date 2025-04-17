using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SampleWebApi.Domain;
using SampleWebApi.Persistence;
using SampleWebApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<PersonValidator>(c => c.BaseAddress = new Uri(builder.Configuration["Api"]));
builder.Services.AddDbContext<DatabaseContext>(context => context.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

app.MapGet("/people", async (DatabaseContext context) =>
{
    var people = await context.People.AsNoTracking().ToArrayAsync();
    return Results.Ok(people);
});
app.MapPost("/people", async (DatabaseContext context, PersonValidator validator, Person person) =>
{
    var isValid = await validator.IsValidAsync(person);
    await context.People.AddAsync(person);
    await context.SaveChangesAsync();
    return Results.Created($"/people/{person.Id}", person);
});
app.MapPut("/people/{id:guid}", async (DatabaseContext context, Guid id, Person person) =>
{
    var existingPerson = await context.People.FindAsync(id);
    if (existingPerson is null)
    {
        return Results.NotFound();
    }

    existingPerson.FirstName = person.FirstName;
    existingPerson.LastName = person.LastName;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
await dbContext.Database.EnsureCreatedAsync();

app.Run();