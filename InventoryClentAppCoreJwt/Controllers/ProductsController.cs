using InventoryClentAppCoreJwt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace InventoryClentAppCoreJwt.Controllers
{
    public class ProductsController : Controller
    {
        public static string baseURL;
        private readonly IConfiguration _configuration;
        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
            baseURL = _configuration.GetValue<string>("BaseURL");

        }

        public async Task<IActionResult> Index()
        {
            return View(await GetProducts());
        }
        public async Task<Product> GetProducts(int id)
        {

            var accessToken = HttpContext.Session.GetString("JWToken");
            Product receivedProducts = new Product();

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                

                using (var response = await httpClient.GetAsync(baseURL + "/api/products/"+id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        receivedProducts = JsonConvert.DeserializeObject<Product>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }

            }
            return receivedProducts;


        }


        [HttpGet]
        public async Task<List<Product>> GetProducts()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            List<Product> receivedProducts = new List<Product>();

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                using (var response = await httpClient.GetAsync(baseURL + "/api/products"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        receivedProducts = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }

            }
            return receivedProducts;

        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Login","Home");
        }



        // GET: ProductsController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            Product productdetails = await GetProducts(id);
            return View(productdetails);
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {

            var accessToken = HttpContext.Session.GetString("JWToken");
            Product receivedProducts = new Product();

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(baseURL+"/api/products", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedProducts = JsonConvert.DeserializeObject<Product>(apiResponse);
                    if(receivedProducts != null)
                    {
                        ViewBag.Message = "Product creation Successfully";
                        return View(receivedProducts);
                    }
                }
                
            }
            ViewBag.Message = "Product creation Failed";
            return View();
            

        }
       

        // GET: ProductsController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Product product = await GetProducts(id);
            return View(product);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Product updatedProducts)
        {
            updatedProducts.ProductId = id;
            var accessToken = HttpContext.Session.GetString("JWToken");
            

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                StringContent contents = new StringContent(JsonConvert.SerializeObject(updatedProducts), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(baseURL + "/api/products/"+id, contents))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //receivedProducts = JsonConvert.DeserializeObject<Product>(apiResponse);

                    if (apiResponse != null)
                        ViewBag.Message = "Product Updated Successfully";
                    else
                        ViewBag.Message = "Product updation Failed";
                }

            }

            return View();

        }



        // GET: ProductsController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            Product product = await GetProducts(id);
            return View(product);
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var httpClient = new HttpClient(clientHandler))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using (var response = await httpClient.DeleteAsync(baseURL+"/api/products/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
