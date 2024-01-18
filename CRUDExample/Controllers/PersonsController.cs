using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
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

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
         
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder;

            return View(sortedPersons);
        }
       
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() }) ;
            new SelectListItem() { Text = "", Value = "" };

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            await _personsService.AddPerson(personAddRequest);

            return RedirectToAction("index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personToEdit = await _personsService.GetPersonById(personId);

            if(personToEdit == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personToEdit.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() });
            new SelectListItem() { Text = "", Value = "" };

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personFetched = await _personsService.GetPersonById(personUpdateRequest.PersonId);
            
            if (personFetched == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personFetched.ToPersonUpdateRequest());
            }

            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? personFound = await _personsService.GetPersonById(personId);
            
            if (personFound == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            return View(personFound);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonResponse personToDelete)
        {
            PersonResponse? personFound = await _personsService.GetPersonById(personToDelete.Id);

            if(personFound == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            
            await _personsService.DeletePerson(personToDelete.Id);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();
                return new ViewAsPdf("PersonsPDF", persons, ViewData)
                {
                    PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }
    }
};
