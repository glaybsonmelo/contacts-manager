using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;
using RespositoryContracts;
using Exceptions;

namespace Services
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsDeleterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }
        public async Task<bool> DeletePerson(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Person? person_from_get = await _personsRepository.GetPersonById(id.Value);

            if (person_from_get == null)
                return false;

            await _personsRepository.DeletePersonByPersonId(person_from_get.Id);
            
            return true;
        }
    }
}

