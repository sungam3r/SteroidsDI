using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases
{
    [TestFixture]
    [Category("Factory")]
    internal class FactoryTests
    {
        [Test]
        public void Factory_And_Named_Bindings_Should_Work()
        {
            using (var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true))
            {
                using (var scope = provider.CreateScope())
                {
                    GenericScope<ServicesBuilder>.CurrentScope = scope;

                    var controller = scope.ServiceProvider.GetRequiredService<Controller>();

                    var builder1 = controller.Factory.AAA();
                    builder1.ShouldBeAssignableTo<Builder>();
                    builder1.Build();

                    var notifier = controller.Factory.BBB();
                    notifier.ShouldBeAssignableTo<Notifier>();
                    notifier.Notify();

                    var builder2 = controller.Factory.CCC("xxx");
                    builder2.ShouldBeAssignableTo<SpecialBuilder>();
                    builder2.Build();

                    var builder3 = controller.Factory.CCC("yyy");
                    builder3.ShouldBeAssignableTo<SpecialBuilder>();
                    builder3.Build();

                    var builder4 = controller.Factory.CCC("oops");
                    builder4.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
                    builder4.Build();

                    var builder5 = controller.Factory.DDD(ManagerType.Good);
                    builder5.ShouldBeAssignableTo<SpecialBuilder>();
                    builder5.Build();

                    var builder6 = controller.Factory.DDD(ManagerType.Bad);
                    builder6.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
                    builder6.Build();
                }
            }
        }

        [Test]
        [Category("Generic")]
        public void Generic_Factory_And_Named_Bindings_Should_Work()
        {
            using (var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true))
            {
                using (var scope = provider.CreateScope())
                {
                    GenericScope<ServicesBuilder>.CurrentScope = scope;

                    var controller = scope.ServiceProvider.GetRequiredService<Controller>();

                    var builder1 = controller.Generic.AAA();
                    builder1.ShouldBeAssignableTo<Builder>();
                    builder1.Build();

                    var notifier = controller.Generic.BBB();
                    notifier.ShouldBeAssignableTo<Notifier>();
                    notifier.Notify();

                    var builder2 = controller.Generic.CCC("xxx");
                    builder2.ShouldBeAssignableTo<SpecialBuilder>();
                    builder2.Build();

                    var builder3 = controller.Generic.CCC("yyy");
                    builder3.ShouldBeAssignableTo<SpecialBuilder>();
                    builder3.Build();

                    var builder4 = controller.Generic.CCC("oops");
                    builder4.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
                    builder4.Build();

                    var builder5 = controller.Generic.DDD(ManagerType.Good);
                    builder5.ShouldBeAssignableTo<SpecialBuilder>();
                    builder5.Build();

                    var builder6 = controller.Generic.DDD(ManagerType.Bad);
                    builder6.ShouldBeAssignableTo<SpecialBuilderOver9000Level>();
                    builder6.Build();
                }
            }
        }

        [Test]
        [Category("Throw")]
        public void Null_Binding_Should_Throw()
        {
            using (var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true))
            {
                var controller = provider.GetRequiredService<Controller>();

                Should.Throw<InvalidOperationException>(() => controller.Factory.CCC(null!));
                Should.Throw<InvalidOperationException>(() => controller.Generic.CCC(null!));
            }
        }

        [Test]
        [Category("Throw")]
        public void Unknown_Binding_Should_Throw()
        {
            using (var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true))
            {
                var controller = provider.GetRequiredService<Controller>();

                Should.Throw<InvalidOperationException>(() => controller.Factory.CCC("Zorro"));
                Should.Throw<InvalidOperationException>(() => controller.Generic.CCC("Zorro"));

                Should.Throw<InvalidOperationException>(() => controller.Factory.DDD(ManagerType.Angry));
                Should.Throw<InvalidOperationException>(() => controller.Generic.DDD(ManagerType.Angry));
            }
        }

        [Test]
        [Category("Throw")]
        public void Named_Binding_With_Invalid_Properties_Should_Throw()
        {
            Should.Throw<ArgumentNullException>(() => ServicesBuilder.BuildDefault().For<IBuilder>().Named<SpecialBuilder>(null!).Services.BuildServiceProvider(validateScopes: true));
            Should.Throw<InvalidOperationException>(() => ServicesBuilder.BuildDefault().For<IBuilder>().Named<SpecialBuilderOver9000Level>("oops", ServiceLifetime.Transient).Services.BuildServiceProvider(validateScopes: true));
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
}
