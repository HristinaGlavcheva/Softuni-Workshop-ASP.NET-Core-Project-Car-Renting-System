using CarRentingSystem.Data;
using CarRentingSystem.Data.Models;
using CarRentingSystem.Models.Cars;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CarRentingSystem.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarRentingDbContext data;

        public CarsController(CarRentingDbContext data)
        {
            this.data = data;
        }

        public IActionResult Add()
        {
            return this.View(new AddCarFormModel
            { 
                Categories = this.GetCarCategories() 
            });
        }

        [HttpPost]
        public IActionResult Add(AddCarFormModel car)
        {
            if(!this.data.Categories.Any(c => c.Id == car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            }
            
            if (!this.ModelState.IsValid)
            {
                car.Categories = this.GetCarCategories();
                return this.View(car);
            }

            var newCar = new Car
            {
                Brand = car.Brand,
                Model = car.Model,
                Description = car.Description,
                ImageUrl = car.ImageUrl,
                Year = car.Year,
                CategoryId = car.CategoryId
            };

            this.data.Cars.Add(newCar);
             
            this.data.SaveChanges();

            return this.RedirectToAction("Index", "Home");
        }

        private IEnumerable<CarCategoryViewModel> GetCarCategories()
        {
            return this.data
                .Categories
                .Select(c => new CarCategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();
        }
    }
}
