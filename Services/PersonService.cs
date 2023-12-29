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
        public PersonService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();

            if (initialize)
            {
                _persons.Add(new Person()
                {
                    Id = Guid.Parse("466dde50-cc3e-492e-aa52-9e4029396ed9"),
                    Name = "Elie",
                    BirthDate = Convert.ToDateTime("7/18/2004"),
                    Email = "eattarge0@jiathis.com",
                    Gender = "Female",
                    CountryId = Guid.Parse("8E38BEC7-A7AE-42E5-BE2E-06504E348305"),
                    ReceiveNewLatters = true,
                    Address = "5 Rockefeller Hill"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("88e6b0eb-935e-4ece-8c1f-21f416585404"),
                    Name = "Shena",
                    BirthDate = Convert.ToDateTime("8/14/2010"),
                    Email = "scorrea1@princeton.edu",
                    CountryId = Guid.Parse("601CA3EE-9716-422B-9899-572C155FB749"),
                    Gender = "Female",
                    ReceiveNewLatters = false,
                    Address = "822 Vernon Alley"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("b5d0805c-66e0-465a-ad17-7bddedc3bca0"),
                    Name = "Ross",
                    BirthDate = Convert.ToDateTime("3/2/2023"),
                    Email = "rmarney2@ocn.ne.jp",
                    CountryId = Guid.Parse("8E38BEC7-A7AE-42E5-BE2E-06504E348305"),
                    Gender = "Male",
                    ReceiveNewLatters = false,
                    Address = "3988 Aberg Point"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("63b9e67e-1610-4eca-b031-bb8822151f79"),
                    Name = "Valene",
                    BirthDate = Convert.ToDateTime("5/1/2018"),
                    Email = "vlawie3@bloomberg.com",
                    CountryId = Guid.Parse("601CA3EE-9716-422B-9899-572C155FB749"),
                    Gender = "Female",
                    ReceiveNewLatters = false,
                    Address = "866 Bluestem Trail"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("c6c61b7d-7b3f-40e2-9607-e145874cc4e8"),
                    Name = "Maribeth",
                    BirthDate = Convert.ToDateTime("11/16/2010"),
                    Email = "mrubens4@nymag.com",
                    CountryId = Guid.Parse("FE05C438-DBAF-4786-B3D9-7328FC5D7E5F"),
                    Gender = "Female",
                    ReceiveNewLatters = true,
                    Address = "5606 Carberry Place"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("af418229-e2cd-4f00-a4fc-4cca179cdb05"),
                    Name = "Martyn",
                    BirthDate = Convert.ToDateTime("4/6/2012"),
                    Email = "mpasby5@parallels.com",
                    CountryId = Guid.Parse("9F0D243A-6C53-4C36-99E1-328590A8C49D"),
                    Gender = "Male",
                    ReceiveNewLatters = true,
                    Address = "923 Welch Center"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("7cbdcd4a-bfac-46b1-9f58-70d688e85e0a"),
                    Name = "Jane",
                    BirthDate = Convert.ToDateTime("11/1/2016"),
                    Email = "jsaunton6@simplemachines.org",
                    CountryId = Guid.Parse("FE05C438-DBAF-4786-B3D9-7328FC5D7E5F"),
                    Gender = "Female",
                    ReceiveNewLatters = true,
                    Address = "027 Parkside Street"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("dfe3e227-8b8c-4ed0-a2c3-241418da436f"),
                    Name = "Garrard",
                    BirthDate = Convert.ToDateTime("10/30/2009"),
                    Email = "ggun7@wikispaces.com",
                    CountryId = Guid.Parse("8E38BEC7-A7AE-42E5-BE2E-06504E348305"),
                    Gender = "Male",
                    ReceiveNewLatters = false,
                    Address = "434 Sherman Plaza"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("bc33a9e0-faad-4336-850c-07ae4dd7ba3a"),
                    Name = "Eberhard",
                    BirthDate = Convert.ToDateTime("2/23/2004"),
                    Email = "etedstone8@hexun.com",
                    CountryId = Guid.Parse("5385A331-A7F4-4AC2-A426-A608ACEB25A2"),
                    Gender = "Male",
                    ReceiveNewLatters = false,
                    Address = "1 Eastlawn Road"
                });

                _persons.Add(new Person()
                {
                    Id = Guid.Parse("1263451a-0b2f-45cb-9a81-d0b527153a39"),
                    Name = "Eirena",
                    BirthDate = Convert.ToDateTime("6/19/2023"),
                    Email = "ecerith9@oaic.gov.au",
                    CountryId = Guid.Parse("5385A331-A7F4-4AC2-A426-A608ACEB25A2"),
                    Gender = "Female",
                    ReceiveNewLatters = true,
                    Address = "48 Quincy Hill"
                });

            }
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
            return _persons.Select(person => ConvertPersonIntoPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonById(Guid? id)
        {
            if(id == null)
                return null;
            
            Person? personFetched = _persons.Find(person => person.Id == id);

            if (personFetched == null)
                return null;

            return ConvertPersonIntoPersonResponse(personFetched);

        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;
            
            switch(searchBy)
            {
                case nameof(PersonResponse.Name):
                  
                    matchingPersons = allPersons.Where(person =>
                     person.Name?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(person =>
                    person.Email?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(PersonResponse.BirthDate):
                    matchingPersons = allPersons.Where(person =>
                    person.BirthDate?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();

                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(person =>
                     person.Gender?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(PersonResponse.Country):
                    matchingPersons = allPersons.Where(person =>
                     person.Country?.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? true).ToList();
                    break;

                case nameof(PersonResponse.Address):
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

            return ConvertPersonIntoPersonResponse(matching_person);
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

