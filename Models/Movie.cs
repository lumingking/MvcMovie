using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMovie.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Display(Name ="电影名")]
        [StringLength(60,MinimumLength =3)]
        [Required]
        public string Title { get; set; }

        [Display(Name ="发布日期")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Required]
        [StringLength(30,ErrorMessage ="类型名称在2-30个字符之间",MinimumLength =2)]
        [Display(Name = "类型")]
        public string Genre { get; set; }

        [Range(1,100)]
        [DataType(DataType.Currency)]
        [Display(Name = "价格")]
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-]*$")]
        [StringLength(5)]
        [Required]
        [Display(Name = "评级")]
        public string Rating { get; set; }

        [StringLength(50,ErrorMessage ="文件名在1-50个字符之间",MinimumLength =1)]
        [Display(Name ="文件名")]
        public string FileName { get; set; }

        [StringLength(10)]
        [Display(Name ="文件类型")]
        public string FileType { get; set; }

        [NotMapped]
        [Required]
        [Display(Name ="附件")]
        //[FileExtensions(ErrorMessage ="附件格式错误",Extensions =".txt")]
        public IFormFile FormFile { get; set; }
    }
}