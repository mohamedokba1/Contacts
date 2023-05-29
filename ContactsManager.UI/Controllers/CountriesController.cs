using Microsoft.AspNetCore.Mvc;
using Core.ServicesContracts;

namespace UI.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountriesService countriesService, ILogger<CountriesController> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }
        [Route("uploadfromexcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("uploadfromexcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.Errors = "Please Select an .xlsx file";
                return View();
            }
            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Errors = "Unsported file only .xlsx file allowed :)";
                return View();
            }

            int numberOfCountriesInserted = await _countriesService.UploadCountriesFromExcelFile(excelFile);
            if(numberOfCountriesInserted > 0)
            {
                ViewBag.Message = $"{numberOfCountriesInserted} countries uploaded successfully!";
                _logger.LogInformation($"{numberOfCountriesInserted} countries uploaded successfully!");
            }
            else
            {
                ViewBag.Message = $"All Countries in the file are already exist :)";
                _logger.LogInformation("Trying to add existing countries");
            }
                
            return View();
        }
    }
}
