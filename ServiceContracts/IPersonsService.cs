﻿using ServiceContracts.DTO;

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
    }
}
