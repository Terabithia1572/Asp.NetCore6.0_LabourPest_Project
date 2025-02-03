namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class FileItem
    {
        public string FileName { get; set; }
        /// <summary>
        /// Web erişim yolu, örn: "/labourpestcustomer/adminDosyalar/Pages.zip"
        /// </summary>
        public string FilePath { get; set; }
        public string Category { get; set; }

        /// <summary>
        /// Dosya boyutu (örneğin "8.8 mb")
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// Dosyanın son düzenlenme bilgisi (örneğin "Edited 5 min ago")
        /// </summary>
        public string LastModifiedAgo { get; set; }

        /// <summary>
        /// Ön izleme resmi URL'si. Eğer dosya görsel ise dosyanın kendisi,
        /// diğer dosya tipleri için uygun bir generic ikon/resim yolu verilebilir.
        /// </summary>
        public string PreviewUrl { get; set; }
    }

}
