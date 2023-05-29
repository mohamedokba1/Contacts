using Core.RepositoryContracts;
using Core.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePerson(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(p => p.PersonID == personId));
            int deletedRows = await _db.SaveChangesAsync();
            return deletedRows > 0;
        }

        public async Task<List<Person>> GetAllPesons()
        {
            return await _db.Persons.Include(c => c.Country).ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Where(predicate).Include("Country").ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonID(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonID == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = _db.Persons.FirstOrDefault(temp => temp.PersonID == person.PersonID);
            if (matchingPerson == null)
                return person;
            matchingPerson.PersonID = person.PersonID;
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.Address = person.Address;
            matchingPerson.Country = person.Country;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.ReceiveNewsLetter = person.ReceiveNewsLetter;

            int updatedRows = await _db.SaveChangesAsync();
            return (updatedRows > 0)? matchingPerson : person;
        }
    }
}
