using Microsoft.Extensions.DependencyInjection;
using SteroidsDI.Core;
using System;

namespace SteroidsDI
{
    /// <summary> Adapter from <see cref="IServiceScopeFactory" /> to <see cref="IScopeFactory" />. </summary>
    public sealed class MicrosoftScopeFactory : IScopeFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary> Initializes a new instance of the <see cref = "MicrosoftScopeFactory" />. </summary>
        /// <param name="serviceScopeFactory"> Native MSDI factory for creating scopes. </param>
        public MicrosoftScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary> Create scope. </summary>
        /// <returns> Scope object which should be destroyed at the end of scope. </returns>
        public IDisposable CreateScope() => _serviceScopeFactory.CreateScope();
    }
}
