﻿using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class SubscribeController : Controller
    {
        SubscribeManager subscribeManager = new(new EfSubscribeRepository());

        [HttpGet]
        public IActionResult Subscribe()
        {
            return View();
        }
      
        [HttpPost]
        public IActionResult AddSubscribes(Subscribe subscribe)
        {
            if (ModelState.IsValid)
            {
                subscribe.SubscribeStatus = true;
                subscribeManager.TAdd(subscribe);
                return RedirectToAction("Deneme", "Home");
            }

            // Hata durumunda, aynı sayfayı tekrar göster
            return View();
        }
    }
}
