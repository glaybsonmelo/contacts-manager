﻿using ServiceContracts;
using ServiceContracts.DTO;
using Services;
namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }
        // when CountriesAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
            // Act
                _countriesService.AddCountry(request);
            });

        }
        // when the country  Name is null, it should throw ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull(){
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { Name = null };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _countriesService.AddCountry(request);
            });
        }
        // when the country Name is duplicate, it should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            // Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { Name = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { Name = "USA" };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }
        // when you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            // Arrange
            CountryAddRequest? request = new CountryAddRequest() { Name = "Brazil" };

            // Act
            CountryResponse response = _countriesService.AddCountry(request);

            // Assert
            Assert.True(response.Id != Guid.Empty);
        }
    }
}
