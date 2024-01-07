using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(countryAddRequest.Name == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.Name));
            }

            int qtyCountryWithSameName = await _db.Countries.CountAsync(country => country.Name == countryAddRequest.Name);

            if(qtyCountryWithSameName > 1)
            {
                throw new ArgumentException("Given country name already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.Id = Guid.NewGuid();

            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? id)
        {
            if(id == null)
            {
                return null;
            }

            Country? country_fetched_from_list = await _db.Countries.FirstOrDefaultAsync(c => c.Id == id);

            if (country_fetched_from_list == null)
            {
                return null;
            }

            return country_fetched_from_list.ToCountryResponse();
        }
    }
}