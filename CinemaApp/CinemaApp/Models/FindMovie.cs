using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaApp.Models
{
    public class FindMovie
    {
        //TEST TEST TEST
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public string FullImageUrl => AppSettings.ApiUrl + ImageUrl;

    }

}
