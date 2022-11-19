using CarRentingSystem.Models.Api.Cars;
using CarRentingSystem.Services.Cars;
using Microsoft.AspNetCore.Mvc;

namespace CarRentingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/cars")]
    public class CarsApiController : ControllerBase
    {
        private readonly ICarService cars;

        public CarsApiController(ICarService cars)
        {
            this.cars = cars;
        }

        [HttpGet]
        public CarQueryServiceModel All([FromQuery] AllCarsApiRequestModel query)
        {
            return this.cars.All(
                query.Brand,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                query.CarsPerPage);
        }
    }
}
