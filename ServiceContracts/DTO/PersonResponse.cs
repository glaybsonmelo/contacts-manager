using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is uset as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public Guid CountryId { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewLatters { get; set; }
        public double? Age { get; set; }
        /// <summary>
        /// Compares the current object data with the parameter object
        /// </summary>
        /// <param name="obj">The person object to compare</param>
        /// <returns>True or false, indicating whether all person details 
        /// are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(obj.GetType() != typeof(PersonResponse))
            { 
                return false; 
            }
            PersonResponse? person_to_compare = (PersonResponse)obj;
            return person_to_compare.Id == Id &&
                person_to_compare.Name == Name &&
                person_to_compare.Email == Email &&
                person_to_compare.BirthDate == BirthDate &&
                person_to_compare.Gender == Gender &&
                person_to_compare.CountryId == CountryId &&
                person_to_compare.Address == Address &&
                person_to_compare.ReceiveNewLatters == ReceiveNewLatters;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object Person into a PersonResponse class
        /// </summary>
        /// <param name="person">The person object to convert</param>
        /// <returns name="person">Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            //person => PersonResponse 
            return new PersonResponse()
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                BirthDate = person.BirthDate,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                ReceiveNewLatters = person.ReceiveNewLatters,
                Age = (person.BirthDate != null) ? Math.Round((DateTime.Now - person.BirthDate).Value.TotalDays / 365.25) : null,
            };
        }
    }
}
