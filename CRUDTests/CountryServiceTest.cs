using ServicesContracts;
using Services;
using ServicesContracts.DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using FluentAssertions;

namespace CRUDTests
{
    public class CountryServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountryServiceTest()
        {
            List<Country> countriesInitialData = new List<Country>();

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(null);
        }
        #region AddCountry
        // When Send CountryRequest with null it should throw 
        // an ArgumentNullException
        [Fact]
        public void AddCountry_NullRequest()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            };
            //Assert.ThrowsAsync<ArgumentNullException>();
            action.Should().ThrowAsync<ArgumentNullException>();
        }

        // When CountryName is null it should throw 
        // an ArgumentException
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null,
            };

            // Act 
            Func<Task> action = async () =>
                await _countriesService.AddCountry(request);
            //Assert

            //Assert.ThrowsAsync<ArgumentException>();
            action.Should().ThrowAsync<ArgumentException>();
        }

        // When CountryName is null it should throw 
        // an ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA",
            };

            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA",
            };
            //Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
               await _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
            });
        }

        // When adding a proper Country object it should return 
        // the country response with the new generated ID 
        [Fact]
        public async void AddCountry_ProperCountryRequest()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Egypt",
            };

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> list_from_geAll = await _countriesService.GetAllCountries();
            //Assert
            //Assert.True(response.CountryId != Guid.Empty);
            //Assert.Contains(response, list_from_geAll);

            response.CountryId.Should().NotBe(Guid.Empty);
            list_from_geAll.Should().Contain(response);
        }
        #endregion

        #region GetAllCountries
        // Retun empty list by default
        [Fact]
        public async void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            //Assert
            //Assert.Empty(countries);
            countries.Should().BeEmpty();
        }

        // Adding Few Countries and return the new list
        [Fact]
        public async void GetAllCountries_AddingNewListOfCountries()
        {
            // Arrange
            List<CountryAddRequest> request_list = new List<CountryAddRequest> 
            {
                new CountryAddRequest {CountryName = "Egypt"},
                new CountryAddRequest { CountryName = "USA"}
            };
            List<CountryResponse> list_from_add_country = new List<CountryResponse>();
            foreach(CountryAddRequest country in request_list)
            {
                list_from_add_country.Add(await _countriesService.AddCountry(country));
            }

            List<CountryResponse> actual_list = await _countriesService.GetAllCountries();

            //foreach(CountryResponse expected_country in list_from_add_country)
            //{
            //    Assert.Contains(expected_country, actual_list);
            //}

            actual_list.Should().BeEquivalentTo(list_from_add_country);
        }
        #endregion

        #region GetCountryById
        [Fact]
        public async void GetCountryById_NullID()
        {
            // Arrange
            Guid? CountryId = null;
            // Act
            CountryResponse? response = await _countriesService.GetCountryById(CountryId);
            // Assert
            //Assert.Null(response);
            response.Should().BeNull();
        }

        [Fact]
        public async void GetCountryById_ValidID()
        {
            // Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "China" };
            CountryResponse country_from_add = await _countriesService.AddCountry(countryAddRequest);
            // Act
            CountryResponse? response = await _countriesService.GetCountryById(country_from_add.CountryId);
            // Assert
            //Assert.Equal(country_from_add, response);
            response.Should().Be(country_from_add);
        }
        #endregion
    }
}