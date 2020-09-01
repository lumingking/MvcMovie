using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.Models
{
    public class User
    {
        public int Id { get; set; }
        public string user_code { get; set; }
        public string user_name { get; set; }
        public string phone { get; set; }
        public string sec_phone { get; set; }
        public string email { get; set; }
        public string Id_card { get; set; }
        public string address_code { get; set; }
        public string address_detail { get; set; }


    }
}
