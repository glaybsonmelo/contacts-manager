using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="listPersonsToSort">Represents list of persons to sort</param>
        /// <param name="sortBy">name of property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Return Sorted persons as PersonResponse list</returns>
        public Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder);
    }
}
