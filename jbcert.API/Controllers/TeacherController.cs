using jbcert.API.Middleware;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class TeacherController : ControllerBase
    {
        GiaoVienProvider _giaoVienProvider = new GiaoVienProvider();
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        LopHocProvider _lopHocProvider = new LopHocProvider();

        [Route("GetGiaoVien")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetGiaoVien(string keyword, int? DonViId)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "";
            }
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _giaoVienProvider.GetGiaoViens(keyword, nguoiDung.DonViId);
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Có lỗi xảy ra khi tải dữ liệu, vui lòng thử lại"));
            }
        }
        [Route("GetAllLopHoc")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllLopHoc(Guid? GiaoVienId, string nienKhoa)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _lopHocProvider.Gets(GiaoVienId, nienKhoa, nguoiDung.PhongBan.DonViId.Value);
            if(res != null)
            {
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Có lỗi xảy ra khi tải dữ liệu, vui lòng thử lại"));
            }
        }
        [Route("InsertLopHoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertLopHoc(LopHocViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.DonViId = nguoiDung.DonViId.Value;
            return Ok(_lopHocProvider.Insert(model));
        }
        [Route("UpdateLopHoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateLopHoc(LopHocViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_lopHocProvider.Update(model));
        }
    }
}
