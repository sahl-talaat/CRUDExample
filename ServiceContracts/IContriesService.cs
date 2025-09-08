using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Return a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">Country object to add</param>
    /// <returns>Return the country object after adding it (including newly generated country id)</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Return all countries from the list
    /// </summary>
    /// <returns>All countries from the list as List of CountryResponse</returns>
    List<CountryResponse> GetAllCountries();

    /// <summary>
    /// Return a country object based on the gevin country id
    /// </summary>
    /// <param name="countryID">CountryID (guid) to search</param>
    /// <returns>Mathcing country as CountryResponse object</returns>
    CountryResponse? GetCountryByCountryID(Guid? countryID);
}
