﻿using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit.Sdk;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(dbContext);

        }
        #region AddCountry
        // when CountriesAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task  AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
            // Act
                await _countriesService.AddCountry(request);
            });

        }
        // when the country  Name is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull(){
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { Name = null };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request);
            });
        }
        // when the country Name is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { Name = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { Name = "USA" };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }
        // when you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { Name = "Brazil" };

            // Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = await _countriesService.GetAllCountries();
            // Assert
            Assert.True(response.Id != Guid.Empty);
            Assert.Contains(response, countriesFromGetAllCountries);
        }
        #endregion

        #region GetAllCountries
        // the list of countries should be empty by default
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            // Acts
            List<CountryResponse> actual_countries_for_response_list = await _countriesService.GetAllCountries();

            // Assert
            Assert.Empty(actual_countries_for_response_list);
        }
        // the list of countries should be empty by default
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            // Arrange
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
            {
                new CountryAddRequest() { Name = "USA" },
                new CountryAddRequest() { Name = "Brazil" }
            };


            // Act
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach(CountryAddRequest country_request in country_request_list)
            {
                countries_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
            }
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            // read each element from countries_list_from_add_country
            foreach(CountryResponse expected_country in countries_list_from_add_country)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
            //Assert.NotEmpty(actual_countries_for_response_list);
        }
        #endregion

        #region GetCountryById
        [Fact]
        //if we supply null as Id, it should return null as CountryResponse
        public async Task GetCountryById_NullId()
        {
            //Arrange
            Guid? countryId = null;

            //Act
            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryById(countryId);

            //Assert
            Assert.Null(country_response_from_get_method);
        }
        [Fact]
        // if we supply a valid id, it should return the matching country details as CountryResponse object
        public async Task GetCountryById_ValidId()
        {
            //Arrange
            CountryAddRequest? country_add_request = new CountryAddRequest() { Name = "USA" };
            CountryResponse new_country_from_add = await _countriesService.AddCountry(country_add_request);

            //Act
            Guid country_id = new_country_from_add.Id;
            CountryResponse? country_response_from_get = await _countriesService.GetCountryById(country_id);

            //Assert 
            Assert.Equal(new_country_from_add, country_response_from_get);
        }
        #endregion
    }
}
