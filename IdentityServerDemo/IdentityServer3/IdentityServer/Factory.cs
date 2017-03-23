using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.Core.Services.Default;

namespace IdentityServer3Demo.IdentityServer
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure()
        {
            var factory = new IdentityServerServiceFactory();

            // select scopes
            var scopes = new InMemoryScopeStore(Scopes.Get());
            factory.ScopeStore = new Registration<IScopeStore>(scopes);

            // select clients
            var clients = new InMemoryClientStore(Clients.Get());
            factory.ClientStore = new Registration<IClientStore>(clients);

            // configure cors policy
            factory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });

            return factory;
        }
    }
}