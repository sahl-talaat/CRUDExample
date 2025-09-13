using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country =
            _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;

            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            // check if PersonAddRequest is not null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            // Model validations
            ValidationHelper.ModelValidation(personAddRequest);

            // convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();

            // generate PersonID
            person.PersonID = Guid.NewGuid();

            // add person object to presons list
            _persons.Add(person);

            // convert the person object to PersonResponse type
            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            Person? person =
            _persons.FirstOrDefault(person => person.PersonID == personID);
            if (person == null)
                return null;

            return person.ToPersonResponse();
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            switch (searchBy)
            {
                case nameof(Person.PersonName):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.PersonName) ?
                    person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Email) ?
                    person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPersons.Where(person =>
                    (person.DateOfBirth != null) ?
                    person.DateOfBirth.Value.ToString("dd MMMM yyyy")
                    .Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Gender) ?
                    person.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(Person.CountryID):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Country) ?
                    person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(Person.Address):
                    matchingPersons = allPersons.Where(person =>
                    !string.IsNullOrEmpty(person.Address) ?
                    person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                default:
                    matchingPersons = allPersons;
                    break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons =
            (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.PersonName,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.PersonName,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Gender,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Gender,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Country,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Country,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.Address,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.Address,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) =>
                allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            // validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // get matching person object to update
            Person? matchingPerson =
            _persons.FirstOrDefault(person =>
            person.PersonID == personUpdateRequest.PersonID);
            if (matchingPerson == null)
                throw new ArgumentException("Given person id doesn't exist");

            // update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            return matchingPerson.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person =
            _persons.FirstOrDefault(person => person.PersonID == personID);

            if (person == null)
                return false;

            _persons.RemoveAll(person => person.PersonID == personID);

            return true;
        }
    }
}