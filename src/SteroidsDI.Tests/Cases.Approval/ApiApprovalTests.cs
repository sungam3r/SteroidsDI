using NUnit.Framework;
using PublicApiGenerator;
using Shouldly;
using SteroidsDI.AspNetCore;
using SteroidsDI.Core;

namespace SteroidsDI.Tests.Cases;

/// <summary> Tests for checking changes to the public API. </summary>
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
        string publicApi = type.Assembly.GeneratePublicApi(new ApiGeneratorOptions
        {
            IncludeAssemblyAttributes = false,
            AllowNamespacePrefixes = ["System", "Microsoft.Extensions.DependencyInjection"],
            ExcludeAttributes = ["System.Diagnostics.DebuggerDisplayAttribute"],
        });

        publicApi.ShouldMatchApproved(options => options!.WithFilenameGenerator((testMethodInfo, discriminator, fileType, fileExtension) => $"{type.Assembly.GetName().Name!}.{fileType}.{fileExtension}"));
    }
}
