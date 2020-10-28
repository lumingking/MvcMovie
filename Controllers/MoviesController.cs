using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using MvcMovie.Data;
using MvcMovie.Models;
using Newtonsoft.Json;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;
        private readonly IDistributedCache _distributedCache;
        private readonly IHostEnvironment _env;

        public MoviesController(MvcMovieContext context,IDistributedCache distributedCache,IHostEnvironment env)
        {
            _context = context;
            _distributedCache = distributedCache;
            _env = env;
        }

        // GET: Movies
        //public async Task<IActionResult> Index(string searchstring)
        //{
        //    var movies = from m in _context.Movie
        //                 select m;
        //    if (!string.IsNullOrEmpty(searchstring))
        //    {
        //        movies = movies.Where(s => s.Title.Contains(searchstring));
        //    }
        //    return View(await movies.ToListAsync());
        //}
        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            MovieGenreViewModel movieGenreVM = null;
            var cacheMovies = _distributedCache.Get("movies2");
            if (cacheMovies == null) {
                // Use LINQ to get list of genres.
                IQueryable<string> genreQuery = from m in _context.Movie
                                                orderby m.Genre
                                                select m.Genre;

                var movies = from m in _context.Movie
                             select m;

                if (!string.IsNullOrEmpty(searchString))
                {
                    movies = movies.Where(s => s.Title.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(movieGenre))
                {
                    movies = movies.Where(x => x.Genre == movieGenre);
                }

                movieGenreVM = new MovieGenreViewModel
                {
                    Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                    Movies = await movies.ToListAsync()
                };

                var serializedMovie = JsonConvert.SerializeObject(movieGenreVM);
                byte[] encodedMovie = Encoding.UTF8.GetBytes(serializedMovie);

                var cacheEntryOptions = new DistributedCacheEntryOptions();
                cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _distributedCache.Set("movies2", encodedMovie,cacheEntryOptions);
            }
            else
            {
                var serializedMoive = Encoding.UTF8.GetString(cacheMovies);
                movieGenreVM = JsonConvert.DeserializeObject<MovieGenreViewModel>(serializedMoive);
            }
            

            return View(movieGenreVM);
        }
        [HttpPost]
        public string Index(string searchstring,bool notUsed)
        {
            return "From [HttpPost]Index:filter on" + searchstring;
        }
        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public async Task<IActionResult> Download(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);
            if(movie == null)
            {
                return NotFound();
            }
            var fileDirectory = Path.Combine(_env.ContentRootPath, "Files/Test");
            var fileName = movie.FileName + movie.FileType;
            var filePath =Path.Combine(fileDirectory,fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var physicalProvider = new PhysicalFileProvider(fileDirectory);
            var downloadFile = physicalProvider.GetFileInfo(fileName);
            return PhysicalFile(downloadFile.PhysicalPath,MediaTypeNames.Application.Octet,fileName);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating,FormFile")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                //string uniqueFileName = null;
                if(movie.FormFile != null)
                {
                    var unsafeFileName = movie.FormFile.FileName;
                    var fileName = Path.GetFileNameWithoutExtension(unsafeFileName);
                    var fileType = Path.GetExtension(unsafeFileName);
                    movie.FileName = fileName;
                    movie.FileType = fileType;

                    if (!System.IO.Directory.Exists("Files/Test")) {
                        Directory.CreateDirectory("Files/Test");
                    }

                    var filePath = Path.Combine(_env.ContentRootPath,"Files/Test",fileName+fileType);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await movie.FormFile.CopyToAsync(stream);
                    }
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
