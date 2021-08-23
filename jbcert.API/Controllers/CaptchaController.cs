using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        [Route("GetCaptcha")]
        [HttpGet]
        public IActionResult GetCaptcha(int width, int height)
        {
            try
            {
                var captchaCode = Captcha.Captcha.GenerateCaptchaCode();
                var result = Captcha.Captcha.GenerateCaptchaImage(width, height, captchaCode);
                Stream s = new MemoryStream(result.CaptchaByteData);
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                option.SameSite = SameSiteMode.None;
                option.Secure = true;
                option.Path = "/";
                //option.HttpOnly = true;
                Response.Cookies.Append("CaptchaCode", captchaCode, option);
                return new FileStreamResult(s, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
