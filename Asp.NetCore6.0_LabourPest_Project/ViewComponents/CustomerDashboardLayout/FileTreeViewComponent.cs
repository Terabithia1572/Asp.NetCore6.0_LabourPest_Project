// ViewComponents/FileTreeViewComponent.cs
using Asp.NetCore6._0_LabourPest_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Asp.NetCore6._0_LabourPest_Project.ViewComponents
{
    public class FileTreeViewComponent : ViewComponent
    {
        private FileItemViewModel GetDirectoryTree(string physicalPath, string relativePath)
        {
            var item = new FileItemViewModel
            {
                Name = Path.GetFileName(physicalPath),
                RelativePath = relativePath,
                IsDirectory = true
            };

            // Alt klasörleri ekle
            foreach (var dir in Directory.GetDirectories(physicalPath))
            {
                string dirName = Path.GetFileName(dir);
                string childRelativePath = Path.Combine(relativePath, dirName).Replace("\\", "/");
                item.Children.Add(GetDirectoryTree(dir, childRelativePath));
            }

            // Alt dosyaları ekle
            foreach (var file in Directory.GetFiles(physicalPath))
            {
                string fileName = Path.GetFileName(file);
                string fileRelativePath = Path.Combine(relativePath, fileName).Replace("\\", "/");
                item.Children.Add(new FileItemViewModel
                {
                    Name = fileName,
                    RelativePath = fileRelativePath,
                    IsDirectory = false
                });
            }

            return item;
        }

        public IViewComponentResult Invoke()
        {
            string rootPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "labourpestcustomer");
            string rootRelativePath = "/labourpestcustomer";
            var model = GetDirectoryTree(rootPhysicalPath, rootRelativePath);
            return View(model);
        }
    }
}
