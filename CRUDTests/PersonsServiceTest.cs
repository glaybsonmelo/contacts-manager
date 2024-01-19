using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(null);
            _countriesService = new CountriesService(null);

            _personsService = new PersonService(null);

        }

        #region AddPerson
        // when we supply a null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            Func<Task> action = (async () =>
            {
            //Assert
                await _personsService.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        // When we supply Null value as Name, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = null
            };

            //Act
            Func<Task> action = (async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // When we supply Null value as Email, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonEmailIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(person => person.Name, null as string).Create();

            //Act
            Func<Task> action = (async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();

        }
        // When we supply propper persons details, it should insert the Person into the list
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(person => person.Email, "email@valid.com").Create();

            //Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            
            List<PersonResponse> all_persons_list = await _personsService.GetAllPersons();

            //Assert
            person_response_from_add.Id.Should().NotBe(Guid.Empty);

            all_persons_list.Should().Contain(person_response_from_add);
        }

        #endregion

        #region GetPersonById


        // if we supply null as person id, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonById_NullId()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person_response_from_get = await _personsService.GetPersonById(personId);

            // Assert
            person_response_from_get.Should().BeNull();
        }

        // if we supply a valid person id, it should return the valid person details as PersonResponse 
        [Fact]
        public async Task GetPersonById_ValidId()
        {
            //Arrange
            //CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            //CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(person => person.Email, "valid@email.com").Create();
            //Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            Guid? personId = person_response_from_add.Id;

            PersonResponse? person_response_from_get = await _personsService.GetPersonById(personId);
            //Assert
            person_response_from_add.Should().Be(person_response_from_get);
        }

        #endregion

        #region GetAllPersons

        // should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {

            //Act
            List<PersonResponse> persons_from_get = await _personsService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }

        //First, we will add few persons; then we call GetAllPersons(); it should returns the same persons that were added
        [Fact]
        public async Task GetAllPersons_NonEmptyList() 
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(person => person.Email, "valild@email.com")
                .With(person => person.CountryId, country_from_add1.Id)
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(person => person.Email, "valild2@email.com")
                .With(person => person.CountryId, country_from_add2.Id)
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            
            //Act
            List<PersonResponse> persons_response_list_from_get = await _personsService.GetAllPersons();

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);

        }
        #endregion

        #region GetFilteredPersons

        // If the search text is empty and search by is "Name", it should return all persons
        [Fact]

        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);
            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(person => person.CountryId, country_from_add2.Id)
                .With(person => person.Email, "valid2@email.com")
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_response_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonResponse.Name), "");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);
        }

        // First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
             //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);
            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(person => person.CountryId, country_from_add2.Id)
                .With(person => person.Email, "valid2@email.com")
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_response_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonResponse.Name), "Gl");

            //print person_response_list_from_get
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person_response_from_get in persons_response_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            persons_response_list_from_search.Should().OnlyContain(person => person.Name.Contains("Gl", StringComparison.OrdinalIgnoreCase));

        }
        #endregion

        #region GetSortedPersons
        // When we sorted person Name in DESC, it should return persons list descending on person Name
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);
            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(person => person.CountryId, country_from_add2.Id)
                .With(person => person.Email, "valid2@email.com")
                .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = await _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            //Act
            List<PersonResponse> persons_response_list_from_sort
                = await _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.Name), SortOrderOptions.DESC);

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
        public async Task UpdatePerson_NullPersonUpdateRequest()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When we supply invalid person ID, it should throws ArgumentException
        [Fact]
        public async Task UpdatePerson_invalidPersonId()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();

            //Assert
           Func<Task> action = (async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // When person Name is null, it should throws ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
             //Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);


            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = null;

            //Assert
            Func<Task> action = (async () =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // First, we will add a new person and try update person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = "Glaybson";
            person_update_request.Email = "valid@email.com";
    
            //Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);
            PersonResponse? person_response_from_get = await _personsService.GetPersonById(person_response_from_update.Id);

            //Assert
            person_response_from_update.Should().Be(person_response_from_get);
        }
        #endregion

        #region DeletePerson

        // if we supply a valid Id, it shuld return true
        [Fact]
        public async Task DeletePerson_ValidPersonId()
        {
            //Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(person => person.Name, "Glaybson Melo")
                .With(person => person.CountryId, country_from_add1.Id)
                .With(Person => Person.Email, "vallid@email.com")
                .Create();

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            // Act
            bool isDeleted = await _personsService.DeletePerson(person_response_from_add.Id);

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
            bool isDeleted = await _personsService.DeletePerson(Id);

            //Asser
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
