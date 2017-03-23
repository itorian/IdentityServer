using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace IdentityServer3Demo.IdentityServer
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[] {

                new Client  // Browser client
                {
                    Enabled = true,
                    ClientId = "website1",
                    ClientName = "website 1 name",
                    Flow = Flows.Hybrid,
                    RequireConsent = false,
                    AllowRememberConsent = true,
                    RedirectUris = new List<string>
                    {
                        "http://localhost:19007/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:19007/"
                    },
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = new List<string>
                    {
                        "openid", "profile", "email", "roles" // configured in Scopes.cs
                    }
                },

                //new Client  // Windows Phone or Android or IOS client
                //{
                //    Enabled = true,
                //    ClientId = "native",
                //    ClientName = "Mobile Device",
                //    Flow = Flows.Implicit,
                //    RequireConsent = false,
                //    RedirectUris = new List<string>
                //    {
                //        ""
                //    },
                //    AllowedScopes = new List<string>
                //    {
                //        "openid", "profile", "email", "roles", "apiname1"
                //    }
                //}

            };
        }
    }


}