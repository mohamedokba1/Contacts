using Core.Entities;
using Core.Helper;
using Core.ServicesContracts;
using Core.DTOs;
using Core.Enums;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Core.RepositoryContracts;
using Serilog;

namespace Core.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly IDiagnosticContext _diagnosticContext;
        public PersonsService(IPersonsRepository personsRepository, IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _diagnosticContext = diagnosticContext;
        }
        
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            // check if the PersonAddRequest is null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }
            ValidationHelper.ModelValidation(personAddRequest);
            // convert  to the mathced_person 
            Person person = personAddRequest.ToPerson();

            //generate new Guid
            person.PersonID = Guid.NewGuid();
            //_db.InsertPerson(person);
            await _personsRepository.AddPerson(person);

            return person.ToPersonResponse();
        }
        
        public async Task<List<PersonResponse>> GetAllPersons()
        {
            return (await _personsRepository.GetAllPesons()).Select(p => p.ToPersonResponse()).ToList();
        }
        
        public async Task<PersonResponse?> GetPersonById(Guid? personId)
        {
            if (personId == null) return null;

            Person? person = await _personsRepository.GetPersonByPersonID(personId.Value);
            if (person == null) return null;
            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.DateOfBirth.Value.Year.ToString().Contains(searchString)),

                nameof(PersonResponse.PersonGender) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.Gender.Equals(searchString)),

                nameof(PersonResponse.CountryID) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                await _personsRepository.GetFilteredPersons(p =>
                p.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPesons()
            };

            _diagnosticContext.Set("Persons", persons);
            return persons.Select(temp => temp.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy)) return allPersons;

            List<PersonResponse> sorted_pesrons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.PersonGender), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.PersonGender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonGender), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.PersonGender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetter), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.ReceiveNewsLetter).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetter), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.ReceiveNewsLetter).ToList(),

                // Default Case
                _ => allPersons
            };
            return sorted_pesrons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Check if the user is exist or not
            Person? mathced_person = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonId);
            if (mathced_person == null)
                throw new ArgumentException("Given Person Doesn't Exist");

            // Update All details if it exist
            mathced_person.PersonName = personUpdateRequest.PersonName;
            mathced_person.Address = personUpdateRequest.Address;
            mathced_person.Email = personUpdateRequest.Email;
            mathced_person.Gender = personUpdateRequest.PersonGender?.ToString();
            mathced_person.DateOfBirth = personUpdateRequest.DateOfBirth;
            mathced_person.ReceiveNewsLetter = personUpdateRequest.ReceiveNewsLetter;
            mathced_person.CountryID = personUpdateRequest.CountryID;

            return (await _personsRepository.UpdatePerson(mathced_person)).ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if (personId == null) return false;

            Person? person = await _personsRepository.GetPersonByPersonID(personId.Value);
            if (person == null) return false;

            await _personsRepository.DeletePerson(personId.Value);

            return true;
        }

        public async Task<MemoryStream> GetAllPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter  streamWriter = new StreamWriter(memoryStream);
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            //csvWriter.WriteField(nameof(PersonResponse.PersonId));
            csvWriter.WriteField(nameof(PersonResponse.PersonName));
            csvWriter.WriteField(nameof(PersonResponse.Age));
            csvWriter.WriteField(nameof(PersonResponse.Email));
            csvWriter.WriteField(nameof(PersonResponse.Address));
            csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
            csvWriter.WriteField(nameof(PersonResponse.PersonGender));
            csvWriter.WriteField(nameof(PersonResponse.Country));
            csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetter));
            await csvWriter.NextRecordAsync();

            List<PersonResponse> persons = await GetAllPersons();

            foreach(PersonResponse person in persons)
            {
                csvWriter.WriteField(person.PersonName);
                csvWriter.WriteField(person.Age);
                csvWriter.WriteField(person.Email);
                csvWriter.WriteField(person.Address);
                if (person.DateOfBirth.HasValue)
                    csvWriter.WriteField(person.DateOfBirth.Value.ToString("yyyy-MM-dd"));
                else 
                    csvWriter.WriteField("");
                csvWriter.WriteField(person.PersonGender);
                csvWriter.WriteField(person.Country);
                csvWriter.WriteField(person.ReceiveNewsLetter);
                await csvWriter.NextRecordAsync();
                await csvWriter.FlushAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> GetAllPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Persons_Sheet");
                worksheet.Cells["A1"].Value = "Person Name";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Date of Birth";
                worksheet.Cells["D1"].Value = "Gender";
                worksheet.Cells["E1"].Value = "Age";
                worksheet.Cells["F1"].Value = "Address";
                worksheet.Cells["G1"].Value = "Country";
                worksheet.Cells["H1"].Value = "Receive News Letters";

                using (ExcelRange headerCells = worksheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
                    headerCells.Style.Font.Bold = true;
                }
                    int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach(PersonResponse person in persons) 
                {
                    worksheet.Cells[row, 1].Value = person.PersonName;
                    worksheet.Cells[row, 2].Value = person.Email;
                    if(person.DateOfBirth.HasValue)
                        worksheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MMM-dd");
                    else
                        worksheet.Cells[row, 3].Value = "_";
                    worksheet.Cells[row, 4].Value = person.PersonGender;
                    worksheet.Cells[row, 5].Value = person.Age;
                    worksheet.Cells[row, 6].Value = person.Address;
                    worksheet.Cells[row, 7].Value = person.Country;
                    worksheet.Cells[row, 8].Value = person.ReceiveNewsLetter;
                    row++;
                }
                worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsync();

                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
