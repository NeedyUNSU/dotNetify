using dotNETify.Controllers;
using dotNETify.Models;
using dotNETify.ModelsDTO;
using dotNETify.Persistance;
using dotNETIFY.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace dotNETify.Tests;

public class LikesHistoryTests
{
    // T61: R16 — Polubienie istniejącej piosenki → Created
    [Fact]
    public async Task T61_LikeSong_NowyPolubienie_ZwracaCreated()
    {
        var mockRepo = new Mock<ILikedSongsRepository>();
        mockRepo.Setup(r => r.ExistsAsync("user1", 1)).ReturnsAsync(false);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Liked_songs>()))
            .ReturnsAsync((Liked_songs ls) => { ls.Id = 1; return ls; });

        var controller = new LikedSongsController(mockRepo.Object);
        var dto = new LikedSongDto { UserId = "user1", SongId = 1 };

        var result = await controller.Create(dto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    // T62: R16 — Polubienie już polubionej piosenki → Conflict
    [Fact]
    public async Task T62_LikeSong_JuzPolubiona_ZwracaConflict()
    {
        var mockRepo = new Mock<ILikedSongsRepository>();
        mockRepo.Setup(r => r.ExistsAsync("user1", 1)).ReturnsAsync(true);

        var controller = new LikedSongsController(mockRepo.Object);
        var dto = new LikedSongDto { UserId = "user1", SongId = 1 };

        var result = await controller.Create(dto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    // T63: R17 — Usunięcie polubienia piosenki → NoContent
    [Fact]
    public async Task T63_DeleteLikedSong_Istnieje_ZwracaNoContent()
    {
        var mockRepo = new Mock<ILikedSongsRepository>();
        mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var controller = new LikedSongsController(mockRepo.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    // T64: R17 — Usunięcie nieistniejącego polubienia → NotFound
    [Fact]
    public async Task T64_DeleteLikedSong_NieIstnieje_ZwracaNotFound()
    {
        var mockRepo = new Mock<ILikedSongsRepository>();
        mockRepo.Setup(r => r.DeleteAsync(9999)).ReturnsAsync(false);

        var controller = new LikedSongsController(mockRepo.Object);

        var result = await controller.Delete(9999);

        Assert.IsType<NotFoundResult>(result);
    }

    // T65: R18 — Polubienie istniejącego artysty → Created
    [Fact]
    public async Task T65_LikeArtist_NowePolubienie_ZwracaCreated()
    {
        var mockRepo = new Mock<ILikedArtistsRepository>();
        mockRepo.Setup(r => r.ExistsAsync("user1", 1)).ReturnsAsync(false);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Liked_artists>()))
            .ReturnsAsync((Liked_artists la) => { la.Id = 1; return la; });

        var controller = new LikedArtistsController(mockRepo.Object);
        var dto = new LikedArtistDto { UserId = "user1", ArtistId = 1 };

        var result = await controller.Create(dto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    // T66: R18 — Polubienie już polubionego artysty → Conflict
    [Fact]
    public async Task T66_LikeArtist_JuzPolubiony_ZwracaConflict()
    {
        var mockRepo = new Mock<ILikedArtistsRepository>();
        mockRepo.Setup(r => r.ExistsAsync("user1", 1)).ReturnsAsync(true);

        var controller = new LikedArtistsController(mockRepo.Object);
        var dto = new LikedArtistDto { UserId = "user1", ArtistId = 1 };

        var result = await controller.Create(dto);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    // T67: R19 — Usunięcie polubienia artysty → NoContent
    [Fact]
    public async Task T67_DeleteLikedArtist_Istnieje_ZwracaNoContent()
    {
        var mockRepo = new Mock<ILikedArtistsRepository>();
        mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var controller = new LikedArtistsController(mockRepo.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    // T68: R19 — Usunięcie nieistniejącego polubienia artysty → NotFound
    [Fact]
    public async Task T68_DeleteLikedArtist_NieIstnieje_ZwracaNotFound()
    {
        var mockRepo = new Mock<ILikedArtistsRepository>();
        mockRepo.Setup(r => r.DeleteAsync(9999)).ReturnsAsync(false);

        var controller = new LikedArtistsController(mockRepo.Object);

        var result = await controller.Delete(9999);

        Assert.IsType<NotFoundResult>(result);
    }

    // T69: R20 — Dodanie piosenki do historii → Created
    [Fact]
    public void T69_AddToHistory_PoprawneDane_ZwracaCreated()
    {
        var context = TestHelper.CreateSeededContext(nameof(T69_AddToHistory_PoprawneDane_ZwracaCreated));
        // Need to add a user
        context.Users.Add(new User { Id = "user1", UserName = "test@test.com", Email = "test@test.com" });
        context.SaveChanges();

        var controller = new UserHistoryController(context);
        var dto = new UserHistoryDto { SongId = 1, UserId = "user1" };

        var result = controller.Post(dto) as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
    }

    // T70: R20 — Pobranie historii użytkownika → lista
    [Fact]
    public void T70_GetHistory_IstniejacyUser_ZwracaHistorie()
    {
        var context = TestHelper.CreateSeededContext(nameof(T70_GetHistory_IstniejacyUser_ZwracaHistorie));
        context.Users.Add(new User { Id = "user1", UserName = "test@test.com", Email = "test@test.com" });
        context.UserHistories.Add(new UserHistory { Id = 1, UserId = "user1", SongId = 1, Timestamp = DateTime.UtcNow });
        context.SaveChanges();

        var controller = new UserHistoryController(context);

        var result = controller.GetByUser("user1") as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}
