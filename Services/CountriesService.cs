using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
using ServicesContracts;
using ServicesContracts.DTOs;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? newCountry)
        {
            if (newCountry == null)
                throw new ArgumentNullException(nameof(newCountry));
            if (string.IsNullOrEmpty(newCountry.CountryName))
                throw new ArgumentException(nameof(newCountry.CountryName));
            if (await _countriesRepository.GetCountryByCountryName(newCountry.CountryName) != null)
            {
                throw new ArgumentException("Duplicate Country Name");
            }

            Country country = newCountry.ToCountry();
            country.CountryId = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryById(Guid? countryID)
        {
            if (countryID == null) return null;

            Country? country =  await _countriesRepository.GetCountryByCountryID(countryID.Value);
            if(country ==null) return null;

            return country.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            int insertedCountries = 0;
            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string countryName = cellValue;

                        if (_countriesRepository.GetCountryByCountryName(countryName) == null)
                        {
                            Country country = new Country() { CountryName = countryName };
                            await _countriesRepository.AddCountry(country);
                            insertedCountries++;
                        }
                    }
                }
            }
            return insertedCountries;
        }
    }
}