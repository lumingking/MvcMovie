using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Models
{
    public class EnumForMovie
    {
        public enum Genres
        {
            [Display(Name = "喜剧片")]
            Comedy = 1,
            [Display(Name = "西部片")]
            Western = 2,
            [Display(Name = "浪漫喜剧")]
            RomanticComedy = 3,
            [Display(Name ="限制")]
            Danger=4

        }
    }
}
