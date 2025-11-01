using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;


namespace ContractManager.Api.Tests.Autofixture;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = GetFixture();

            return fixture.Customize(new AutoMoqCustomization());
        })
    {
    }

    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        fixture.Customize<Fixture>(composer => composer.FromFactory(() => fixture));

        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }

    public AutoMoqDataAttribute(Type typeCustomization)
        : base(() =>
        {
            var fixture = GetFixture();

            var customization = (ICustomization?)Activator.CreateInstance(typeCustomization);
            if (customization == null)
            {
                throw new ArgumentNullException(nameof(customization));
            }

            fixture.Customize(customization);


            return fixture.Customize(new AutoMoqCustomization());
        })
    {
    }
}
