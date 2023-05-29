using Entities;
using ServicesContracts.Enums;
using System.Runtime.CompilerServices;

namespace ServicesContracts.DTOs
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PersonGender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public double? Age { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(PersonResponse)) return false;
            else
            {
                PersonResponse person = obj as PersonResponse;
                return person.PersonName == PersonName &&
                       person.PersonGender == PersonGender &&
                       person.Email == Email &&
                       person.Address == Address &&
                       person.ReceiveNewsLetter == ReceiveNewsLetter &&
                       person.Age == Age;

            }
            
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonId}, Person Name: {PersonName}," +
                $"Address: {Address}, Email: {Email}, Gender: {PersonGender}," +
                $"Country ID: {CountryID}, Country Name: {Country}," +
                $"Age: {Age}, Receive Newsletter: {ReceiveNewsLetter}";
        }
        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = PersonId,
                Address = Address,
                CountryID = CountryID,
                Email = Email,
                ReceiveNewsLetter = ReceiveNewsLetter,
                PersonGender = (Gender) Enum.Parse(typeof(Gender), PersonGender, true),
                DateOfBirth = DateOfBirth,
                PersonName = PersonName,
            };
        }
    }
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonId = person.PersonID,
                PersonName = person.PersonName,
                PersonGender = person.Gender,
                Address = person.Address,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                ReceiveNewsLetter = person.ReceiveNewsLetter,
                CountryID = person.CountryID,
                Country = person.Country?.CountryName,
                Age = (person.DateOfBirth != null)?
                Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)  : null,
            };
        }
    }
}
