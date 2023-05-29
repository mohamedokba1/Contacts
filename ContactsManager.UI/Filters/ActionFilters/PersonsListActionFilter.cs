using UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Core.DTOs;

namespace Contacts_Manager.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{filter}.{method} called", nameof(PersonsListActionFilter), nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? _parameters = (IDictionary<string, object?>?)context.HttpContext.Items["Parameters"];
            if (_parameters?.Count() > 0)
            {
                if (_parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(_parameters["searchBy"]);
                }
                if (_parameters.ContainsKey("searchString"))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(_parameters["searchString"]);
                }
                if (_parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(_parameters["sortBy"]);
                }
                if (_parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(_parameters["sortOrder"]);
                }
            }

            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name"},
                { nameof(PersonResponse.Email), "Email"},
                { nameof(PersonResponse.DateOfBirth), "Birth Date"},
                { nameof(PersonResponse.Address), "Address"},
                { nameof(PersonResponse.PersonGender), "Gender"},
                { nameof(PersonResponse.CountryID), "Country"},
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["Parameters"] = context.ActionArguments;

            _logger.LogInformation("{filter}.{method} called", nameof(PersonsListActionFilter), nameof(OnActionExecuting));
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    List<string> searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.PersonGender),
                        nameof(PersonResponse.CountryID),
                    };
                    if(!searchByOptions.Any(temp => temp == searchBy))
                    {
                        _logger.LogInformation("actual search by string is {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("Updated search by string is {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
