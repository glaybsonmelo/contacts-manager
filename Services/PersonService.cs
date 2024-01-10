using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class PersonService : IPersonsService
    {
        private readonly ApplicationDbContext _db;

        public PersonService(ApplicationDbContext personsDbContext)
        {
            _db = personsDbContext;
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

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //return _db.Persons.Select(person => ConvertPersonIntoPersonResponse(person)).ToList();

            var persons = await _db.Persons.Include("Country").ToListAsync();
             
            return persons.Select(person => person.ToPersonResponse()).ToList();
        }
        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            if(id == null)
                return null;
            
            Person? personFetched = await _db.Persons.FirstOrDefaultAsync(person => person.Id == id);

            if (personFetched == null)
                return null;

            return personFetched.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {

            List<PersonResponse> allPersons = await GetAllPersons();
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
            Person? matching_person = await _db.Persons.FirstOrDefaultAsync(person => person.Id == personUpdateRequest.PersonId);

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
            matching_person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _db.SaveChangesAsync();

            return matching_person.ToPersonResponse();
        } 

        public async Task<bool> DeletePerson(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Person? person_from_get = await _db.Persons.FirstOrDefaultAsync(person => person.Id == id);

            if (person_from_get == null)
                return false;

            _db.Persons.Remove(person_from_get);
            await _db.SaveChangesAsync();
            
            return true;
        }
    }
}

