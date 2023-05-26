using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add new person to the data store 
        /// </summary>
        /// <param name="person">Person object to add it to the data store</param>
        /// <returns>Object of the added person</returns>
        Task<Person> AddPeson(Person person);

        /// <summary>
        /// Return all persons from the data store
        /// </summary>
        /// <returns>All persons from the table</returns>
        Task<List<Person>> GetAllPesons();

        /// <summary>
        /// Returns the person by its ID
        /// </summary>
        /// <param name="personId">ID for the required person</param>
        /// <returns>Object of person based on the passed ID or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personId);

        /// <summary>
        /// Returns a list of persons based on the expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check the returned list</param>
        /// <returns>Filtered list based on the predicate</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Returns a boolean value indicates the remove process 
        /// </summary>
        /// <param name="personId">Person Id to be removed</param>
        /// <returns>True or false based on the person successfully deleted or not</returns>
        Task<bool> DeletePerson(Guid personId);

        /// <summary>
        /// Return person object after modification
        /// </summary>
        /// <param name="person">Object of person to be updated</param>
        /// <returns>Modified object of person</returns>
        Task<Person> UpdatePerson(Person person);
    }
}
