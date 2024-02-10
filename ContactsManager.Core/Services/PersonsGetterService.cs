using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;
using RespositoryContracts;
using Exceptions;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsGetterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
             
            return persons.Select(person => person.ToPersonResponse()).ToList();
        }
        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if(id == null)
                return null;

            Person? personFetched = await _personsRepository.GetPersonById(id.Value);

            if (personFetched == null)
                return null;

            return personFetched.ToPersonResponse();
        }
        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons;

            persons = searchBy switch
            {
                nameof(PersonResponse.Name) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Name.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Email.Contains(searchString)),

                nameof(PersonResponse.BirthDate) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.BirthDate.Value.ToString("dd MMMM yyyy").Contains(searchString)),


                nameof(PersonResponse.Gender) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Gender.Contains(searchString)),

                nameof(PersonResponse.Country) =>
                 await _personsRepository.GetFilteredPersons(temp =>
                 temp.Country.Name.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                await _personsRepository.GetFilteredPersons(temp =>
                temp.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(person => person.ToPersonResponse()).ToList();

        }

    }
}

