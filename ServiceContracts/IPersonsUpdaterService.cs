using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Update the specific person based on the given PersonId
        /// </summary>
        /// <param name="personUpdateRequest">Persons details to updated incluing Person Id</param>
        /// <returns>Returns PersonResponse</returns>
        public Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
