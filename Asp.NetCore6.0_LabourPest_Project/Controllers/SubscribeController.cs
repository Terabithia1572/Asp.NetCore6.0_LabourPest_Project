﻿using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        public IActionResult SubscribeList() // Abone listesi
        {
            var values = subscribeManager.GetAll();
            return View(values);
        }
        public IActionResult DeleteSubscribe(int id) // Abone silme
        {
            var values = subscribeManager.TGetByID(id);
            subscribeManager.TDelete(values);
            return RedirectToAction("SubscribeList", "Subscribe");
        }
    }
}
