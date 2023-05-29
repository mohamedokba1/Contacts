using Microsoft.AspNetCore.Mvc;
using ServicesContracts;
using ServicesContracts.DTOs;
using ServicesContracts.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using Contacts_Manager.Filters.ActionFilters;

namespace Contacts_Manager.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonService personService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personService;
            _countriesService = countriesService;
            _logger = logger;
        }
        private async void  PopulateCountries()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries
                .Select(c => new SelectListItem() { Text = c.CountryName, Value = c.CountryId.ToString() });
        }
        [Route("[action]")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
            string sortBy =  nameof(PersonResponse.PersonName),
            SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            // Logging
            _logger.LogInformation("Index action from persons controller");
            _logger.LogDebug($"searchBy: {searchBy?? "Not inserted"}, searchString:{searchString ?? "Not inserted"}, sortBy: {sortBy}, sortOrder: {sortOrder}");
            List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            
            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            PopulateCountries();
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                PopulateCountries();
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personAddRequest);
            }
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? response = await _personService.GetPersonById(personID);
            if (response == null)
                return RedirectToAction("Index");
            PersonUpdateRequest personUpdateRequest = response.ToPersonUpdateRequest();
            PopulateCountries();
            return View(personUpdateRequest);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? response = await _personService.GetPersonById(personUpdateRequest.PersonId);
            if(response == null)
            {
                return RedirectToAction("Index");
            }
            else if(ModelState.IsValid)
            {
                await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                PopulateCountries();
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
        }

        [Route("[action]/{PersonID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personService.GetPersonById(personID);
            if (personResponse == null)
                return RedirectToAction("Index");
            return View(personResponse);
        }

        [Route("[action]/{PersonID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonResponse person)
        {
            PersonResponse? personResponse = await _personService.GetPersonById(person.PersonId);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                await _personService.DeletePerson(personResponse.PersonId);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", persons)
            {
                PageMargins = new Margins
                {
                    Left = 0,
                    Top = 0,
                    Right = 0,
                    Bottom = 0,
                },
                PageOrientation = Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetAllPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsEXCEL")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetAllPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
