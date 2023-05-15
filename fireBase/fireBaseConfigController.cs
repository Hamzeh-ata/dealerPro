using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace test.fireBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class fireBaseConfigController : Controller
    {
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public IActionResult getFirebaseConfig()
        {
            var fireBaseConfig = new
            {
                AuthSecret = "Au1R5GOJuXI654VqsOZYAlYatLlnylajYFoPu75v",
                BasePath = "https://test-811c3-default-rtdb.firebaseio.com"
            };
            if (fireBaseConfig == null)
            {
                return BadRequest("Firebase configuration not available");
            }
            return Ok(fireBaseConfig);
        }
    }
}
