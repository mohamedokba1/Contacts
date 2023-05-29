using Core.Entities;
using Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    /// <summary>
    /// Act as a DTO for inserting a new Person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name should not be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage = "Email should not be blank or empty")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter a valid Date")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Select Gender")]
        public Gender? PersonGender { get; set; }
        [Required(ErrorMessage = "Select a Country")]
        public Guid? CountryID { get; set; }
        [Required(ErrorMessage = "Enter your addrress")]
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
                CountryID = CountryID,
                Gender = PersonGender.HasValue ? PersonGender.ToString() : "Male",
                Address = Address,
                ReceiveNewsLetter = ReceiveNewsLetter,
            };
        }
    }
}
