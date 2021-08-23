using jbcert.API.Middleware;
using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.Providers;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class LoaiVanBangController : ControllerBase
    {
        ILoaiVanBang _loaiVanBang;
        NguoiDungProvider _nguoiDungProvider;
        public LoaiVanBangController()
        {
            _loaiVanBang = new LoaiVanBangProvider();
            _nguoiDungProvider = new NguoiDungProvider();
        }

        #region truongdulieu
        [Route("GetTruongDuLieus")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTruongDuLieus(bool? isChungChi)
        {
            ResponseViewModel<List<TruongDuLieuViewModel>> responseViewModel = new ResponseViewModel<List<TruongDuLieuViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetTruongDuLieus(isChungChi, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("AddTruongDuLieu")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddTruongDuLieu(TruongDuLieuViewModel truongDuLieu)
        {
            ResponseViewModel<List<TruongDuLieuViewModel>> responseViewModel = new ResponseViewModel<List<TruongDuLieuViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                truongDuLieu.Code = nguoiDung.DonViId.Value + "-" + Configuration.RandomString(5);
                truongDuLieu.NgayCapNhat = DateTime.Now;
                truongDuLieu.NgayTao = DateTime.Now;
                truongDuLieu.NguoiCapNhat = nguoiDung.NguoiDungId;
                truongDuLieu.NguoiTao = nguoiDung.NguoiDungId;
                truongDuLieu.DonViId = nguoiDung.DonViId;
                _loaiVanBang.AddTruongDuLieu(truongDuLieu);
                responseViewModel.Data = _loaiVanBang.GetTruongDuLieus(truongDuLieu.IsChungChi, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateTruongDuLieu")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTruongDuLieu(TruongDuLieuViewModel truongDuLieu)
        {
            ResponseViewModel<List<TruongDuLieuViewModel>> responseViewModel = new ResponseViewModel<List<TruongDuLieuViewModel>>();
            try
            {

                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                truongDuLieu.NguoiCapNhat = nguoiDung.NguoiDungId;
                truongDuLieu.NguoiTao = nguoiDung.NguoiDungId;
                truongDuLieu.DonViId = nguoiDung.DonViId.Value;
                _loaiVanBang.UpdateTruongDuLieu(truongDuLieu);
                responseViewModel.Data = _loaiVanBang.GetTruongDuLieus(truongDuLieu.IsChungChi, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("DeleteTruongDuLieu")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult DeleteTruongDuLieu(string code, bool? isChungChi)
        {
            ResponseViewModel<List<TruongDuLieuViewModel>> responseViewModel = new ResponseViewModel<List<TruongDuLieuViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                TruongDuLieuViewModel truongDuLieu = _loaiVanBang.GetTruongDuLieu(code, nguoiDung.DonViId.Value);
                if (truongDuLieu != null)
                {
                    truongDuLieu.IsDeleted = true;
                    truongDuLieu.NgayCapNhat = DateTime.Now;
                    truongDuLieu.NguoiCapNhat = nguoiDung.NguoiDungId;
                    truongDuLieu.DonViId = nguoiDung.DonViId.Value;
                    _loaiVanBang.DeleteTruongDuLieu(truongDuLieu);
                }
                responseViewModel.Data = _loaiVanBang.GetTruongDuLieus(isChungChi, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }
        #endregion

        #region LoaiBang
        [Route("AddLoaiBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddLoaiBang(LoaiBangViewModel loaiBang)
        {
            ResponseViewModel<List<TruongDuLieuViewModel>> responseViewModel = new ResponseViewModel<List<TruongDuLieuViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (loaiBang.TruongDuLieuLoaiBangs == null)
                {
                    loaiBang.TruongDuLieuLoaiBangs = new List<TruongDuLieuLoaiBangViewModel>();
                }
                if (loaiBang != null)
                {
                    loaiBang.NgayTao = DateTime.Now;
                    loaiBang.NguoiTao = nguoiDung.NguoiDungId;
                    loaiBang.NgayCapNhat = DateTime.Now;
                    loaiBang.NguoiCapNhat = nguoiDung.NguoiDungId;
                    _loaiVanBang.AddLoaiBang(loaiBang, nguoiDung.DonViId.Value);
                }
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateTruongDuLieuLoaiBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTruongDuLieuLoaiBang(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangs)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (truongDuLieuLoaiBangs != null)
                {
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in truongDuLieuLoaiBangs)
                    {
                        truongDuLieuLoaiBangViewModel.NgayTao = DateTime.Now;
                        truongDuLieuLoaiBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                        truongDuLieuLoaiBangViewModel.NgayCapNhat = DateTime.Now;
                        truongDuLieuLoaiBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    }
                    _loaiVanBang.UpdateTruongDuLieuLoaiBang(truongDuLieuLoaiBangs, nguoiDung.DonViId.Value);
                }
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateTruongDuLieuLoaiBangBanSao")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTruongDuLieuLoaiBangBanSao(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangs)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (truongDuLieuLoaiBangs != null)
                {
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in truongDuLieuLoaiBangs)
                    {
                        truongDuLieuLoaiBangViewModel.NgayTao = DateTime.Now;
                        truongDuLieuLoaiBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                        truongDuLieuLoaiBangViewModel.NgayCapNhat = DateTime.Now;
                        truongDuLieuLoaiBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    }
                    _loaiVanBang.UpdateTruongDuLieuLoaiBangBanSao(truongDuLieuLoaiBangs, nguoiDung.DonViId.Value);
                }
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateTruongDuLieuLoaiBangBanSaoTheoBangGoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTruongDuLieuLoaiBangBanSaoTheoBangGoc(LoaiBangViewModel loaiBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                _loaiVanBang.UpdateTruongDuLieuLoaiBangBanSaoTheoBangGoc(loaiBangViewModel.Id.Value, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiBang(int loaiBangId)
        {
            ResponseViewModel<LoaiBangViewModel> responseViewModel = new ResponseViewModel<LoaiBangViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetLoaiBang(loaiBangId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetLoaiBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiBangs(string ten, int? hinhThucCapId, bool? isChungChi, int currentPage)
        {
            ten = string.IsNullOrEmpty(ten) ? "" : ten;

            ResponseViewModel<LoaiBangsWithPaginationViewModel> responseViewModel = new ResponseViewModel<LoaiBangsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetLoaiBangs(ten, hinhThucCapId, isChungChi, currentPage, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }


        [Route("GetLoaiBangTheoCapDonVis")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiBangTheoCapDonVis(string ten, int? hinhThucCapId)
        {
            ten = string.IsNullOrEmpty(ten) ? "" : ten;

            ResponseViewModel<List<LoaiBangViewModel>> responseViewModel = new ResponseViewModel<List<LoaiBangViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetLoaiBangTheoCapDonVis(ten, hinhThucCapId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetAllLoaiBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllLoaiBangs()
        {
            ResponseViewModel<List<LoaiBangViewModel>> responseViewModel = new ResponseViewModel<List<LoaiBangViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetLoaiBangs();
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateLoaiBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateLoaiBang(LoaiBangViewModel loaiBang)
        {
            ResponseViewModel<LoaiBangViewModel> responseViewModel = new ResponseViewModel<LoaiBangViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (loaiBang.TruongDuLieuLoaiBangs == null)
                {
                    loaiBang.TruongDuLieuLoaiBangs = new List<TruongDuLieuLoaiBangViewModel>();
                }
                loaiBang.NgayCapNhat = DateTime.Now;
                loaiBang.NguoiCapNhat = nguoiDung.NguoiDungId;
                _loaiVanBang.UpdateLoaiBang(loaiBang, nguoiDung.DonViId.Value);
                responseViewModel.Data = _loaiVanBang.GetLoaiBang(loaiBang.Id.Value, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("DeleteLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult DeleteLoaiBang(int loaiBangId)
        {
            ResponseViewModel<LoaiBangViewModel> responseViewModel = new ResponseViewModel<LoaiBangViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                LoaiBangViewModel loaiBang = _loaiVanBang.GetLoaiBang(loaiBangId, nguoiDung.DonViId.Value);
                if (loaiBang != null)
                {
                    loaiBang.NgayCapNhat = DateTime.Now;
                    loaiBang.NguoiCapNhat = nguoiDung.NguoiDungId;
                    _loaiVanBang.DeleteLoaiBang(loaiBang, nguoiDung.DonViId.Value);
                }

                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("AddAnhLoaiBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddAnhLoaiBang(List<LoaiBangFileDinhKemViewModel> loaiBangFileDinhKemViewModels)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                foreach (LoaiBangFileDinhKemViewModel loaiBangFileDinhKemViewModel in loaiBangFileDinhKemViewModels)
                {
                    loaiBangFileDinhKemViewModel.NgayTao = DateTime.Now;
                    loaiBangFileDinhKemViewModel.NguoiTao = nguoiDung.NguoiDungId;
                }
                _loaiVanBang.AddAnhLoaiBang(loaiBangFileDinhKemViewModels, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetAnhLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAnhLoaiBang(int loaiBangId)
        {
            ResponseViewModel<LoaiBangFileDinhKemViewModel> responseViewModel = new ResponseViewModel<LoaiBangFileDinhKemViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _loaiVanBang.GetAnhLoaiBang(loaiBangId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("AddAnhLoaiBangCu")]
        [ClaimRequirement("ALL")]
        [HttpPost]
        public IActionResult AddAnhLoaiBangCu(AttachFileViewModel attachFileViewModel)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (attachFileViewModel != null)
                {
                    attachFileViewModel.NgayTao = DateTime.Now;
                    attachFileViewModel.NguoiTao = nguoiDung.NguoiDungId;
                    attachFileViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    attachFileViewModel.NgayCapNhat = DateTime.Now;
                    attachFileViewModel.IsDeleted = false;
                }
                _loaiVanBang.AddAnhLoaiBangCu(attachFileViewModel, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
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

        [Route("GetAnhLoaiBangCu")]
        [ClaimRequirement("ALL")]
        [HttpGet]
        public IActionResult GetAnhLoaiBangCu(int loaiBangId)
        {
            ResponseViewModel<AttachFileViewModel> responseViewModel = new ResponseViewModel<AttachFileViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetAnhLoaiBangCu(loaiBangId, nguoiDung.DonViId.Value);
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

        [Route("GetLoaiBangsChoTruong")]
        [ClaimRequirement("ALL")]
        [HttpGet]
        public IActionResult GetLoaiBangsChoTruong()
        {
            ResponseViewModel<List<LoaiBangViewModel>> responseViewModel = new ResponseViewModel<List<LoaiBangViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _loaiVanBang.GetLoaiBangsChoTruong(nguoiDung.DonViId.Value);
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

        #endregion


    }
}
