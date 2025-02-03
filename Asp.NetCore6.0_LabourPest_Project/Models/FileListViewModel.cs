namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    // Models/FileListViewModel.cs
    using System.Collections.Generic;
    using System.Linq;

    public class FileListViewModel
    {
        public List<FileItem> AdminFiles { get; set; } = new List<FileItem>();
        public List<FileItem> MusteriFiles { get; set; } = new List<FileItem>();

        /// <summary>
        /// Tüm dosyalar (Admin + Müşteri)
        /// </summary>
        public List<FileItem> AllFiles
        {
            get { return AdminFiles.Concat(MusteriFiles).ToList(); }
        }

        /// <summary>
        /// Son eklenen 8 dosya (her iki kategoriden)
        /// </summary>
        public List<FileItem> RecentFiles { get; set; } = new List<FileItem>();
    }

}
