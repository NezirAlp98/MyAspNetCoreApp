using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MyAspNetCoreApp.Web.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Remote(action:"HasProductName",controller:"Products")]
        [StringLength(50,ErrorMessage ="İsim alanına en fazla 50 karakter girilebilir.")]
        [Required(ErrorMessage ="İsim alanı boş olamaz.")]
        public string? Name { get; set; }

        //[RegularExpression(@"^[0-9]+(\,[0-9]{1,2})",ErrorMessage ="Fiyat alanında noktadan sonra en fazla iki basamak olmalıdır.")]
        [Range(1, 4000, ErrorMessage = "Fiyat alanı 1 ile 4000 arasında bir değer olmalıdır.")]
        [Required(ErrorMessage = "Fiyat alanı boş olamaz.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok alanı boş olamaz.")]
        [Range(1,200,ErrorMessage = "Stok alanı 1 ile 200 arasında bir değer olmalıdır.")]
        public int? Stock { get; set; }

        [StringLength(300,MinimumLength =50, ErrorMessage = "Açıklama alanına en fazla 50 ile 300 karakter girilebilir.")]
        [Required(ErrorMessage = "Açıklama alanı boş olamaz.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Renk seçimi boş olamaz.")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Yayınlanma tarihi boş olamaz.")]
        public DateTime? PublishDate { get; set; }
        public bool IsPublish { get; set; }

        [Required(ErrorMessage = "Yayınlanma süresi boş olamaz.")]
        public int? Expire { get; set; }

        public IFormFile Image { get; set; }

        [ValidateNever]
        public string ImagePath { get; set; }


    }
}
