using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public const string API_URl = "https://dummyjson.com/products";

        //List
        [HttpGet]
        [Route("/ListProducts")]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await Products());
        }

        //Search
        [HttpGet]
        [Route("/SearchByTitle/{title}")]
        public async Task<IActionResult> SearchProductsByTitle(string title)
        {
            var products = await Products();

            //title search can be on a segment of the title too, hence used Contains()
            var filteredProducts = products
                .Where(x => x.Title.ToLower().Contains(title.ToLower()))
                .ToList();

            return Ok(filteredProducts);
        }

        //Search
        [HttpGet]
        [Route("/SearchByBrand/{brand}")]
        public async Task<IActionResult> SearchProductsByBrand(string brand)
        {
            var products = await Products();

            var filteredProducts = products
                .Where(x => x.Brand.ToLower().Equals(brand.ToLower()))
                .ToList();

            return Ok(filteredProducts);
        }

        //Sort
        [HttpGet]
        [Route("/SortProducts/{column}/{order}")]
        public async Task<IActionResult> SortProducts(string column = "Id", string order = Constants.ASCENDING)
        {
            var products = await Products();

            var propertyInfo = typeof(Product).GetProperty(column);

            var sortedProducts = order == Constants.ASCENDING ?
                products.OrderBy(x => propertyInfo.GetValue(x, null)).ToList()
            : products.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();

            return Ok(sortedProducts);
        }

        //Paging
        [HttpGet]
        [Route("/PagedProducts/{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetPagedProducts(int pageNumber = 1, int pageSize = 10)
        {
            var products = await Products();

            var items = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(items);
        }

        private async Task<List<Product>> Products()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync(API_URl);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Product>();
            }

            var apiResponse = await response.Content.ReadAsStringAsync();

            var products = JsonConvert.DeserializeObject<ProductAPIResponse>(apiResponse).Products;

            return products;
        }
    }
}