﻿using CarRentingSystem.Data;
using CarRentingSystem.Data.Models;
using CarRentingSystem.Infrastructure;
using CarRentingSystem.Models.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CarRentingSystem.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarRentingDbContext data;

        public CarsController(CarRentingDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (!this.UserIsDealer())
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            return this.View(new AddCarFormModel
            {
                Categories = this.GetCarCategories()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(AddCarFormModel car)
        {
            var dealerId = this.data
                .Dealers
                .Where(d => d.UserId == this.User.GetId())
                .Select(d => d.Id)
                .FirstOrDefault();

            if (dealerId == 0)
            {
                return RedirectToAction(nameof(DealersController.Become), "Dealers");
            }

            if (!this.data.Categories.Any(c => c.Id == car.CategoryId))
            {
                this.ModelState.AddModelError(nameof(car.CategoryId), "Category does not exist.");
            }

            if (!this.ModelState.IsValid)
            {
                car.Categories = this.GetCarCategories();
                return this.View(car);
            }

            var carData = new Car
            {
                Brand = car.Brand,
                Model = car.Model,
                Description = car.Description,
                ImageUrl = car.ImageUrl,
                Year = car.Year,
                CategoryId = car.CategoryId,
                DealerId = dealerId,
            };

            this.data.Cars.Add(carData);

            this.data.SaveChanges();

            return this.RedirectToAction(nameof(All));
        }

        public IActionResult All(AllCarsQueryModel query)
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
                .Skip((query.CurrentPage - 1) * AllCarsQueryModel.CarsPerPage)
                .Take(AllCarsQueryModel.CarsPerPage)
                .Select(c => new CarListingViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    ImageUrl = c.ImageUrl,
                    Year = c.Year,
                    Category = c.Category.Name
                })
                .ToList();

            var carBrands = this.data
                .Cars
                .Select(c => c.Brand)
                .Distinct()
                .ToList();

            query.TotalCars = totalCars;
            query.Brands = carBrands;
            query.Cars = cars;
            
            return this.View(query);
        }

        private bool UserIsDealer()
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == this.User.GetId());
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
