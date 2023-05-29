using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }
        [StringLength(40)] // nvarchar(40)
        public string? PersonName { get; set; }
        [StringLength(40)] // nvarchar(40)
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;
        // Unique Identifier
        public Guid? CountryID { get; set; }
        [StringLength(50)]
        public string? Address { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Address: {Address}, Date of birth: {DateOfBirth}, Country: {Country?.CountryName}";
        }
    }
}
