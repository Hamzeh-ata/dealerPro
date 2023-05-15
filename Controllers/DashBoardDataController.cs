using DealerPro.Models;
using FirebaseAdmin.Auth;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace DealerPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardDataController : ControllerBase
    {
        IFirebaseClient client;
        private Dictionary<string, dynamic> _data;
        private readonly string endPoint = "/customStores";
        private readonly IConfiguration configuration;
        IFirebaseConfig config;
        public DashBoardDataController(IConfiguration configuration)
        {
            this.configuration = configuration;
            var firebaseConfig = configuration.GetSection("Firebase");

            config = new FirebaseConfig
            {
                BasePath = firebaseConfig["DatabaseUrl"],
                AuthSecret = firebaseConfig["ApiKey"]
            };
        }


        [HttpPost("userid")]
        public IActionResult GetUserId([FromBody] TokenData tokenData)
        {
            try
            {
                // Validate the token and check if it has expired
                var decodedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenData.Token).Result;
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(decodedToken.ExpirationTimeSeconds);
                var currentTime = DateTimeOffset.UtcNow;

                if (currentTime > expirationTime)
                {
                    // Token has expired
                    return Unauthorized();
                }

                var userId = decodedToken.Uid;
                return Ok(userId);
            }
            catch (FirebaseAuthException)
            {
                // Token validation failed
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid token: " + ex.Message);
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewStore(string UID, string storeName)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var response = await client.GetAsync(endPoint);
                _data = response.ResultAs<Dictionary<string, dynamic>>();

                if (_data != null)
                {
                    if (await CheckForStoreNameDuplicates(storeName) && await CheckForUIDDuplicates(UID))
                    {
                        await client.PushAsync(endPoint, new { StoreName = storeName, UserId = UID });
                        return Ok("New store created");
                    }
                    else if (!await CheckForStoreNameDuplicates(storeName))
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, "Store name already exists");
                    }
                    else if (!await CheckForUIDDuplicates(UID))
                    {
                        return StatusCode((int)HttpStatusCode.Forbidden, "You can only have one store");
                    }
                }
                else if (_data == null)
                {
                    await client.PushAsync(endPoint, new { StoreName = storeName, UserId = UID });
                    return Ok("New store created");
                }

                return Ok("Store created");
            }
            catch (Exception e)
            {
                // Log the exception
                return BadRequest("An error occurred while processing the request");
            }
        }
        [HttpPut("changeName")]
        public async Task<IActionResult> ChangeStoreName(string UID, string existStoreName, string newStoreName)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var response = await client.GetAsync(endPoint);
                _data = response.ResultAs<Dictionary<string, dynamic>>();

                if (!await CheckForStoreNameDuplicates(existStoreName) && !await CheckForUIDDuplicates(UID))
                {
                    var existingItem = _data.FirstOrDefault(x => x.Value.StoreName == existStoreName && x.Value.UserId == UID);
                    var existingKey = existingItem.Key;
                    await client.UpdateAsync(endPoint + "/" + existingKey, new { StoreName = newStoreName, UserId = UID });

                    return Ok("Store name changed");
                }
                else
                {
                    return BadRequest("Error: Invalid store name or UID");
                }
            }
            catch (Exception e)
            {
                // Log the exception
                return BadRequest("An error occurred while processing the request");
            }
        }
        [HttpDelete("deleteStore")]
        public async Task<IActionResult> DeleteStore(string UID, string storeName)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var response = await client.GetAsync(endPoint);
                _data = response.ResultAs<Dictionary<string, dynamic>>();

                if (!await CheckForStoreNameDuplicates(storeName) && !await CheckForUIDDuplicates(UID))
                {
                    var existingItem = _data.FirstOrDefault(x => x.Value.StoreName == storeName && x.Value.UserId == UID);
                    if (existingItem.Value != null)
                    {
                        var existingKey = existingItem.Key;
                        await client.DeleteAsync(endPoint + "/" + existingKey);
                        return Ok("Store removed");
                    }
                    else
                    {
                        return NotFound("Store not found");
                    }
                }
                else
                {
                    return BadRequest("Error: Invalid store name or UID");
                }
            }
            catch (Exception e)
            {
                // Log the exception
                return BadRequest("An error occurred while processing the request");
            }
        }
        private async Task<bool> CheckForStoreNameDuplicates(string storeName)
        {
            foreach (var item in _data)
            {
                var existingStoreName = item.Value.StoreName;
                if (existingStoreName == storeName)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> CheckForUIDDuplicates(string uID)
        {
            foreach (var item in _data)
            {
                var existingUID = item.Value.UserId;
                if (existingUID == uID)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
