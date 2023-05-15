using DealerPro.Models;
using Firebase.Database;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using test.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Net;

namespace DealerPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterUserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private static bool firebaseInitialized = false;

        public RegisterUserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> registerUser([FromBody] RegistrationData registrationData)
        {
            try
            {
                // Load Firebase configuration from app settings
                var firebaseConfig = configuration.GetSection("Firebase");
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
                // Initialize FirebaseApp with Firebase configuration from app settings
                if (await IsEmailExists(registrationData.Email))
                {
                    return Ok("Email already exists");
                }
                if (!await IsValidEmail(registrationData.Email))
                {
                    return Ok("Invalid email address");
                }
                if (!await passwordCount(registrationData.Password))
                {
                    return Ok("Password must be at least 6 characters long");
                }
                if (!await HasNumberAndLetter(registrationData.Password))
                {
                    return Ok("Password contains both numbers and letters");
                }
                // Create a new user with email and password
                var auth = FirebaseAuth.DefaultInstance;
                UserRecord user;
                user = await auth.CreateUserAsync(new UserRecordArgs
                {
                    Email = registrationData.Email,
                    Password = registrationData.Password,
                    DisplayName = registrationData.userName
                });
                var user2 = await auth.GetUserAsync(user.Uid);
                if (user2 != null)
                {
                    string displayName = user.DisplayName;
                   
                }
                // Check if email already exists
                // User registration successful
                // Generate an authentication token for the registered user
                var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

                // Return the authentication token to the client
                return Ok(customToken);
              
            }

            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
    }
private async Task<bool> IsValidEmail(string email)
        {
            // Use a regular expression to validate the email format
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);
        }
private async Task<bool> IsEmailExists(string email)
        {
            try
            {
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
                return user != null;
            }
            catch (FirebaseAuthException)
            {
                return false; // User does not exist
            }

        }
 private async Task<bool> passwordCount(string password)
        {
            if (password.Count() < 6)
            {
                return false;
            }
            return true;
        }
 private async Task<bool> HasNumberAndLetter(string password)
        {
           // Regular expression to match at least one digit and one letter
    Regex regex = new Regex(@"^(?=.*[0-9])(?=.*[a-zA-Z]).*$");
            // Test the password against the regular expression
            return regex.IsMatch(password);
        }

    }
   
    }


