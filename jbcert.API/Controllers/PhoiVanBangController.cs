using jbcert.API.Middleware;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoiVanBangController : ControllerBase
    {
        NhapPhoiProvider _nhapPhoiProvider = new NhapPhoiProvider();
        PhoiProvider _phoiProvider = new PhoiProvider();
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        #region NhapPhoi
        [Route("GetDanhSachNhapPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachNhapPhoi(int? LoaiBangId, int? pageNum, int? pageSize)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var result = _nhapPhoiProvider.GetAll(LoaiBangId, nguoiDung.PhongBan.DonViId.Value);
            if(result != null)
            {
                int sizeOfPage = (pageSize ?? 5);
                int pageNumber = (pageNum ?? 1);
                var res = new ListPagingViewModel();
                res.Total = result.Count();
                res.numberOfPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(res.Total) / sizeOfPage));
                res.listOfObj = result.ToPagedList(pageNumber, sizeOfPage);
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Tải dữ liệu thất bại, vui lòng thử lại sau"));
            }
        }
        [Route("GetChiTietNhapPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietNhapPhoi(int id, int pageSize, int currentPage)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var result = _nhapPhoiProvider.GetChiTietNhapPhoi(id, nguoiDung.PhongBan.DonViId.Value, pageSize, currentPage);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Ok(new ResultModel(false, "Tải dữ liệu thất bại, vui lòng thử lại sau"));
            }
        }
        [Route("NhapPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult NhapPhoi(NhapPhoiViewModel model)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                model.NguoiTao = nguoiDung.NguoiDungId;
                model.DonViId = nguoiDung.PhongBan.DonViId.Value;
                return Ok(_nhapPhoiProvider.InsertNhapPhoi(model, ip));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetLichSuPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLichSuPhoi(int phoiId)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_phoiProvider.GetLichSuPhoi(phoiId));
        }

        [Route("GetLogPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLogPhoi(int phoiId)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_phoiProvider.GetLogPhoi(phoiId));
        }

        [Route("CapNhatNhapPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult CapNhatNhapPhoi(NhapPhoiViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_nhapPhoiProvider.UpdateNhapPhoi(model));
        }
        #endregion
        #region SoHieu

        [Route("GetAllPhoiVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllPhoiVanBang(string soHieu, int ? trangThaiId, int? pageNum, int? pageSize, int? loaiBangId, int? nam)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var result = _phoiProvider.GetAllPhoiVanBang(soHieu, trangThaiId, loaiBangId, nam, pageNum.Value, pageSize.Value, nguoiDung.DonViId.Value);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Ok(new ResultModel(false, "Tải dữ liệu thất bại, vui lòng thử lại sau"));
            }
        }
        [Route("NhapSoHieu")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult NhapSoHieu(PhoiVanBangViewModel model)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                model.NguoiTao = nguoiDung.NguoiDungId;
                model.DonViId = nguoiDung.PhongBan.DonViId.Value;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                return Ok(_phoiProvider.InsertPhoi(model, ip));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("NhapSoHieus")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult NhapSoHieus(List<PhoiVanBangViewModel> models)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                foreach (var model in models)
                {
                    model.NguoiTao = nguoiDung.NguoiDungId;
                    model.DonViId = nguoiDung.PhongBan.DonViId.Value;
                }
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                return Ok(_phoiProvider.InsertPhois(models, ip));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("CapNhatPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult CapNhatPhoi(PhoiVanBangViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiTao = nguoiDung.NguoiDungId;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(_phoiProvider.UpdatePhoiV2(model, ip));
        }
        #endregion

        [Route("ThongSoPhoiNamCu")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongSoPhoiNamCu(int nam, int loaiBangId)
        {
            ResponseViewModel<ThongSoPhoiNamCuViewModel> responseViewModel = new ResponseViewModel<ThongSoPhoiNamCuViewModel>();
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _phoiProvider.ThongSoPhoiNamCu(nam - 1, loaiBangId, nguoiDung.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("ThongSoDeNghiCapPhoiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongSoDeNghiCapPhoiBang(int nam, int loaiBangId)
        {
            ResponseViewModel<ThongSoDeNghiCapPhoiBangViewModel> responseViewModel = new ResponseViewModel<ThongSoDeNghiCapPhoiBangViewModel>();
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _phoiProvider.ThongSoDeNghiCapPhoiBang(nam, loaiBangId, nguoiDung.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        /// <summary>
        /// Thêm file vào yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertFile2Phoi")]
        [HttpPost]
        [ClaimRequirement("All")]
        public IActionResult InsertFile2Phoi(List<PhoiFileDinhKemViewModel> model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            foreach (var file in model)
            {
                file.NguoiTao = nguoiDung.NguoiDungId;
            }
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(_phoiProvider.InsertFile(model, ip));
        }

        /// <summary>
        /// Xóa 1 file trong yêu cầu 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileDinhKemPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteFileDinhKemPhoi(PhoiFileDinhKemViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiTao = nguoiDung.NguoiDungId;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var result = _phoiProvider.DeleteFile(model, ip);
            if (result.Status)
            {
                string path = Directory.GetCurrentDirectory() + model.Url;
                FileInfo file = new FileInfo(path);
                if (file.Exists)//check file exsit or not
                {
                    file.Delete();
                }

            }
            return Ok(result);
        }

        [Route("GetThongSoPhoiTheoNamVaLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetThongSoPhoiTheoNamVaLoaiBang(int? loaiBangId, int? nam)
        {
            try
            {
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                return Ok(_phoiProvider.GetThongSoPhoiTheoNamVaLoaiBang(loaiBangId, nam, nguoiDung.DonViId.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
