using jbcert.API.Middleware;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaiChinhController : ControllerBase
    {
        CaiChinhProvider _caiChinhProvider = new CaiChinhProvider();
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        [Route("CaiChinhVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult CaiChinhVanBang(CaiChinhVanBangViewModel model)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                model.NguoiThucHien = nguoiDung.NguoiDungId;
                model.DonViId = nguoiDung.PhongBan.DonViId.Value;
                return Ok(_caiChinhProvider.CaiChinhVanBang(model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetAllVanBangCaiChinh")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllVanBangCaiChinh(string keyword, int? pageNum, int? pageSize)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            int sizeOfPage = (pageSize ?? 5);
            int pageNumber = (pageNum ?? 1);
            var res = new ListPagingViewModel();
            var result = _caiChinhProvider.GetVanBangDaCaiChinh(keyword, nguoiDung.DonViId.Value);
            res.Total = result.Count();
            res.numberOfPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(res.Total) / sizeOfPage));
            res.listOfObj = result.ToPagedList(pageNumber, sizeOfPage);
            return Ok(res);
        }
        [Route("GetChiTietVanBangCaiChinh")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietVanBangCaiChinh(int id)
        {
            return Ok(_caiChinhProvider.GetChiTietVanBangCaiChinh(id));
        }
    }
}
