using Entities;
using System.Linq.Expressions;

namespace RespositoryContracts
{
    /// <summary>
    /// Represents data acess logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add person object to the data store
        /// </summary>
        /// <param name="person">person object to add</param>
        /// <returns>the Person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);
        /// <summary>
        /// Returns all persons in the data store
        /// </summary>
        /// <returns>List of persons object from table</returns>
        Task<List<Person>> GetAllPersons();
        /// <summary>
        /// It returns an person object based on the given id; otherwise returns null
        /// </summary>
        /// <param name="id"> Id (Guid) to search</param>
        /// <returns> person object or null</returns>
        Task<Person?> GetPersonById(Guid id);
        /// <summary>
        /// returns all persons object based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with the given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">person Id</param>
        /// <returns>Returns true if deletion is successful; otherwise false</returns>
        Task<bool> DeletePersonByPersonId(Guid Id);
        /// <summary>
        /// Updates a person object (name and other details) based on the given person id
        /// </summary>
        /// <param name="person">Person                                                                                      object to update</param>
        /// <returns>Person object updated</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
