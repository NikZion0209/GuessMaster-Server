using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        [Route("SetSession")]
        [HttpPost]
        public IActionResult SetSessionValue(string Key, string Value)
        {
            // Store a simple string value in a cookie
            Response.Cookies.Append(Key, Value, new CookieOptions
            {
                // Set cookie expiration (optional)
                Expires = DateTime.UtcNow.AddDays(7),  // Cookie expires in 7 days
                HttpOnly = true,  // Makes the cookie accessible only through HTTP(S) requests
                Secure = true,    // Ensure cookie is sent over HTTPS
                SameSite = SameSiteMode.Strict // Optional: Prevents the cookie from being sent in cross-site requests
            });
            return Ok();
        }
        [Route("SetSessionObject")]
        [HttpPost]
        public IActionResult SetObject(string key, object value)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(serializedObject);
            HttpContext.Session.Set(key, bytes);
            return Ok();
        }
        [Route("GetObject")]
        [HttpGet]
        public object GetObject<T>(string key)
        {
            object myObject = new object();

            var bytes = HttpContext.Session.Get(key);
            if (bytes != null)
            {
                var serializedObject = Encoding.UTF8.GetString(bytes);
                myObject = JsonConvert.DeserializeObject<T>(serializedObject);
            }

            return myObject;
        }
    }
}
