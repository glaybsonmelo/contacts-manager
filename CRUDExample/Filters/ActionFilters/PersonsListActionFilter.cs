using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() {
                        nameof(PersonResponse.Name),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.BirthDate),
                        nameof(PersonResponse.Country),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Address),
                    };
                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    {
                        ;
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.Name);
                    }
                }
            }

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            PersonsController personsController = (PersonsController) context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?) context.HttpContext.Items["arguments"];
            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personsController.ViewBag.CurrentSearchBy = Convert.ToString(parameters["searchBy"]);
                }
                else
                {
                    personsController.ViewBag.CurrentSearchBy = nameof(PersonResponse.Name);

                }
                if (parameters.ContainsKey("searchString"))
                {
                    personsController.ViewBag.CurrentSearchString = Convert.ToString(parameters["searchString"]);
                }
               
                if (parameters.ContainsKey("sortBy"))
                {
                    personsController.ViewBag.CurrentSortBy = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewBag.CurrentSortBy = nameof(PersonResponse.Name);
                }
                if (parameters.ContainsKey("sortOrder"))
                {
                    personsController.ViewBag.CurrentSortOrder = (SortOrderOptions) parameters["sortOrder"];
                }
                else
                {
                    personsController.ViewBag.CurrentSortOrder = SortOrderOptions.ASC;
                }
            }
            personsController.ViewBag.SearchFields = new Dictionary<string, string>(){
                { nameof(PersonResponse.Name), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.BirthDate), "Birth Date" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
            };
        }


    }
}
