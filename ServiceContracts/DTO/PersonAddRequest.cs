using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "{0} cann't be blank")]
        [MinLength(2, ErrorMessage = "Min length is {1}")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "{0} cann't be blank")]
        [EmailAddress(ErrorMessage = "{0} Email value should be a valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Please select a gender")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        public Guid CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        /// <summary>
        /// Converts the current object AddPersonRequeste into a new object of Person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person()
            {
                Id = Guid.NewGuid(),
                Name = Name,
                Email = Email,
                BirthDate = BirthDate,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters,
            };
        }
    }
}
