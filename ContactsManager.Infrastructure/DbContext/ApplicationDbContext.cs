using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<Country> Countries { get; set;}
        public virtual DbSet<Person> Persons { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            // Seed data for Countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country>? countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);
            if(countries != null)
            {
                foreach (Country country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
            }
            // Seed data for Persons
            string personsJson = File.ReadAllText("Persons.json");
            List<Person>? persons = JsonSerializer.Deserialize<List<Person>>(personsJson);
            if(persons != null)
            {
                foreach (Person person in persons)
                {
                    modelBuilder.Entity<Person>().HasData(person);
                }
            }
        }
        public List<Person> SP_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int InsertPerson(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetter", person.ReceiveNewsLetter),
            };
            // return the number of rows affected
            return Database.ExecuteSqlRaw(@"EXECUTE [dbo].[insertPerson]
                   @PersonID, @PersonName, @Email, @DateOfBirth, @Gender,
                   @CountryID, @Address, @ReceiveNewsLetter", sqlParameters);
        }
    }
}
