using Core.Entities;
namespace Core.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Add new country the data store
        /// </summary>
        /// <param name="country">object of new country to add it</param>
        /// <returns>returns country object after adding it to the data store</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in the data store
        /// </summary>
        /// <returns>All countries from the table of countries</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns the country by its ID
        /// </summary>
        /// <param name="countryId">ID for the required country</param>
        /// <returns>Object of country based on the passed ID or null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryId);

        /// <summary>
        /// Returns the country by its Name
        /// </summary>
        /// <param name="countryName">Name for the required country</param>
        /// <returns>Object of country based on the passed Name or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}