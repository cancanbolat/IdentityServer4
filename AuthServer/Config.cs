using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer
{
    static public class Config
    {
        #region Scopes [API'lerde kullanıalcak izinleri tanımlar]
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("Garanti.Write","Garanti bankası yazma izni"),
                new ApiScope("Garanti.Read","Garanti bankası okuma izni"),
                new ApiScope("HalkBank.Write","HalkBank bankası yazma izni"),
                new ApiScope("HalkBank.Read","HalkBank bankası okuma izni")
            };
        }
        #endregion

        #region Resources [API'ler tanımlanır]
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("Garanti"){ Scopes = { "Garanti.Write", "Garanti.Read" } },
                new ApiResource("HalkBank"){ Scopes = { "HalkBank.Write", "HalkBank.Read" } }
            };
        }
        #endregion

        #region Clients [API'leri kullanacak client'lar tanımlanır]
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
            new Client
                    {
                        ClientId = "GarantiBankasi",
                        ClientName = "GarantiBankasi",
                        ClientSecrets = { new Secret("garanti".Sha256()) },
                        AllowedGrantTypes = { GrantType.ClientCredentials },
                        AllowedScopes = { "Garanti.Write", "Garanti.Read" }
                    },
            new Client
                    {
                        ClientId = "HalkBankasi",
                        ClientName = "HalkBankasi",
                        ClientSecrets = { new Secret("halkbank".Sha256()) },
                        AllowedGrantTypes = { GrantType.ClientCredentials },
                        AllowedScopes = { "HalkBank.Write", "HalkBank.Read" }
                    },
            new Client
                    {
                        ClientId = "AuthClientMvc",
                        ClientName = "AuthClientMvc",
                        ClientSecrets = { new Secret("authclientmvc".Sha256()) },
                        AllowedGrantTypes = GrantTypes.Hybrid,
                        AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId,
                                          IdentityServerConstants.StandardScopes.Profile,
                                          IdentityServerConstants.StandardScopes.OfflineAccess, //client refresh token'a erişebilir.
                                          "Garanti.Write", "Garanti.Read", "PositionAndAuthority", "Roles"
                                        },
                        AllowOfflineAccess = true, //refresh token için
                        RefreshTokenUsage = TokenUsage.OneTimeOnly,
                        RefreshTokenExpiration = TokenExpiration.Absolute,
                        AbsoluteRefreshTokenLifetime = 2 * 60 * 60 + (10 * 60),
                        RedirectUris = { "https://localhost:5001/signin-oidc" },
                        PostLogoutRedirectUris = { "https://localhost:4000/signout-callback-oidc" },
                        RequirePkce = true,
                        RequireConsent = true
                    },
            new Client{
                        ClientId = "AngularClient",
                        ClientName = "Angular Client",
                        RequireClientSecret = false,
                        AllowedScopes = {
                            "Garanti.Write", "Garanti.Read", 
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Email,
                            "Roles"
                        },
                        RedirectUris = { "http://localhost:4200/callback" },
                        AllowedCorsOrigins = { "http://localhost:4200" },
                        PostLogoutRedirectUris = { "http://localhost:4200" },
                        AllowedGrantTypes = GrantTypes.Code,
                        RequirePkce = true
                    }
            };
        }
        #endregion

        #region Test Users
        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser{
                    SubjectId="test-user-1",
                    Username="test-user-1",
                    Password="12345",
                    Claims =
                    {
                        new Claim(JwtRegisteredClaimNames.Email, "testUser@gmail.com"),
                        new Claim("role", "admin"),
                        new Claim("authority", "true")
                    }
                }
            };
        }
        #endregion

        #region IdentityResources [Girş yapan kullanıcıların bilgileri]
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "Roles",
                    DisplayName = "Roles",
                    Description = "Kullanıcı Rolleri",
                    UserClaims = { "role", "authority" }
                }
            };
        }
        #endregion
    }
}
