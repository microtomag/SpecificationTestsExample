namespace SampleWebApi.Domain;

public class Person(string firstName, string lastName)
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = firstName;
    public string LastName { get; set; } = lastName;
}