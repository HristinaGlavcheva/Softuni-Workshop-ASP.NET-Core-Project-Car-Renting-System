using System.Collections.Generic;

namespace CarRentingSystem.Models.Api.Cars
{
    public class AllCarsApiResponseModel
    {
        public int CurrentPage { get; init; }

        public int TotalCars { get; set; }

        public IEnumerable<CarResponseModel> Cars { get; init; }
    }
}
