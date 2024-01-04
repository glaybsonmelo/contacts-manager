using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "{0} cann't be blank")]
        public Guid? PersonId { get; set; }
        [Required(ErrorMessage = "{0} cann't be blank")]
        [MinLength(2, ErrorMessage = "Min length is {1}")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "{0} cann't be blank")]
        [EmailAddress(ErrorMessage = "{0} Email value should be a valid email")]
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderOptions? Gender { get; set; }
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
