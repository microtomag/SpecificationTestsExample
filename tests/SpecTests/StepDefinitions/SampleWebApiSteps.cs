using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Reqnroll;
using SampleWebApi.Domain;
using Tests.Common;
using Xunit;

namespace SpecTests.StepDefinitions;

[Binding]
public class SampleWebApiSteps
{
    private static readonly SampleWebApiContainer SampleWebApiContainer = new();    
    
    [BeforeFeature]
    public static async Task BeforeFeature()
    {
        await SampleWebApiContainer.InitializeAsync();
    }
    
    [Given("Database is empty")]
    public void GivenDatabaseIsEmpty()
    {
    }

    [When("I add a new user with name {string} {string}")]
    public async Task WhenIAddANewUserWithName(string firstName, string lastName)
    {
        await SampleWebApiContainer.PostAsJsonAsync("/people", new Person(firstName, lastName));
    }

    [Then("the user should be added to the database")]
    public async Task ThenTheUserShouldBeAddedToTheDatabase()
    {
        var people = await SampleWebApiContainer.GetFromJsonAsync<IEnumerable<Person>>("people");
        Assert.NotNull(people);
        Assert.NotEmpty(people);
    }

    [Given("People exist in the database")]
    public async Task GivenPeopleExistInTheDatabase()
    {
        await SampleWebApiContainer.PostAsJsonAsync("/people", new Person("John", "Doe"));
        await SampleWebApiContainer.PostAsJsonAsync("/people", new Person("Jane", "Doe"));
        await SampleWebApiContainer.PostAsJsonAsync("/people", new Person("James", "Doe"));
        await SampleWebApiContainer.PostAsJsonAsync("/people", new Person("Jessy", "Doe"));
    }

    private IEnumerable<Person> _people = [];
    [When("I request all people")]
    public async Task WhenIRequestAllPeople()
    {
        _people = await SampleWebApiContainer.GetFromJsonAsync<IEnumerable<Person>>("people");
    }

    [Then("I should receive a list of people")]
    public void ThenIShouldReceiveAListOfPeople()
    {
        Assert.NotEmpty(_people);
    }
    
    [AfterFeature]
    public static async Task AfterFeature()
    {
        await SampleWebApiContainer.DisposeAsync();
    }
}