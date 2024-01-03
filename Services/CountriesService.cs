using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.Xml.Linq;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
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

            int qtyCountryWithSameName = _db.Countries.Count(country => country.Name == countryAddRequest.Name);

            if(qtyCountryWithSameName > 1)
            {
                throw new ArgumentException("Given country name already exists");
            }

            Country country = countryAddRequest.ToCountry();
            country.Id = Guid.NewGuid();

            _db.Countries.Add(country);
            
            _db.SaveChanges();
            
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryById(Guid? id)
        {
            if(id == null)
            {
                return null;
            }

            Country? country_fetched_from_list = _db.Countries.FirstOrDefault(c => c.Id == id);

            if (country_fetched_from_list == null)
            {
                return null;
            }

            return country_fetched_from_list.ToCountryResponse();
        }
    }
}