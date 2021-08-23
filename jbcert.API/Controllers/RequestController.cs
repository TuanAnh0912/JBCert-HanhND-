using jbcert.API.Hubs;
using jbcert.API.Hubs.Clients;
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
using Microsoft.AspNetCore.SignalR;
using Spire.Xls;
using Spire.Xls.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class RequestController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _chatHub;
        private YeuCauProvider _provider = new YeuCauProvider();
        private NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        private DonViProvider _donViProvider = new DonViProvider();
        private IHocSinh _hocSInhProvider = new HocSinhProvider();
        private IThongTinVanBang _thongTinVanBangProvider = new ThongTinVanBangProvider();
        private ILoaiVanBang _loaiVanBang = new LoaiVanBangProvider();
        private INotification _notificationProvider;
        private ITuDien _tuDienProvider;
        private ILoaiVanBang _loaiBangProvider;
        private HocSinhProvider _hocSinhProvider;
        public RequestController(IHubContext<NotificationHub, INotificationClient> chatHub)
        {
            _chatHub = chatHub;
            _notificationProvider = new NotificationProvider();
            _tuDienProvider = new TuDienProvider();
            _loaiBangProvider = new LoaiVanBangProvider();
            _hocSinhProvider = new HocSinhProvider();
        }

        [Route("GetYeuCauCapBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauCapBangs(int? phongGiaoDucId, string maTrangThai, int? nam, int? loaiBangId, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (user.DonVi.CapDonVi.Code == "PHONGGD")
                {
                    responseViewModel.Data = _provider.GetYeuCauCapBangs(user.DonViId.Value, nam, maTrangThai, loaiBangId, currentPage, null);
                }
                else if (user.DonVi.CapDonVi.Code == "SOGD")
                {
                    if (maTrangThai == "abort" || maTrangThai == "init" || maTrangThai == "retrieval" || maTrangThai == "waiting")
                    {
                        responseViewModel.Data = null;
                    }
                    responseViewModel.Data = _provider.GetYeuCauCapBangs(phongGiaoDucId, nam, maTrangThai, loaiBangId, currentPage, user.DonViId.Value);
                }
                else
                {
                    responseViewModel.Data = null;
                }
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("GetYeuCauXinCapPhoisCuaConGui")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauXinCapPhoisCuaConGui(string maTrangThai, int? nam, int? loaiYeuCau, int? loaiBangId, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _provider.GetYeuCauXinCapPhoisCuaConGui(nam, maTrangThai, loaiYeuCau, loaiBangId, currentPage, user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("GetYeuCauXinCapPhoisCuaChaNhan")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauXinCapPhoisCuaChaNhan(int? donViId, string maTrangThai, int? nam, int? loaiYeuCau, int? loaiBangId, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _provider.GetYeuCauXinCapPhoisCuaChaNhan(donViId, nam, maTrangThai, loaiYeuCau, loaiBangId, currentPage, user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }


        [Route("GetYeuCauXinCapPhoiBanSaos")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauXinCapPhoiBanSaos(int? phongGiaoDucId, string maTrangThai, int? nam, int? loaiBangId, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (user.DonVi.CapDonVi.Code == "PHONGGD")
                {
                    responseViewModel.Data = _provider.GetYeuCauXinCapPhoiBanSaos(user.DonViId.Value, nam, maTrangThai, loaiBangId, currentPage, null);
                }
                else if (user.DonVi.CapDonVi.Code == "SOGD")
                {
                    if (maTrangThai == "abort" || maTrangThai == "init" || maTrangThai == "retrieval" || maTrangThai == "waiting")
                    {
                        responseViewModel.Data = null;
                    }
                    responseViewModel.Data = _provider.GetYeuCauXinCapPhoiBanSaos(phongGiaoDucId, nam, maTrangThai, loaiBangId, currentPage, user.DonViId.Value);
                }
                else
                {
                    responseViewModel.Data = null;
                }
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("GetYeuCauXetDuyets")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauXetDuyets(int? truongHocId, string maTrangThai, int? nam, int currentPage)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (user.DonVi.CapDonVi.Code == "TIEUHOC" || user.DonVi.CapDonVi.Code == "THCS" || user.DonVi.CapDonVi.Code == "THPT" || user.DonVi.CapDonVi.Code == "TTGD")
                {
                    responseViewModel.Data = _provider.GetYeuCauXetDuyets(user.DonViId.Value, nam, maTrangThai, currentPage, null);
                }
                else if (user.DonVi.CapDonVi.Code == "PHONGGD" || user.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _provider.GetYeuCauXetDuyets(truongHocId, nam, maTrangThai, currentPage, user.DonViId.Value);
                }
                else
                {
                    responseViewModel.Data = null;
                }
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("ThongTinDonViGuiYeuCauXetDuyetViewModel")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongTinDonViGuiYeuCauXetDuyetViewModel()
        {
            ResponseViewModel<ThongTinDonViGuiYeuCauXetDuyetViewModel> responseViewModel = new ResponseViewModel<ThongTinDonViGuiYeuCauXetDuyetViewModel>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _provider.ThongTinDonViGuiYeuCauXetDuyetViewModel(user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("ThongTinDonViGuiYeuCauPheDuyetTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ThongTinDonViGuiYeuCauPheDuyetTotNghiep()
        {
            ResponseViewModel<ThongTinDonViGuiYeuCauXetDuyetViewModel> responseViewModel = new ResponseViewModel<ThongTinDonViGuiYeuCauXetDuyetViewModel>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _provider.ThongTinDonViGuiYeuCauPheDuyetTotNghiep(user.DonViId.Value);
                responseViewModel.Message = "";
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw ex;
            }

            return Ok(responseViewModel);
        }

        [Route("InsertYeuCauCapBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertYeuCauCapBang(YeuCauViewModel model)
        {
            try
            {
                ResponseViewModel<int> responseViewModel = new ResponseViewModel<int>();
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var lst = _provider.GetLoaiYeuCauByCapDonViId(user.PhongBan.DonVi.CapDonViId.Value);
                model.NguoiTao = user.NguoiDungId;
                model.NguoiTaoYeuCau = user.HoTen;
                model.NgayTao = DateTime.Now;
                model.NguoiCapNhat = user.NguoiDungId;
                model.NgayGuiYeuCau = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                model.LoaiYeuCauId = 1;
                model.DonViId = user.PhongBan.DonViId;
                model.DonViYeuCauId = user.PhongBan.DonViId;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                responseViewModel.Data = _provider.InsertYeuCauGuiThongTinHocSinhCongNhanTotNghiep(model, user.DonViId.Value, ip);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                return Ok(responseViewModel);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm mới yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertYeuCauXetDuyet")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> InsertYeuCauXetDuyet(YeuCauViewModel model)
        {
            try
            {
                ResponseViewModel<int> responseViewModel = new ResponseViewModel<int>();
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var lst = _provider.GetLoaiYeuCauByCapDonViId(user.PhongBan.DonVi.CapDonViId.Value);
                model.NguoiTao = user.NguoiDungId;
                model.NguoiTaoYeuCau = user.HoTen;
                model.NgayTao = DateTime.Now;
                model.NguoiCapNhat = user.NguoiDungId;
                model.NgayGuiYeuCau = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                model.LoaiYeuCauId = 6;
                model.DonViId = user.PhongBan.DonViId;
                model.DonViYeuCauId = user.PhongBan.DonViId;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                responseViewModel.Data = _provider.InsertYeuCauXetDuyet(model, user.DonViId.Value, ip);
                responseViewModel.Status = true;
                responseViewModel.Message = "";

                // noti
                // get room don vi nhan
                YeuCauViewModel yeuCauViewModel = _provider.GetYeuCauById(responseViewModel.Data);
                List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(3, model.DonViDichId.Value);
                if (roomViewModels != null && roomViewModels.Count > 0)
                {
                    NotificationMessage notificationMessage = new NotificationMessage();
                    notificationMessage.Message = user.DonVi.TenDonVi + " gửi yêu cầu phê duyệt công nhận tốt nghiệp";
                    notificationMessage.Code = "";
                    notificationMessage.Url = "/danh-sach-yeu-cau-cong-nhan-tot-nghiep/chi-tiet-danh-sach-yeu-cau-cong-nhan-tot-nghiep?idYeuCau=" + yeuCauViewModel.Id;
                    await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                    // add thong bao
                    List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                    foreach (RoomViewModel roomViewModel in roomViewModels)
                    {
                        ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                        thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                        thongBaoViewModel.NoiDung = notificationMessage.Message;
                        thongBaoViewModel.Title = "Yêu cầu phê duyệt công nhận tốt nghiệp";
                        thongBaoViewModel.ThongBaoTypeId = 3;
                        thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                        thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                        thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                        thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                        thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                        thongBaoViewModel.DaDoc = false;
                        thongBaoViewModel.NgayTao = DateTime.Now;
                        thongBaoViewModel.Code = notificationMessage.Code;
                        thongBaoViewModel.Url = notificationMessage.Url;
                        thongBaoViewModels.Add(thongBaoViewModel);
                    }
                    _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                }

                return Ok(responseViewModel);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm mới yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertYeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertYeuCau(YeuCauViewModel model)
        {
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                model.DonViId = user.PhongBan.DonViId;
                model.DonViYeuCauId = user.PhongBan.DonViId;
                model.NgayTao = DateTime.Now;
                model.NgayCapNhat = DateTime.Now;
                model.NguoiTao = user.NguoiDungId;
                model.NguoiCapNhat = user.NguoiDungId;
                model.NguoiTaoYeuCau = user.NguoiDungId.ToString();
                model.IsDeleted = false;
                model.MaTrangThaiYeuCau = "init";
                model.GhiChu = "";
                model.MaYeuCau = Configuration.RandomString(10);
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                return Ok(_provider.Insert(model, ip));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Thêm file vào yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertFile2YeuCau")]
        [HttpPost]
        [ClaimRequirement("All")]
        public IActionResult InsertFile2YeuCau(YeuCauViewModel model)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(_provider.InsertFile(model.Files, model.Id, ip));
        }
        /// <summary>
        /// Thêm danh sách học sinh vào yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("InsertHocSinh2YeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertHocSinh2YeuCau(List<LienKetHocSinhYeuCauViewModel> model)
        {
            return Ok(new LienKetHocSinhYeuCauProvider().InsertLienKet(model));
        }

        /// <summary>
        /// Get all loại yêu cầu
        /// </summary>
        /// <returns></returns>
        [Route("GetAllLoaiYeuCau")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllLoaiYeuCau()
        {
            return Ok(_provider.GetAllLoaiYeuCau());
        }
        /// <summary>
        /// Xóa 1 file trong yêu cầu 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileFromYeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteFileFromYeuCau(FileDinhKemYeuCauViewModel model)
        {
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            var result = _provider.DeleteFile(model, ip);
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

        /// <summary>
        /// Get chi tiết yêu cầu
        /// </summary>
        /// <param name="YeuCauId"></param>
        /// <returns></returns>
        [Route("GetChiTietYeuCauCuaNguoiNhan")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietYeuCauCuaNguoiNhan(int YeuCauId)
        {
            // donViDichId = current donviId

            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (!_provider.IsTruyCapYeuCau(YeuCauId, user.DonViId.Value, true))
                {
                    return BadRequest("UNAUTHORIZE");
                }
                var result = _provider.GetYeuCauById(YeuCauId);
                if (result.MaTrangThaiYeuCau == "retrieval" || result.MaTrangThaiYeuCau == "abort")
                {
                    Exception exception = new Exception("UNAUTHORIZE");
                    throw exception;
                }
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(new ResultModel(false, "Get data fail"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get chi tiết yêu cầu
        /// </summary>
        /// <param name="YeuCauId"></param>
        /// <returns></returns>
        [Route("GetChiTietYeuCauCuaNguoiGui")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietYeuCauCuaNguoiGui(int YeuCauId)
        {
            try
            {
                // donViGuiId = current donviId
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (!_provider.IsTruyCapYeuCau(YeuCauId, user.DonViId.Value, false))
                {
                    return BadRequest("UNAUTHORIZE");
                }
                var result = _provider.GetYeuCauById(YeuCauId);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(new ResultModel(false, "Get data fail"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Get chi tiết yêu cầu
        /// </summary>
        /// <param name="YeuCauId"></param>
        /// <returns></returns>
        [Route("GetChiTietYeuCau")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietYeuCau(int YeuCauId)
        {
            var result = _provider.GetYeuCauById(YeuCauId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return Ok(new ResultModel(false, "Get data fail"));
            }
        }

        /// <summary>
        /// Xóa học sinh trong yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteHocSinhInYeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteHocSinhInYeuCau(List<LienKetHocSinhYeuCauViewModel> model)
        {
            return Ok(new LienKetHocSinhYeuCauProvider().DeleteLienKet(model));
        }
        /// <summary>
        /// Get yêu cầu đã gửi đi của đơn vị
        /// </summary>
        /// <param name="MaTrangThai"></param>
        /// <returns></returns>
        [Route("GetYeuCauDaGui")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauDaGui(string MaTrangThai)
        {
            // string uname = HttpContext.Current.User.Identity.Name;
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_provider.GetYeuCauDaGui(user.DonViId.Value, MaTrangThai));
        }
        /// <summary>
        /// Get yêu cầu chờ duyệt và đã duyệt của đơn vị đích
        /// </summary>
        /// <param name="MaTrangThai"></param>
        /// <returns></returns>
        [Route("GetYeuCauPheDuyet")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetYeuCauPheDuyet(string MaTrangThai)
        {
            // string uname = HttpContext.Current.User.Identity.Name;
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            return Ok(_provider.GetYeuCauPheDuyet(user.DonVi, MaTrangThai));
        }
        /// <summary>
        /// Gửi yêu cầu đến đơn vị phê duyệt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendRequest")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> SendRequest(YeuCauViewModel model)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                YeuCauViewModel yeuCauViewModel_1 = _provider.GetYeuCauById(model.Id);
                if (yeuCauViewModel_1.MaTrangThaiYeuCau == "abort" || yeuCauViewModel_1.MaTrangThaiYeuCau == "approved" || yeuCauViewModel_1.MaTrangThaiYeuCau == "rejected")
                {
                    Exception exception = new Exception("Hành động bị từ chối!");
                    throw exception;
                }

                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                model.DonViId = user.DonViId.Value;
                _provider.SendRequest(model, user.NguoiDungId, ip);
                YeuCauViewModel yeuCauViewModel = _provider.GetYeuCauById(model.Id);
                // noti
                // get room don vi nhan
                if (yeuCauViewModel.LoaiYeuCauId == 6) // phe duyet hoc sinh
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(3, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Message = user.DonVi.TenDonVi + " gửi yêu cầu phê duyệt công nhận tốt nghiệp";
                        notificationMessage.Url = "/danh-sach-yeu-cau-cong-nhan-tot-nghiep/chi-tiet-danh-sach-yeu-cau-cong-nhan-tot-nghiep?idYeuCau=" + model.Id;
                        notificationMessage.Code = "SEND_REQUEST";
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                        // add thong bao
                        List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                        foreach (RoomViewModel roomViewModel in roomViewModels)
                        {
                            ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                            thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                            thongBaoViewModel.NoiDung = notificationMessage.Message;
                            thongBaoViewModel.Title = "Yêu cầu phê duyệt công nhận tốt nghiệp";
                            thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                            thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                            thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                            thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                            thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                            thongBaoViewModel.DaDoc = false;
                            thongBaoViewModel.NgayTao = DateTime.Now;
                            thongBaoViewModel.Code = notificationMessage.Code;
                            thongBaoViewModel.Url = notificationMessage.Url;
                            thongBaoViewModels.Add(thongBaoViewModel);
                        }
                        _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 2)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(1, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Message = user.DonVi.TenDonVi + " gửi yêu cầu mua phôi Văn bằng gốc";
                        notificationMessage.Url = "/yeu-cau-mua-phoi/chi-tiet-tiep-nhan-yeu-cau-mua-phoi?idYeuCau=" + model.Id;
                        notificationMessage.Code = "SEND_REQUEST";
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                        // add thong bao
                        List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                        foreach (RoomViewModel roomViewModel in roomViewModels)
                        {
                            ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                            thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                            thongBaoViewModel.NoiDung = notificationMessage.Message;
                            thongBaoViewModel.Title = "Yêu cầu mua phôi Văn bằng gốc";
                            thongBaoViewModel.ThongBaoTypeId = 1;
                            thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                            thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                            thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                            thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                            thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                            thongBaoViewModel.DaDoc = false;
                            thongBaoViewModel.NgayTao = DateTime.Now;
                            thongBaoViewModel.Code = notificationMessage.Code;
                            thongBaoViewModel.Url = notificationMessage.Url;
                            thongBaoViewModels.Add(thongBaoViewModel);
                        }
                        _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 7)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(1, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Message = user.DonVi.TenDonVi + " gửi yêu cầu mua phôi Văn bằng bản sao";
                        notificationMessage.Url = "/yeu-cau-mua-phoi/chi-tiet-tiep-nhan-yeu-cau-mua-phoi?idYeuCau=" + model.Id;
                        notificationMessage.Code = "SEND_REQUEST";
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                        // add thong bao
                        List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                        foreach (RoomViewModel roomViewModel in roomViewModels)
                        {
                            ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                            thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                            thongBaoViewModel.NoiDung = notificationMessage.Message;
                            thongBaoViewModel.Title = "Yêu cầu mua phôi Văn bằng bản sao";
                            thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                            thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                            thongBaoViewModel.ThongBaoTypeId = 1;
                            thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                            thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                            thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                            thongBaoViewModel.DaDoc = false;
                            thongBaoViewModel.NgayTao = DateTime.Now;
                            thongBaoViewModel.Code = notificationMessage.Code;
                            thongBaoViewModel.Url = notificationMessage.Url;
                            thongBaoViewModels.Add(thongBaoViewModel);
                        }
                        _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                    }
                }
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                responseViewModel.Data = null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }
        /// <summary>
        /// CHuyển tiếp yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ForwardRequest")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ForwardRequest(YeuCauViewModel model)
        {
            //string uname = HttpContext.Current.User.Identity.Name;
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(_provider.ForwardRequest(model, user.NguoiDungId, ip));
        }
        /// <summary>
        /// Thu hồi hoặc hủy yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RetrievalOrAbort")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> RetrievalOrAbort(YeuCauViewModel model)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                YeuCauViewModel yeuCauViewModel_1 = _provider.GetYeuCauById(model.Id);
                if (yeuCauViewModel_1.MaTrangThaiYeuCau != "waiting")
                {
                    Exception exception = new Exception("Hành động bị từ chối!");
                    throw exception;
                }
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _provider.RetrievalOrAbort(model, user.NguoiDungId, ip);

                YeuCauViewModel yeuCauViewModel = _provider.GetYeuCauById(model.Id);

                if (yeuCauViewModel.LoaiYeuCauId == 6) // phe duyet hoc sinh
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(3, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "abort")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã hủy cầu phê duyệt công nhận tốt nghiệp";
                            notificationMessage.Code = "ABORT_YEU_CAU_CONG_NHAN_TOT_NGHIEP";
                            notificationMessage.Url = "/danh-sach-yeu-cau-cong-nhan-tot-nghiep";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Hủy yêu cầu phê duyệt công nhận tốt nghiệp";
                                thongBaoViewModel.ThongBaoTypeId = 3;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "retrieval")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã thu hồi yêu cầu phê duyệt công nhận tốt nghiệp";
                            notificationMessage.Code = "RETRIEVAL_YEU_CAU_CONG_NHAN_TOT_NGHIEP";
                            notificationMessage.Url = "/danh-sach-yeu-cau-cong-nhan-tot-nghiep";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Thu hồi yêu cầu phê duyệt công nhận tốt nghiệp";
                                thongBaoViewModel.ThongBaoTypeId = 3;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 2)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(1, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "abort")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã hủy yêu cầu mua phôi Văn bằng gốc";
                            notificationMessage.Code = "ABORT_MUA_PHOI_VAN_BANG_GOC";
                            notificationMessage.Url = "/yeu-cau-mua-phoi";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Hủy yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 1;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "retrieval")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã thu hồi yêu cầu mua phôi Văn bằng gốc";
                            notificationMessage.Code = "RETRIEVAL_MUA_PHOI_VAN_BANG_GOC";
                            notificationMessage.Url = "/yeu-cau-mua-phoi";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Thu hồi yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 1;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 7)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(1, yeuCauViewModel.DonViDichId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "abort")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã hủy yêu cầu mua phôi Văn bằng bản sao";
                            notificationMessage.Code = "ABORT_MUA_PHOI_VAN_BANG_BAN_SAO";
                            notificationMessage.Url = "/yeu-cau-mua-phoi";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Hủy yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 1;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "retrieval")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã thu hồi yêu cầu mua phôi Văn bằng bản sao";
                            notificationMessage.Code = "RETRIEVAL_MUA_PHOI_VAN_BANG_BAN_SAO";
                            notificationMessage.Url = "/yeu-cau-mua-phoi";

                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Thu hồi yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 1;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }

                responseViewModel.Status = true;
                responseViewModel.Message = "";
                responseViewModel.Data = null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }
        /// <summary>
        /// Phê duyệt hoặc Từ chối yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ApproveOrReject")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> ApproveOrReject(YeuCauViewModel model)
        {
            var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NgayCapNhat = DateTime.Now;
            model.NguoiCapNhat = user.NguoiDungId;
            model.DonViId = user.DonViId.Value;
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                YeuCauViewModel yeuCauViewModel_1 = _provider.GetYeuCauById(model.Id);
                if (yeuCauViewModel_1.MaTrangThaiYeuCau == "abort" || yeuCauViewModel_1.MaTrangThaiYeuCau == "approved" || yeuCauViewModel_1.MaTrangThaiYeuCau == "rejected")
                {
                    Exception exception = new Exception("Hành động bị từ chối!");
                    throw exception;
                }
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                // doc file
                List<DanTocViewModel> danTocViewModels = _tuDienProvider.GetDanTocs();

                //if (yeuCauViewModel_1.DuongDanFiles == null || yeuCauViewModel_1.DuongDanFiles.Count == 0)
                //{
                //    Exception exception = new Exception("Không tìm thấy file học sinh");
                //    throw exception;
                //}

                if (model.MaTrangThaiYeuCau != "approved")
                {
                    _provider.ApproveOrReject(model, user.NguoiDungId, ip);
                }
                YeuCauViewModel yeuCauViewModel = _provider.GetYeuCauById(model.Id);

                if ((model.MaTrangThaiYeuCau == "approved") && (model.LoaiYeuCauId == null || ((model.LoaiYeuCauId != 2) && (model.LoaiYeuCauId != 7))))
                {
                    DonViViewModel donViViewModel = _donViProvider.GetDonViSo(user.DonViId.Value);

                    DonViViewModel truongHoc = _donViProvider.GetDonViById(yeuCauViewModel.DonViYeuCauId.Value);
                    // doc file
                    List<NhomTaoVanBangViewModel> nhomTaoVanBangViewModels = new List<NhomTaoVanBangViewModel>();
                    foreach (string duongDanFile in yeuCauViewModel_1.FileHocSinhYeuCauDuXetTotNghieps.Select(x => x.Url))
                    {
                        // read file
                        Workbook workbook = new Workbook();
                        workbook.LoadFromFile(Directory.GetCurrentDirectory() + duongDanFile);
                        if (workbook.Worksheets.Where(x => x.Name == "TTHS").Count() == 0)
                        {
                            Exception exception = new Exception("Không tìm thấy sheet TTHS");
                            throw exception;
                        }
                        if (workbook.Worksheets.Where(x => x.Name == "Quy tắc").Count() == 0)
                        {
                            Exception exception = new Exception("Không tìm thấy sheet Quy tắc");
                            throw exception;
                        }
                        IWorksheet ruleSheet = workbook.Worksheets.Where(x => x.Name == "Quy tắc").FirstOrDefault();
                        var rows = ruleSheet.Rows.ToList();
                        rows.RemoveAt(0);
                        List<HocSinhTotNghiepRuleExcelViewModel> rules = new List<HocSinhTotNghiepRuleExcelViewModel>();
                        foreach (var row in rows)
                        {
                            HocSinhTotNghiepRuleExcelViewModel hocSinhTotNghiepRuleExcelViewModel = new HocSinhTotNghiepRuleExcelViewModel();
                            hocSinhTotNghiepRuleExcelViewModel.Row = Regex.Match(row.CellList[0].DisplayedText, @"\d+").Value;
                            hocSinhTotNghiepRuleExcelViewModel.Col = Regex.Match(row.CellList[0].DisplayedText, @"[A-Z]+").Value;
                            hocSinhTotNghiepRuleExcelViewModel.CodeTruongDuLieu = row.CellList[1].DisplayedText;
                            hocSinhTotNghiepRuleExcelViewModel.RangeAddressLocal = row.CellList[0].DisplayedText;
                            hocSinhTotNghiepRuleExcelViewModel.Format = row.CellList[2].DisplayedText;
                            if (string.IsNullOrEmpty(hocSinhTotNghiepRuleExcelViewModel.Format) || string.IsNullOrEmpty(hocSinhTotNghiepRuleExcelViewModel.RangeAddressLocal)
                                    || string.IsNullOrEmpty(hocSinhTotNghiepRuleExcelViewModel.CodeTruongDuLieu))
                            {
                                continue;
                            }
                            rules.Add(hocSinhTotNghiepRuleExcelViewModel);
                        }

                        int colSchool = -1;
                        // check format file
                        LoaiBangViewModel loaiBangViewModel = _loaiBangProvider.GetLoaiBang(yeuCauViewModel_1.LoaiVanBangId.Value, user.DonViId.Value);
                        if (loaiBangViewModel == null)
                        {
                            Exception exception = new Exception("Không tồn tại loại bằng");
                            throw exception;
                        }
                        else
                        {
                            bool same = loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => !x.DungChung && (x.KieuDuLieu == 1) && (x.TenTruongDuLieu != "SoVaoSo")).All(x => rules.Any(k => k.CodeTruongDuLieu == x.TruongDuLieuCode));
                            if (!same)
                            {
                                Exception exception = new Exception("Thiếu mã trường dữ liệu loại bằng, vui lòng tải lại file mẫu và nhập lại thông tin");
                                throw exception;
                            }
                            else
                            {
                                foreach (var rule in rules.Where(x => loaiBangViewModel.TruongDuLieuLoaiBangs.Any(k => k.TruongDuLieuCode == x.CodeTruongDuLieu)))
                                {
                                    rule.TenTruongDuLieu = loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => x.TruongDuLieuCode == rule.CodeTruongDuLieu).FirstOrDefault().TenTruongDuLieu;
                                }
                                // check cột trường học có kèm id trường
                                IWorksheet tthsWS = workbook.Worksheets.Where(x => x.Name == "TTHS").FirstOrDefault();
                                foreach (var cell in tthsWS.Cells)
                                {
                                    if (cell.Value.Contains(";") && (cell.Value.Trim().Length == (cell.Value.Trim().Replace(";", "").Count() + 1)))
                                    {
                                        colSchool = cell.Columns.FirstOrDefault().Column;
                                        break;
                                    }
                                }

                                if (colSchool == -1)
                                {
                                    Exception exception = new Exception("Không đúng định dạng cột trường học");
                                    throw exception;
                                }
                                else
                                {
                                    colSchool--;
                                }
                            }
                        }

                        // doc du lieu file --> thong tin hoc sinh
                        BangExcelFormatExcelViewModel bangExcelFormatExcelViewModel = new BangExcelFormatExcelViewModel();
                        bangExcelFormatExcelViewModel.ThongTinChungs = new List<ThongTinTruongDuLieuHocSinhExcelViewModel>();
                        bangExcelFormatExcelViewModel.HocSinhs = new List<HocSinhExcelViewModel>();
                        IWorksheet hocSinhSheet = workbook.Worksheets.Where(x => x.Name == "TTHS").FirstOrDefault();
                        //foreach (var rule in rules.Where(x => x.Format == "1").ToList())
                        //{
                        //    var cell = hocSinhSheet.Cells.Where(x => x.RangeAddressLocal == rule.RangeAddressLocal).FirstOrDefault();
                        //    if (cell != null && !string.IsNullOrEmpty(cell.Text))
                        //    {
                        //        ThongTinTruongDuLieuHocSinhExcelViewModel thongTinTruongDuLieuHocSinhExcelViewModel = new ThongTinTruongDuLieuHocSinhExcelViewModel();
                        //        thongTinTruongDuLieuHocSinhExcelViewModel.Value = cell.Text;
                        //        thongTinTruongDuLieuHocSinhExcelViewModel.CodeTruongDuLieu = rule.CodeTruongDuLieu;
                        //        bangExcelFormatExcelViewModel.ThongTinChungs.Add(thongTinTruongDuLieuHocSinhExcelViewModel);
                        //    }
                        //}


                        // get thong tin rieng
                        int rowIndex = Convert.ToInt32(rules.Where(x => x.Format == "2").FirstOrDefault().Row);
                        int rowCount = hocSinhSheet.Range.RowCount;
                        // get attribute HocSinh
                        Type businessEntityType = typeof(HocSinhViewModel);
                        Hashtable hashtable = new Hashtable();
                        PropertyInfo[] properties = businessEntityType.GetProperties();
                        foreach (PropertyInfo info in properties)
                        {
                            hashtable[info.Name.ToUpper()] = info;
                        }
                        // đếm từng dòng
                        while (rowIndex < rowCount)
                        {
                            var row = hocSinhSheet.Rows[rowIndex];
                            HocSinhExcelViewModel hocSinhExcelViewModel = new HocSinhExcelViewModel();
                            hocSinhExcelViewModel.TTHS = new HocSinhViewModel();
                            hocSinhExcelViewModel.ThongTinRiengs = new List<ThongTinTruongDuLieuHocSinhExcelViewModel>();

                            foreach (var rule in rules.Where(x => x.Format == "2" && loaiBangViewModel.TruongDuLieuLoaiBangs.Any(k => k.TruongDuLieuCode == x.CodeTruongDuLieu)))
                            {
                                var cell = row.CellList.Where(x => Regex.Match(x.RangeAddressLocal, @"[A-Z]+").Value == rule.Col).FirstOrDefault();
                                if ((cell != null) && (string.IsNullOrEmpty(cell.DisplayedText)))
                                {
                                    rowIndex++;
                                    goto SkipLoop;
                                }
                                ThongTinTruongDuLieuHocSinhExcelViewModel thongTinTruongDuLieuHocSinhExcelViewModel = new ThongTinTruongDuLieuHocSinhExcelViewModel();
                                thongTinTruongDuLieuHocSinhExcelViewModel.CodeTruongDuLieu = rule.CodeTruongDuLieu;
                                thongTinTruongDuLieuHocSinhExcelViewModel.Value = cell.DisplayedText;
                                hocSinhExcelViewModel.ThongTinRiengs.Add(thongTinTruongDuLieuHocSinhExcelViewModel);
                                var info = (PropertyInfo)hashtable[rule.TenTruongDuLieu.ToUpper()];
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(info.PropertyType);
                                object propValue = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), cell.DisplayedText);
                                info.SetValue(hocSinhExcelViewModel.TTHS, propValue, null);

                            }

                            // get thong tin truong hoc
                            rowIndex++;
                            string colSchoolString = row.CellList[colSchool].Value.Trim().Split(";").FirstOrDefault();
                            int schoolId;
                            if (int.TryParse(colSchoolString, out schoolId))
                            {
                                hocSinhExcelViewModel.TruongHocId = Convert.ToInt32(row.CellList[colSchool].Value.Split(";").FirstOrDefault());
                                hocSinhExcelViewModel.TTHS.TruongHocId = hocSinhExcelViewModel.TruongHocId;
                                hocSinhExcelViewModel.TTHS.CongNhanTotNghiep = true;
                                hocSinhExcelViewModel.TTHS.SoLanXet = 1;
                                hocSinhExcelViewModel.TTHS.TrangThaiBangId = 2;
                                hocSinhExcelViewModel.TTHS.NienKhoa = DateTime.Now.Year.ToString();
                                hocSinhExcelViewModel.TTHS.NamTotNghiep = DateTime.Now.Year;
                                hocSinhExcelViewModel.TTHS.DaInBangGoc = false;
                                hocSinhExcelViewModel.TTHS.KQ = true;
                                hocSinhExcelViewModel.TTHS.CongNhanTotNghiep = true;
                                hocSinhExcelViewModel.TTHS.SoLanXet = 1;
                                hocSinhExcelViewModel.TTHS.NguoiTao = user.NguoiDungId;
                                hocSinhExcelViewModel.TTHS.NguoiCapNhat = user.NguoiDungId;
                                hocSinhExcelViewModel.TTHS.NgayTao = DateTime.Now;
                                hocSinhExcelViewModel.TTHS.NgayCapNhat = DateTime.Now;
                                hocSinhExcelViewModel.TTHS.TrangThaiBang = "Đã tạo thông tin";
                                if (!string.IsNullOrEmpty(hocSinhExcelViewModel.TTHS.DanToc))
                                {
                                    DanTocViewModel danTocViewModel = danTocViewModels.Where(x => x.Ten.Trim().ToLower() == hocSinhExcelViewModel.TTHS.DanToc.Trim().ToLower()).FirstOrDefault();
                                    if (danTocViewModel == null)
                                    {
                                        Exception exception = new Exception(string.Format("Không tìm thấy dân tộc {0} tại hàng {1}", hocSinhExcelViewModel.TTHS.DanToc, rowIndex));
                                        throw exception;
                                    }
                                    hocSinhExcelViewModel.TTHS.DanTocId = danTocViewModel.Id;

                                }

                                if (!string.IsNullOrEmpty(hocSinhExcelViewModel.TTHS.GioiTinh))
                                {
                                    hocSinhExcelViewModel.TTHS.GioiTinhId = ((!string.IsNullOrEmpty(hocSinhExcelViewModel.TTHS.GioiTinh)) && (hocSinhExcelViewModel.TTHS.GioiTinh == "Nam")) ? 1 : 2;
                                }

                                // doc thong tin khac cua hoc sinh
                                var hkCell = rules.Where(x => x.CodeTruongDuLieu == "HK").FirstOrDefault();
                                if (hkCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.HK = row[hkCell.Col + rowIndex].EnvalutedValue;
                                }

                                var utCell = rules.Where(x => x.CodeTruongDuLieu == "UT").FirstOrDefault();
                                if (utCell != null)
                                {
                                    try
                                    {
                                        hocSinhExcelViewModel.TTHS.UT = Convert.ToInt32(row[utCell.Col + rowIndex].EnvalutedValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        hocSinhExcelViewModel.TTHS.UT = null;
                                    }
                                }

                                var kkCell = rules.Where(x => x.CodeTruongDuLieu == "KK").FirstOrDefault();
                                if (kkCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.KK = row[kkCell.Col + rowIndex].EnvalutedValue;
                                }

                                var soLanXetCell = rules.Where(x => x.CodeTruongDuLieu == "LAN-XET").FirstOrDefault();
                                if (soLanXetCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.SoLanXet = Convert.ToInt32(row[soLanXetCell.Col + rowIndex].EnvalutedValue);
                                }

                                var xlCell = rules.Where(x => x.CodeTruongDuLieu == "XL").FirstOrDefault();
                                if (xlCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.XepLoaiTotNghiep = row[xlCell.Col + rowIndex].EnvalutedValue;
                                }

                                var hdtCell = rules.Where(x => x.CodeTruongDuLieu == "HDT").FirstOrDefault();
                                if (hdtCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.HinhThucDaoTao = row[hdtCell.Col + rowIndex].EnvalutedValue;
                                }

                                var diemCell = rules.Where(x => x.CodeTruongDuLieu == "Diem").FirstOrDefault();
                                if (diemCell != null)
                                {
                                    hocSinhExcelViewModel.TTHS.DiemThi = row[diemCell.Col + rowIndex].EnvalutedValue;
                                }
                            }
                            else
                            {
                                Exception exception = new Exception("Không đúng định dạng cột trường học tại hàng " + (rowIndex));
                                throw exception;
                            }


                            bangExcelFormatExcelViewModel.HocSinhs.Add(hocSinhExcelViewModel);
                        SkipLoop: continue;
                        }

                        // add thong tin học sinh vào db, tạo nhóm in văn bằng

                        foreach (int truongHocId in bangExcelFormatExcelViewModel.HocSinhs.GroupBy(x => x.TruongHocId).Select(x => x.Key))
                        {
                            int tt = _hocSinhProvider.GetSTTHocSinhDuocXet(DateTime.Now.Year, truongHocId);
                            DonViViewModel donViSo = _donViProvider.GetDonViSo(truongHocId);
                            DonViViewModel donViTruongHoc = _donViProvider.GetDonViById(truongHocId);
                            foreach (var hocSinh in bangExcelFormatExcelViewModel.HocSinhs.Where(x => x.TruongHocId == truongHocId))
                            {
                                hocSinh.TTHS.TT = tt;
                                hocSinh.TTHS.TruongHoc = donViTruongHoc.TenDonVi;
                                hocSinh.TTHS.SoVaoSo = string.Format("{0}/{1}/{2}/{3}", donViSo.MaDonVi, donViTruongHoc.MaDonVi, tt, DateTime.Now.Year);
                                tt++;
                            }

                            NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                            nhomTaoVanBangViewModel.Title = "Nhóm in văn bằng trường " + donViTruongHoc.TenDonVi + " năm " + DateTime.Now.Year;
                            nhomTaoVanBangViewModel.TruongHocId = truongHocId;
                            nhomTaoVanBangViewModel.DonViId = user.DonViId.Value;
                            nhomTaoVanBangViewModel.LoaiBangId = yeuCauViewModel_1.LoaiVanBangId.Value;
                            nhomTaoVanBangViewModel.NgayTao = DateTime.Now;
                            nhomTaoVanBangViewModel.NguoiTao = user.NguoiDungId;
                            nhomTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                            nhomTaoVanBangViewModel.NguoiCapNhat = user.NguoiDungId;
                            nhomTaoVanBangViewModel.IsDeleted = false;
                            nhomTaoVanBangViewModel.CanDelete = true;
                            nhomTaoVanBangViewModel.AddedByImport = true;
                            nhomTaoVanBangViewModel.ChoPhepTaoLai = true;
                            nhomTaoVanBangViewModel.TrangThaiBangId = 1;
                            nhomTaoVanBangViewModel.HocSinhs = new List<HocSinhTaoVanBangViewModel>();
                            List<HocSinh> hocSinhs = _hocSinhProvider.AddHocSinhs(bangExcelFormatExcelViewModel.HocSinhs.Where(x => x.TruongHocId == truongHocId).Select(x => x.TTHS).ToList(), user.DonViId.Value, ip);
                            foreach (HocSinh hocSinh in hocSinhs)
                            {
                                nhomTaoVanBangViewModel.HocSinhs.Add(new HocSinhTaoVanBangViewModel()
                                {
                                    Id = hocSinh.Id,
                                    BangId = null,
                                    TrangThaiBangId = 1,
                                    DonViId = user.DonViId.Value,
                                    HoVaTen = hocSinh.HoVaTen,
                                    SoVaoSo = hocSinh.SoVaoSo,
                                    TruongHoc = hocSinh.TruongHoc,
                                    NamTotNghiep = hocSinh.NamTotNghiep
                                });
                            }
                            nhomTaoVanBangViewModel.TongSohocSinh = nhomTaoVanBangViewModel.HocSinhs.Count();
                            nhomTaoVanBangViewModels.Add(nhomTaoVanBangViewModel);

                        }

                        _provider.ApproveOrReject(model, user.NguoiDungId, ip);

                        foreach (NhomTaoVanBangViewModel nhomTaoVanBangViewModel in nhomTaoVanBangViewModels)
                        {
                            _thongTinVanBangProvider.AddNhomTaoVanBang(nhomTaoVanBangViewModel, user.DonViId.Value);
                        }

                    }

                    //// them so vao so
                    //_hocSInhProvider.CongNhanTotNghiepHocSinhTrongYeuCau(model.Id, truongHoc.MaDonVi, donViViewModel.MaDonVi, user.DonViId.Value);

                    //LoaiBangViewModel loaiBangViewModel = _loaiVanBang.GetLoaiBang(model.Id, user.DonViId.Value);
                    //List<HocSinhViewModel> hocSinhViewModels = _provider.GetHocSinhByYeuCau(model.Id);
                    ////_hocSInhProvider.UpdateSoVaoSo(hocSinhViewModels, user.DonViId.Value);

                    //NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                    //nhomTaoVanBangViewModel.Title = (loaiBangViewModel.IsChungChi.HasValue && loaiBangViewModel.IsChungChi.Value ? "Danh sách tạo chứng chỉ " : "Danh sách tạo văn bằng ") + yeuCauViewModel.TenDonViYeuCau;
                    //nhomTaoVanBangViewModel.TruongHocId = yeuCauViewModel.DonViYeuCauId;
                    //nhomTaoVanBangViewModel.DonViId = user.DonViId.Value;
                    //nhomTaoVanBangViewModel.LoaiBangId = yeuCauViewModel.LoaiVanBangId;
                    //nhomTaoVanBangViewModel.NgayTao = DateTime.Now;
                    //nhomTaoVanBangViewModel.NguoiTao = user.NguoiDungId;
                    //nhomTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                    //nhomTaoVanBangViewModel.NguoiCapNhat = user.NguoiDungId;
                    //nhomTaoVanBangViewModel.IsDeleted = false;
                    //nhomTaoVanBangViewModel.TrangThaiBangId = 1;
                    //nhomTaoVanBangViewModel.CanDelete = false;
                    //nhomTaoVanBangViewModel.AddedByImport = false;
                    //nhomTaoVanBangViewModel.HocSinhs = new List<HocSinhTaoVanBangViewModel>();
                    //foreach (HocSinhViewModel item in hocSinhViewModels)
                    //{
                    //    nhomTaoVanBangViewModel.HocSinhs.Add(new HocSinhTaoVanBangViewModel()
                    //    {
                    //        Id = item.Id,
                    //        BangId = null,
                    //        TrangThaiBangId = 1,
                    //        DonViId = user.DonViId.Value,
                    //        HoVaTen = item.HoVaTen,
                    //        SoVaoSo = item.SoVaoSo,
                    //        TruongHoc = item.TruongHoc,
                    //        NamTotNghiep = item.NamTotNghiep
                    //    });
                    //}
                    //nhomTaoVanBangViewModel.TongSohocSinh = nhomTaoVanBangViewModel.HocSinhs.Count();
                    //nhomTaoVanBangViewModel.ChoPhepTaoLai = true;
                    //_thongTinVanBangProvider.AddNhomTaoVanBang(nhomTaoVanBangViewModel, user.DonViId.Value);

                }

                if (yeuCauViewModel.LoaiYeuCauId == 6) // phe duyet hoc sinh
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(4, yeuCauViewModel.DonViYeuCauId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Code = "APPROVED";
                        notificationMessage.Url = "/danh-sach-yeu-cau-xet-duyet/chi-tiet-yeu-cau-xet-duyet?idYeuCau=" + model.Id;
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "approved")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã công nhận tốt nghiệp";
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Phê duyệt yêu cầu";
                                thongBaoViewModel.ThongBaoTypeId = 4;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);

                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "rejected")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã từ chối yêu cầu ";
                            notificationMessage.Code = "APPROVED_YEU_CAU_CONG_NHAN_TOT_NGHIEP";
                            notificationMessage.Url = "/danh-sach-yeu-cau-xet-duyet/chi-tiet-yeu-cau-xet-duyet?idYeuCau=" + model.Id;
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Từ chối yêu cầu";
                                thongBaoViewModel.ThongBaoTypeId = 4;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }

                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 2)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(2, yeuCauViewModel.DonViYeuCauId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Code = "APPROVE_MUA_PHOI_VAN_BANG_GOC";
                        notificationMessage.Url = "/mua-phoi/chi-tiet-yeu-cau-mua-phoi?idYeuCau=" + model.Id;
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "approved")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã phê duyệt yêu cầu mua phôi Văn bằng gốc";
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Phê duyệt yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 2;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "rejected")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã từ chối yêu cầu mua phôi Văn bằng gốc";
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Từ chối yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 2;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }


                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }
                else if (yeuCauViewModel.LoaiYeuCauId == 7)
                {
                    List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(2, yeuCauViewModel.DonViYeuCauId.Value);
                    if (roomViewModels != null && roomViewModels.Count > 0)
                    {
                        NotificationMessage notificationMessage = new NotificationMessage();
                        notificationMessage.Code = "APPROVE_MUA_PHOI_VAN_BANG_BAN_SAO";
                        notificationMessage.Url = "/mua-phoi/chi-tiet-yeu-cau-mua-phoi?idYeuCau=" + model.Id;
                        if (yeuCauViewModel.MaTrangThaiYeuCau == "approved")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã phê duyệt yêu cầu mua phôi Văn bằng bản sao";
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Phê duyệt yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 2;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }
                        else if (yeuCauViewModel.MaTrangThaiYeuCau == "rejected")
                        {
                            notificationMessage.Message = user.DonVi.TenDonVi + " đã từ chối yêu cầu mua phôi Văn bằng bản sao";
                            // add thong bao
                            List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                            foreach (RoomViewModel roomViewModel in roomViewModels)
                            {
                                ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                                thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                                thongBaoViewModel.NoiDung = notificationMessage.Message;
                                thongBaoViewModel.Title = "Từ chối yêu cầu mua phôi";
                                thongBaoViewModel.ThongBaoTypeId = 2;
                                thongBaoViewModel.NguoiGuiId = user.NguoiDungId.ToString();
                                thongBaoViewModel.DonViGuiId = user.DonViId.Value;
                                thongBaoViewModel.PhongBanGuiId = user.PhongBan.PhongBanId;
                                thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                                thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                                thongBaoViewModel.DaDoc = false;
                                thongBaoViewModel.NgayTao = DateTime.Now;
                                thongBaoViewModel.Code = notificationMessage.Code;
                                thongBaoViewModel.Url = notificationMessage.Url;
                                thongBaoViewModels.Add(thongBaoViewModel);
                            }
                            _notificationProvider.AddThongBao(thongBaoViewModels, user.DonViId.Value);
                        }

                        await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);
                    }
                }


                responseViewModel.Status = true;
                responseViewModel.Message = "";
                responseViewModel.Data = null;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

       

        /// <summary>
        /// Get all trạng thái yêu cầu
        /// </summary>
        /// <returns></returns>
        [Route("GetAllTrangThaiYeuCau")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllTrangThaiYeuCau()
        {
            var res = _provider.GetAllTrangThaiYeuCau();
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Lấy dữ liệu trạng thái không thành công"));
            }
        }
        /// <summary>
        /// Cập nhật thông tin yêu cầu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateThongTinYeuCau")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateThongTinYeuCau(YeuCauViewModel model)
        {
            var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiCapNhat = user.NguoiDungId;
            model.NgayCapNhat = DateTime.Now;
            model.NguoiTao = user.NguoiDungId;
            var ip = HttpContext.Connection.RemoteIpAddress.ToString();
            return Ok(_provider.Update(model, ip));
        }
        /// <summary>
        /// Get danh sách học sinh trong yêu cầu
        /// </summary>
        /// <param name="YeuCauId"></param>
        /// <returns></returns>
        [Route("GetListHocSinhByYeuCau")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetListHocSinhByYeuCau(int YeuCauId)
        {
            var res = _provider.GetHocSinhByYeuCau(YeuCauId);
            if (res == null)
            {
                return Ok(new ResultModel(false, "Lấy dữ liệu học sinh thất bại!"));
            }
            else
            {
                return Ok(res);
            }
        }
        /// <summary>
        /// Get loại yêu cầu của đơn vị
        /// </summary>
        /// <returns></returns>
        [Route("GetLoaiYeuCau")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLoaiYeuCau()
        {
            //string uname = HttpContext.Current.User.Identity.Name;
            var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _provider.GetLoaiYeuCauByCapDonViId(user.PhongBan.DonVi.CapDonViId.Value);
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return Ok(new ResultModel(false, "Lấy danh sách loại yêu cầu thất bại"));
            }
        }

        [Route("KetQuaKiemTraPheDuyet")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult KetQuaKiemTraPheDuyet(int yeuCauId)
        {
            ResponseViewModel<KetQuaKiemTraPheDuyetViewModel> responseViewModel = new ResponseViewModel<KetQuaKiemTraPheDuyetViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                KetQuaKiemTraPheDuyetViewModel ketQuaKiemTraPheDuyetViewModel = _provider.KiemTraPheDuyet(yeuCauId, nguoiDung.DonViId.Value);
                ketQuaKiemTraPheDuyetViewModel.MaCapDonVi = nguoiDung.DonVi.CapDonVi.Code;
                responseViewModel.Data = ketQuaKiemTraPheDuyetViewModel;
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
