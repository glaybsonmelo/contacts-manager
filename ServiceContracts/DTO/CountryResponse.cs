using Entities;
using System.Diagnostics;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if(obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }
            CountryResponse? country_to_compare = (CountryResponse) obj;
            return Id == country_to_compare.Id && Name == country_to_compare.Name;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { Id = country.Id, Name = country.Name };
        }
    }
}
