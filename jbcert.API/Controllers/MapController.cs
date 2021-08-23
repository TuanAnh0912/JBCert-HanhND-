using jbcert.API.Middleware;
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
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class MapController : ControllerBase
    {
        IDiaGioiHanhChinh _diaGioiHanhChinh;
        NguoiDungProvider _nguoiDungProvider;
        public MapController()
        {
            _diaGioiHanhChinh = new DiaGioiHanhChinhProvider();
            _nguoiDungProvider = new NguoiDungProvider();
        }
        [Route("GetTinhs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetTinhs(string name)
        {
            ResponseViewModel<List<TinhViewModel>> responseViewModel = new ResponseViewModel<List<TinhViewModel>>();
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }
            try
            {
                responseViewModel.Data = _diaGioiHanhChinh.GetTinhs(name);
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
        [Route("GetHuyens")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetHuyens(string id, string name)
        {
            ResponseViewModel<List<HuyenViewModel>> responseViewModel = new ResponseViewModel<List<HuyenViewModel>>();
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            try
            {
                responseViewModel.Data = _diaGioiHanhChinh.GetHuyens(id, name);
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
        [Route("GetXas")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetXas(string id, string name)
        {
            ResponseViewModel<List<XaViewModel>> responseViewModel = new ResponseViewModel<List<XaViewModel>>();
            if (string.IsNullOrEmpty(id))
            {
                id = "";
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "";
            }

            try
            {
                responseViewModel.Data = _diaGioiHanhChinh.GetXas(id, name);
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
        public IActionResult GetAllLoaiBang()
        {
            NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            List<LoaiBangViewModel> lst = new LoaiVanBangProvider().GetLoaiBangs();
           
            return Ok(lst.Where(x => nguoiDung.DonVi.CapDonVi.Level < x.Level || nguoiDung.DonVi.CapDonVi.Code == x.CodeCapDonVi).ToList());
        }
    }
}
