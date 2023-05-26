using AutoFixture;
using Moq;
using ServicesContracts;
using ServicesContracts.DTOs;
using Contacts_Manager.Controllers;
using ServicesContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace CRUDTests
{
    public class PersonControllerTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonService _personsService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonService> _personsServiceMock;
        private readonly Fixture _fixture;
        public PersonControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            _personsServiceMock = new Mock<IPersonService>();
            _personsService = _personsServiceMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            // Arrange
            List<PersonResponse> persons = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            _personsServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons);

            _personsServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .ReturnsAsync(persons);

            // Act
            IActionResult actual_result = await personsController.Index(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actual_result);
            viewResult.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.Model.Should().Be(persons);

        }
        #endregion

        #region Create

        [Fact]
        public async Task Create_IfModelErrorsExist_ToReturnCreateView()
        {
            // Arrange
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            List<CountryResponse> list_countries_response = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(list_countries_response);

            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act
            personsController.ModelState.AddModelError("PersonName", "Name can't be blank");
            IActionResult actual_result =  await personsController.Create(person_add_request);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(actual_result);
            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(person_add_request);
        }

        [Fact]
        public async Task Create_IfNoModelErrorsExist_ToRedirectToIndexView()
        {
            // Arrange
            PersonResponse person_response = _fixture.Create<PersonResponse>();
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();
            List<CountryResponse> list_countries_response = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(list_countries_response);

            _personsServiceMock.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act
            
            IActionResult actual_result = await personsController.Create(person_add_request);

            // Assert
            RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actual_result);
            result.ActionName.Should().Be("Index");
        }
        #endregion
    }
}
