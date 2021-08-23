using jbcert.API.Middleware;
using jbcert.DATA.Helpers;
using jbcert.DATA.IdentityModels;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class SystemController : ControllerBase
    {
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        DonViProvider donViProvider = new DonViProvider();
        private readonly UserManager<ApplicationUser> _userManager;
        public SystemController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #region ChucNang
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("GetChucNangByNguoiDung")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChucNangByNguoiDung()
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            if (user != null)
            {
                return Ok(new ChucNangProvider().GetByNhomNguoiDungId(user.NhomNguoiDungId.Value));
            }
            else
            {
                return Ok(new ResultModel(false, "Không tìm thấy người dùng!"));
            }
        }
        /// <summary>
        /// Get all chức năng
        /// </summary>
        /// <returns></returns>
        [Route("GetAllChucNang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllChucNang()
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(new ChucNangProvider().GetAll(user.DonVi.CapDonViId));
        }
        #endregion
        #region NguoiDung
        /// <summary>
        /// Insert người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertNguoiDung")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertNguoiDung(NguoiDungViewModel model)
        {
            return Ok(_nguoiDungProvider.InsertUser(model));
        }
        /// <summary>
        /// cập nhật người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateNguoiDung")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateNguoiDung(NguoiDungViewModel model)
        {
            return Ok(_nguoiDungProvider.UpdateUser(model));
        }
        /// <summary>
        /// Get all người dùng
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("GetAllNguoiDung")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllNguoiDung(string keyword, int? pageNum, int? pageSize)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
            var result = new ListReturnViewModel();
            if (user.DonVi.TenDonVi.ToLower() == "jbtech")
            {
                result = _nguoiDungProvider.GetAll(keyword, null);
            }
            else
            {
                result = _nguoiDungProvider.GetAll(keyword, user.PhongBan.DonViId.Value);
            }
            if (result.Status)
            {
                int sizeOfPage = (pageSize ?? 5);
                int pageNumber = (pageNum ?? 1);
                var res = new ListPagingViewModel();
                res.Total = result.Data.Count();
                res.numberOfPage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(res.Total) / sizeOfPage));
                res.listOfObj = result.Data.ToPagedList(pageNumber, sizeOfPage);
                return Ok(res);
            }
            else
            {
                return Ok(result);
            }

        }
        /// <summary>
        /// Get người dùng by id
        /// </summary>
        /// <param name="NguoiDungId"></param>
        /// <returns></returns>
        [Route("GetNguoiDungById")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNguoiDungById(string NguoiDungId)
        {
            var res = _nguoiDungProvider.GetNguoiDungById(NguoiDungId);
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Không tìm thấy người dùng"));
            }
        }
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DoiMatKhau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));
            var a = await _userManager.ChangePasswordAsync(user, model.MatKhau, model.MatKhauMoi);
            if (a.Succeeded)
            {
                return Ok(new ResultModel(true, "Đổi mật khẩu thành công!"));
            }
            else
            {
                return Ok(new ResultModel(false, "Mật khẩu cũ không chính xác!"));
            }
        }
        #endregion
        #region LOG
        /// <summary>
        /// Insert User Log action
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Route("InsertLog")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertLog(LogNguoiDungViewModel mode)
        {
            return Ok(new LogNguoiDungProvider().InsertLog(mode));
        }
        #endregion
        #region NhomNguoiDung
        /// <summary>
        /// Get Nhóm người dùng by phòng ban
        /// </summary>
        /// <param name="PhongBanId"></param>
        /// <returns></returns>
        [Route("GetNhomNguoiDungByDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomNguoiDungByDonVi(int? phongBanId)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            if (user.DonVi.TenDonVi.ToLower() == "jbtech")
            {
                return Ok(new NhomNguoiDungProvider().GetAll(null, phongBanId));
            }
            else
            {
                return Ok(new NhomNguoiDungProvider().GetAll(user.DonViId, phongBanId));
            }

        }
        /// <summary>
        /// Thêm mới nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertNhomNguoiDung")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertNhomNguoiDung(NhomNguoiDungViewModel model)
        {
            return Ok(new NhomNguoiDungProvider().Insert(model));
        }
        /// <summary>
        /// Update Nhom nguoi dung
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateNhom")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateNhom(NhomNguoiDungViewModel model)
        {
            return Ok(new NhomNguoiDungProvider().Update(model));
        }
        /// <summary>
        /// Update Quyền nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateQuyenNhomNguoiDung")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateQuyenNhomNguoiDung(NhomNguoiDungViewModel model)
        {
            return Ok(new ChucNangProvider().UpdateLienKetChucNangNhomNguoiDung(model));
        }
        /// <summary>
        /// Get chi tiết nhóm người dùng
        /// </summary>
        /// <param name="NhomNguoiDungId"></param>
        /// <returns></returns>
        [Route("GetNhomNguoiDungById")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomNguoiDungById(int NhomNguoiDungId)
        {
            return Ok(new NhomNguoiDungProvider().GetByNhomNguoiDungId(NhomNguoiDungId));
        }
        #endregion
        #region PhongBan
        /// <summary>
        /// Get phòng ban by Đơn vị
        /// </summary>
        /// <param name="DonViId"></param>
        /// <returns></returns>
        [Route("GetPhongBanByDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetPhongBanByDonVi(int? DonViId)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            if (user.DonVi.TenDonVi.ToLower() == "jbtech")
            {
                return Ok(new PhongBanProvider().GetAll(null));
            }
            else
            {
                return Ok(new PhongBanProvider().GetAll(user.PhongBan.DonViId));
            }
        }
        /// <summary>
        /// Thêm mới phòng ban
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertPhongBan")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertPhongBan(PhongBanViewModel model)
        {
            return Ok(new PhongBanProvider().Insert(model));
        }
        /// <summary>
        /// Cập nhật phòng ban
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdatePhongBan")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdatePhongBan(PhongBanViewModel model)
        {
            return Ok(new PhongBanProvider().Update(model));
        }
        #endregion
        #region DonVi
        /// <summary>
        /// Get danh sách đơn vị
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="DiaGioiHC"></param>
        /// <returns></returns>
        [Route("GetAllDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllDonVi(string keyword, string DiaGioiHC)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
            DiaGioiHC = string.IsNullOrEmpty(DiaGioiHC) ? "" : DiaGioiHC;
            if (user.DonVi.TenDonVi.ToLower() == "jbtech")
            {
                return Ok(new { data = new DonViProvider().GetAll(keyword, DiaGioiHC) , isBo = true});
            }
            else
            {
                return Ok(new { data = new DonViProvider().GetByNguoiDung(user) , isBo = false});
            }
        }

        [Route("GetDonViByNguoiDung")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDonViByNguoiDung(string keyword, string DiaGioiHC)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(new DonViProvider().GetDonViByNguoiDung(user));
        }
        /// <summary>
        /// Get all cấp đơn vị
        /// </summary>
        /// <returns></returns>
        [Route("GetAllCapDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllCapDonVi()
        {
            return Ok(new DonViProvider().GetAllCapDonVi());
        }
        /// <summary>
        /// Thêm mới đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertDonVi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertDonVi(DonViViewModel model)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            if (!model.KhoaChaId.HasValue)
            {
                model.KhoaChaId = user.DonViId.Value;
            }
            model.MaDonVi = string.IsNullOrEmpty(model.MaDonVi) ? Configuration.RandomString(6) : model.MaDonVi;
            return Ok(new DonViProvider().Insert(model));
        }
        /// <summary>
        /// Cập nhật đơn vị
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateDonVi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateDonVi(DonViViewModel model)
        {
            return Ok(new DonViProvider().Update(model));
        }

        [Route("GetCodeCapDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCodeCapDonVi()
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = user.DonVi.CapDonVi.Code;
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetTruongHocsByDonViCha")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTruongHocsByDonViCha()
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = donViProvider.GetTruongHocsByDonViCha(user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetTruongHocsByDonViChaVaKieuBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTruongHocsByDonViChaVaKieuBang(bool isChungChi)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = donViProvider.GetTruongHocsByDonViChaVaKieuBang(user.DonViId.Value, isChungChi);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }


        [Route("GetPhongGiaoDucsBySo")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetPhongGiaoDucsBySo()
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = donViProvider.GetPhongGiaoDucsBySo(user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        #endregion
    }
}
