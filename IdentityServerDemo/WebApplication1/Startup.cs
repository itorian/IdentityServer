using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json.Linq;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Thinktecture.IdentityModel.Client;
using WebApplication1.Helper;
using WebApplication1.Models;

//Steps:-
//1. Create MVC project with Individual Authentication to get all the necessary packages
//2. Install-Packages Thinktecture.IdentityModel.Client
//3. 

[assembly: OwinStartupAttribute(typeof(WebApplication1.Startup))]
namespace WebApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //// Allow this app to register new users
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "unique_user_key";

            //app.UseResourceAuthorization(new AuthorizationManager());

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "website1",
                Authority = "https://localhost:44364/identity/", //authority url
                RedirectUri = "http://localhost:19007/", //client url
                PostLogoutRedirectUri = "http://localhost:19007/", //client url
                SignInAsAuthenticationType = "Cookies",
                ResponseType = "id_token code token",
                Scope = "openid profile email roles apiname1",
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    SecurityTokenValidated = async n =>
                    {
                        var userInfo = await EndpointAndTokenHelper.CallUserInfoEndpoint(n.ProtocolMessage.AccessToken, "https://localhost:44364/identity/");
                        var newIdentity = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, JwtClaimTypes.GivenName, JwtClaimTypes.Role);

                        var id_token = n.ProtocolMessage.IdToken;
                        var access_token = n.ProtocolMessage.AccessToken;
                        var userid = userInfo.Value<string>("sub"); // unique identifier

                        var incomingClaims = n.AuthenticationTicket.Identity.Claims.ToList();
                        string identityProvider = incomingClaims.First(c => c.Type == JwtClaimTypes.IdentityProvider).Value;  //Facebook, Twitter, Google, idsrv

                        // TODO: Get roles into newIdentity for MVC Client
                        try
                        {
                            // If more than one role
                            var roles = userInfo.Value<JArray>("role").ToList();
                            foreach (var role in roles)
                            {
                                newIdentity.AddClaim(new Claim(JwtClaimTypes.Role, role.ToString()));
                            }
                        }
                        catch
                        {
                            // If single role
                            var role = userInfo.Value<string>("role");
                            if (role != null)
                                newIdentity.AddClaim(new Claim(JwtClaimTypes.Role, role.ToString()));
                        }

                        // Get display name on application
                        var displayName = "";
                        if (identityProvider == "idsrv")
                            displayName = userInfo.Value<string>("preferred_username");
                        else
                            displayName = userInfo.Value<string>("name");

                        // Create list of claims, this can be used in the app
                        newIdentity.AddClaim(new Claim("id_token", id_token));
                        newIdentity.AddClaim(new Claim("access_token", access_token));
                        newIdentity.AddClaim(new Claim("userid", userid));
                        newIdentity.AddClaim(new Claim("username", displayName));
                        newIdentity.AddClaim(new Claim("identityProvider", identityProvider));

                        // Generate unique_user_key and add into claim
                        var issuerClaim = n.AuthenticationTicket.Identity.FindFirst(JwtClaimTypes.Issuer);
                        var subjectClaim = n.AuthenticationTicket.Identity.FindFirst(JwtClaimTypes.Subject);
                        newIdentity.AddClaim(new Claim("unique_user_key", issuerClaim.Value + "_" + subjectClaim.Value));

                        // Generate auth ticket
                        n.AuthenticationTicket = new AuthenticationTicket(newIdentity, n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
