using Microsoft.EntityFrameworkCore;
using SampleWebApi.Domain;

namespace SampleWebApi.Persistence;

sealed class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
}