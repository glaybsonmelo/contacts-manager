using AutoFixture;
using Moq;
using ServiceContracts;
using ServiceContracts.Enums;
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
        public async Task Index_ToReturnIndexViewWithPersonsList()
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
        #region Create
        
        [Fact]
        public async Task Create_IfModelValidationErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);
            //Act
            personsController.ModelState.AddModelError("Name", "Name can't be blank");
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
          
            // RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
           //viewResult.ActionName.Should().Be("index");
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(person_add_request);
        }
        [Fact]
        public async Task Create_IfNoModelValidationErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);
            //Act

            IActionResult result = await personsController.Create(person_add_request);

            //Assert
          
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
        }
        #endregion

        #region Edit
        [Fact]
        public async Task Edit_IfInvalidPersonId_ToReturnIndexView()
        {
            //Arrange
            PersonUpdateRequest person_update_request = _fixture.Create<PersonUpdateRequest>();
            PersonResponse? person_response = null;

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            _personsServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person_response);

            //Act
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        [Fact]
        public async Task Edit_IfModelValidationErrors_ToReturnEditView()
        {
            //Arrange
            PersonResponse? person_response = _fixture.Build<PersonResponse>()
                .With(person => person.Gender, GenderOptions.Male.ToString())
                .Create();
            PersonUpdateRequest person_update_request = person_response.ToPersonUpdateRequest();

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _personsServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person_response);
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            //Act
            personsController.ModelState.AddModelError("Name", "Name can't be blank");
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.Model.Should().BeAssignableTo<PersonUpdateRequest>();
            viewResult.Model.Should().BeEquivalentTo(person_update_request);
        }
        #endregion

    }
}
