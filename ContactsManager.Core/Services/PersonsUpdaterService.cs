using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helpers;
using ServiceContracts.Enums;
using RespositoryContracts;
using Exceptions;

namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsUpdaterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
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
                throw new InvalidPersonIDException("Given personID doesn't exist");
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
    }
}

