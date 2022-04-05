using InventoryClentAppCoreJwt.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace InventoryClentAppCoreJwt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(UserInfo user)
        {
            if(user.Email != null && user.Password != null)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                using (var httpClient = new HttpClient(clientHandler))
                {
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("http://localhost:5246/api/token", stringContent))
                    {
                        string token = await response.Content.ReadAsStringAsync();
                        if (token == "Invalid credentials")
                        {
                            ViewBag.Message = "Incorrect Email and Password";
                            return View();
                        }

                        HttpContext.Session.SetString("JWToken", token);
                    }
                    return Redirect("~/Products/Index");
                }
            }

            ViewBag.Message = "Incorrect Email and Password";
            return View();
            
            
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}