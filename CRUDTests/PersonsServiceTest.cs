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
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
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
                BirthDate = Convert.ToDateTime("07/16/2003"),
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
        #region GetAllPersons

        // should return an empty list by default
        [Fact]
        public void GetAllPersons_EmptyList()
        {

            //Act
            List<PersonResponse> persons_from_get = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        //First, we will add few persons; then we call GetAllPersons(); it should returns the same persons that were added
        [Fact]
        public void GetAllPersons_NonEmptyList()
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

            CountryResponse country_from_add1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewLatters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewLatters = true,
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            
            //Act
            List<PersonResponse> persons_response_list_from_get = _personsService.GetAllPersons();

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

        public void GetFilteredPersons_EmptySearchText()
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

            CountryResponse country_from_add1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewLatters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewLatters = true,
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_response_list_from_search = _personsService.GetFilteredPersons(nameof(PersonResponse.Name), "");

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
        public void GetFilteredPersons_SearchByPersonName()
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

            CountryResponse country_from_add1 = _countriesService.AddCountry(countryAddRequest1);
            CountryResponse country_from_add2 = _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                Name = "Glaybson",
                Address = "addd",
                BirthDate = Convert.ToDateTime("07/16/2003"),
                CountryId = country_from_add1.Id,
                Email = "valid@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewLatters = true,
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                Name = "Buula Mota",
                Address = "address",
                BirthDate = Convert.ToDateTime("01/6/2000"),
                CountryId = country_from_add2.Id,
                Email = "valid2323@email.com",
                Gender = GenderOptions.Female,
                ReceiveNewLatters = true,
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            {
                personAddRequest1,
                personAddRequest2
            };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personsService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_response_list_from_search = _personsService.GetFilteredPersons(nameof(PersonResponse.Name), "Gl");

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
    }
}
