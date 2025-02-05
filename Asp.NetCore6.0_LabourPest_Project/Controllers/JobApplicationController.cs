using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
	[Authorize(Roles = "Admin")]
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

		[AllowAnonymous]
		[HttpGet]
		public IActionResult AddJobApplication()
		{
			ViewData["ShowComponents"] = false;
			return View();
		}
        [AllowAnonymous]
        [HttpPost]
		public async Task<IActionResult> AddJobApplication(JobApplication jobApplication, IFormFile JobApplicationCV)
		{
			// Dosya kontrolü: Dosya gönderilmiş mi ve içeriği dolu mu?
			if (JobApplicationCV != null && JobApplicationCV.Length > 0)
			{
				// Dosya uzantısını alıp küçük harfe çeviriyoruz
				var extension = Path.GetExtension(JobApplicationCV.FileName).ToLower();

				// Sadece .pdf ve .doc/.docx uzantılarına izin veriyoruz
				if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
				{
					ModelState.AddModelError("JobApplicationCV", "Lütfen sadece .pdf veya .doc/.docx uzantılı dosya yükleyin.");
					return View(jobApplication);
				}

				// Dosya adının çakışmaması için benzersiz bir isim oluşturuyoruz
				var fileNameWithoutExt = Path.GetFileNameWithoutExtension(JobApplicationCV.FileName);
				var uniqueFileName = $"{fileNameWithoutExt}_{Guid.NewGuid()}{extension}";

				// Dosyanın kaydedileceği klasörün yolunu belirtiyoruz (wwwroot klasörü altındaki yol)
				var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer", "jobApplicationCV");

				// Klasör mevcut değilse oluşturuyoruz
				if (!Directory.Exists(uploadPath))
				{
					Directory.CreateDirectory(uploadPath);
				}

				// Tam dosya yolu
				var filePath = Path.Combine(uploadPath, uniqueFileName);

				// Dosyayı belirtilen yola kaydediyoruz
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await JobApplicationCV.CopyToAsync(stream);
				}

				// Veritabanında saklamak üzere, dosya yolunu modelin ilgili alanına atıyoruz.
				// Örneğin, dosya yolunu "/labourpestcustomer/jobApplicationCV/benzersizdosyaadi.pdf" olarak kaydediyoruz.
				jobApplication.JobApplicationCV = $"/labourpestcustomer/jobApplicationCV/{uniqueFileName}";
			}
			else
			{
				ModelState.AddModelError("JobApplicationCV", "CV dosyası seçilmelidir.");
				return View(jobApplication);
			}

			// Diğer bilgiler ekleniyor
			jobApplication.JobApplicationCity = "Türkiye";
			jobApplication.JobApplicationStatus = "1";

			// İş başvurusunu ekliyoruz
			jobApplicationManager.TAdd(jobApplication);
			return RedirectToAction("Deneme","Home");
		}

		public IActionResult DeleteJobApplication(int id)
		{
			var value = jobApplicationManager.TGetByID(id);
			jobApplicationManager.TDelete(value);
			return RedirectToAction("JobApplicationList", "JobApplication");
		}
	}
}
