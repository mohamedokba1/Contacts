using Entities;
using ServicesContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServicesContracts.DTOs
{
    /// <summary>
    /// Represents the DTO class for the person details to be update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person ID can't be blank")]
        public Guid PersonId { get; set; }
        [Required(ErrorMessage = "Person Name should'nt be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email must be not blank or empty")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? PersonGender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }

        /// <summary>
        /// Convert the current object of PersonAddRequest to person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (PersonGender.HasValue) ? PersonGender.ToString() : "male",
                Address = Address,
                ReceiveNewsLetter = ReceiveNewsLetter,
            };
        }
    }
}
