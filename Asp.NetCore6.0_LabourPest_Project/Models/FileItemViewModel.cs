namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class FileItemViewModel
    {
        public string Name { get; set; }
        public string RelativePath { get; set; }  // Örn: "/labourpestcustomer/klasorAdi/dosya.txt"
        public bool IsDirectory { get; set; }
        public List<FileItemViewModel> Children { get; set; } = new List<FileItemViewModel>();
    }
}
