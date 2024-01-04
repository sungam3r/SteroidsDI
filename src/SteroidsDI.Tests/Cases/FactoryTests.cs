using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

[TestFixture]
[Category("Factory")]
public class FactoryTests
{
    [Test]
    [Category("Throw")]
    public void Named_Binding_Should_Throw_On_Unknown_Lifetime()
    {
        var services = new ServiceCollection();
        var context = services.For<IBuilder>();
        Should.Throw<InvalidOperationException>(() => context.Named<SpecialBuilder>("xxx")).Message.ShouldBe(@"The DI container does not register type 'SteroidsDI.Tests.IBuilder', so it is not possible to determine the value of Lifetime.
Use the 'Named'/'Default' overloads with explicit Lifetime or first register 'SteroidsDI.Tests.IBuilder' in the DI container.");
        Should.Throw<InvalidOperationException>(() => context.Default<SpecialBuilder>()).Message.ShouldBe(@"The DI container does not register type 'SteroidsDI.Tests.IBuilder', so it is not possible to determine the value of Lifetime.
Use the 'Named'/'Default' overloads with explicit Lifetime or first register 'SteroidsDI.Tests.IBuilder' in the DI container.");
    }

    [Test]
    public void Named_Binding_Should_Allow_The_Same_Type_With_Different_Names()
    {
        var services = new ServiceCollection()
            .AddTransient<IBuilder, Builder>()
            .For<IBuilder>()
                .Named<SpecialBuilder>("aaa")
                .Named<SpecialBuilder>("bbb")
                .Named<SpecialBuilder>("ccc")
                .Named<SpecialBuilder>("ddd")
                .Named<SpecialBuilder>("eee")
                .Services;

        services.Count.ShouldBe(7);
    }

    [Test]
    public void Default_Binding_Should_Allow_Redeclaration()
    {
        var services = new ServiceCollection()
            .AddTransient<IBuilder, Builder>()
            .For<IBuilder>()
                .Default<SpecialBuilder>()
                .Default<SpecialBuilder>()
            .Services;
        services.Count.ShouldBe(3);
        var def = (NamedBinding)services.Last().ImplementationInstance!;
        def.Name.ShouldBe(NamedBinding.DefaultName);
        def.ImplementationType.ShouldBe(typeof(SpecialBuilder));
    }

    [Test]
    public void Default_Binding_Should_Allow_Replace()
    {
        var services = new ServiceCollection()
            .AddTransient<IBuilder, Builder>()
            .For<IBuilder>()
                .Default<SpecialBuilder>()
                .Default<SpecialBuilderOver9000Level>()
            .Services;
        services.Count.ShouldBe(4);
        var def = (NamedBinding)services[2].ImplementationInstance!;
        def.Name.ShouldBe(NamedBinding.DefaultName);
        def.ImplementationType.ShouldBe(typeof(SpecialBuilderOver9000Level));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Factory_And_Named_Bindings_Should_Work(bool useDefault)
    {
        var services = ServicesBuilder.BuildDefault(addDefalt: useDefault);

        // not specific to this test - just add some additional registration for another type and place it in front of other registrations to increase code coverage
        services.For<IComparable>().Named<string>("some", ServiceLifetime.Singleton);
        services.Insert(0, services.Last());
        services.RemoveAt(services.Count - 1);

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        GenericScope<ServicesBuilder>.CurrentScope = scope;

        var controller = scope.ServiceProvider.GetRequiredService<Controller>();

        var builder1 = controller.NonGenericFactory.AAA();
        builder1.ShouldBeAssignableTo<Builder>();
        builder1.Build();

        var notifier = controller.NonGenericFactory.BBB();
        notifier.ShouldBeAssignableTo<Notifier>();
        notifier.Notify();

        var builder2 = controller.NonGenericFactory.CCC("xxx");
        builder2.ShouldBeAssignableTo<SpecialBuilder>();
        builder2.Build();

        var builder3 = controller.NonGenericFactory.CCC("yyy");
        builder3.ShouldBeAssignableTo<SpecialBuilder>();
        builder3.Build();

        var builder4 = controller.NonGenericFactory.CCC("oops");
        builder4.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
        builder4.Build();

        var builder5 = controller.NonGenericFactory.DDD(ManagerType.Good);
        builder5.ShouldBeAssignableTo<SpecialBuilder>();
        builder5.Build();

        var builder6 = controller.NonGenericFactory.DDD(ManagerType.Bad);
        builder6.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
        builder6.Build();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [Category("Generic")]
    public void Generic_Factory_And_Named_Bindings_Should_Work(bool useDefault)
    {
        using var provider = ServicesBuilder.BuildDefault(addDefalt: useDefault).BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        GenericScope<ServicesBuilder>.CurrentScope = scope;

        var controller = scope.ServiceProvider.GetRequiredService<Controller>();

        var builder1 = controller.GenericFactory.AAA();
        builder1.ShouldBeAssignableTo<Builder>();
        builder1.Build();

        var notifier = controller.GenericFactory.BBB();
        notifier.ShouldBeAssignableTo<Notifier>();
        notifier.Notify();

        var builder2 = controller.GenericFactory.CCC("xxx");
        builder2.ShouldBeAssignableTo<SpecialBuilder>();
        builder2.Build();

        var builder3 = controller.GenericFactory.CCC("yyy");
        builder3.ShouldBeAssignableTo<SpecialBuilder>();
        builder3.Build();

        var builder4 = controller.GenericFactory.CCC("oops");
        builder4.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
        builder4.Build();

        var builder5 = controller.GenericFactory.DDD(ManagerType.Good);
        builder5.ShouldBeAssignableTo<SpecialBuilder>();
        builder5.Build();

        var builder6 = controller.GenericFactory.DDD(ManagerType.Bad);
        builder6.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
        builder6.Build();
    }

    [Test]
    [Category("Throw")]
    public void Null_Binding_Should_Throw_When_No_Default()
    {
        using var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true);
        var controller = provider.GetRequiredService<Controller>();

        string msg1 = Should.Throw<InvalidOperationException>(() => controller.NonGenericFactory.CCC(null!)).Message;
        msg1.ShouldBe("Destination type not found for named binding '' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");
        string msg2 = Should.Throw<InvalidOperationException>(() => controller.GenericFactory.CCC(null!)).Message;
        msg2.ShouldBe("Destination type not found for named binding '' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");
    }

    [Test]
    public void Null_Binding_Should_Work_When_Default()
    {
        using var provider = ServicesBuilder.BuildDefault(addDefalt: true).BuildServiceProvider(validateScopes: true);
        var controller = provider.GetRequiredService<Controller>();

        var builder1 = controller.NonGenericFactory.CCC(null!);
        builder1.ShouldBeAssignableTo<DefaultBuilder>();
        builder1.Build();

        var builder2 = controller.GenericFactory.CCC(null!);
        builder2.ShouldBeAssignableTo<DefaultBuilder>();
        builder2.Build();
    }

    [Test]
    public void Unknown_Binding_Should_Throw_When_No_Default()
    {
        using var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true);
        var controller = provider.GetRequiredService<Controller>();

        string msg1 = Should.Throw<InvalidOperationException>(() => controller.NonGenericFactory.CCC("Zorro")).Message;
        msg1.ShouldBe("Destination type not found for named binding 'Zorro' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");
        string msg2 = Should.Throw<InvalidOperationException>(() => controller.GenericFactory.CCC("Zorro")).Message;
        msg2.ShouldBe("Destination type not found for named binding 'Zorro' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");

        string msg3 = Should.Throw<InvalidOperationException>(() => controller.NonGenericFactory.DDD(ManagerType.Angry)).Message;
        msg3.ShouldBe("Destination type not found for named binding 'Angry' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");
        string msg4 = Should.Throw<InvalidOperationException>(() => controller.GenericFactory.DDD(ManagerType.Angry)).Message;
        msg4.ShouldBe("Destination type not found for named binding 'Angry' to type 'SteroidsDI.Tests.IBuilder' and no default binding exists. Verify that either a named binding or default binding is specified in the DI container.");
    }

    [Test]
    [Category("Throw")]
    public void Unknown_Binding_Should_Work_When_Default()
    {
        using var provider = ServicesBuilder.BuildDefault(addDefalt: true).BuildServiceProvider(validateScopes: true);
        var controller = provider.GetRequiredService<Controller>();

        var builder1 = controller.NonGenericFactory.CCC("Zorro");
        builder1.ShouldBeAssignableTo<DefaultBuilder>();
        builder1.Build();

        var builder2 = controller.GenericFactory.CCC("Zorro");
        builder2.ShouldBeAssignableTo<DefaultBuilder>();
        builder2.Build();

        var builder3 = controller.NonGenericFactory.DDD(ManagerType.Angry);
        builder3.ShouldBeAssignableTo<DefaultBuilder>();
        builder3.Build();

        var builder4 = controller.GenericFactory.DDD(ManagerType.Angry);
        builder4.ShouldBeAssignableTo<DefaultBuilder>();
        builder4.Build();
    }

    [Test]
    [Category("Throw")]
    public void Named_Binding_With_Invalid_Properties_Should_Throw()
    {
        string msg1 = Should.Throw<ArgumentNullException>(() => ServicesBuilder.BuildDefault().For<IBuilder>().Named<SpecialBuilder>(null!).Services.BuildServiceProvider(validateScopes: true)).Message;
        msg1.ShouldBe("No binding name specified. (Parameter 'name')");
        string msg2 = Should.Throw<InvalidOperationException>(() => ServicesBuilder.BuildDefault().For<IBuilder>().Named<SpecialBuilderOver9000Level>("oops", ServiceLifetime.Transient).Services.BuildServiceProvider(validateScopes: true)).Message;
        msg2.ShouldBe(@"It is not possible to add a named binding 'oops' for type SteroidsDI.Tests.IBuilder, because the DI container
already has a binding on type SteroidsDI.Tests.SpecialBuilderOver9000Level with different characteristics. This is a limitation of the current implementation.");
    }

    [Test]
    [Category("Throw")]
    public void Default_Binding_With_Invalid_Properties_Should_Throw()
    {
        string msg = Should.Throw<InvalidOperationException>(() => ServicesBuilder.BuildDefault(addDefalt: true).For<IBuilder>().Default<DefaultBuilder>(ServiceLifetime.Transient).Services.BuildServiceProvider(validateScopes: true)).Message;
        msg.ShouldBe(@"It is not possible to add a default binding for type SteroidsDI.Tests.IBuilder, because the DI container
already has a binding on type SteroidsDI.Tests.DefaultBuilder with different characteristics. This is a limitation of the current implementation.");
    }

    [Test]
    [Category("Throw")]
    public void Null_Factory_Type_Should_Throw()
    {
        Should.Throw<ArgumentNullException>(() => new ServiceCollection().AddFactory(null!));
        Should.Throw<ArgumentNullException>(() => FactoryGenerator.Generate(null!));
    }

    [Test]
    [Category("Throw")]
    [TestCase(typeof(int))]
    [TestCase(typeof(IFactoryWithEvent))]
    [TestCase(typeof(IFactoryWithProperty))]
    [TestCase(typeof(IFactoryWithMethodWithManyArgs))]
    public void Wrong_Factory_Type_Should_Throw(Type factoryType) => Should.Throw<InvalidOperationException>(() => new ServiceCollection().AddFactory(factoryType));
}
