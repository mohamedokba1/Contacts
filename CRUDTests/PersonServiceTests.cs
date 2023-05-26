using Entities;
using Services;
using ServicesContracts;
using ServicesContracts.DTOs;
using ServicesContracts.Enums;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using System.Linq.Expressions;

namespace CRUDTests
{
    public class PersonServiceTests
    {
        private readonly IPersonService _personService;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonServiceTests(ITestOutputHelper testOutputHelper)
        {
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
            _fixture = new Fixture();
            _personService = new PersonsService(_personsRepository);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        // when pass a null personAddRequest it should throw an ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_BeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? person = null;

            // Assert
            Func<Task> action = async () =>
            {
                PersonResponse? response = await _personService.AddPerson(person);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
           //await Assert.ThrowsAsync<ArgumentNullException>();
        }

        // when pass a null personName in personAddRequest it should throw an ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_BeArgumentException()
        {
            // Arrange
            PersonAddRequest personRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person perosn = personRequest.ToPerson();
            _personsRepositoryMock.Setup(temp => temp.AddPeson(It.IsAny<Person>()))
                .ReturnsAsync(perosn);

            // Act
            Func<Task> action = async () =>
            {
                PersonResponse? response = await _personService.AddPerson(personRequest);
            };

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // when pass a proper personAddRequest it should return the newly added person
        [Fact]
        public async Task AddPerson_FullPropertires_ShouldBeSuccessfull()
        {
            // Arrange
            PersonAddRequest personRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "Someone@example.com")
                .Create();
            Person person = personRequest.ToPerson();
            PersonResponse personResponse_excpected = person.ToPersonResponse();

             _personsRepositoryMock.Setup(temp => temp.AddPeson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            // Act
            PersonResponse ? response = await _personService.AddPerson(personRequest);
            personResponse_excpected.PersonId = response.PersonId;
            // Assert

            response.PersonId.Should().NotBe(Guid.Empty);
            response.Should().Be(personResponse_excpected);
        }
        #endregion

        #region GetPersonById
        // when we supply null id it will return null
        [Fact]
        public async Task GetPeronById_NullPersonId_ToBeNull()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person_from_getbyid = await _personService.GetPersonById(personId);
            
            // Assert

            person_from_getbyid.Should().BeNull();
        }

        // If we supply a valid userId it will return valid person details 
        [Fact]
        public async Task GetPeronById_ValidPersonId_ToBeSuccessfull()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponse_excpected = person.ToPersonResponse();
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
            .ReturnsAsync(person);
            // Act
            PersonResponse? person_from_getbyid = await _personService.GetPersonById(person.PersonID);

            // Assert
            person_from_getbyid.Should().Be(personResponse_excpected);
        }
        #endregion

        #region GetAllPerson
        // When calling GetAllPerson() it will return empty list by default
        [Fact]
        public async Task GetAllPerson_EmptyList_ToBeEmptyList()
        {
            // Arrange 
            List<Person> persons = new List<Person>();
            _personsRepositoryMock.Setup(temp => temp.GetAllPesons())
                .ReturnsAsync(persons);
            List<PersonResponse> list_from_get = await _personService.GetAllPersons();

            //Assert
            list_from_get.Should().BeEmpty();
        }

        // Adding few Persons and call GetAllPerson() should return the added persons
        [Fact]
        public async Task GetAllPerson_WithFewPerson_ToBeSuccessful()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            _personsRepositoryMock.Setup(temp => temp.GetAllPesons())
                .ReturnsAsync(persons);
            List<PersonResponse> response_list_expected = persons
                .Select(temp => temp.ToPersonResponse()).ToList();
            
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person in response_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }
            // Act
            List<PersonResponse> List_person_from_get = await _personService.GetAllPersons(); ;

            _testOutputHelper.WriteLine("Actual: ");
            foreach(PersonResponse person in List_person_from_get)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            // Assert
            List_person_from_get.Should().BeEquivalentTo(response_list_expected);
        }
        #endregion

        #region GetFilteredPersons
        // when search by person name and search string is empty it should return the whole list
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString_ToBeAllPersons()
        {
            // Act
            List<Person> persons = new List<Person>()
            {
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> response_list_expected = persons
                .Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person in response_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }
            // Act
            List<PersonResponse> response_list_actual = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person in response_list_actual)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            // Assert
            response_list_actual.Should().BeEquivalentTo(response_list_expected);
        }

        // when search by person name and with specified name it should return
        // a list of matched persons
        
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            // Arrange
            List<Person> persons = new List<Person>()
            {
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.PersonName, "mahmoud")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.PersonName, "mohamed")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.PersonName, "ahmed")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> response_list_expected = persons
                .Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);
            
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person in response_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }
            // Act
            List<PersonResponse> response_list_actual = await _personService.GetFilteredPersons(nameof(Person.PersonName), "Mo");

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person in response_list_actual)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            // Assert
            response_list_actual.Should().BeEquivalentTo(response_list_actual);
        }
        #endregion

        #region GetSortedPersons
        // get the list of persons sorted by ASC order

        [Fact]
        public async Task GetSortedPersons_AscOrderByPersonName_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "Somene@example.com")
                .With(temp => temp.Country, null as Country)
                .Create()
            };
            List<PersonResponse> response_list_expected = persons
                .Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetAllPesons())
                .ReturnsAsync(persons);

            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse person in response_list_expected)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }
            // Act
            List<PersonResponse> response_list_actual = await _personService.GetSortedPersons(response_list_expected, nameof(Person.PersonName), SortOrderOptions.ASC);

            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse person in response_list_actual)
            {
                _testOutputHelper.WriteLine(person.ToString());
            }

            // Assert
            response_list_actual.Should().BeInAscendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson
        // when passing a null PersonUpdateRequest it should
        // throw an ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullRequest_ToBeArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            // Assert
           await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                // Act
                await _personService.UpdatePerson(personUpdateRequest));
        }

        // when passing an invalid person ID it should
        // throw an ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonId = Guid.NewGuid()
            };

            // Act
            Func<Task> action = async () =>
                await _personService.UpdatePerson(personUpdateRequest);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        // when passing person name equal to null it should
        // throw an ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse personResponse = person.ToPersonResponse();
            PersonUpdateRequest personRequest = personResponse.ToPersonUpdateRequest();

            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act
            Func<Task> action = async () =>
                await _personService.UpdatePerson(personRequest);

            // Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }
        // When trying to update the person with valid data it should pass
        // and update the person data
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "Someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();
            PersonUpdateRequest personUpdateRequest = person_response_expected.ToPersonUpdateRequest();
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_from_update = await _personService.UpdatePerson(personUpdateRequest);

            // Assert
            person_from_update.Should().Be(person_response_expected);
        }

        #endregion

        #region DeletePerson

        // If a valid Id is passed it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonId_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.Gender, "Male")
                .Create();

            _personsRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            // Act 
            bool isDeleted = await _personService.DeletePerson(person.PersonID);
            // Assert
            isDeleted.Should().BeTrue();
        }

        // If a valid Id is passed it should return true
        [Fact]
        public async Task DeletePerson_InvalidPersonId()
        {
            // Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());
            // Assert
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}
