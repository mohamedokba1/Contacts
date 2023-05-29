using Microsoft.AspNetCore.Http;
using ServicesContracts.DTOs;

namespace ServicesContracts
{
    /// <summary>
    /// Contains the business logic for manipulating Country Entities
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a new Country to a list of countries
        /// </summary>
        /// <param name="newCountry">new Dto of country</param>
        /// <returns>newly add country with new ID generated</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? newCountry);

        /// <summary>
        /// Returns all countries in the list
        /// </summary>
        /// <returns></returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// return the country by its ID
        /// </summary>
        /// <param name="countryID"> represent the ID of the required Country</param>
        /// <returns> return the expected country based on the provided ID</returns>
        Task<CountryResponse?> GetCountryById(Guid? countryID);

        /// <summary>
        /// Upload list of countries from Excel sheet
        /// </summary>
        /// <param name="formFile">List of countries to be uploaded</param>
        /// <returns>Number of countries uploaded</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}