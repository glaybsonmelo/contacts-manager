using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Net;
using System.Reflection;
using System.Xml.Linq;

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

        #region GetSortedPersons
        // When we sorted person Name in DESC, it should return persons list descending on person Name
        [Fact]
        public void GetSortedPersons()
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
            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            //Act
            List<PersonResponse> persons_response_list_from_sort
                = _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.Name), SortOrderOptions.DESC);

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
        public void UpdatePerson_NullPersonUpdateRequest()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }
        // When we supply invalid person ID, it should throws ArgumentException
        [Fact]
        public void UpdatePerson_invalidPersonId()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid(),
            };

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }
        // When we supply person Id as null, it should throws ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPersonId()
        {
            //Arrange
            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = null,
            };

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }
        // When person Name is null, it should throws ArgumentNullException
        [Fact]
        public void UpdatePerson_PersonNameIsNull()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "USA",
            };
            CountryResponse country_response_from_add = _countriesService.AddCountry(countryAddRequest);

     

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                CountryId = country_response_from_add.Id,
            };

            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);


            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(person_update_request);
            });
        }
        // First, we will add a new person and try update person name and email
        [Fact]
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest()
            {
                Name = "USA",
            };
            CountryResponse country_response_from_add = _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Glaybson",
                CountryId = country_response_from_add.Id,
                Address = "Street Abc",
                BirthDate = Convert.ToDateTime("07-16-2003"),
                Email = "email@email.com",
                Gender = GenderOptions.Male,
                ReceiveNewLatters = true

            };

            PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest? person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.Name = "Glaybson";
            person_update_request.Email = "valid@email.com";
    
            //Act
            PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);
            PersonResponse? person_response_from_get = _personsService.GetPersonById(person_response_from_update.Id);

            //Assert
            Assert.Equal(person_response_from_update, person_response_from_get);
            
        }
        #endregion
    }
}
