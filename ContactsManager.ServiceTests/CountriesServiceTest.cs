using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.VisualBasic;
using Moq;
using RespositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System.Diagnostics.Metrics;
using Xunit.Sdk;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly IFixture _fixture;
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
        }
        #region AddCountry
        // when CountriesAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task  AddCountry_NullCountry_ToBeArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            Func<Task> action = (async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            });
            await action.Should().ThrowAsync<ArgumentNullException>();

        }
        // when the country  Name is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException(){
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { Name = null };
            Country country = request.ToCountry();
            // Assert
            Func<Task> action = (async () =>
            {
                // Act
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
                await _countriesService.AddCountry(request);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // when the country Name is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { Name = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { Name = "USA" };
            Country country = request1.ToCountry();
            // Assert
            Func<Task> action = (async () =>
            {
                // Act
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
                _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(country);
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // when you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails_ToBeSuccessful()
        {
            // Arrange
            Country country = _fixture.Build<Country>()
                .With(country => country.Name, "Brazil")
                .With(country => country.Persons, null as List<Person>)
                .Create();
            List<Country> countries = new List<Country>() { country };
            // Act
            CountryResponse countryResponse = country.ToCountryResponse();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            List<CountryResponse> countriesFromGetAllCountries = await _countriesService.GetAllCountries();
            // Assert

            country.Id.Should().NotBe(Guid.Empty);
            countriesFromGetAllCountries.Should().Contain(countryResponse);
        }
        #endregion

        #region GetAllCountries
        // the list of countries should be empty by default
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeSuccessful()
        {
            // Acts
            List<Country> countries = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            List<CountryResponse> actual_countries_for_response_list = await _countriesService.GetAllCountries();

            // Assert
            //Assert.Empty(actual_countries_for_response_list);
            actual_countries_for_response_list.Should().BeEmpty();
        }
        // the list of countries should be empty by default
        [Fact]
        public async Task GetAllCountries_AddFewCountries_ToBeSuccessful()
        {
            // Arrange
            List<Country> countries = new List<Country>()
            {
                _fixture.Build<Country>()
                    .With(country => country.Name, "EUA")
                    .With(country => country.Persons, null as List<Person>)
                    .Create(),
                _fixture.Build<Country>()
                    .With(country => country.Name, "Br")
                    .With(country => country.Persons, null as List<Person>)
                    .With(temp => temp.Persons, null as List<Person>).Create()
            };

            // Act
            List<CountryResponse> countries_response_list_expeected = countries.Select(person => person.ToCountryResponse()).ToList();
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            // read each element from countries_list_from_add_country
            //foreach(CountryResponse expected_country in countries_list_from_add_country)
            //{
            //    Assert.Contains(expected_country, actualCountryResponseList);
            //}
            actual_country_response_list.Should().BeEquivalentTo(countries_response_list_expeected);
        }
        #endregion

        #region GetCountryById
        [Fact]
        //if we supply null as Id, it should return null as CountryResponse
        public async Task GetCountryById_NullId_ToBeSuccessful()
        {
            //Arrange
            Guid? countryId = null;

            //Act
            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(null as Country);
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryById(countryId);

            //Assert
            country_response_from_get_method.Should().BeNull();
        }
        [Fact]
        // if we supply a valid id, it should return the matching country details as CountryResponse object
        public async Task GetCountryById_ValidId()
        {
            //Arrange
            Country? country = _fixture.Build<Country>()
                .With(country => country.Persons, null as List<Person>)
                .Create();
            //Act
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock.Setup(temp => temp.GetCountryById(It.IsAny<Guid>())).ReturnsAsync(country); ;

            CountryResponse? country_response_expected = await _countriesService.GetCountryById(country.Id);

            //Assert 
            country_response.Should().Be(country_response_expected);
        }
        #endregion
    }
}
