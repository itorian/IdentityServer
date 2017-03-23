using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer3Demo.IdentityServer
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "admin",
                    Password = "admin",
                    Subject = "1", //pk

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Administrator"),
                        new Claim(Constants.ClaimTypes.Role, "Admin"),
                        new Claim(Constants.ClaimTypes.Role, "Author"),
                        new Claim(Constants.ClaimTypes.Role, "Editor")
                    }
                },
                new InMemoryUser
                {
                    Username = "abhimanyu",
                    Password = "abhimanyu",
                    Subject = "2", //pk

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Abhimanyu"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Vatsa"),
                        new Claim(Constants.ClaimTypes.Role, "Admin"),
                        new Claim(Constants.ClaimTypes.Role, "Author")
                    }
                }
            };
        }
    }
}