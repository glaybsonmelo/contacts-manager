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
        private readonly ICountriesService _countriesService;

        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;

        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;

        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();

            _countriesServiceMock = new Mock<ICountriesService>();

            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;

            _countriesService = _countriesServiceMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ToReturnIndexViewWithPersonsList()
        {
            
            List<PersonResponse> person_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );

            _personsGetterServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(person_response_list);
            _personsSorterServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(person_response_list);

            //Act
            IActionResult result = await personsController.Index(
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
        public async Task Create_IfNoModelValidationErrors_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            _personsAdderServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );
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

            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );

            _personsGetterServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person_response);

            //Act
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }

        [Fact]
        public async Task Edit_IfNoModelValidationErrors_ToReturnEditView()
        {
            //Arrange
            PersonResponse? person_response = _fixture.Build<PersonResponse>()
                .With(person => person.Gender, GenderOptions.Male.ToString())
                .Create();
            PersonUpdateRequest person_update_request = person_response.ToPersonUpdateRequest();

            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _personsGetterServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person_response);
            _personsUpdaterServiceMock.Setup(temp => temp.UpdatePerson(It.IsAny<PersonUpdateRequest>())).ReturnsAsync(person_response);
            _countriesServiceMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            //Act
            IActionResult result = await personsController.Edit(person_update_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_IfInvalidPersonId_ToReturnViewIndex()
        {
            //Arrange
            PersonResponse? person_response = _fixture.Build<PersonResponse>()
                .With(person => person.Gender, GenderOptions.Female.ToString())
                .Create();

            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );

            _personsGetterServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(null as PersonResponse);
            _personsDeleterServiceMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);
            //Act
            IActionResult result = await personsController.Delete(person_response);
            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        [Fact]
        public async Task Delete_IfValidPersonId_ToReturnViewIndex()
        {
            //Arrange

            PersonResponse? person_response = _fixture.Build<PersonResponse>()
                .With(person => person.Gender, GenderOptions.Female.ToString())
                .Create();

            PersonsController personsController = new PersonsController(
                _countriesService,
                _personsAdderService,
                _personsDeleterService,
                _personsGetterService,
                _personsSorterService,
                _personsUpdaterService
        );

            _personsGetterServiceMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person_response);
            _personsDeleterServiceMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>())).ReturnsAsync(true);
            //Act
            IActionResult result = await personsController.Delete(person_response);
            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Persons");
        }
        #endregion
    }
}
