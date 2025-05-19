using System;

namespace Asp.NetCore6._0_LabourPest_Project.Models
{
    public class CommentTreeViewModel
    {
        public int CommentID { get; set; }
        public int? ParentCommentID { get; set; }
        public string Title { get; set; }          // ✅ Burayı ekle
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; }
        public int WriterID { get; set; }          // Düzenleme kontrolü için
        public DateTime CommentDate { get; set; }
        public List<CommentTreeViewModel> Replies { get; set; } = new();
    }

}
