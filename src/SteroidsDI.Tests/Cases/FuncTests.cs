using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases
{
    [TestFixture]
    [Category("Func")]
    internal class FuncTests
    {
        [Test]
        public void Func_Should_Work_With_Proper_Create_And_Dispose()
        {
            Controller controller1, controller2, controller3;
            ScopedService service1, service2, service3;
            TransientService transient1, transient2, transient3, transient11, transient22, transient33;

            using (var provider = ServicesBuilder.BuildDefault().BuildServiceProvider(validateScopes: true))
            {
                using (var scope1 = provider.CreateScope())
                {
                    GenericScope<ServicesBuilder>.CurrentScope = scope1;

                    controller1 = scope1.ServiceProvider.GetService<Controller>();
                    service1 = controller1.ScopedFunc();
                    var service11 = controller1.ScopedFunc();
                    service11.ShouldBeSameAs(service1);
                    service1.Disposed.ShouldBeFalse();

                    transient1 = controller1.TransientFunc();
                    transient11 = controller1.TransientFunc();
                    transient11.ShouldNotBeSameAs(transient1);

                    var defer = controller1.ScopedDefer.Value;
                    defer.ShouldBeSameAs(service1);

                    var transientDefer1 = controller1.TransientDefer.Value;
                    var transietransientDefer11 = controller1.TransientDefer.Value;
                    transietransientDefer11.ShouldNotBeSameAs(transientDefer1);
                }

                Console.WriteLine();

                service1.Disposed.ShouldBeTrue();
                transient1.Disposed.ShouldBeTrue();
                transient11.Disposed.ShouldBeTrue();

                using (var scope2 = provider.CreateScope())
                {
                    GenericScope<ServicesBuilder>.CurrentScope = scope2;

                    controller2 = scope2.ServiceProvider.GetService<Controller>();
                    controller2.ShouldBeSameAs(controller1);

                    service2 = controller2.ScopedFunc();
                    var service22 = controller2.ScopedFunc();
                    service22.ShouldBeSameAs(service2);
                    service2.Disposed.ShouldBeFalse();

                    transient2 = controller2.TransientFunc();
                    transient22 = controller2.TransientFunc();
                    transient22.ShouldNotBeSameAs(transient2);
                }

                Console.WriteLine();

                service2.Disposed.ShouldBeTrue();
                transient2.Disposed.ShouldBeTrue();
                transient22.Disposed.ShouldBeTrue();

                using (var scope3 = provider.CreateScope())
                {
                    GenericScope<ServicesBuilder>.CurrentScope = scope3;

                    controller3 = scope3.ServiceProvider.GetService<Controller>();
                    controller3.ShouldBeSameAs(controller1);

                    service3 = controller3.ScopedFunc();
                    var service33 = controller3.ScopedFunc();
                    service33.ShouldBeSameAs(service3);
                    service3.Disposed.ShouldBeFalse();

                    transient3 = controller3.TransientFunc();
                    transient33 = controller3.TransientFunc();
                    transient33.ShouldNotBeSameAs(transient3);
                }

                Console.WriteLine();

                service3.Disposed.ShouldBeTrue();
                transient3.Disposed.ShouldBeTrue();
                transient33.Disposed.ShouldBeTrue();
            }
        }

        [Test]
        [Category("Throw")]
        public void Scoped_Func_Call_Without_ScopeProvider_Should_Throw()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                using (var provider = ServicesBuilder.BuildDefault(addScopeProvider: false).BuildServiceProvider(validateScopes: true))
                {
                    using (var scope = provider.CreateScope())
                    {
                        var controller = scope.ServiceProvider.GetService<Controller>();
                        var service = controller.ScopedFunc();
                    }
                }
            });
        }
    }
}
