using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new Person into the list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns the same persons details, along with newly generated PersonId</returns>
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Returns all Persons 
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        public List<PersonResponse> GetAllPersons();


        /// <summary>
        /// Returns PersonResponse based on the given person id
        /// </summary>
        /// <param name="id">person id to search</param>
        /// <returns>Returns maching PersonResponse object</returns>
        public PersonResponse? GetPersonById(Guid? id);

        /// <summary>
        /// Returns all person object that mathches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching parsons based on the given search field and search string</returns>
        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="listPersonsToSort">Represents list of persons to sort</param>
        /// <param name="sortBy">name of property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Return Sorted persons as PersonResponse list</returns>
        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Update the specific person based on the given PersonId
        /// </summary>
        /// <param name="personUpdateRequest">Persons details to updated incluing Person Id</param>
        /// <returns>Returns PersonResponse</returns>
        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        ///  Deletes a person based on the given person id
        /// </summary>
        /// <param name="id">Id to delete</param>
        /// <returns>Returns true if the deletion is successfull; otherwise false</returns>
        public bool DeletePerson(Guid? id);
    }
}
