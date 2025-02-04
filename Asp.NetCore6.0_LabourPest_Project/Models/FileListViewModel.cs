namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    // Models/FileListViewModel.cs
    using System.Collections.Generic;
    using System.Linq;

    public class FileListViewModel
    {
        public List<FileItem> AdminFiles { get; set; } = new List<FileItem>();
        public List<FileItem> MusteriFiles { get; set; } = new List<FileItem>();
        public List<FileItem> WriterFiles { get; set; } = new List<FileItem>();
        public List<FileItem> AllFiles { get; set; } = new List<FileItem>();
        public List<FileItem> RecentFiles { get; set; } = new List<FileItem>();

        // Yazarlara ait klasör isimlerini adminin kategori seçiminde sunmak için:
        public List<string> WriterFolders { get; set; } = new List<string>();
    }

}
