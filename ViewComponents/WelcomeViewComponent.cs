using Microsoft.AspNetCore.Mvc;
using MvcMovie.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcMovie.ViewComponents
{
    public class WelcomeViewComponent:ViewComponent
    {
        private readonly MvcMovieContext context;

        public WelcomeViewComponent(MvcMovieContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var count = context.Movie.Count();
            return View("Default", count);
        }
    }
}
