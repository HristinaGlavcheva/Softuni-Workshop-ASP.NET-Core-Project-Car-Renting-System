using CarRentingSystem.Data;
using CarRentingSystem.Models;
using CarRentingSystem.Models.Api.Cars;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CarRentingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/cars")]
    public class CarsApiController : ControllerBase
    {
        private readonly CarRentingDbContext data;

        public CarsApiController(CarRentingDbContext data)
        {
            this.data = data;
        }

        [HttpGet]
        public ActionResult<AllCarsApiResponseModel> All([FromQuery] AllCarsApiRequestModel query)
        {
            var carsQuery = this.data.Cars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Brand))
            {
                carsQuery = carsQuery.Where(c => c.Brand == query.Brand);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                carsQuery = carsQuery.Where(c =>
                (c.Brand + " " + c.Model).ToLower().Contains(query.SearchTerm.ToLower()) ||
                c.Description.ToLower().Contains(query.SearchTerm.ToLower()));
            }

            carsQuery = query.Sorting switch
            {
                CarSorting.Year => carsQuery.OrderByDescending(c => c.Year),
                CarSorting.BrandAndModel => carsQuery.OrderBy(c => c.Brand).ThenBy(c => c.Model),
                CarSorting.DateCreated or _ => carsQuery.OrderByDescending(c => c.Id)
            };

            var totalCars = carsQuery.Count();

            var cars = carsQuery
                .Skip((query.CurrentPage - 1) * query.CarsPerPage)
                .Take(query.CarsPerPage)
                .Select(c => new CarResponseModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    ImageUrl = c.ImageUrl,
                    Year = c.Year,
                    Category = c.Category.Name
                })
                .ToList();

            return new AllCarsApiResponseModel
            {
                CurrentPage = query.CurrentPage,
                TotalCars = totalCars,
                Cars = cars,
            };
        }
    }
}
