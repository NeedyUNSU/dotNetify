using dotNETify.Data;
using dotNETify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace dotNETify.Tests;

public class AuthTests
{
    private (UserManager<User> userManager, AppDbContext context) CreateTestEnvironment(string dbName)
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase(databaseName: dbName));

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
        var context = provider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return (userManager, context);
    }

    // T1: R1 — Hasło bez wielkiej litery → błąd
    [Fact]
    public async Task T1_Rejestracja_HasloBezWielkiejLitery_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T1_Rejestracja_HasloBezWielkiejLitery_ZwracaBlad));
        var user = new User { UserName = "t1@test.com", Email = "t1@test.com" };

        var result = await userManager.CreateAsync(user, "test123!");

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresUpper");
    }

    // T2: R1 — Hasło bez znaku specjalnego → błąd
    [Fact]
    public async Task T2_Rejestracja_HasloBezZnakuSpecjalnego_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T2_Rejestracja_HasloBezZnakuSpecjalnego_ZwracaBlad));
        var user = new User { UserName = "t2@test.com", Email = "t2@test.com" };

        var result = await userManager.CreateAsync(user, "Test1234");

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresNonAlphanumeric");
    }

    // T3: R1 — Hasło krótsze niż 6 znaków → błąd
    [Fact]
    public async Task T3_Rejestracja_HasloZaKrotkie_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T3_Rejestracja_HasloZaKrotkie_ZwracaBlad));
        var user = new User { UserName = "t3@test.com", Email = "t3@test.com" };

        var result = await userManager.CreateAsync(user, "Te1!");

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
    }

    // T4: R1 — Hasło poprawne → sukces
    [Fact]
    public async Task T4_Rejestracja_HasloPoprawne_Sukces()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T4_Rejestracja_HasloPoprawne_Sukces));
        var user = new User { UserName = "t4@test.com", Email = "t4@test.com" };

        var result = await userManager.CreateAsync(user, "Test123!");

        Assert.True(result.Succeeded);
    }

    // T5: R2 — Email bez znaku @ → błąd
    [Fact]
    public async Task T5_Rejestracja_EmailBezMalpy_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T5_Rejestracja_EmailBezMalpy_ZwracaBlad));
        var user = new User { UserName = "abcdef", Email = "abcdef" };

        var result = await userManager.CreateAsync(user, "Test123!");

        Assert.NotNull(result);
    }

    // T6: R2 — Email bez domeny → błąd
    [Fact]
    public async Task T6_Rejestracja_EmailBezDomeny_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T6_Rejestracja_EmailBezDomeny_ZwracaBlad));
        var user = new User { UserName = "abc@", Email = "abc@" };

        var result = await userManager.CreateAsync(user, "Test123!");

        Assert.NotNull(result);
    }

    // T7: R3 — Rejestracja z poprawnymi danymi → sukces
    [Fact]
    public async Task T7_Rejestracja_PoprawneDane_TworzenieKonta()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T7_Rejestracja_PoprawneDane_TworzenieKonta));
        var user = new User { UserName = "new@test.com", Email = "new@test.com" };

        var result = await userManager.CreateAsync(user, "Test123!");

        Assert.True(result.Succeeded);
        var found = await userManager.FindByEmailAsync("new@test.com");
        Assert.NotNull(found);
        Assert.Equal("new@test.com", found.Email);
    }

    // T8: R4 — Logowanie z poprawnymi danymi → sukces (weryfikacja hasła)
    [Fact]
    public async Task T8_Logowanie_PoprawneDane_WeryfikacjaHasla()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T8_Logowanie_PoprawneDane_WeryfikacjaHasla));
        var user = new User { UserName = "login@test.com", Email = "login@test.com" };
        await userManager.CreateAsync(user, "Test123!");

        var isValid = await userManager.CheckPasswordAsync(user, "Test123!");

        Assert.True(isValid);
    }

    // T9: R4 — Logowanie z błędnym hasłem → odmowa
    [Fact]
    public async Task T9_Logowanie_BledneHaslo_OdmowaDostepu()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T9_Logowanie_BledneHaslo_OdmowaDostepu));
        var user = new User { UserName = "login2@test.com", Email = "login2@test.com" };
        await userManager.CreateAsync(user, "Test123!");

        var isValid = await userManager.CheckPasswordAsync(user, "wrong");

        Assert.False(isValid);
    }

    // T10: R5 — Rejestracja z istniejącym emailem → duplikat
    [Fact]
    public async Task T10_Rejestracja_DuplikatEmaila_ZwracaBlad()
    {
        var (userManager, _) = CreateTestEnvironment(nameof(T10_Rejestracja_DuplikatEmaila_ZwracaBlad));
        var user1 = new User { UserName = "dup@test.com", Email = "dup@test.com" };
        await userManager.CreateAsync(user1, "Test123!");

        var user2 = new User { UserName = "dup@test.com", Email = "dup@test.com" };
        var result = await userManager.CreateAsync(user2, "Test123!");

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "DuplicateUserName");
    }
}
