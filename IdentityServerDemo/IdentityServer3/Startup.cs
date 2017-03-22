using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3Demo.IdentityServer;
using Owin;
using Serilog;
using System;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer3Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Install-Package Serilog
            //Install-Package Serilog.Sinks.Trace
            //Web.config update <system.diagnostics> tree
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();

            app.Map("/identity", core =>
            {
                // ASP.NET Identity Database
                var idSvrFactory = Factory.Configure();
                idSvrFactory.ConfigureUserService("DefaultConnection");

                //// Custom View Service - include content/app+custom+libs folders
                //idSvrFactory.ViewService = new Registration<IViewService>(typeof(CustomViewService));
                //idSvrFactory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService { AllowAll = true });
                //List<LoginPageLink> loginPageLinks = new List<LoginPageLink>();
                //loginPageLinks.Add( new LoginPageLink { Href = "", Text = "Forgot Password ?", Type = "" });
                //loginPageLinks.Add(new LoginPageLink { Href = "", Text = "Not Registered ?", Type = "" });

                var options = new IdentityServerOptions
                {
                    SiteName = "Identity Server Name",
                    IssuerUri = "https://localhost:44364/identity/.well-known/openid-configuration", //not required: this app url with openid spec
                    SigningCertificate = LoadCertificate(),
                    Factory = idSvrFactory,
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnablePostSignOutAutoRedirect = true,
                        EnableSignOutPrompt = false
                    }
                };

                core.UseIdentityServer(options);
            });
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    AuthenticationType = "Google",
            //    Caption = "Sign-in with Google",
            //    SignInAsAuthenticationType = signInAsType,
            //    ClientId = "322890526852-jqbj4ldbebmrdiqt144h55ekook1vap2.apps.googleusercontent.com",
            //    ClientSecret = "sQldgdaOLh-qob0obVclF-BK",
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
            //    AppId = "875148929265560",
            //    AppSecret = "639518a75de0a74911568f756854b984",
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
            //    ConsumerKey = "46yidNCE2d6rxlkd6JN0zEIsO",
            //    ConsumerSecret = "Lh79xlWFKa3o1mOKZaba73vr2kAtwyZEJtOahm57u5TU18fpxV",
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

        // Load your purchased SSL Certificate, paste that certificate in bin as well as Certificates folder
        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(string.Format(@"{0}\bin\identityserver\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}