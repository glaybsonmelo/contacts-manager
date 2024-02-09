using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new Person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns the same persons details, along with newly generated PersonId</returns>
        public Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
