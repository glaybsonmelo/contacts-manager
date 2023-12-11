using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewLatters { get; set; }
        /// <summary>
        /// Converts the current object AddPersonRequeste into a new object of Person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person() {
                Name = Name,
                Email = Email,
                BirthDate = BirthDate,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address, 
                ReceiveNewLatters = ReceiveNewLatters,
            };
    }
}
