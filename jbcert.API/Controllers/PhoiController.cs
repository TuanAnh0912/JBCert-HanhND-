using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
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
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class PhoiController : ControllerBase
    {
        IPhoi _phoiProvider;
        NguoiDungProvider _nguoiDungProvider;
        public PhoiController()
        {
            _phoiProvider = new PhoiProvider();
            _nguoiDungProvider = new NguoiDungProvider();
        }

        [Route("AddPhois")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddPhois(AddPhoiViewModel addPhoiViewModel)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (addPhoiViewModel != null)
                {
                    addPhoiViewModel.NgayTao = DateTime.Now;
                    addPhoiViewModel.NguoiTao = nguoiDung.NguoiDungId;
                    addPhoiViewModel.NgayCapNhat = DateTime.Now;
                    addPhoiViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    _phoiProvider.AddPhois(addPhoiViewModel, nguoiDung.DonViId.Value);
                }

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

        [Route("UpdatePhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdatePhoi(PhoiViewModel phoiViewModel)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (phoiViewModel != null)
                {
                    phoiViewModel.NgayCapNhat = DateTime.Now;
                    phoiViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    _phoiProvider.UpdatePhoi(phoiViewModel, nguoiDung.DonViId.Value);
                }

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

        [Route("AddAnhPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddAnhPhoi(List<AttachFileViewModel> attachFileViewModels)
        {
            ResponseViewModel<List<AttachFileViewModel>> responseViewModel = new ResponseViewModel<List<AttachFileViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (attachFileViewModels != null)
                {
                    foreach (AttachFileViewModel attachFileViewModel in attachFileViewModels)
                    {
                        attachFileViewModel.NgayTao = DateTime.Now;
                        attachFileViewModel.NguoiTao = nguoiDung.NguoiDungId;
                        attachFileViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                        attachFileViewModel.NgayCapNhat = DateTime.Now;
                        attachFileViewModel.IsDeleted = false;
                    }
                }
                _phoiProvider.AddAnhPhoi(attachFileViewModels, nguoiDung.DonViId.Value);
                responseViewModel.Data = _phoiProvider.GetAnhPhois(attachFileViewModels.FirstOrDefault().ObjectId, nguoiDung.DonViId.Value); ;
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

        [Route("DeleteAnhPhoi")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteAnhPhoi(List<AttachFileViewModel> attachFileViewModels)
        {
            ResponseViewModel<List<AttachFileViewModel>> responseViewModel = new ResponseViewModel<List<AttachFileViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                foreach (AttachFileViewModel attachFileViewModel in attachFileViewModels)
                {
                    attachFileViewModel.NgayCapNhat = DateTime.Now;
                    attachFileViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                    attachFileViewModel.IsDeleted = true;
                }

                _phoiProvider.DeleteAnhPhoi(attachFileViewModels, nguoiDung.DonViId.Value);
                responseViewModel.Data = _phoiProvider.GetAnhPhois(attachFileViewModels.FirstOrDefault().ObjectId, nguoiDung.DonViId.Value);
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

        [Route("GetPhoi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetPhoi(int phoiId)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _phoiProvider.GetPhoi(phoiId, nguoiDung.DonViId.Value); ;
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

        [Route("GetPhoisTrongLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetPhoisTrongLoaiBang(int loaiBangId, int trangThaiPhoiId, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _phoiProvider.GetPhoisTrongLoaiBang(loaiBangId, trangThaiPhoiId, nguoiDung.DonViId.Value, currentPage); ;
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


        [Route("GetTongSoPhoiTungLoaiPhoiTrongLoaiBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTongSoPhoiTungLoaiPhoiTrongLoaiBang(int loaiBangId)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _phoiProvider.GetTongSoPhoiTungTrangThaiPhoiTrongLoaiBang(loaiBangId, nguoiDung.DonViId.Value);
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

        [Route("GetSoPhoiNhanTheoTungThang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetSoPhoiNhanTheoTungThang(int loaiBangId, int year, int month)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _phoiProvider.GetSoPhoiNhanTheoTungThang(loaiBangId, year, month, nguoiDung.DonViId.Value);
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
