using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using RespositoryContracts;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
           // throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(countryAddRequest.Name == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.Name));
            }

            if(await _countriesRepository.GetCountryByName(countryAddRequest.Name) != null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.Id = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);
            
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if(id == null)
            {
                return null;
            }

            Country? country_fetched_from_list = await _countriesRepository.GetCountryById(id.Value);

            if (country_fetched_from_list == null)
            {
                return null;
            }

            return country_fetched_from_list.ToCountryResponse();
        }
    }
}