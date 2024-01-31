using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilter;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(HandleExceptionFilter))]
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
        [HttpGet]
        [TypeFilter(typeof(PersonsListActionFilter))]
        [TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.Name), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            return View(sortedPersons);
        }
       
        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(FeatureDisabledResourceFilter), Arguments = new object[] { false })]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Custom_Key", "My-Custom-Value", 1 })]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() }) ;
            new SelectListItem() { Text = "", Value = "" };

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]   
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _personsService.AddPerson(personRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? personToEdit = await _personsService.GetPersonById(personId);

            if(personToEdit == null)
            {
                return RedirectToAction("Index", "Persons");
            }
            PersonUpdateRequest personUpdateRequest = personToEdit.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.Name, Value = country.Id.ToString() });
            new SelectListItem() { Text = "", Value = "" };

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personFetched = await _personsService.GetPersonById(personRequest.PersonId);
            
            if (personFetched == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            PersonResponse updatedPerson = await _personsService.UpdatePerson(personRequest);

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
