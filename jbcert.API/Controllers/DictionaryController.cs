using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.Providers;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class DictionaryController : ControllerBase
    {
        private ITuDien _tudien;
        private NguoiDungProvider _nguoiDungProvider;

        public DictionaryController()
        {
            _tudien = new TuDienProvider();
            _nguoiDungProvider = new NguoiDungProvider();
        }

        [Route("GetLoaiNhomVanTaoBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiNhomVanTaoBangs()
        {
            ResponseViewModel<List<LoaiNhomTaoVanBangViewModel>> responseViewModel = new ResponseViewModel<List<LoaiNhomTaoVanBangViewModel>>();

            try
            {
                responseViewModel.Data = _tudien.GetLoaiNhomVanBangs();
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

        [Route("GetAllLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllLoaiBang(bool? IsChungChi)
        {
            NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            List<LoaiBangViewModel> lst = new LoaiVanBangProvider().GetLoaiBangs(IsChungChi);

            return Ok(lst.Where(x => nguoiDung.DonVi.CapDonVi.Level < x.Level || nguoiDung.DonVi.CapDonVi.Code == x.CodeCapDonVi).ToList());
        }

        [Route("GetNamTotNghiepsByTruong")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNamTotNghiepsByTruong(int truongHocId)
        {
            ResponseViewModel<List<int>> responseViewModel = new ResponseViewModel<List<int>>();

            try
            {
                responseViewModel.Data = _tudien.GetNamTotNghiepsByTruong(truongHocId);
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

        [Route("GetNamTotNghiepByDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNamTotNghiepByDonVi()
        {
            ResponseViewModel<GetNamTotNghiepByDonViViewModel> responseViewModel = new ResponseViewModel<GetNamTotNghiepByDonViViewModel>();
            NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

            try
            {
                responseViewModel.Data = _tudien.GetNamTotNghiepByDonVi(nguoiDung.DonViId.Value);
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

        [Route("GetCodeCapDonViByNguoiDung")]
        [ClaimRequirement("GetCodeCapDonViByNguoiDung")]
        [HttpGet]
        public IActionResult GetCodeCapDonViByNguoiDung()
        {
            ResponseViewModel<string> responseViewModel = new ResponseViewModel<string>();

            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _tudien.GetCodeCapDonViByNguoiDung(nguoiDung.DonViId.Value);
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

        [Route("GetGioiTinhs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetGioiTinhs()
        {
            ResponseViewModel<List<GioiTinhViewModel>> responseViewModel = new ResponseViewModel<List<GioiTinhViewModel>>();

            try
            {
                responseViewModel.Data = _tudien.GetGioiTinhs();
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

        [Route("GetDanTocs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanTocs()
        {
            ResponseViewModel<List<DanTocViewModel>> responseViewModel = new ResponseViewModel<List<DanTocViewModel>>();

            try
            {
                responseViewModel.Data = _tudien.GetDanTocs();
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

        [Route("GethinhThucCaps")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GethinhThucCaps()
        {
            ResponseViewModel<List<HinhThucCapViewModel>> responseViewModel = new ResponseViewModel<List<HinhThucCapViewModel>>();
            try
            {
                responseViewModel.Data = _tudien.GetHinhThucCaps();
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

        [Route("GetTrangThaiPhois")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTrangThaiPhois()
        {
            ResponseViewModel<List<TrangThaiPhoiViewModel>> responseViewModel = new ResponseViewModel<List<TrangThaiPhoiViewModel>>();
            try
            {
                responseViewModel.Data = _tudien.GetTrangThaiPhois();
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

        [Route("GetTrangThaiBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTrangThaiBangs()
        {
            ResponseViewModel<List<TrangThaiBangViewModel>> responseViewModel = new ResponseViewModel<List<TrangThaiBangViewModel>>();
            try
            {
                responseViewModel.Data = _tudien.GetTrangThaiBangs();
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

        [Route("GetTrangThaiBangChuaIn")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTrangThaiBangChuaIn()
        {
            ResponseViewModel<List<TrangThaiBangViewModel>> responseViewModel = new ResponseViewModel<List<TrangThaiBangViewModel>>();
            try
            {
                responseViewModel.Data = _tudien.GetTrangThaiBangs().Where(x => x.Id <= 3).ToList();
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

        [Route("GetTruongDuLieusThongTinHocSinh")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTruongDuLieusThongTinHocSinh()
        {
            ResponseViewModel<List<string>> responseViewModel = new ResponseViewModel<List<string>>();
            try
            {
                responseViewModel.Data = _tudien.GetTruongDuLieusThongTinHocSinh();
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

        [Route("GetXepLoaiTotNghiepByCapDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetXepLoaiTotNghiepByCapDonVi()
        {
            ResponseViewModel<List<XepLoaiTotNghiepViewModel>> responseViewModel = new ResponseViewModel<List<XepLoaiTotNghiepViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _tudien.GetXepLoaiTotNghiepByCapDonVi(nguoiDung.DonVi.CapDonVi.Code);
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

        [Route("GetMauCongVans")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetMauCongVans(int loaiYeuCauId)
        {
            ResponseViewModel<List<MauCongVanViewModel>> responseViewModel = new ResponseViewModel<List<MauCongVanViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _tudien.GetMauCongVan(loaiYeuCauId);
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

        [Route("GetCapDonVis")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCapDonVis()
        {
            ResponseViewModel<List<CapDonViViewModel>> responseViewModel = new ResponseViewModel<List<CapDonViViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _tudien.GetAllCapDonVis();
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

        [Route("GetLoaiBangTheoHinhThuc")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiBangTheoHinhThuc(int? hinhThucCapId)
        {
            ResponseViewModel<List<LoaiBangViewModel>> responseViewModel = new ResponseViewModel<List<LoaiBangViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _tudien.GetLoaiBangTheoHinhThuc(hinhThucCapId);
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

        [Route("GetLoaiYeuCauMuaPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiYeuCauMuaPhoi()
        {
            ResponseViewModel<List<LoaiYeuCauViewModel>> responseViewModel = new ResponseViewModel<List<LoaiYeuCauViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                List<LoaiYeuCauViewModel> lst = _tudien.GetLoaiYeuCauMuaPhoi();
                foreach (var item in lst)
                {
                    item.LoaiBangs = item.LoaiBangs.Where(x => nguoiDung.DonVi.CapDonVi.Level < x.Level || nguoiDung.DonVi.CapDonVi.Code == x.CodeCapDonVi).ToList();
                }
                responseViewModel.Data = lst;
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
        [Route("GetCapDonViForAddDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCapDonViForAddDonVi()
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = new DonViProvider().GetCapDonVis(user.PhongBan.DonVi.CapDonVi.Level);
            return Ok(res);
        }
        [Route("GetDonVis")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDonVis()
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = new DonViProvider().GetDonVis(user.PhongBan.DonVi.DonViId);
            return Ok(res);
        }
        [Route("GetPhongBanByDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetPhongBanByDonVi(int DonViId)
        {
            var res = new PhongBanProvider().GetByDonViId(DonViId);
            return Ok(res);
        }

        [Route("GetNhomNguoiDungByPhongBanId")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomNguoiDungByPhongBanId(int PhongBanId)
        {
            var res = new NhomNguoiDungProvider().GetByPhongBanId(PhongBanId);
            return Ok(res);
        }

        [Route("GetLopHocByNam")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLopHocByNam(string nam)
        {
            ResponseViewModel<List<LopHocViewModel>> responseViewModel = new ResponseViewModel<List<LopHocViewModel>>();
            try
            {
                if (string.IsNullOrEmpty(nam))
                {
                    responseViewModel.Data = new List<LopHocViewModel>();
                    responseViewModel.Status = true;
                    responseViewModel.Message = "";
                }
                else
                {
                    NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                    responseViewModel.Data = _tudien.GetLopHocByNam(nam, nguoiDung.DonViId.Value);
                    responseViewModel.Status = true;
                    responseViewModel.Message = "";
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetLopHocByNamVaDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLopHocByNamVaDonVi(int nam, int truongHocId)
        {
            ResponseViewModel<List<LopHocViewModel>> responseViewModel = new ResponseViewModel<List<LopHocViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _tudien.GetLopHocByNamVaDonVi(nam, truongHocId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetCodeCapDonViByDonViId")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCodeCapDonViByDonViId(int donViId)
        {
            ResponseViewModel<DonViViewModel> responseViewModel = new ResponseViewModel<DonViViewModel>();
            try
            {
                responseViewModel.Data = _tudien.GetCodeCapDonViByDonViId(donViId);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetDonViByDonViCha")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDonViByDonViCha()
        {
            ResponseViewModel<List<DonViViewModel>> responseViewModel = new ResponseViewModel<List<DonViViewModel>>();
            try
            {
                    NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                    responseViewModel.Data = _tudien.GetDonViByDonViCha(nguoiDung.DonViId.Value);
                    responseViewModel.Status = true;
                    responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetAllDonViByDonViCha")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllDonViByDonViCha(int? donViChaId)
        {
            ResponseViewModel<List<DonViViewModel>> responseViewModel = new ResponseViewModel<List<DonViViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                int donViId = donViChaId.HasValue ? donViChaId.Value : nguoiDung.DonViId.Value;
                responseViewModel.Data = _tudien.GetAllDonViByDonViCha(donViId);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetTruongHocByDonViChaVaNhomTaoVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTruongHocByDonViChaVaNhomTaoVanBang()
        {
            ResponseViewModel<List<DonViViewModel>> responseViewModel = new ResponseViewModel<List<DonViViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _tudien.GetTruongHocByDonViChaVaNhomTaoVanBang(nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetThongBaoTypes")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetThongBaoTypes()
        {
            ResponseViewModel<List<ThongBaoTypeViewModel>> responseViewModel = new ResponseViewModel<List<ThongBaoTypeViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _tudien.GetThongBaoTypes(nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetCapDonVisForUpdating")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCapDonVisForUpdating(int? currentDonViId)
        {
            ResponseViewModel<List<CapDonViViewModel>> responseViewModel = new ResponseViewModel<List<CapDonViViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _tudien.GetCapDonVisForUpdating(currentDonViId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetCurrentDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetCurrentDonVi(int? currentDonViId)
        {
            ResponseViewModel<List<DonViViewModel>> responseViewModel = new ResponseViewModel<List<DonViViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _tudien.GetCurrentDonVi(currentDonViId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }
    }
}