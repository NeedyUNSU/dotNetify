using dotNETify.Controllers;
using dotNETify.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace dotNETify.Tests;

public class ArtistTests
{
    private ArtistController CreateController(string dbName)
    {
        var context = TestHelper.CreateInMemoryContext(dbName);
        return new ArtistController(context);
    }

    private ArtistController CreateSeededController(string dbName)
    {
        var context = TestHelper.CreateSeededContext(dbName);
        return new ArtistController(context);
    }

    // T41: R11 — Pobranie wszystkich artystów → lista
    [Fact]
    public void T41_GetArtists_ZwracaListeArtystow()
    {
        var controller = CreateSeededController(nameof(T41_GetArtists_ZwracaListeArtystow));

        var result = controller.Get() as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var list = result.Value as System.Collections.IList;
        Assert.NotNull(list);
        Assert.Equal(3, list.Count);
    }

    // T42: R12 — Pobranie artysty po istniejącym ID
    [Fact]
    public void T42_GetArtistById_IstniejaceId_ZwracaArtyste()
    {
        var controller = CreateSeededController(nameof(T42_GetArtistById_IstniejaceId_ZwracaArtyste));

        var result = controller.GetById(1) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    // T43: R13 — Dodanie artysty z poprawnymi danymi → Created
    [Fact]
    public void T43_CreateArtist_PoprawneDane_ZwracaCreated()
    {
        var controller = CreateController(nameof(T43_CreateArtist_PoprawneDane_ZwracaCreated));
        var artist = new Artist { Nickname = "Metallica", ImageUrl = "http://img.com/metallica.jpg" };

        var result = controller.Post(artist) as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
    }

    // T44: R14 — Edycja istniejącego artysty → NoContent
    [Fact]
    public void T44_UpdateArtist_IstniejacyArtysta_ZwracaNoContent()
    {
        var controller = CreateSeededController(nameof(T44_UpdateArtist_IstniejacyArtysta_ZwracaNoContent));
        var updatedArtist = new Artist { Nickname = "AC/DC Updated", ImageUrl = "http://img.com/new.jpg" };

        var result = controller.Put(1, updatedArtist) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }

    // T45: R15 — Usunięcie istniejącego artysty → NoContent
    [Fact]
    public void T45_DeleteArtist_IstniejacyArtysta_ZwracaNoContent()
    {
        var context = TestHelper.CreateSeededContext(nameof(T45_DeleteArtist_IstniejacyArtysta_ZwracaNoContent));
        var songArtists = context.SongArtists.Where(sa => sa.ArtistId == 1).ToList();
        context.SongArtists.RemoveRange(songArtists);
        context.SaveChanges();

        var controller = new ArtistController(context);
        var result = controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    // T46: R12 — Pobranie artysty po nieistniejącym ID → NullReferenceException (bug w kodzie)
    [Fact]
    public void T46_GetArtistById_NieistniejaceId_RzucaWyjatek()
    {
        var controller = CreateSeededController(nameof(T46_GetArtistById_NieistniejaceId_RzucaWyjatek));

        Assert.Throws<NullReferenceException>(() => controller.GetById(9999));
    }

    // T47: R14 — Edycja nieistniejącego artysty → NotFound
    [Fact]
    public void T47_UpdateArtist_NieistniejacyArtysta_ZwracaNotFound()
    {
        var controller = CreateSeededController(nameof(T47_UpdateArtist_NieistniejacyArtysta_ZwracaNotFound));
        var updatedArtist = new Artist { Nickname = "Ghost", ImageUrl = "" };

        var result = controller.Put(9999, updatedArtist) as NotFoundResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    // T48: R15 — Usunięcie nieistniejącego artysty → NotFound
    [Fact]
    public void T48_DeleteArtist_NieistniejacyArtysta_ZwracaNotFound()
    {
        var controller = CreateSeededController(nameof(T48_DeleteArtist_NieistniejacyArtysta_ZwracaNotFound));

        var result = controller.Delete(9999) as NotFoundResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    // T49: R13 — Dodanie artysty bez imageUrl → sukces
    [Fact]
    public void T49_CreateArtist_BezImageUrl_ZwracaCreated()
    {
        var controller = CreateController(nameof(T49_CreateArtist_BezImageUrl_ZwracaCreated));
        var artist = new Artist { Nickname = "NoImage" };

        var result = controller.Post(artist) as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
    }

    // T50: R11 — Pobranie artystów z pustej bazy → pusta lista
    [Fact]
    public void T50_GetArtists_PustaBaza_PustaLista()
    {
        var controller = CreateController(nameof(T50_GetArtists_PustaBaza_PustaLista));

        var result = controller.Get() as OkObjectResult;

        Assert.NotNull(result);
        var list = result.Value as System.Collections.IList;
        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
