using System.Linq;
using CarRentingSystem.Data;
using CarRentingSystem.Models.Api.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace CarRentingSystem.Controllers.Api
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly CarRentingDbContext data;

        public StatisticsApiController(CarRentingDbContext data)
        {
            this.data = data;
        }

        [HttpGet]
        public StatisticsResponseModel GetStatistics()
        {
            var totalCars = this.data.Cars.Count();
            var totalUsers = this.data.Users.Count();

            return new StatisticsResponseModel()
            {
                TotalRents = 0,
                TotalCars = totalCars,
                TotalUsers = totalUsers,
            };
        }
    }
}
