using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvcMovie.Models;

namespace MvcMovie.Data
{
    public class MvcMovieContext : DbContext
    {
        //如果配置LoggerFactory，则配置在program中的日志将失效，DbContext日志将使用Microsoft.Extensions.Logging;
        /*
        public static readonly ILoggerFactory myLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information)
                .AddConsole();
            });
        */
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<User> User { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLoggerFactory(myLoggerFactory);
        }
        */
    }
}