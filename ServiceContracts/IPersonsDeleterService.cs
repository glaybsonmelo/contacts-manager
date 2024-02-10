using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        ///  Deletes a person based on the given person id
        /// </summary>
        /// <param name="id">Id to delete</param>
        /// <returns>Returns true if the deletion is successfull; otherwise false</returns>
        public Task<bool> DeletePerson(Guid? id);
    }
}
