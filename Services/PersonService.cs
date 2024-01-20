using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;
using RespositoryContracts;

namespace Services
{
    public class PersonService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            // Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            Person? person = personAddRequest.ToPerson();

            person.Id = Guid.NewGuid();

            Person ps = await _personsRepository.AddPerson(person);

            return ps.ToPersonResponse();

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
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.Name) =>
                     await _personsRepository.GetFilteredPersons(person =>
                         person.Name.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                    await _personsRepository.GetFilteredPersons(person =>
                        person.Email.Contains(searchString)),

                nameof(PersonResponse.BirthDate) =>
                    await _personsRepository.GetFilteredPersons(person =>
                        person.BirthDate.Value.ToString("dd MMMM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                    await _personsRepository.GetFilteredPersons(person =>
                        person.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryId) =>
                    await _personsRepository.GetFilteredPersons(person =>
                        person.Country.Name.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                    await _personsRepository.GetFilteredPersons(person =>
                        person.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };
            return persons.Select(person => person.ToPersonResponse()).ToList();

        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder)
        {

            if (string.IsNullOrEmpty(sortBy)) 
                return allPersons;
            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.Name), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Name), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.BirthDate), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.BirthDate).ToList(),
                (nameof(PersonResponse.BirthDate), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.BirthDate).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Gender).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Gender).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            
            // Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get maching person
            Person? matchingPerson = await _personsRepository.GetPersonById(personUpdateRequest.PersonId.Value);

            if(matchingPerson == null)
            {
                throw new ArgumentException("Given personID doesn't exist");
            }

            //Updating data
            matchingPerson.Name = personUpdateRequest.Name;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.BirthDate = personUpdateRequest.BirthDate;
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);

            return matchingPerson.ToPersonResponse();
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

