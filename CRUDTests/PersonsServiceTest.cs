using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;

        public PersonsServiceTest()
        {
            _personsService = new PersonService();
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
                BirthDate = Convert.ToDateTime("16/07/2003"),
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

    }
}
