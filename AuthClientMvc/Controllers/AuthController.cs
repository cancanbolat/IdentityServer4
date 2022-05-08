using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthClientMvc.Controllers
{
    public class AuthController : Controller
    {

        public async Task<IActionResult> NewAccessToken()
        {
            //Refresh token değeri elde edilir.
            string refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            HttpClient httpClient = new HttpClient();

            #region //Refresh token ile request yapabilmek için client'ın bilgilerini tutuyoruz.
            // IdentityServer4 sunucusuna grant_type değeri refresh_token olan istek atılır.
            // Geriye TokenResponse yani access token, id token ve refresh token döner.
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest()
            {
                ClientId = "AuthClientMvc",
                ClientSecret = "authclientmvc",
                RefreshToken = refreshToken,
                Address = (await httpClient.GetDiscoveryDocumentAsync("https://localhost:1000")).TokenEndpoint
            };
            #endregion

            TokenResponse tokenResponse = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            AuthenticationProperties properties = (await HttpContext.AuthenticateAsync()).Properties;

            #region AuthenticationProperty'ler ile gelen yeni değerleri olan StoreTokens func ile değiştiriyoruz.
            properties.StoreTokens(
                new List<AuthenticationToken>
                {
                    new AuthenticationToken{ Name = OpenIdConnectParameterNames.IdToken , Value = tokenResponse.IdentityToken },
                    new AuthenticationToken{ Name = OpenIdConnectParameterNames.AccessToken , Value = tokenResponse.AccessToken },
                    new AuthenticationToken{ Name = OpenIdConnectParameterNames.RefreshToken , Value = tokenResponse.RefreshToken },
                    new AuthenticationToken
                        {
                            Name = OpenIdConnectParameterNames.ExpiresIn,
                            Value = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("O")
                        }
                });
            #endregion

            //Kullanıcıya yeni authentication propertyleri atıyoruz.
            await HttpContext.SignInAsync("AuthClientMvcCookie", (await HttpContext.AuthenticateAsync()).Principal, properties);
            return RedirectToAction(nameof(HomeController.Index));
        }

        //Authentication propertyler çekilip listelenir.
        public async Task<IActionResult> Index()
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync();
            IOrderedEnumerable<KeyValuePair<string, string>> properties = authenticateResult.Properties.Items.OrderBy(x => x.Key);
            return View(properties);
        }
    }
}
