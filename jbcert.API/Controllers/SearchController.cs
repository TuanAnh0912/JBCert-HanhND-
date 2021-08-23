using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
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
    public class SearchController : ControllerBase
    {
        SearchProvider _searchProvider;
        IHocSinh _hocSinhProvider;

        public SearchController()
        {
            _hocSinhProvider = new HocSinhProvider();
            _searchProvider = new SearchProvider();
        }

        [Route("SearchBang")]
        //[ClaimRequirement("All")]
        [HttpGet]
        public IActionResult SearchBang(string keyword, int pageNum, int pageSize, string captcha)
        {
            ResponseViewModel<SearchedCertsWithPaginationViewModel> responseViewModel = new ResponseViewModel<SearchedCertsWithPaginationViewModel>();
            try
            {
                string captchaCookie = Request.Cookies["CaptchaCode"];
                if (string.IsNullOrEmpty(captchaCookie))
                {
                    return BadRequest("Cookie hết hạn");
                }
                else if (captchaCookie != captcha)
                {
                    return BadRequest("Sai captcha");
                }
                responseViewModel.Data = _searchProvider.SearchBang(keyword, pageNum, pageSize);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(responseViewModel);
        }
        [Route("AdvancedSearch")]
        //[ClaimRequirement("All")]
        [HttpGet]
        public IActionResult SearchAdvanceBang(string keyword, int pageNum, int pageSize, string captcha, string hvt, int? nam, int? idtruong)
        {
            ResponseViewModel<SearchedCertsWithPaginationViewModel> responseViewModel = new ResponseViewModel<SearchedCertsWithPaginationViewModel>();
            try
            {

                string captchaCookie = Request.Cookies["CaptchaCode"];
                if (string.IsNullOrEmpty(captchaCookie))
                {
                    return BadRequest("Cookie hết hạn");
                }
                else if (captchaCookie != captcha)
                {
                    return BadRequest("Sai captcha");
                }
                responseViewModel.Data = _searchProvider.AdvancedSearch(keyword, pageNum, pageSize, hvt, nam, idtruong);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(responseViewModel);
        }


        [Route("Detail")]
        //[ClaimRequirement("All")]
        [HttpGet]
        public IActionResult Detail(string keyword, int bangId)
        {
            ResponseViewModel<SearchedCertsViewModel> responseViewModel = new ResponseViewModel<SearchedCertsViewModel>();
            try
            {
                SearchedCertsViewModel searchedCertsViewModel = _searchProvider.Detail(keyword, bangId);
                responseViewModel.Data = searchedCertsViewModel;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(responseViewModel.Data);
        }

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
                option.Expires = DateTime.Now.AddMinutes(2);
                option.SameSite = SameSiteMode.None;
                option.Secure = true;
                //option.Path = "/";
                //option.HttpOnly = true;
                Response.Cookies.Append("CaptchaCode", captchaCode, option);
                return new FileStreamResult(s, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDanhSachTruongHoc")]
        [HttpGet]
        public IActionResult GetDanhSachTruongHoc(string tenDonVi, int maximumReturn)
        {
            try
            {
                return Ok(_searchProvider.GetDanhSachTruongHoc(tenDonVi, maximumReturn));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
