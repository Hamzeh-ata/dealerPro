using DealerPro.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace DealerPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private static bool firebaseInitialized = false;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignInUser([FromBody] signInData signInData)
        {
            try
            {

                // Load Firebase configuration from app settings
                var firebaseConfig = configuration.GetSection("Firebase");
                var apiKey = firebaseConfig["AIzaSyALF_NBlHu5YgYPavUklfiemW69LBImXjw"];
                var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyALF_NBlHu5YgYPavUklfiemW69LBImXjw";
                if (!firebaseInitialized)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(new
                        {
                            type = firebaseConfig["type"],
                            project_id = firebaseConfig["project_id"],
                            private_key_id = firebaseConfig["private_key_id"],
                            private_key = firebaseConfig["private_key"],
                            client_email = firebaseConfig["client_email"],
                            client_id = firebaseConfig["client_id"],
                            auth_uri = firebaseConfig["auth_uri"],
                            token_uri = firebaseConfig["token_uri"],
                            auth_provider_x509_cert_url = firebaseConfig["auth_provider_x509_cert_url"],
                            client_x509_cert_url = firebaseConfig["client_x509_cert_url"],
                            universe_domain = firebaseConfig["universe_domain"]
                        }))
                    });
                    firebaseInitialized = true;
                }
                // Create the request body
                var requestBody = new
                {
                    email = signInData.Email,
                    password = signInData.Password,
                    returnSecureToken = true
                };
                var requestBodyJson = JsonConvert.SerializeObject(requestBody);

                // Send the request to Firebase Authentication REST API
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(requestUri, new StringContent(requestBodyJson, Encoding.UTF8, "application/json"));
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response to retrieve the ID token
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        var idToken = responseObject.idToken?.ToString();

                        // Check if idToken is null or empty
                        if (!string.IsNullOrEmpty(idToken))
                        {
                            if (firebaseInitialized)
                            {
                                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                                var userEmail = decodedToken.Claims["email"].ToString();
                                var userId = decodedToken.Uid;
                                return Ok(idToken);
                                // Rest of the code...
                            }
                            // Return the ID token to the client
                            else
                            {
                                // Handle the error: Firebase Admin SDK not initialized
                                return BadRequest("Firebase Admin SDK not initialized.");
                            }
                        }
                        else
                        {
                            // Handle the error: idToken is null or empty
                            return BadRequest("Invalid ID token.");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        // Handle specific error cases, such as incorrect password
                        var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        var errorMessage = errorResponse.error.message.ToString();

                        if (errorMessage == "INVALID_PASSWORD")
                        {
                            // Handle the error: Incorrect password
                            return BadRequest("Incorrect password.");
                        }
                        else if (errorMessage == "EMAIL_NOT_FOUND")
                        {
                            // Handle the error: Email not found
                            return BadRequest("Email not found.");
                        }
                        else
                        {
                            // Handle other error cases
                            return BadRequest(errorMessage);
                        }
                    }
                    else
                    {
                        // Handle general error cases
                        return BadRequest("An error occurred during login.");
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


    }
}
