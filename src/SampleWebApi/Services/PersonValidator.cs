using SampleWebApi.Domain;

namespace SampleWebApi.Services;

sealed class PersonValidator(HttpClient httpClient)
{
    public async Task<bool> IsValidAsync(Person person)
    {
        var response =await httpClient.GetFromJsonAsync<ValidatePersonResult>($"api/ValidatePerson");
        return response.IsValid;
    }
}

record ValidatePersonResult(bool IsValid);