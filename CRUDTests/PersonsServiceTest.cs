using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;

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

            _countriesService = new CountriesService(dbContext);
            _countriesService = new CountriesService(dbContext);

            _personsService = new PersonService(dbContext);

        }

        #region AddPerson
        // when we supply a null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
            //Assert
                await _personsService.AddPerson(personAddRequest);
            });
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
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
        }
        // When we supply Null value as Email, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonEmailIsNull()
        {
            // Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Email = null
            };

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _personsService.AddPerson(personAddRequest);
            });
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

            Assert.True(person_response_from_add.Id != Guid.Empty);
            Assert.Contains(person_response_from_add, all_persons_list);
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
            Assert.Null(person_response_from_get);
        }

        // if we supply a valid person id, it should return the valid person details as PersonResponse 
        [Fact]
        public async Task GetPersonById_ValidId()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "Brazil"
            };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);
            
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "MyName",
                Email = "email@valid.com",
                Address = "street 101, brooklin",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = countryResponse.Id,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,

            };
            //Act
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            Guid? personId = person_response_from_add.Id;

            PersonResponse? person_response_from_get = await _personsService.GetPersonById(personId);
            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);

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
            Assert.Empty(persons_from_get);
        }

        //First, we will add few persons; then we call GetAllPersons(); it should returns the same persons that were added
        [Fact]
        public async Task GetAllPersons_NonEmptyList() 
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                Name = "USA"
            };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                Name = "Canada"
            };

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

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
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                Assert.Contains(person_response, persons_response_list_from_get);
            }

        }
        #endregion

        #region GetFilteredPersons

        // If the search text is empty and search by is "Name", it should return all persons
        [Fact]

        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                Name = "USA"
            };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                Name = "Canada"
            };

            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);
            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

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
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                Assert.Contains(person_response, persons_response_list_from_search);
            }

        }

        // First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                Name = "USA"
            };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                Name = "Canada"
            };

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

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
            foreach (PersonResponse person_response in person_response_list_from_add)
            {
                if(person_response.Name == null)
                {
                    return;
                }
                if (person_response.Name.Contains("Gl", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response, persons_response_list_from_search);

                }
            }

        }
        #endregion

        #region GetSortedPersons
        // When we sorted person Name in DESC, it should return persons list descending on person Name
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest()
            {
                Name = "USA"
            };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest()
            {
                Name = "Canada"
            };

            CountryResponse country_from_add1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

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

            person_response_list_from_add = person_response_list_from_add.OrderByDescending(person => person.Name).ToList();

            //Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], persons_response_list_from_sort[i]);
            }
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
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        // When we supply invalid person ID, it should throws ArgumentException
        [Fact]
        public async Task UpdatePerson_invalidPersonId()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid(),
                
            };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }
        // When person Name is null, it should throws ArgumentNullException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "USA",
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(countryAddRequest);



            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                CountryId = country_response_from_add.Id,
                Email = "valid@email.com",
                BirthDate = Convert.ToDateTime("07-08-1999"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetters = true

            };

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);


            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personsService.UpdatePerson(person_update_request);
            });
        }
        // First, we will add a new person and try update person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "USA",
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                CountryId = country_response_from_add.Id,
                Address = "Street Abc",
                BirthDate = Convert.ToDateTime("07-16-2003"),
                Email = "email@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true

            };

            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = "Glaybson";
            person_update_request.Email = "valid@email.com";
    
            //Act
            PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);
            PersonResponse? person_response_from_get = await _personsService.GetPersonById(person_response_from_update.Id);

            //Assert
            Assert.Equal(person_response_from_update, person_response_from_get);
            
        }
        #endregion

        #region DeletePerson

        // if we supply a valid Id, it shuld return true
        [Fact]
        public async Task DeletePerson_ValidPersonId()
        {
            //Arrange 
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "USA"
            };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                CountryId = country_response_from_add.Id,
                Email = "valid@email.com",
                BirthDate = Convert.ToDateTime("07-08-1999"),
                Gender = GenderOptions.Other,
                ReceiveNewsLetters = true
            };
            
            PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
            // Act
            bool isDeleted = await _personsService.DeletePerson(person_response_from_add.Id);

            //Assert
            Assert.True(isDeleted);
        }
        // if we supply a valid Id, it shuld return true
        [Fact]
        public async Task DeletePerson_InvalidPersonId()
        {
            //Arrange 
            Guid? Id = Guid.NewGuid();
       
            // Act
            bool isDeleted = await _personsService.DeletePerson(Id);

            //Assert
            Assert.False(isDeleted);
        }
        #endregion
    }
}
