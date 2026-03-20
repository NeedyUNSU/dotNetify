using dotNETify.Data;
using dotNETify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace dotNETify.Tests;

public static class TestHelper
{
    public static AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static AppDbContext CreateSeededContext(string dbName)
    {
        var context = CreateInMemoryContext(dbName);

        // Seed genres
        context.Genres.AddRange(
            new Genre { Id = 1, GenreName = "Rock" },
            new Genre { Id = 2, GenreName = "Pop" },
            new Genre { Id = 3, GenreName = "Rap" }
        );

        // Seed artists
        context.Artists.AddRange(
            new Artist { Id = 1, Nickname = "AC/DC", ImageUrl = "http://img.com/acdc.jpg" },
            new Artist { Id = 2, Nickname = "Queen", ImageUrl = "http://img.com/queen.jpg" },
            new Artist { Id = 3, Nickname = "Eminem", ImageUrl = "http://img.com/eminem.jpg" }
        );

        // Seed songs
        context.Songs.AddRange(
            new Song { Id = 1, Title = "Highway to Hell", GenreId = 1, SongLength = 207, ReleaseYear = 1979, ViewCount = 100, CoverUrl = "http://img.com/hth.jpg", SongUrl = "http://audio.com/hth.mp3" },
            new Song { Id = 2, Title = "Bohemian Rhapsody", GenreId = 1, SongLength = 355, ReleaseYear = 1975, ViewCount = 200, CoverUrl = "http://img.com/br.jpg", SongUrl = "http://audio.com/br.mp3" },
            new Song { Id = 3, Title = "Lose Yourself", GenreId = 3, SongLength = 326, ReleaseYear = 2002, ViewCount = 150, CoverUrl = "http://img.com/ly.jpg", SongUrl = "http://audio.com/ly.mp3" }
        );

        // Seed song-artist relationships
        context.SongArtists.AddRange(
            new SongArtist { Id = 1, SongId = 1, ArtistId = 1 },
            new SongArtist { Id = 2, SongId = 2, ArtistId = 2 },
            new SongArtist { Id = 3, SongId = 3, ArtistId = 3 }
        );

        context.SaveChanges();
        return context;
    }

    public static (UserManager<User>, SignInManager<User>) CreateUserManager(AppDbContext context)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(context.Database.GetConnectionString() ?? "test"));
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddLogging();

        var provider = services.BuildServiceProvider();
        var userManager = provider.GetRequiredService<UserManager<User>>();
        var signInManager = provider.GetRequiredService<SignInManager<User>>();
        return (userManager, signInManager);
    }
}
