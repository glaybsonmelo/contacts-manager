using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.Xml.Linq;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();

            if (initialize)
            {
                _countries.AddRange(new List<Country>(){
                    new Country()
                    {
                        Id = Guid.Parse("8E38BEC7-A7AE-42E5-BE2E-06504E348305"),
                        Name = "Brazil"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("601CA3EE-9716-422B-9899-572C155FB749"),
                        Name = "USA"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("FE05C438-DBAF-4786-B3D9-7328FC5D7E5F"),
                        Name = "German"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("9F0D243A-6C53-4C36-99E1-328590A8C49D"),
                        Name = "Canada"
                    },
                    new Country()
                    {
                        Id = Guid.Parse("5385A331-A7F4-4AC2-A426-A608ACEB25A2"),
                        Name = "Mexico"
                    }
                });
            }
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(countryAddRequest.Name == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.Name));
            }

            Country? existCountryWithSameName = _countries.Find(country => country.Name == countryAddRequest.Name);

            if(existCountryWithSameName != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.Id = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            if(id == null)
            {
                return null;
            }

            Country? country_fetched_from_list = _countries.Find(c => c.Id == id);

            if (country_fetched_from_list == null)
            {
                return null;
            }

            return country_fetched_from_list.ToCountryResponse();
        }
    }
}