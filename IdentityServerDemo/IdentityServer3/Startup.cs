using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3Demo.IdentityServer;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer3Demo
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    app.Map("/identity", idsrvApp =>
        //    {
        //        idsrvApp.UseIdentityServer(new IdentityServerOptions
        //        {
        //            SiteName = "Embedded IdentityServer",
        //            SigningCertificate = LoadCertificate(),

        //            // In memory configuration
        //            Factory = new IdentityServerServiceFactory()
        //                        .UseInMemoryUsers(Users.Get())
        //                        .UseInMemoryClients(Clients.Get())
        //                        .UseInMemoryScopes(StandardScopes.All)
        //        });
        //    });
        //}

        public void Configuration(IAppBuilder app)
        {
            //Install-Package Serilog
            //Install-Package Serilog.Sinks.Trace
            //Web.config update <system.diagnostics> tree
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();

            app.Map("/identity", core =>
            {
                // Configure clients, scopes and users services - asp.net identity database
                var idSvrFactory = Factory.Configure();
                idSvrFactory.ConfigureUserService("DefaultConnection");

                // Custom View Service - include content/app+custom+libs folders
                idSvrFactory.ViewService = new Registration<IViewService>(typeof(CustomViewService));
                idSvrFactory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });
                List<LoginPageLink> loginPageLinks = new List<LoginPageLink>();
                loginPageLinks.Add(new LoginPageLink { Href = "", Text = "Forgot Password ?", Type = "" });
                loginPageLinks.Add(new LoginPageLink { Href = "", Text = "Not Registered ?", Type = "" });

                var options = new IdentityServerOptions
                {
                    SiteName = "Identity Server Name",
                    IssuerUri = "https://localhost:44364/identity/.well-known/openid-configuration", //not required: this app url with openid spec
                    SigningCertificate = LoadCertificate(),
                    Factory = idSvrFactory,
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureOAuthIdentityProviders,
                        EnablePostSignOutAutoRedirect = true,
                        EnableSignOutPrompt = false
                    }
                };

                core.UseIdentityServer(options);
            });
        }

        private void ConfigureOAuthIdentityProviders(IAppBuilder app, string signInAsType)
        {
            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    AuthenticationType = "Google",
            //    Caption = "Sign-in with Google",
            //    SignInAsAuthenticationType = signInAsType,
            //    ClientId = "*****",
            //    ClientSecret = "****",
            //    Provider = new GoogleOAuth2AuthenticationProvider()
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            return Task.FromResult(context);
            //        }
            //    }
            //});

            //app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            //{
            //    AuthenticationType = "Facebook",
            //    Caption = "Sign-in with Facebook",
            //    SignInAsAuthenticationType = signInAsType,
            //    AppId = "****",
            //    AppSecret = "*****",
            //    Provider = new FacebookAuthenticationProvider()
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            return Task.FromResult(context);
            //        }
            //    }
            //});

            //app.UseTwitterAuthentication(new TwitterAuthenticationOptions()
            //{
            //    AuthenticationType = "Twitter",
            //    Caption = "Sign-in with Twitter",
            //    SignInAsAuthenticationType = signInAsType,
            //    ConsumerKey = "****",
            //    ConsumerSecret = "******",
            //    Provider = new TwitterAuthenticationProvider()
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            return Task.FromResult(context);
            //        }
            //    }
            //});

            //app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
            //{
            //    AuthenticationType = "Microsoft",
            //    Caption = "Sign-in with Microsoft",
            //    SignInAsAuthenticationType = signInAsType,
            //    ClientId = "",
            //    ClientSecret = "",
            //    Provider = new MicrosoftAccountAuthenticationProvider()
            //    {
            //        OnAuthenticated = (context) =>
            //        {
            //            return Task.FromResult(0);
            //        }
            //    }
            //});
        }

        // Load your purchased SSL Certificate, or use free from here https://github.com/IdentityServer/IdentityServer3.Samples/tree/master/source/Certificates
        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(string.Format(@"{0}\bin\identityserver\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}