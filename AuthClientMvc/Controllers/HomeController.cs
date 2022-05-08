using AuthClientMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AuthClientMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Read()
        {
            var authenticationProperties = (await HttpContext.AuthenticateAsync()).Properties.Items;
            string accessToken = authenticationProperties.FirstOrDefault(x => x.Key == ".Token.access_token").Value;

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage responseMessage = await httpClient.GetAsync("https://localhost:2000/api/weatherforecast/read");
            string apiResponse = await responseMessage.Content.ReadAsStringAsync();

            ViewBag.apiResponseMessage = apiResponse;

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize(Roles = "admin")]
        public IActionResult AdminRol()
        {
            return Ok("hg admin");
        }


        [Authorize(Roles = "moderator")]
        public IActionResult ModRol()
        {
            return Ok("hg mod");
        }

        [Authorize(Roles = "admin, moderator")]
        public IActionResult AdminModRol()
        {
            return Ok("hg admin ve mod");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("AuthClientMvcCookie"); //client'dan çıkış yapar
            await HttpContext.SignOutAsync("oidc"); //open'id den çıkış yapar
        }
    }
}
