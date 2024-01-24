using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        #region
        [Fact]
        public async void Index_ToReturnView()
        {
            //Arrange
            //Act
            HttpResponseMessage message = await _client.GetAsync("Persons/Index");
            //Assert
            message.Should().BeSuccessful();
        }
        #endregion
    }
}
