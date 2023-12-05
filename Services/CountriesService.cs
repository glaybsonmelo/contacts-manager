using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;
        public CountriesService()
        {
            _countries = new List<Country>();
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

            if(country_fetched_from_list == null)
            {
                return null;
            }

            return country_fetched_from_list.ToCountryResponse();
        }
    }
}