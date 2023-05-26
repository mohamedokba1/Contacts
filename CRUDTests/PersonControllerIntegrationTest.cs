using Contacts_Manager;
using FluentAssertions;

namespace CRUDTests
{
    public class PersonControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonControllerIntegrationTest(CustomWebApplicationFactory WebFactory)
        {
            _client = WebFactory.CreateClient();
        }
        [Fact]
        public async Task Index_ToReturnView()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful();
        }
    }
}
