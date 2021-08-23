using jbcert.API.Hubs;
using jbcert.API.Hubs.Clients;
using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _chatHub;
        NguoiDungProvider _nguoiDungProvider;
        INotification _notificationProvider;

        public NotificationController(IHubContext<NotificationHub, INotificationClient> chatHub)
        {
            _chatHub = chatHub;
            _nguoiDungProvider = new NguoiDungProvider();
            _notificationProvider = new NotificationProvider();
        }

        [HttpPost("messages")]
        public async Task Post(NotificationMessage message)
        {
            await _chatHub.Clients.All.ReceiveMessage(message);
        }

        //Configuration
        [Route("GetPhongBansChoThongBaoType")]
        [HttpGet]
        [ClaimRequirement("All")]
        public IActionResult GetPhongBansChoThongBaoType(int thongBaoTypeId)
        {

            try
            {
                ResponseViewModel<List<RoomViewModel>> responseViewModel = new ResponseViewModel<List<RoomViewModel>>();
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _notificationProvider.GetPhongBansChoThongBaoType(thongBaoTypeId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdatePhongBansTrongThongBaoType")]
        [HttpPost]
        [ClaimRequirement("All")]
        public IActionResult UpdatePhongBansTrongThongBaoType(UpdatePhongBansTrongThongBaoTypeViewModel updatePhongBansTrongThongBaoTypeViewModel)
        {
            try
            {
                ResponseViewModel<List<ThongBaoTypeViewModel>> responseViewModel = new ResponseViewModel<List<ThongBaoTypeViewModel>>();
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _notificationProvider.UpdatePhongBansTrongThongBaoType(updatePhongBansTrongThongBaoTypeViewModel, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetThongBaosByPhongBan")]
        [HttpGet]
        [ClaimRequirement("All")]
        public IActionResult GetThongBaosByPhongBan(int currentPage, int pageSize)
        {
            try
            {
                ResponseViewModel<ThongBaoWithPaginationViewModel> responseViewModel = new ResponseViewModel<ThongBaoWithPaginationViewModel>();
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                
                responseViewModel.Data = _notificationProvider.GetThongBaosByPhongBan(currentPage, pageSize, nguoiDung.PhongBan.PhongBanId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateDaDoc")]
        [HttpPost]
        [ClaimRequirement("All")]
        public IActionResult UpdateDaDoc(ThongBaoViewModel thongBaoViewModel)
        {
            try
            {
                ResponseViewModel<ThongBaoWithPaginationViewModel> responseViewModel = new ResponseViewModel<ThongBaoWithPaginationViewModel>();
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _notificationProvider.UpdateDaDoc(thongBaoViewModel, nguoiDung.PhongBan.PhongBanId, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateDaDocAll")]
        [HttpPost]
        [ClaimRequirement("All")]
        public IActionResult UpdateDaDocAll()
        {
            try
            {
                ResponseViewModel<ThongBaoWithPaginationViewModel> responseViewModel = new ResponseViewModel<ThongBaoWithPaginationViewModel>();
                var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _notificationProvider.UpdateDaDocAll(nguoiDung.PhongBan.PhongBanId, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
