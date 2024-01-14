using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
