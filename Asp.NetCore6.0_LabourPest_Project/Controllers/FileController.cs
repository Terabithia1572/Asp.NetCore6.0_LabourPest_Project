using Asp.NetCore6._0_LabourPest_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCore6._0_LabourPest_Project.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /File/FileList
        public IActionResult FileList()
        {
            var model = new FileListViewModel();

            // Klasör yollarını belirleyelim
            string adminFolder = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", "adminDosyalar");
            string musteriFolder = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", "musteriDosyalar");

            // Admin dosyalarını oku
            if (Directory.Exists(adminFolder))
            {
                foreach (var file in Directory.GetFiles(adminFolder))
                {
                    var fileInfo = new FileInfo(file);
                    string url = "/labourpestcustomer/adminDosyalar/" + fileInfo.Name;
                    model.AdminFiles.Add(new FileItem
                    {
                        FileName = fileInfo.Name,
                        FilePath = url,
                        Category = "Admin",
                        FileSize = FormatFileSize(fileInfo.Length),
                        LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                        PreviewUrl = GetPreviewUrl(url)
                    });
                }
            }

            // Müşteri dosyalarını oku
            if (Directory.Exists(musteriFolder))
            {
                foreach (var file in Directory.GetFiles(musteriFolder))
                {
                    var fileInfo = new FileInfo(file);
                    string url = "/labourpestcustomer/musteriDosyalar/" + fileInfo.Name;
                    model.MusteriFiles.Add(new FileItem
                    {
                        FileName = fileInfo.Name,
                        FilePath = url,
                        Category = "Müşteri",
                        FileSize = FormatFileSize(fileInfo.Length),
                        LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                        PreviewUrl = GetPreviewUrl(url)
                    });
                }
            }

            // Tüm dosyaları al ve son eklenen 8 dosyayı (dosya değiştirilme tarihine göre) belirle
            var allFiles = model.AllFiles;
            model.RecentFiles = allFiles
                .Select(f =>
                {
                    // Dosyanın fiziksel yolunu oluşturuyoruz
                    string physicalPath = Path.Combine(_hostingEnvironment.WebRootPath,
                        f.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    DateTime lastWrite = System.IO.File.GetLastWriteTime(physicalPath);
                    return new { File = f, LastWrite = lastWrite };
                })
                .OrderByDescending(x => x.LastWrite)
                .Take(8)
                .Select(x => x.File)
                .ToList();

            return View(model);
        }

        // POST: /File/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string kategori)
        {
            if (file != null && file.Length > 0)
            {
                // Yükleme yapılacak klasörü belirle: kategori "Admin" veya "Müşteri"
                string folderName = kategori == "Admin" ? "adminDosyalar" : "musteriDosyalar";
                string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", folderName);

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Dosya adını al ve tam yolu oluştur
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            // Yükleme sonrası liste sayfasına yönlendir
            return RedirectToAction("FileList");
        }
        public IActionResult Download(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return NotFound();

            string physicalPath = Path.Combine(_hostingEnvironment.WebRootPath,
                filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            // İçerik tipini belirleyebilirsiniz (örneğin, MIME mapping ile)
            string contentType = "application/octet-stream";
            return PhysicalFile(physicalPath, contentType, Path.GetFileName(physicalPath));
        }

        // Yeniden Adlandır aksiyonu (POST)
        [HttpPost]
        public IActionResult Rename(string filePath, string newName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(newName))
                return BadRequest();

            // Orijinal dosyanın fiziksel yolunu oluşturun.
            string physicalPath = Path.Combine(
                _hostingEnvironment.WebRootPath,
                filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            // Eğer yeni dosya adında uzantı yoksa, orijinal dosyanın uzantısını ekleyelim.
            if (!Path.HasExtension(newName))
            {
                newName += Path.GetExtension(physicalPath);
            }

            string directory = Path.GetDirectoryName(physicalPath);
            string newPath = Path.Combine(directory, newName);

            try
            {
                System.IO.File.Move(physicalPath, newPath);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction("FileList");
        }



        // Sil aksiyonu (POST)
        [HttpPost]
        public IActionResult Delete(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest();

            string physicalPath = Path.Combine(
                _hostingEnvironment.WebRootPath,
                filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            return RedirectToAction("FileList");
        }


        // Dosya boyutunu uygun biçime dönüştüren yardımcı metot
        private string FormatFileSize(long bytes)
        {
            if (bytes >= 1073741824)
                return string.Format("{0:##.##} GB", (double)bytes / 1073741824);
            if (bytes >= 1048576)
                return string.Format("{0:##.##} MB", (double)bytes / 1048576);
            if (bytes >= 1024)
                return string.Format("{0:##.##} KB", (double)bytes / 1024);
            if (bytes > 0 && bytes < 1024)
                return bytes.ToString() + " B";
            return "0 B";
        }

        // Dosyanın son değiştirilme zamanını "Edited X ago" formatında döndüren örnek yardımcı metot
        private string GetTimeAgo(DateTime lastWriteTime)
        {
            var timeSpan = DateTime.Now - lastWriteTime;

            if (timeSpan.TotalDays >= 1)
                return string.Format("{0:0} gün önce", timeSpan.TotalDays);
            if (timeSpan.TotalHours >= 1)
                return string.Format("{0:0} saat önce", timeSpan.TotalHours);
            if (timeSpan.TotalMinutes >= 1)
                return string.Format("{0:0} dakika önce", timeSpan.TotalMinutes);
            return "Şimdi";
        }

        // Dosya türüne göre önizleme resmi seçen metot
        private string GetPreviewUrl(string fileUrl)
        {
            string ext = Path.GetExtension(fileUrl).ToLowerInvariant();
            // Resim uzantıları: .jpg, .jpeg, .png, .gif
            var imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (imageExtensions.Contains(ext))
            {
                // Eğer dosya resimse, doğrudan dosyanın URL'sini döndür.
                return fileUrl;
            }
            else if (ext == ".pdf")
            {
                return "/labourpestcustomer/fileIcons/pdf.webp"; // PDF için generic önizleme resmi (dosya yolunu güncelleyin)
            }
            else if (ext == ".xlsx" || ext == ".xls")
            {
                return "/labourpestcustomer/fileIcons/excel.png"; // Excel için generic önizleme resmi
            }
            else if (ext == ".doc" || ext == ".docx")
            {
                return "/labourpestcustomer/fileIcons/word.png"; // Word için generic önizleme resmi
            }
            else
            {
                return "/assets/img/file-manager/file-generic.png"; // Genel bir dosya ikonu
            }
        }
    }
}
