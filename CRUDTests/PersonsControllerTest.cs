using AutoFixture;
using Moq;
using ServiceContracts;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using CRUDExample.Controllers;
using ServiceContracts.DTO;
using Microsoft.AspNetCore.Mvc;
namespace CRUDTests
{
    public class PersonsControllerTest 
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsServiceMock = new Mock<IPersonsService>();
            _countriesServiceMock = new Mock<ICountriesService>();

            _personsService = _personsServiceMock.Object;
            _countriesService = _countriesServiceMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            
            List<PersonResponse> person_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personController = new PersonsController(_personsService, _countriesService);

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(person_response_list);
            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(person_response_list);

            //Act
            IActionResult result = await personController.Index(
                _fixture.Create<string>(), 
                _fixture.Create<string>(), 
                _fixture.Create<string>(), 
                _fixture.Create<SortOrderOptions>()
                );
            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            //ViewResult viewResult =  result.Should().BeOfType<ViewResult>();
            viewResult.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(person_response_list);
        }
        #endregion

    }
}
