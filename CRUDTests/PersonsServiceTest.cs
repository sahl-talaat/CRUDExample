using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;

        // constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personService = new PersonsService();
            _countriesService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        [Fact]
        // When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        public void AddPerson_NullPerson()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            { _personService.AddPerson(personAddRequest); });
        }

        [Fact]
        // When we supply null value as PersonName, it should throw ArgumentException
        public void AddPerson_PersonNameNull()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            { _personService.AddPerson(personAddRequest); });
        }

        [Fact]
        // When we supply proper person details, it should insert the person into the persons list
        // and it should return object of PersonResponse which includes with the newly generated person id
        public void AddPerson_ProperPersonDetails()
        {
            // Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName = "Person Name...",
                Email = "person@example.com",
                Address = "sample address",
                CountryID = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };

            // Act 
            PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);
            List<PersonResponse> person_list = _personService.GetAllPersons();

            // Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, person_list);
        }


        #endregion

        #region GetPersonByPersonID
        // If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            // Arrange
            Guid? personID = null;

            // Act
            PersonResponse? person_response_from_get =
            _personService.GetPersonByPersonID(personID);

            // Assert
            Assert.Null(person_response_from_get);
        }

        // If we supply a valid PersonID, 
        // it should return the valid person details as PersonResponse object
        [Fact]
        public void GetCountryByCountryID_WithPersonID()
        {
            // Arrange
            CountryAddRequest country_request =
            new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse country_response = _countriesService.AddCountry(country_request);

            // Act
            PersonAddRequest person_request = new PersonAddRequest()
            {
                PersonName = "Person Name...",
                Email = "person@example.com",
                Address = "sample address",
                CountryID = country_response.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add =
            _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get =
            _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            // Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }
        #endregion

        #region GetAllPersons
        [Fact]
        // The GetAllPersons() should return an empty list by default
        public void GetAllPersons_EmptyList()
        {
            // Act
            List<PersonResponse> persons_from_get =
            _personService.GetAllPersons();

            // Assert
            Assert.Empty(persons_from_get);
        }

        [Fact]
        // First, we will add few persons, and then when we call GetAllPersons(),
        // it should return the same persons that were added
        public void GetAllPersons_AddFewPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 =
            new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 =
            new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response_1 =
            _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 =
            _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Maru",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            { person_request_1,person_request_2,person_request_3};

            List<PersonResponse> person_response_list_from_add =
            new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_get =
            _personService.GetAllPersons();

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in person_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_list_from_get)
            {
                Assert.Contains(person_response_from_add, person_list_from_get);
            }
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        // If the search text is empty and search by is "PersonName", it should return all persons
        public void GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest country_request_1 =
            new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 =
            new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 =
            _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 =
            _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Maru",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            { person_request_1,person_request_2,person_request_3};

            List<PersonResponse> person_response_list_from_add =
            new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_search =
            _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_search in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_search.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                Assert.Contains(person_response_from_add, person_list_from_search);
            }
        }

        [Fact]
        // First we will add few persons, and then we will search based on PersonName with some search string.
        // It should return the matching persons
        public void GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest country_request_1 =
            new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 =
            new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 =
            _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 =
            _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            { person_request_1,person_request_2,person_request_3};

            List<PersonResponse> person_response_list_from_add =
            new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            // Act
            List<PersonResponse> person_list_from_search =
            _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_search in person_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_search.ToString());
            }

            // Assert
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                if (person_response_from_add.PersonName != null)
                {
                    if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(person_response_from_add, person_list_from_search);
                    }
                }
            }
        }

        #endregion

        #region GetSortedPersons
        [Fact]
        // When we sort based on PersonName in DESC, 
        // it should return persons list in descending on PersonName.
        public void GetSortedPersons()
        {
            // Arrange
            CountryAddRequest country_request_1 =
            new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 =
            new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 =
            _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 =
            _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest()
            {
                PersonName = "Smith",
                Email = "smith@example.com",
                Gender = GenderOptions.Male,
                Address = "address of smith",
                CountryID = country_response_1.CountryID,
                DateOfBirth = DateTime.Parse("2002-05-06"),
                ReceiveNewsLetters = true
            };

            PersonAddRequest person_request_2 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "mary@example.com",
                Gender = GenderOptions.Female,
                Address = "address of mary",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("2000-02-02"),
                ReceiveNewsLetters = false
            };

            PersonAddRequest person_request_3 = new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "rahman@example.com",
                Gender = GenderOptions.Male,
                Address = "address of rahman",
                CountryID = country_response_2.CountryID,
                DateOfBirth = DateTime.Parse("1999-03-03"),
                ReceiveNewsLetters = true
            };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>()
            { person_request_1,person_request_2,person_request_3};

            List<PersonResponse> person_response_list_from_add =
            new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
            {
                PersonResponse person_response = _personService.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            List<PersonResponse> allPersons = _personService.GetAllPersons();

            // Act
            List<PersonResponse> person_list_from_sort =
            _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            // print person_response_list_from_add
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_sort in person_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_sort.ToString());
            }

            person_response_list_from_add =
            person_response_list_from_add.OrderByDescending(person => person.PersonName).ToList();

            // Assert
            for (int i = 0; i < person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_list_from_sort[i]);
            }
        }

        #endregion

        #region UpdatePerson
        [Fact]
        // When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        public void UpdatePerson_NullPerson()
        {
            // Arrange
            PersonUpdateRequest? person_update_request = null;

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        // When we supply invalid PersonID, it should throw ArgumentException
        public void UpdatePerson_InvalidPersonID()
        {
            // Arrange
            PersonUpdateRequest? person_update_request =
            new PersonUpdateRequest()
            { PersonID = Guid.NewGuid() };

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        // When PersonName is null, it should throw ArgumentException
        public void UpdatePerson_PersonNameIsNull()
        {
            // Arrange
            CountryAddRequest country_add_request =
            new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request =
            new PersonAddRequest()
            {
                PersonName = "John",
                Email = "John@example.com",
                Address = "sample address",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add =
            _personService.AddPerson(person_add_request);

            PersonUpdateRequest? person_update_request =
            person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = null;

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                _personService.UpdatePerson(person_update_request);
            });
        }

        [Fact]
        // First, add a new person and try to update the person name and email
        public void UpdatePerson_PersonFullDetailsUpdation()
        {
            // Arrange
            CountryAddRequest country_add_request =
            new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request =
            new PersonAddRequest()
            {
                PersonName = "John",
                Email = "John@example.com",
                Address = "sample address",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add =
            _personService.AddPerson(person_add_request);

            PersonUpdateRequest? person_update_request =
            person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "William@example.com";

            // Act
            PersonResponse person_response_from_update =
            _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get =
            _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            // Assert
            Assert.Equal(person_response_from_get, person_response_from_update);
        }
        #endregion

        #region DeletePerson
        [Fact]
        //If wu supply an invalid PersonID, it should return false
        public void DeletePerson_InvalidPersinID()
        {
            // Act
            bool isDeleted =
            _personService.DeletePerson(Guid.NewGuid());

            // Assert
            Assert.False(isDeleted);
        }

        [Fact]
        //If wu supply a valid PersonID, it should return true
        public void DeletePerson_ValidPersinID()
        {
            // Arrange
            CountryAddRequest country_add_request =
            new CountryAddRequest() { CountryName = "UK" };
            CountryResponse country_response_from_add =
            _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request =
            new PersonAddRequest()
            {
                PersonName = "Johns",
                Email = "John@example.com",
                Address = "sample address",
                CountryID = country_response_from_add.CountryID,
                Gender = GenderOptions.Male,
                DateOfBirth = Convert.ToDateTime("2000-01-01"),
                ReceiveNewsLetters = true
            };
            PersonResponse person_response_from_add =
            _personService.AddPerson(person_add_request);

            // Act
            bool isDeleted =
            _personService.DeletePerson(person_response_from_add.PersonID);

            // Assert
            Assert.True(isDeleted);
        }
        #endregion
    }
}