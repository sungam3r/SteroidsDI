using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;
using System;

namespace SteroidsDI.AspNetCore
{
    /// <summary> <see cref="IScopeProvider"/> for ASP.NET Core working with <see cref="IHttpContextAccessor"/>. </summary>
    public sealed class AspNetCoreHttpScopeProvider : IScopeProvider
    {
        /// <summary> Gets scoped <see cref="IServiceProvider" />, for the current HTTP request. </summary>
        /// <param name="rootProvider"> The root <see cref="IServiceProvider" /> object to obtain <see cref="IHttpContextAccessor"/>. </param>
        /// <returns> The scoped <see cref="IServiceProvider" /> object or <c>null</c> if there is no current HTTP request. </returns>
        public IServiceProvider? GetScopedServiceProvider(IServiceProvider rootProvider) => rootProvider.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices;
    }
}
