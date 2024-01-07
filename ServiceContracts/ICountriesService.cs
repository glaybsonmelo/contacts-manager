namespace ServiceContracts;
using ServiceContracts.DTO;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Return country object after adding it (including newly generated country id)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    /// <summary>
    /// Returns all countries from the list 
    /// </summary>
    /// <returns>All countries from the list as list of CountryResponse</returns>
    Task<List<CountryResponse>> GetAllCountries();
    /// <summary>
    /// returns a country object based on the given country id
    /// </summary>
    /// <param name="id">Id (Guid) to search</param>
    /// <returns>Matching country as CountryResponse object</returns>
    Task<CountryResponse?> GetCountryById(Guid? id);
}