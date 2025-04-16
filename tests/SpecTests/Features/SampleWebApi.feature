Feature: SampleWebApi
    
Scenario: Add new person
    Given Database is empty
    When I add a new user with name "John" "Doe"
    Then the user should be added to the database
    
Scenario: Get all people
    Given People exist in the database
    When I request all people
    Then I should receive a list of people