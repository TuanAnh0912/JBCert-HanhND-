using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
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
    public class BaoCaoThongKeController : ControllerBase
    {
        private IBaoCaoThongKe _baoCaoThongKe;
        private NguoiDungProvider _nguoiDungProvider;

        public BaoCaoThongKeController()
        {
            _baoCaoThongKe = new BaoCaoThongKeProvider();
            _nguoiDungProvider = new NguoiDungProvider();
        }

        [Route("GetSoLuongBangDaInTungNam")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetSoLuongBangDaInTungNam()
        {
            ResponseViewModel<SoLuongThongKeBangDaInTungNamViewModel> responseViewModel = new ResponseViewModel<SoLuongThongKeBangDaInTungNamViewModel>();
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

            try
            {
                responseViewModel.Data = _baoCaoThongKe.GetSoLuongBangDaInTungNam(nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                responseViewModel.Message = ex.Message;
            }

            return Ok(responseViewModel);
        }

        [Route("ThongKeVanBangChungChiTheoNam")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongKeVanBangChungChiTheoNam(int? nam)
        {
            ResponseViewModel<ThongKeVanBangChungChiTheoNamViewModel> responseViewModel = new ResponseViewModel<ThongKeVanBangChungChiTheoNamViewModel>();
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

            try
            {
                responseViewModel.Data = _baoCaoThongKe.ThongKeVanBangChungChiTheoNam(nam, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                responseViewModel.Message = ex.Message;
            }

            return Ok(responseViewModel);
        }

        [Route("ThongKeChungTheoDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongKeChungTheoDonVi()
        {
            ResponseViewModel<ThongKeChungTheoDonViViewModel> responseViewModel = new ResponseViewModel<ThongKeChungTheoDonViViewModel>();
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

            try
            {
                responseViewModel.Data = _baoCaoThongKe.ThongKeChungTheoDonVi(nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                responseViewModel.Message = ex.Message;
            }

            return Ok(responseViewModel);
        }
    }
}
