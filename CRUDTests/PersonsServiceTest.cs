using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        public PersonsServiceTest()
        {
            _personsService = new PersonService();
            _countriesService = new CountriesService();
        }

        #region AddPerson
        // when we supply a null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Assert.Throws<ArgumentNullException>(() =>
            {
            //Assert
                _personsService.AddPerson(personAddRequest);
            });
        }

        // When we supply Null value as Name, it should throw ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = null
            };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }
        // When we supply Null value as Email, it should throw ArgumentException
        [Fact]
        public void AddPerson_PersonEmailIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Email = null
            };

            //Act
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.AddPerson(personAddRequest);
            });
        }
        // When we supply propper persons details, it should insert the Person into the list
        [Fact]
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = Guid.NewGuid(),
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewLatters = true,
            };

            //Act
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> all_persons_list = _personsService.GetAllPersons();

            //Assert

            Assert.True(person_response_from_add.Id != Guid.Empty);
            Assert.Contains(person_response_from_add, all_persons_list);
        }

        #endregion
        #region GetPersonById


        // if we supply null as person id, it should return null as PersonResponse
        [Fact]
        public void GetPersonById_NullId()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person_response_from_get = _personsService.GetPersonById(personId);

            // Assert
            Assert.Null(person_response_from_get);
        }

        // if we supply a valid person id, it should return the valid person details as PersonResponse 
        [Fact]
        public void GetPersonById_ValidId()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "Brazil"
            };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
            
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "MyName",
                Email = "email@valid.com",
                Address = "street 101, brooklin",
                BirthDate = Convert.ToDateTime("07/156/2003"),
                CountryId = countryResponse.Id,
                Gender = GenderOptions.Female,
                ReceiveNewLatters = true,

            };
            //Act
            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
            Guid? personId = person_response_from_add.Id;

            PersonResponse? person_response_from_get = _personsService.GetPersonById(personId);
            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);

        }

        #endregion  
    }
}
