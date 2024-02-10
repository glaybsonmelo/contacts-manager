using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsGetterService
    {
        /// <summary>
        /// Returns all Persons 
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        public Task<List<PersonResponse>> GetAllPersons();


        /// <summary>
        /// Returns PersonResponse based on the given person id
        /// </summary>
        /// <param name="id">person id to search</param>
        /// <returns>Returns maching PersonResponse object</returns>
        public Task<PersonResponse?> GetPersonById(Guid? id);

        /// <summary>
        /// Returns all person object that mathches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching parsons based on the given search field and search string</returns>
        public Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
    }
}
