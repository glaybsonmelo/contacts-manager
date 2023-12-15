using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.ComponentModel.DataAnnotations;
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
                    !string.IsNullOrEmpty(person.Name) ? (person.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) : true).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Email) ? person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.BirthDate):
                    matchingPersons = allPersons.Where(person =>
                    person.BirthDate != null ? person.BirthDate.Value.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Gender) ? person.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Country):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Country) ? person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Address) ? person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;

            throw new NotImplementedException();
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> listPersonsToSort, string? sortBy, SortOrderOptions sortOrder)
        {
            throw new NotImplementedException();
        }
    }
}
