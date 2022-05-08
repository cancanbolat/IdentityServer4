using IdentityModel.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GarantiClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            DiscoveryDocumentResponse discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:1000");
            ClientCredentialsTokenRequest tokenRequest = new ClientCredentialsTokenRequest();

            tokenRequest.ClientId = "GarantiBankasi";
            tokenRequest.ClientSecret = "garanti";
            tokenRequest.Address = discovery.TokenEndpoint;

            TokenResponse tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);
            httpClient.SetBearerToken(tokenResponse.AccessToken);

            HttpResponseMessage response = await httpClient.GetAsync("https://localhost:2000/api/weatherforecast");
            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<List<WeatherForecast>>(await response.Content.ReadAsStringAsync());


                foreach (var item in result)
                {
                    Console.WriteLine($"{item.ClientName}");
                    Console.WriteLine($"{item.Date}");
                }
            }
        }
    }

    public class WeatherForecast
    {
        public string ClientName { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
