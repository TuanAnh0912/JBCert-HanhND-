using jbcert.API.Middleware;
using jbcert.DATA.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace jbcert.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Route("AddCapDonViChucNang")]
        [HttpGet]
        //[ClaimRequirement("All")]
        public string AddCapDonViChucNang(int id)
        {
            AddLienKetCapDonViChucNangProvider addLienKetCapDonViChucNangProvider = new AddLienKetCapDonViChucNangProvider();
            addLienKetCapDonViChucNangProvider.Add(id);
            return "Ok";
        }

        [Route("Get1")]
        [HttpGet]
        [ClaimRequirement("All, re-authen")]
        public string Get1()
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return "Code xịn: " + ip;
        }

        [Route("Get2")]
        [HttpGet]
        //[ClaimRequirement("All")]
        public IActionResult Get2()
        {
            try
            {
                int width = 100;
                int height = 36;
                var captchaCode = Captcha.Captcha.GenerateCaptchaCode();
                var result = Captcha.Captcha.GenerateCaptchaImage(width, height, captchaCode);
                Stream s = new MemoryStream(result.CaptchaByteData);
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddMinutes(2);
                Response.Cookies.Append("CaptchaCode", captchaCode, option);
                return new FileStreamResult(s, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Get3")]
        [HttpGet]
        //[ClaimRequirement("All")]
        [HttpGet]
        public IActionResult Get3(string captcha)
        {
            try
            {
                var cookieValue = Request.Cookies["CaptchaCode"];
                return Ok(new string[]{ cookieValue, captcha });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}