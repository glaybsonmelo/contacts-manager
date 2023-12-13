using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;

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
            List<PersonResponse> listPersonsResponse = new List<PersonResponse>();
            foreach(Person person in _persons)
            {
                listPersonsResponse.Add(person.ToPersonResponse());
            }
            return listPersonsResponse;
        }

        public PersonResponse? GetPersonById(Guid? id)
        {
            throw new NotImplementedException();
        }
    }
}
