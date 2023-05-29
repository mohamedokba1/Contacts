using Entities;
namespace ServicesContracts.DTOs
{
    /// <summary>
    /// Dto class used as a return type of most of CountriesSevice methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(CountryResponse)) return false;
            CountryResponse country = obj as CountryResponse;
            return country?.CountryId == CountryId && country?.CountryName == CountryName;
        }
    }

    public static class CountryExtensionsMethod
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
