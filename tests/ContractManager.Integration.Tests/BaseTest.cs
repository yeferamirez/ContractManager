using AutoFixture;
using Bogus;

namespace ContractManager.Integration.Tests;
public class BaseTest
{
    protected Fixture Fixture { get; set; } = new Fixture();

    protected Faker Faker { get; set; } = new Faker();

    public BaseTest()
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
}
