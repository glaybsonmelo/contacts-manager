using Entities;

namespace RespositoryContracts
{
    /// <summary>
    /// it represents data acess logic for managing Peron Entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add a new country object to the data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>return the country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);
        /// <summary>
        /// Return all countries in the data store
        /// </summary>
        /// <returns>all countries from the table</returns>
        Task<List<Country>> GetAllCountries();
        /// <summary>
        /// Returns a Country object based on the given Id; otherwise returns null;
        /// </summary>
        /// <param name="id">id of country to search</param>
        /// <returns>Matching Country object ou null</returns>
        Task<Country?> GetCountryById(Guid id);
        /// <summary>
        /// Returns country based on the given name; otherwise returns null
        /// </summary>
        /// <param name="name">name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByName(string name);
    }
}