using Asp.NetCore6._0_LabourPest_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Asp.NetCore6._0_Labourpest_Project.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FileController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // Giriş yapan kullanıcının (writer) klasör adını döndürür.
        // (User.Identity.Name’nin writer mailini içerdiğini varsayıyoruz.)
        private string GetWriterFolderName()
        {
            return User.Identity.Name ?? "defaultUser";
        }

        // GET: /File/FileList?folder=...
        public IActionResult FileList(string folder)
        {
            var model = new FileListViewModel();
            string basePath = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer");

            // --- Sabit Klasörler: Admin Dosyaları & Müşteri Dosyaları ---
            // Admin Dosyaları
            string adminFolder = Path.Combine(basePath, "adminDosyalar");
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
                        Category = "Admin Dosyaları",
                        FileSize = FormatFileSize(fileInfo.Length),
                        LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                        PreviewUrl = GetPreviewUrl(url)
                    });
                }
            }

            // Müşteri Dosyaları
            string musteriFolder = Path.Combine(basePath, "musteriDosyalar");
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
                        Category = "Müşteri Dosyaları",
                        FileSize = FormatFileSize(fileInfo.Length),
                        LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                        PreviewUrl = GetPreviewUrl(url)
                    });
                }
            }

            // --- Writer Klasörleri ---
            if (User.IsInRole("Admin"))
            {
                // Admin: Sadece sabit klasörler hariç; yalnızca writerMail formatında klasörleri ekleyelim.
                var directories = Directory.GetDirectories(basePath);
                foreach (var dir in directories)
                {
                    string folderNameCandidate = Path.GetFileName(dir);
                    if (folderNameCandidate.Equals("adminDosyalar", StringComparison.OrdinalIgnoreCase)
                        || folderNameCandidate.Equals("musteriDosyalar", StringComparison.OrdinalIgnoreCase)
                        || folderNameCandidate.Equals("customerImage", StringComparison.OrdinalIgnoreCase)
                        || folderNameCandidate.Equals("AddcustomerImage", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Yalnızca writerMail formatı taşıyan klasörler (örneğin içerisinde '@' varsa)
                    if (!folderNameCandidate.Contains("@"))
                        continue;

                    if (!model.WriterFolders.Contains(folderNameCandidate))
                    {
                        model.WriterFolders.Add(folderNameCandidate);
                    }

                    // Dosyaları da listeye ekle
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        var fileInfo = new FileInfo(file);
                        string url = $"/labourpestcustomer/{folderNameCandidate}/" + fileInfo.Name;
                        model.WriterFiles.Add(new FileItem
                        {
                            FileName = fileInfo.Name,
                            FilePath = url,
                            Category = folderNameCandidate,
                            FileSize = FormatFileSize(fileInfo.Length),
                            LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                            PreviewUrl = GetPreviewUrl(url)
                        });
                    }
                }
            }
            else
            {
                // Müşteri (writer): Yalnızca kendi klasörünü göster.
                string writerFolderName = GetWriterFolderName();
                if (!model.WriterFolders.Contains(writerFolderName))
                {
                    model.WriterFolders.Add(writerFolderName);
                }
                string writerFolder = Path.Combine(basePath, writerFolderName);
                if (Directory.Exists(writerFolder))
                {
                    foreach (var file in Directory.GetFiles(writerFolder))
                    {
                        var fileInfo = new FileInfo(file);
                        string url = $"/labourpestcustomer/{writerFolderName}/" + fileInfo.Name;
                        model.WriterFiles.Add(new FileItem
                        {
                            FileName = fileInfo.Name,
                            FilePath = url,
                            Category = writerFolderName,
                            FileSize = FormatFileSize(fileInfo.Length),
                            LastModifiedAgo = GetTimeAgo(fileInfo.LastWriteTime) + " Yüklendi",
                            PreviewUrl = GetPreviewUrl(url)
                        });
                    }
                }
            }

            // Tüm dosyaları birleştirin.
            model.AllFiles = new List<FileItem>();
            model.AllFiles.AddRange(model.AdminFiles);
            model.AllFiles.AddRange(model.MusteriFiles);
            model.AllFiles.AddRange(model.WriterFiles);

            // Eğer bir klasör filtresi geldiyse, AllFiles listesini filtreleyelim.
            if (!string.IsNullOrEmpty(folder))
            {
                if (folder == "Admin Dosyaları")
                {
                    model.AllFiles = model.AdminFiles;
                }
                else if (folder == "Müşteri Dosyaları")
                {
                    model.AllFiles = model.MusteriFiles;
                }
                else if (folder == "Benim Dosyalarım")
                {
                    string writerFolder = GetWriterFolderName();
                    model.AllFiles = model.WriterFiles.Where(f => f.Category == writerFolder).ToList();
                }
                else
                {
                    // Writer klasörüne ait dosyalar
                    model.AllFiles = model.WriterFiles.Where(f => f.Category == folder).ToList();
                }
            }

            // Son eklenen 8 dosya (filtrelenmiş AllFiles üzerinden)
            model.RecentFiles = model.AllFiles
                .Select(f =>
                {
                    string physicalPath = Path.Combine(_hostingEnvironment.WebRootPath,
                        f.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    DateTime lastWrite = System.IO.File.GetLastWriteTime(physicalPath);
                    return new { File = f, LastWrite = lastWrite };
                })
                .OrderByDescending(x => x.LastWrite)
                .Take(8)
                .Select(x => x.File)
                .ToList();

            // Seçili klasörü view'e aktarmak için (isteğe bağlı)
            ViewBag.ActiveFolder = folder;

            return View(model);
        }

        // POST: /File/Upload, Download, Rename, Delete, vs. (değişmeden kalabilir)
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string kategori)
        {
            if (file != null && file.Length > 0)
            {
                string folderName = string.Empty;
                // "Admin Dosyaları" veya "Müşteri Dosyaları" sabit; aksi halde kategori değeri writer klasör adı.
                if (kategori == "Admin")
                {
                    folderName = "adminDosyalar";
                }
                else if (kategori == "Müşteri")
                {
                    folderName = "musteriDosyalar";
                }
                else if (kategori == "Benim Dosyalarım")
                {
                    folderName = GetWriterFolderName();
                }
                else
                {
                    folderName = kategori;
                }

                string uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "labourpestcustomer", folderName);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

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

            string contentType = "application/octet-stream";
            return PhysicalFile(physicalPath, contentType, Path.GetFileName(physicalPath));
        }

        [HttpPost]
        public IActionResult Rename(string filePath, string newName)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(newName))
                return BadRequest();

            string physicalPath = Path.Combine(
                _hostingEnvironment.WebRootPath,
                filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

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

        private string GetPreviewUrl(string fileUrl)
        {
            string ext = Path.GetExtension(fileUrl).ToLowerInvariant();
            var imageExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (imageExtensions.Contains(ext))
                return fileUrl;
            else if (ext == ".pdf")
                return "/labourpestcustomer/fileIcons/pdf.webp";
            else if (ext == ".xlsx" || ext == ".xls")
                return "/labourpestcustomer/fileIcons/excel.png";
            else if (ext == ".doc" || ext == ".docx")
                return "/labourpestcustomer/fileIcons/word.png";
            else
                return "/assets/img/file-manager/file-generic.png";
        }
    }
}
