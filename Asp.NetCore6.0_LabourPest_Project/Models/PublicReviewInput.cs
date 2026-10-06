using System.ComponentModel.DataAnnotations;

namespace Asp.NetCore6._0_LabourPest_Project.Models;

// Web form limits; the existing required database strings are nvarchar(max).
// Identity, date and visibility are deliberately absent from the binding contract.
public sealed class PublicReviewInput
{
    public const string DefaultAvatar = "/labourpestcustomer/assets/img/team/avatar-placeholder.webp";

    [Required(ErrorMessage = "Adınızı ve soyadınızı yazın.")]
    [StringLength(100, ErrorMessage = "Adınız ve soyadınız en fazla 100 karakter olabilir.")]
    public string? CommentUserName { get; set; }

    [Required(ErrorMessage = "Yorumunuzun konusunu yazın.")]
    [StringLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir.")]
    public string? CommentTitle { get; set; }

    [Required(ErrorMessage = "Mesajınızı yazın.")]
    [StringLength(5000, ErrorMessage = "Mesajınız en fazla 5000 karakter olabilir.")]
    public string? CommentContent { get; set; }

    [StringLength(512, ErrorMessage = "Fotoğraf bağlantısı geçersiz. Fotoğrafı yeniden seçin.")]
    [RegularExpression(@"^/canabicom/profilePhoto/[A-Za-z0-9_-]+\.(?i:png|jpe?g|webp|gif|avif)$",
        ErrorMessage = "Fotoğraf bağlantısı geçersiz. Fotoğrafı yeniden seçin veya fotoğrafsız gönderin.")]
    public string? ImageUrl { get; set; }
}
