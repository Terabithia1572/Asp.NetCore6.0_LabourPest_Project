﻿
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class JobApplicationController : Controller
    {
        JobApplicationManager jobApplicationManager = new JobApplicationManager(new EfJobApplicationRepository());
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult JobApplicationList()
        {
            var values = jobApplicationManager.GetAll();
            return View(values);
        }
     
    }
}
