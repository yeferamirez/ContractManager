using AutoFixture;
using Bogus;
using ContractManager.Shared.Api.Controllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ContractManager.Api.Tests;

public class BaseApiTest
{
    protected Fixture Fixture { get; set; } = new Fixture();

    protected Faker Faker { get; set; } = new Faker();

    public BaseApiTest()
    {
        SetupFixture();
    }

    private void SetupFixture()
    {
        Fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public ClaimsPrincipal GetPrincipal()
    {
        var claims = new List<Claim>
        {
            new Claim("id", "123"),
            new Claim("name", "GC"),
            new Claim(ClaimTypes.Email, "email@email.com"),
            new Claim("permissions", "ContractCreate")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        return new ClaimsPrincipal(identity);
    }

    public void LoadPrincipal(BaseApiController controller)
    {
        controller.ControllerContext.HttpContext = new DefaultHttpContext
        {
            User = GetPrincipal()
        };
    }
}
