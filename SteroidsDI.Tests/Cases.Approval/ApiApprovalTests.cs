using NUnit.Framework;
using PublicApiGenerator;
using Shouldly;
using SteroidsDI;
using SteroidsDI.AspNetCore;
using SteroidsDI.Core;
using System;

namespace Opn.Lib.Dotnet.DependencyInjection.Tests.Cases
{
    /// <summary> Tests for checking changes to the public API. </summary>
    /// <see href="https://github.com/JakeGinnivan/ApiApprover"/>
    [TestFixture]
    public class ApiApprovalTests
    {
        /// <summary> Check for changes to the public APIs. </summary>
        /// <param name="type"> The type used as a marker for the assembly whose public API change you want to check. </param>
        [TestCase(typeof(MicrosoftScopeFactory))]
        [TestCase(typeof(IScopeProvider))]
        [TestCase(typeof(AspNetCoreHttpScopeProvider))]
        public void PublicApi(Type type)
        {
            string? publicApi = type?.Assembly.GeneratePublicApi(new ApiGeneratorOptions
            {
                IncludeAssemblyAttributes = false,
                WhitelistedNamespacePrefixes = new[] { "Microsoft.Extensions.DependencyInjection" },
                ExcludeAttributes = new[] { "System.Diagnostics.DebuggerDisplayAttribute" },
            });

            publicApi.ShouldMatchApproved(options => options!.WithDescriminator(type.Assembly.GetName().Name));
        }
    }
}
