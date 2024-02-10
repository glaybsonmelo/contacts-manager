using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using AutoFixture;
using FluentAssertions;
using RespositoryContracts;
using Moq;
using System.Linq.Expressions;
using Xunit.Abstractions;
namespace CRUDTests
{
    public class PersonsServiceTest
    {

        private readonly Mock<IPersonsRepository> _personRepositoryMock;

        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();
            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            _personsAdderService = new PersonsAdderService(_personsRepository);
            _personsDeleterService = new PersonsDeleterService(_personsRepository);
            _personsGetterService = new PersonsGetterService(_personsRepository);
            _personsSorterService = new PersonsSorterService(_personsRepository);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository);
        }

        #region AddPerson
        // when we supply a null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = (async () =>
            {
            //Assert
                await _personsAdderService.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        // When we supply Null value as Name, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, null as string)
                .Create();
            Person person = personAddRequest.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            Func<Task> action = (async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // When we supply Null value as Email, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonEmailIsNull_ToBeArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(person => person.Name, null as string).Create();
            Person person = personAddRequest.ToPerson();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            Func<Task> action = (async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();

        }
        // When we supply propper persons details, it should insert the Person into the list
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.Email, "someone@example.com")
             .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            //If we supply any argument value to the AddPerson method, it should return the same return value
            _personRepositoryMock.Setup
             (temp => temp.AddPerson(It.IsAny<Person>()))
             .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personsAdderService.AddPerson(personAddRequest);

            person_response_expected.Id = person_response_from_add.Id;

            //Assert
            person_response_from_add.Id.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);
        }

        #endregion

        #region GetPersonById

        // if we supply null as person id, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonById_NullId_ToBeNull()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonById(personId);

            // Assert
            person_response_from_get.Should().BeNull();
        }

        // if we supply a valid person id, it should return the valid person details as PersonResponse 
        [Fact]
        public async Task GetPersonById_ValidId_ToBeSuccessful()
        {
            //Arrange 
            Person person = _fixture.Build<Person>()
                .With(person => person.Email, "valid@email.com")
                .With(person => person.Country, null as Country)
                .Create();
            PersonResponse person_response_expect = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            //PersonResponse person_response_from_add = await _personsAdderService.AddPerson(personAddRequest);
           // Guid? personId = person_response_from_add.Id;

            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonById(person.Id);

            //Assert
            person_response_from_get.Should().Be(person_response_expect);
        }

        #endregion

        #region GetAllPersons

        // should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeSucessful()
        {
            //Act
            List<Person> persons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();
            //Assert
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons; then we call GetAllPersons(); it should returns the same persons that were added
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(person => person.Email, "valild@email.com")
                    .With(person => person.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(person => person.Email, "valild2@email.com")
                    .With(person => person.Country, null as Country)
                    .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_response_list_from_get = await _personsGetterService.GetAllPersons();

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_get.Should().BeEquivalentTo(person_response_list_expected);

        }
        #endregion

        #region GetFilteredPersons

        // If the search text is empty and search by is "Name", it should return all persons
        [Fact]

        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(person => person.Email, "valild@email.com")
                    .With(person => person.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(person => person.Email, "valild2@email.com")
                    .With(person => person.Country, null as Country)
                    .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_response_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(PersonResponse.Name), "");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        // First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                    .With(person => person.Email, "valild@email.com")
                    .With(person => person.Country, null as Country)
                    .Create(),

                _fixture.Build<Person>()
                    .With(person => person.Email, "valild2@email.com")
                    .With(person => person.Country, null as Country)
                    .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_response_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(PersonResponse.Name), "Gl");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().BeEquivalentTo(person_response_list_expected);

        }
        #endregion

        #region GetSortedPersons
        // When we sorted person Name in DESC, it should return persons list descending on person Name
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person> {
            _fixture.Build<Person>()
                .With(person => person.Email, "vallid@email.com")
                .With(person => person.Country, null as Country)
                .Create(),
            _fixture.Build<Person>()
                .With(person => person.Email, "valid2@email.com")
                .With(person => person.Country, null as Country)
                .Create()
                };

            List<PersonResponse> person_response_list_expected = persons.Select(person => person.ToPersonResponse()).ToList();

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();

            //Act
            List<PersonResponse> persons_response_list_from_sort
                = await _personsSorterService.GetSortedPersons(allPersons, nameof(PersonResponse.Name), SortOrderOptions.DESC);

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            //Assert
            persons_response_list_from_sort.Should().BeInDescendingOrder(person => person.Name);
        }
        #endregion

        #region UpdatePerson
        // When we supply RequestAddPerson as null, it should throws ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPersonUpdateRequest_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When we supply invalid person ID, it should throws ArgumentException
        [Fact]
        public async Task UpdatePerson_invalidPersonId_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();
            Person person = personUpdateRequest.ToPerson();
            //Assert
           Func<Task> action = (async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(personUpdateRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // When person Name is null, it should throws ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
             //Arrange

            Person person = _fixture.Build<Person>()
                .With(person => person.Name, null as string)
                .With(person => person.Country, null as Country)
                .With(Person => Person.Email, "vallid@email.com")
                .With(Person => Person.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();
            PersonUpdateRequest? person_update_request = personResponse.ToPersonUpdateRequest();

            //Assert
            Func<Task> action = (async () =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(person_update_request);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // First, we will add a new person and try update person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.Country, null as Country)
                .With(person => person.Gender, GenderOptions.Female.ToString())
                .With(Person => Person.Email, "vallid@email.com")
                .Create();

            PersonResponse person_response_expect = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_expect.ToPersonUpdateRequest();
            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);
            //Act           
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expect);
        }
        #endregion

        #region DeletePerson

        // if we supply a valid Id, it shuld return true
        [Fact]
        public async Task DeletePerson_ValidPersonId_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.Country, null as Country)
                .With(Person => Person.Gender, GenderOptions.Male.ToString())
                .With(Person => Person.Email, "vallid@email.com")
                .Create();

            _personRepositoryMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            bool isDeleted = await _personsDeleterService.DeletePerson(person.Id);

            //Assert
            isDeleted.Should().BeTrue();
        }
        // if we supply a valid Id, it shuld return true
        [Fact]
        public async Task DeletePerson_InvalidPersonId()
        {
            //Arrange 
            Guid? Id = Guid.NewGuid();
       
            // Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Id);

            //Asser
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
