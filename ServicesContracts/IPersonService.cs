using ServicesContracts.DTOs;
using ServicesContracts.Enums;
namespace ServicesContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// Add new Person to our person list
        /// </summary>
        /// <param name="personAddRequest">person request Dto object</param>
        /// <returns>returns a newly add person object as personsResponse Dto</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Get all persons in the list
        /// </summary>
        /// <returns> return a list of person response</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Return the person object according to the supplied ID
        /// </summary>
        /// <param name="personID"> ID to Serach for</param>
        /// <returns>Person Response Dto</returns>
        Task<PersonResponse?> GetPersonById(Guid? personID);

        /// <summary>
        /// Return list of persons based on the filter string passed and the 
        /// property filtered by
        /// </summary>
        /// <param name="searchBy">property to filter by it</param>
        /// <param name="searchString">string to search by it</param>
        /// <returns>list of matched persons</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string searchString);

        /// <summary>
        /// Returns a sorted list based on the passed parameters
        /// </summary>
        /// <param name="allPersons">list passed to be sorteed</param>
        /// <param name="sortBy">property to sort the list based on it</param>
        /// <param name="sortOrder">type of order of the sort</param>
        /// <returns>sorted list</returns>
        Task<List<PersonResponse>> GetSortedPersons(
            List<PersonResponse> allPersons,
            string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Return the response of the new detailed person
        /// </summary>
        /// <param name="personUpdateRequest"> personUpdateRequest DTO to update it</param>
        /// <returns>A PersonResponse DTO object with the new data</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Return true or false if Person deleted successfully or not
        /// </summary>
        /// <param name="PersonId">required PersonID to be deleted</param>
        /// <returns> True or false</returns>
        Task<bool> DeletePerson(Guid? PersonId);

        /// <summary>
        /// Returns the Persons as CSV file
        /// </summary>
        /// <returns>CSV file of all persons</returns>
        Task<MemoryStream> GetAllPersonsCSV();

        /// <summary>
        /// Returns the Persons as Excel Sheet
        /// </summary>
        /// <returns>Excel sheet of all Persons</returns>
        Task<MemoryStream> GetAllPersonsExcel();

 
    }
}
