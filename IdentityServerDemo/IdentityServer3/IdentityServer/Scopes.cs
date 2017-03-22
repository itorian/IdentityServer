using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace IdentityServer3Demo.IdentityServer
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                new Scope
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Type = ScopeType.Identity,
                    Enabled = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role", true)
                    }
                },
                new Scope
                {
                    Name = "apiname1",
                    DisplayName = "API Scope",
                    Type = ScopeType.Resource,
                    Enabled = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role", true)
                    }
                }
            };

            return scopes;
        }
    }
}