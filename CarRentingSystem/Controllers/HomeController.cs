﻿using System.Diagnostics;
using System.Linq;
using CarRentingSystem.Data;
using CarRentingSystem.Models;
using CarRentingSystem.Models.Cars;
using CarRentingSystem.Models.Home;
using CarRentingSystem.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace CarRentingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStatisticsService statistics;
        private readonly CarRentingDbContext data;

        public HomeController(
             IStatisticsService statistics,
             CarRentingDbContext data)
        {
            this.statistics = statistics;
            this.data = data;
        }

        public IActionResult Index()
        {
            var cars = this.data
               .Cars
               .OrderByDescending(c => c.Id)
               .Select(c => new CarIndexViewModel
               {
                   Id = c.Id,
                   Brand = c.Brand,
                   Model = c.Model,
                   ImageUrl = c.ImageUrl,
                   Year = c.Year,
               })
               .Take(3)
               .ToList();

            var totalStatistics = this.statistics.Total();

            return this.View(new IndexViewModel
            {
                TotalCars = totalStatistics.TotalCars,
                TotalUsers = totalStatistics.TotalUsers,
                Cars = cars
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
