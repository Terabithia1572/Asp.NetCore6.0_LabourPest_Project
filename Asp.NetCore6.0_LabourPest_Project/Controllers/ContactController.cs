﻿using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class ContactController : Controller
    {
        ContactManager contactManager = new ContactManager(new EfContactRepository());
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddContact()
        {
			ViewData["ShowComponents"] = false; // Bileşenlerin görüntülenmesini kapatır
			return View();
        }
        [HttpPost]
        public IActionResult AddContact(Contact contact)
        {

            contact.ContactStatus = true;
            contact.ContactDate= DateTime.Parse(DateTime.Now.ToShortDateString());
            contactManager.TAdd(contact);
            return RedirectToAction("Deneme","Home");
        }
    }
}
