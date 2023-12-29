using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }
        [Route("[action]")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>(){
                { nameof(PersonResponse.Name), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.BirthDate), "Birth Date" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
            };
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);

            foreach (PersonResponse p in persons)
            {
                Console.WriteLine($"coutnries {p.CountryId}");
            }

            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
         
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder;

            return View(sortedPersons);
        }
       
        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() }) ;
            new SelectListItem() { Text = "", Value = "" };

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            _personsService.AddPerson(personAddRequest);

            return RedirectToAction("index", "Persons");
        }
    }
}
