using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;

namespace SteroidsDI.Tests;

internal class ServicesBuilder
{
    public static IServiceCollection BuildDefault(bool addScopeProvider = true, bool addDefalt = false)
    {
        var services = new ServiceCollection()
            .AddDefer()
            .AddMicrosoftScopeFactory()
            .Configure<ServiceProviderAdvancedOptions>(opt => opt.AllowRootProviderResolve = true)

            .AddScoped<ScopedService>().AddFunc<ScopedService>()
            .AddTransient<TransientService>().AddFunc<TransientService>()

            .AddSingleton<Controller>()
            .AddFactory<IMegaFactory>()
            .AddFactory<IGenericFactory<IBuilder, INotifier>>()
            .AddTransient<IBuilder, Builder>()
            .AddSingleton<INotifier, Notifier>();

        var context = services.For<IBuilder>()
            .Named<SpecialBuilder>("xxx")
            .Named<SpecialBuilder>("yyy")
            .Named<SpecialBuilderOver9000Level>("oops", ServiceLifetime.Singleton)
            .Named<SpecialBuilder>(ManagerType.Good)
            .Named<SpecialBuilderOver9000Level>(ManagerType.Bad, ServiceLifetime.Singleton);

        if (addDefalt)
            context.Default<DefaultBuilder>(ServiceLifetime.Singleton);

        if (addScopeProvider)
            services.AddSingleton<IScopeProvider, GenericScopeProvider<ServicesBuilder>>();

        return services;
    }
}
