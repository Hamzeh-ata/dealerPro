using Microsoft.AspNetCore.Mvc;
using test.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly firebaseOptions _firebaseOptions;
        private FirebaseClient _firebaseClient;
        private readonly IMemoryCache _cache;
        private readonly List<Components> _components = new List<Components>();
        public ProductsController(IOptions<firebaseOptions> firebaseOptions, IMemoryCache cache)
        {
            _firebaseOptions = firebaseOptions.Value;
            _firebaseClient = new FirebaseClient(_firebaseOptions.DatabaseUrl);
            _cache = cache;
        }

        [HttpGet("{category}")]
        public async Task<ActionResult<List<Components>>> GetProductsByCategory(string category)
        {
            if (_cache.TryGetValue(category, out List<Components> components))
            {
                return components;
            }

            components = new List<Components>();

            var productsQuery = await _firebaseClient
                .Child("Products")
                .Child(category)
                .OnceAsync<Dictionary<string, object>>();

            foreach (var productSnapshot in productsQuery)
            {
                var component = new Components
                {
                    ProductId = productSnapshot.Key,
                    Name = productSnapshot.Object["Name"] as string,
                    Price = (string)productSnapshot.Object["Price"],
                    OldPrice = productSnapshot.Object.ContainsKey("OldPrice") ? (string?)productSnapshot.Object["OldPrice"] : null,
                    Category = category,
                    Image = productSnapshot.Object["Image"] as string,
                    StoreName = productSnapshot.Object["StoreName"] as string,
                };
                components.Add(component);
            }
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _cache.Set(category, components, cacheOptions);
            return components;
        }

        [HttpGet("{category}/{id}")]
        public async Task<ActionResult<List<Components>>> GetProductDeatils(string category, string id)
        {
            if (_cache.TryGetValue($"{category}_{id}", out List<Components> components))
            {
                return components;
            }

            components = new List<Components>();

            var productQuery = await _firebaseClient
                .Child("Products")
                .Child(category)
                .Child(id)
                .OnceSingleAsync<Dictionary<string, object>>();
            // Split the description string into a list of strings using the newline character as the delimiter
            var descriptionJArray = (JArray)productQuery["description"];
            var descriptionList = descriptionJArray.ToObject<List<string>>();
            var component = new Components
            {
                ProductId = id,
                Name = productQuery["Name"] as string,
                Price = (string)productQuery["Price"],
                OldPrice = productQuery.ContainsKey("OldPrice") ? (string?)productQuery["OldPrice"] : "0",
                Category = category,
                Image = productQuery["Image"] as string,
                StoreName = productQuery["StoreName"] as string,
                Brand = productQuery["Brand"] as string,
                ProductUrl = productQuery["ProductUrl"] as string,
                Time = productQuery["Time"] as string,
                Date = productQuery["Date"] as string,
                description = descriptionList,
            };

            components.Add(component);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _cache.Set($"{category}_{id}", components, cacheOptions);

            return components;
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<List<Components>>> GetProductsByName(string name)
        {
            if (_cache.TryGetValue(name, out List<Components> components))
            {
                return components;
            }

            var categoriesQuery = await _firebaseClient.Child("Products").OnceAsync<Dictionary<string, object>>();
            var tasks = new List<Task>();
            components = new List<Components>();

            foreach (var categorySnapshot in categoriesQuery)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var productsQuery = await _firebaseClient.Child("Products").Child(categorySnapshot.Key).OnceAsync<Dictionary<string, object>>();
                    foreach (var productSnapshot in productsQuery)
                    {
                        var productName = productSnapshot.Object["Name"] as string;
                        if (!string.IsNullOrEmpty(productName) && productName.Contains(name, StringComparison.OrdinalIgnoreCase))
                        {
                            var component = new Components
                            {
                                ProductId = productSnapshot.Key,
                                Name = productName,
                                Price = (string)productSnapshot.Object["Price"],
                                OldPrice = productSnapshot.Object.ContainsKey("OldPrice") ? (string?)productSnapshot.Object["OldPrice"] : null,
                                Category = categorySnapshot.Key,
                                Image = productSnapshot.Object["Image"] as string,
                                StoreName = productSnapshot.Object["StoreName"] as string,
                            };
                            lock (components)
                            {
                                components.Add(component);
                            }
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);

            if (components.Count == 0)
            {
                return NotFound();
            }

            var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            _cache.Set(name, components, cacheOptions);

            return components;
        }





    }
}
  

 

  

   
   

