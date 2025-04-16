using System.Net;
using System.Net.Http.Json;
using SampleWebApi.Domain;

namespace IntegrationTests;

public class SimpleApiIntegrationTests : IClassFixture<SampleWebApiContainer>
{
    private readonly SampleWebApiContainer _sampleWebApiContainer;

    public SimpleApiIntegrationTests(SampleWebApiContainer sampleWebApiContainer)
    {
        _sampleWebApiContainer = sampleWebApiContainer;
    }

    [Fact]
    public async Task GetPeople()
    {
        // Arrange
        var client = _sampleWebApiContainer;

        // Act
        var response = await client.GetFromJsonAsync<IEnumerable<Person>>("/people");

        // Assert
        Assert.NotNull(response);
    }
    
    [Fact]
    public async Task PostPerson()
    {
        // Arrange
        var client = _sampleWebApiContainer;
        var person = new Person("John", "Doe");
        var request = new HttpRequestMessage(HttpMethod.Post, "/people")
        {
            Content = JsonContent.Create(person)
        };

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task PutPerson_WhenPersonDoesNotExist_ReturnNotFound()
    {
        // Arrange
        var client = _sampleWebApiContainer;
        var person = new Person("Jane", "Doe");
        var request = new HttpRequestMessage(HttpMethod.Put, $"/people/{Guid.NewGuid()}")
        {
            Content = JsonContent.Create(person)
        };

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task PutPerson_WhenPersonExists_ReturnNoContent()
    {
        // Arrange
        var client = _sampleWebApiContainer;
        var person = new Person("Jane", "Doe");
        var postResponse = await client.PostAsJsonAsync("/people", person);
        var createdPerson = await postResponse.Content.ReadFromJsonAsync<Person>();

        // Act
        var updatedPerson = new Person("Jane", "Smith");
        var request = new HttpRequestMessage(HttpMethod.Put, $"/people/{createdPerson.Id}")
        {
            Content = JsonContent.Create(updatedPerson)
        };
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}