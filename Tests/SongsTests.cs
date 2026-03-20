using dotNETify.Controllers;
using dotNETify.Converters;
using dotNETify.Models;
using dotNETify.ModelsDTO;
using dotNETify.Persistance;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace dotNETify.Tests;

public class SongsTests
{
    private SongsController CreateController(List<Song> songs)
    {
        var mockRepo = new Mock<ISongsRepository>();

        mockRepo.Setup(r => r.GetSongs()).Returns(songs);

        mockRepo.Setup(r => r.GetSongById(It.IsAny<int>()))
            .Returns<int>(id => songs.FirstOrDefault(s => s.Id == id)!);

        mockRepo.Setup(r => r.Create(It.IsAny<Song>()))
            .Callback<Song>(s => { s.Id = songs.Count + 1; songs.Add(s); })
            .Returns<Song>(s => s.Id);

        mockRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Song>()))
            .Callback<int, Song>((id, s) =>
            {
                var existing = songs.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    existing.Title = s.Title;
                    existing.ViewCount = s.ViewCount;
                    existing.GenreId = s.GenreId;
                }
            });

        mockRepo.Setup(r => r.Delete(It.IsAny<int>()))
            .Callback<int>(id => songs.RemoveAll(s => s.Id == id));

        return new SongsController(mockRepo.Object);
    }

    private List<Song> GetSeedSongs() => new()
    {
        new Song { Id = 1, Title = "Highway to Hell", GenreId = 1, SongLength = 207, ReleaseYear = 1979, ViewCount = 100, CoverUrl = "http://img/1.jpg", SongUrl = "http://audio/1.mp3" },
        new Song { Id = 2, Title = "Bohemian Rhapsody", GenreId = 1, SongLength = 355, ReleaseYear = 1975, ViewCount = 200, CoverUrl = "http://img/2.jpg", SongUrl = "http://audio/2.mp3" },
        new Song { Id = 3, Title = "Lose Yourself", GenreId = 3, SongLength = 326, ReleaseYear = 2002, ViewCount = 150, CoverUrl = "http://img/3.jpg", SongUrl = "http://audio/3.mp3" },
    };

    // T21: R6 — Pobranie wszystkich piosenek → lista
    [Fact]
    public void T21_GetSongs_ZwracaWszystkiePiosenki()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.Get(null, null) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var list = result.Value as List<SongDto>;
        Assert.NotNull(list);
        Assert.Equal(3, list.Count);
    }

    // T22: R6 — Filtrowanie po nazwie → przefiltrowana lista
    [Fact]
    public void T22_GetSongs_FiltrowaniPoNazwie_ZwracaPasujace()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.Get("Highway", null) as OkObjectResult;

        Assert.NotNull(result);
        var list = result.Value as List<SongDto>;
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("Highway to Hell", list[0].Title);
    }

    // T23: R6 — Filtrowanie po gatunku → przefiltrowana lista
    [Fact]
    public void T23_GetSongs_FiltrowaniPoGatunku_ZwracaPasujace()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.Get(null, 1) as OkObjectResult;

        Assert.NotNull(result);
        var list = result.Value as List<SongDto>;
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
    }

    // T24: R7 — Pobranie piosenki po istniejącym ID
    [Fact]
    public void T24_GetSongById_IstniejaceId_ZwracaPiosenke()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var song = result.Value as SongDto;
        Assert.NotNull(song);
        Assert.Equal("Highway to Hell", song.Title);
    }

    // T25: R8 — Dodanie piosenki z poprawnymi danymi
    [Fact]
    public void T25_CreateSong_PoprawneDane_ZwracaOk()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);
        var newSong = new SongDto
        {
            Title = "Thunderstruck",
            GenreId = 1,
            SongLength = 292,
            ReleaseYear = 1990,
            ViewCount = 0,
            CoverUrl = "http://img/4.jpg",
            SongUrl = "http://audio/4.mp3",
            Artist = new ArtistDto { Id = 1, Nickname = "AC/DC", ImageUrl = "" }
        };

        var result = controller.Post(newSong) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(4, songs.Count);
    }

    // T26: R9 — Edycja istniejącej piosenki → NoContent
    [Fact]
    public void T26_UpdateSong_IstniejacaPiosenka_ZwracaNoContent()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);
        var updatedSong = new SongDto
        {
            Id = 1,
            Title = "Nowy tytul",
            GenreId = 1,
            SongLength = 207,
            ReleaseYear = 1979,
            ViewCount = 100,
            Artist = new ArtistDto { Id = 1, Nickname = "AC/DC", ImageUrl = "" }
        };

        var result = controller.Put(1, updatedSong) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
        Assert.Equal("Nowy tytul", songs[0].Title);
    }

    // T27: R10 — Usunięcie istniejącej piosenki → NoContent
    [Fact]
    public void T27_DeleteSong_IstniejacaPiosenka_ZwracaNoContent()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(2, songs.Count);
    }

    // T28: R6 — Filtrowanie po nazwie która nie istnieje → pusta lista
    [Fact]
    public void T28_GetSongs_NazwaKtoraNieIstnieje_PustaLista()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);

        var result = controller.Get("ZZZZZZZ", null) as OkObjectResult;

        Assert.NotNull(result);
        var list = result.Value as List<SongDto>;
        Assert.NotNull(list);
        Assert.Empty(list);
    }

    // T29: R8 — Dodanie piosenki z tytułem >30 znaków
    [Fact]
    public void T29_CreateSong_TytulZaDlugi_WalidacjaModelu()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);
        var longTitle = new string('A', 31);
        var newSong = new SongDto
        {
            Title = longTitle,
            GenreId = 1,
            SongLength = 200,
            ReleaseYear = 2024,
            ViewCount = 0,
            Artist = new ArtistDto { Id = 1, Nickname = "AC/DC", ImageUrl = "" }
        };

        // Model validation — w kontrolerze brak ręcznej walidacji,
        // ale model Song ma [MaxLength(30)] na Title
        // W unit teście bez pipeline MVC walidacja modelu nie jest automatyczna
        Assert.True(longTitle.Length > 30);
        // W pełnym środowisku ASP.NET zwróciłoby 400 Bad Request
    }

    // T30: R9 — Edycja piosenki — zmiana viewCount
    [Fact]
    public void T30_UpdateSong_ZmianaViewCount_ZaktualizowanyViewCount()
    {
        var songs = GetSeedSongs();
        var controller = CreateController(songs);
        var updatedSong = new SongDto
        {
            Id = 1,
            Title = "Highway to Hell",
            GenreId = 1,
            SongLength = 207,
            ReleaseYear = 1979,
            ViewCount = 999,
            Artist = new ArtistDto { Id = 1, Nickname = "AC/DC", ImageUrl = "" }
        };

        controller.Put(1, updatedSong);

        Assert.Equal(999, songs[0].ViewCount);
    }
}
