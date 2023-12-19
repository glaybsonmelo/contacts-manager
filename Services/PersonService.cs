using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;
        public PersonService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }
        private PersonResponse ConvertPersonIntoPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryById(person.CountryId)?.Name;
            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            // Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            Person? person = personAddRequest.ToPerson();

            person.Id = Guid.NewGuid();

            _persons.Add(person);

            return ConvertPersonIntoPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonById(Guid? id)
        {
            if(id == null)
                return null;
            
            Person? personFetched = _persons.Find(person => person.Id == id);

            if (personFetched == null)
                return null;

            return personFetched.ToPersonResponse();

        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;
            
            switch(searchBy)
            {
                case nameof(Person.Name):
                  
                    matchingPersons = allPersons.Where(person =>
                     person.Name?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(person =>
                    person.Email?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(Person.BirthDate):
                    matchingPersons = allPersons.Where(person =>
                    person.BirthDate?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();

                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(person =>
                     person.Gender?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(PersonResponse.Country):
                    matchingPersons = allPersons.Where(person =>
                     person.Country?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(person =>
                     person.Address?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;

            throw new NotImplementedException();
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder)
        {
            if(string.IsNullOrEmpty(sortBy)) 
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

                (nameof(PersonResponse.ReceiveNewLatters), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.ReceiveNewLatters).ToList(),
                (nameof(PersonResponse.ReceiveNewLatters), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.ReceiveNewLatters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            
            // Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get maching person
            Person? matching_person = _persons.Find(person => person.Id == personUpdateRequest.PersonId);

            if(matching_person == null)
            {
                throw new ArgumentException("Given personID doesn't exist");
            }

            //Updating data
            matching_person.Name = personUpdateRequest.Name;
            matching_person.Address = personUpdateRequest.Address;
            matching_person.BirthDate = personUpdateRequest.BirthDate;
            matching_person.CountryId = personUpdateRequest.CountryId;
            matching_person.Gender = personUpdateRequest.Gender.ToString();
            matching_person.Email = personUpdateRequest.Email;
            matching_person.ReceiveNewLatters = personUpdateRequest.ReceiveNewLatters;

            return matching_person.ToPersonResponse();
        }

        public bool DeletePerson(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Person? person_from_get = _persons.Find(person => person.Id == id);

            if (person_from_get == null)
                return false;

            return true;
        }
    }
}
