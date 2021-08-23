using jbcert.API.Hubs;
using jbcert.API.Hubs.Clients;
using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.Providers;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class ThongTinVanBangController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationClient> _chatHub;
        IThongTinVanBang _thongTinVanBang;
        NguoiDungProvider _nguoiDungProvider;
        YeuCauProvider _yeuCauProvider;
        DonViProvider _donViProvider;
        ILoaiVanBang _loaiVanBang;
        IPhoi _phoi;
        INotification _notificationProvider;
        public ThongTinVanBangController(IHubContext<NotificationHub, INotificationClient> chatHub)
        {
            _thongTinVanBang = new ThongTinVanBangProvider();
            _nguoiDungProvider = new NguoiDungProvider();
            _phoi = new PhoiProvider();
            _yeuCauProvider = new YeuCauProvider();
            _donViProvider = new DonViProvider();
            _notificationProvider = new NotificationProvider();
            _loaiVanBang = new LoaiVanBangProvider();
            _chatHub = chatHub;
        }

        [Route("UpdateThongTinVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateThongTinVanBang(ShowDanhSachHocSinhTaoBangViewModel danhSachHocSinhTaoBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (danhSachHocSinhTaoBangViewModel != null)
                {
                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                    foreach (HocSinhTaoVanBangViewModel hocSinh in danhSachHocSinhTaoBangViewModel.HocSinhs)
                    {
                        foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in hocSinh.TruongDuLieuTrongBangs)
                        {
                            truongDuLieuTrongBangViewModel.NgayCapNhat = DateTime.Now;
                            truongDuLieuTrongBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                            truongDuLieuTrongBangViewModels.Add(truongDuLieuTrongBangViewModel);
                        }
                    }
                    _thongTinVanBang.UpdateThongTinVanBang(truongDuLieuTrongBangViewModels, nguoiDung.DonViId.Value);
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


        [Route("DownloadFileAnh")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DownloadFileAnh(TaoAnhVanBangViewModel taoAnhVanBangViewModel)
        {
            ResponseViewModel<string> responseViewModel = new ResponseViewModel<string>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                taoAnhVanBangViewModel.DonViId = nguoiDung.DonViId.Value;
                List<BangViewModel> bangViewModels = _thongTinVanBang.DownLoadFileAnh(taoAnhVanBangViewModel);
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(taoAnhVanBangViewModel.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);

                string url = Path.Combine(nhomTaoVanBangViewModel.DuongDanFileAnhDeIn, DateTime.Now.Year.ToString());
                string fileName = "file-in-bang.pdf";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), url);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string pdfpath = Path.Combine(folderPath, fileName);
                string rootUrl = Directory.GetCurrentDirectory();
                // Create a new PDF document
                PdfDocument document = new PdfDocument();
                document.Info.Title = "Created with PDFsharp";
                foreach (BangViewModel bangViewModel in bangViewModels)
                {
                    // Create an empty page
                    PdfPage page = document.AddPage();
                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    page.Orientation = PdfSharp.PageOrientation.Landscape;
                    page.Size = PdfSharp.PageSize.A4;
                    page.Rotate = 0;
                    //// Create a font
                    //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                    //// Draw the text
                    //gfx.DrawString("Hello, World!", font, XBrushes.Black,
                    //  new XRect(0, 0, page.Width, page.Height),
                    //  XStringFormats.Center);

                    XImage image = XImage.FromFile(rootUrl + bangViewModel.DuongDanFileDeIn);
                    gfx.DrawImage(image, 0, 0, bangViewModel.AnhLoaiBangWidth.Value * 3.7795275591, bangViewModel.AnhLoaiBangHeight.Value * 3.7795275591);
                    image.Dispose();
                }
                    
                if (document.PageCount == 0)
                {
                    Exception exception = new Exception("Vui lòng nhấn nút tạo ảnh văn bằng trước khi tải về");
                    throw exception;
                }

                // Save the document...
                document.Save(pdfpath);
                document.Close();

                responseViewModel.Data = "/" + Path.Combine(url, fileName);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("PhatVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult PhatVanBang(PhatVanBangViewModel phatVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                //string username = HttpContext.Current.User.Identity.Name;
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                phatVanBangViewModel.NgayCapNhat = DateTime.Now;
                phatVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;

                _thongTinVanBang.PhatVanBang(phatVanBangViewModel, nguoiDung.DonViId.Value);

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

        [Route("GetVanBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetVanBangs(int? truongHocId, int? loaiBangId, string hoVaTen, string nienKhoa, int? trangThaiBangId, int currentPage)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT"
                    || nguoiDung.DonVi.CapDonVi.Code == "TTGD")
                {
                    responseViewModel.Data = _thongTinVanBang.GetVanBangs(nguoiDung.DonViId.Value, loaiBangId, hoVaTen, nienKhoa, trangThaiBangId, nguoiDung.DonViId, currentPage);
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _thongTinVanBang.GetVanBangs(truongHocId, loaiBangId, hoVaTen, nienKhoa, trangThaiBangId, nguoiDung.DonViId, currentPage);
                }
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(responseViewModel);
        }

        [Route("GetSoGocs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetSoGocs(int? truongHocId, string hoVaTen, int? namTotNghiep, int currentPage)
        {
            ResponseViewModel<DanhSachSoGocViewModel> responseViewModel = new ResponseViewModel<DanhSachSoGocViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT"
                    || nguoiDung.DonVi.CapDonVi.Code == "TTGD")
                {
                    //responseViewModel.Data = _thongTinVanBang.GetSoGocs(nguoiDung.DonViId.Value, hoVaTen, namTotNghiep, nguoiDung.DonViId, currentPage);
                    responseViewModel.Data = new DanhSachSoGocViewModel();
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _thongTinVanBang.GetSoGocs(truongHocId, hoVaTen, namTotNghiep, nguoiDung.DonViId, currentPage);
                }
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("DetailThongTinBangSoGoc")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult DetailThongTinBang(int BangId)
        {
            ResponseViewModel<DanhSachSoGocViewModel> responseViewModel = new ResponseViewModel<DanhSachSoGocViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _thongTinVanBang.DetailThongTinBang(BangId, nguoiDung.DonViId.Value);

                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        //[Route("UpdateThongTinBangSoGoc")]
        //[ClaimRequirement("All")]
        //[HttpGet]
        //public IActionResult UpdateThongTinBangSoGoc()
        //{
        //    ResponseViewModel<DanhSachSoGocViewModel> responseViewModel = new ResponseViewModel<DanhSachSoGocViewModel>();
        //    try
        //    {
        //        NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
        //        _thongTinVanBang.UpdateThongTinBangSoGoc();

        //        responseViewModel.Status = true;
        //        responseViewModel.Message = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    return Ok(responseViewModel);
        //}

        [Route("GetChiTietSoGoc")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietSoGoc(int bangId)
        {
            ResponseViewModel<ChiTietSoGocViewModel> responseViewModel = new ResponseViewModel<ChiTietSoGocViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _thongTinVanBang.GetChiTietSoGoc(bangId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachHocSinhDeTaoAnhVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachHocSinhDeTaoAnhVanBang(int nhomTaoVanBangId, string hoVaTen, int currentPage)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.TrangThaiBangId == 1)
                {
                    return BadRequest("Tải dữ liệu thất bại");
                }
                DanhSachBangViewModel danhSachVanBangViewModel = _thongTinVanBang.GetDanhSachHocSinhDeTaoAnhVanBang(nhomTaoVanBangId, hoVaTen, nguoiDung.DonViId, currentPage);
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel_1 = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                danhSachVanBangViewModel.DonViIn = nhomTaoVanBangViewModel_1.DonViIn;
                danhSachVanBangViewModel.IsSoGD = nguoiDung.DonVi.CapDonVi.Code == "SOGD";
                danhSachVanBangViewModel.TrangThaiBangId = nhomTaoVanBangViewModel.TrangThaiBangId;
                danhSachVanBangViewModel.Title = nhomTaoVanBangViewModel_1.Title;
                danhSachVanBangViewModel.TruongHoc = nhomTaoVanBangViewModel_1.TruongHoc;
                danhSachVanBangViewModel.CanDelete = nhomTaoVanBangViewModel_1.CanDelete;
                responseViewModel.Data = danhSachVanBangViewModel;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachHocSinhDeCapNhatSoHieu")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachHocSinhDeCapNhatSoHieu(int nhomTaoVanBangId, int? trangThaiBangId, string hoVaTen, int currentPage)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.TrangThaiBangId != 3)
                {
                    return BadRequest("Tải dữ liệu thất bại");
                }
                DanhSachBangViewModel danhSachVanBangViewModel = _thongTinVanBang.GetDanhSachHocSinhDeCapNhatSoHieu(nhomTaoVanBangId, hoVaTen, trangThaiBangId, nguoiDung.DonViId, currentPage);
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel_1 = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                danhSachVanBangViewModel.TrangThaiBangId = nhomTaoVanBangViewModel.TrangThaiBangId;
                danhSachVanBangViewModel.ChoPhepTaoLai = nhomTaoVanBangViewModel_1.ChoPhepTaoLai;
                danhSachVanBangViewModel.NhomTaoVanBangId = nhomTaoVanBangViewModel_1.Id;
                danhSachVanBangViewModel.DonViIn = nhomTaoVanBangViewModel_1.DonViIn;
                danhSachVanBangViewModel.IsSoGD = nguoiDung.DonVi.CapDonVi.Code == "SOGD";
                danhSachVanBangViewModel.Title = nhomTaoVanBangViewModel_1.Title;
                danhSachVanBangViewModel.TruongHoc = nhomTaoVanBangViewModel_1.TruongHoc;
                responseViewModel.Data = danhSachVanBangViewModel;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachHocSinhDeCapNhatSoHieu2FA")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachHocSinhDeCapNhatSoHieu2FA(int nhomTaoVanBangId, int? trangThaiBangId, string hoVaTen, int currentPage)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.TrangThaiBangId < 4)
                {
                    return BadRequest("Tải dữ liệu thất bại");
                }
                DanhSachBangViewModel danhSachVanBangViewModel = _thongTinVanBang.GetDanhSachHocSinhDeCapNhatSoHieu(nhomTaoVanBangId, hoVaTen, trangThaiBangId, nguoiDung.DonViId, currentPage);
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel_1 = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                danhSachVanBangViewModel.TrangThaiBangId = nhomTaoVanBangViewModel.TrangThaiBangId;
                danhSachVanBangViewModel.ChoPhepTaoLai = nhomTaoVanBangViewModel_1.ChoPhepTaoLai;
                danhSachVanBangViewModel.LoaiNhomTaoVanBangId = nhomTaoVanBangViewModel_1.LoaiNhomTaoVanBangId;
                danhSachVanBangViewModel.NhomTaoVanBangId = nhomTaoVanBangViewModel_1.Id;
                danhSachVanBangViewModel.DonViIn = nhomTaoVanBangViewModel_1.DonViIn;
                danhSachVanBangViewModel.IsSoGD = nguoiDung.DonVi.CapDonVi.Code == "SOGD";
                danhSachVanBangViewModel.Title = nhomTaoVanBangViewModel_1.Title;
                danhSachVanBangViewModel.TruongHoc = nhomTaoVanBangViewModel_1.TruongHoc;
                responseViewModel.Data = danhSachVanBangViewModel;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetHocSinhTaoVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetHocSinhTaoVanBang(int nhomTaoVanBangId, string hoVaTen, int currentPage)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.ChoPhepTaoLai.HasValue && !nhomTaoVanBangViewModel.ChoPhepTaoLai.Value)
                {
                    return BadRequest("Tải dữ liệu thất bại");
                }
                DanhSachHocSinhTaoVanBangViewModel danhSachHocSinhTaoBangViewModel = _thongTinVanBang.GetHocSinhTaoVanBang(nhomTaoVanBangId, hoVaTen, nguoiDung.DonViId.Value, currentPage);
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel_1 = _thongTinVanBang.GetNhomTaoVanBang(nhomTaoVanBangId, nguoiDung.DonViId.Value);
                danhSachHocSinhTaoBangViewModel.TrangThaiBangId = nhomTaoVanBangViewModel.TrangThaiBangId;
                danhSachHocSinhTaoBangViewModel.Title = nhomTaoVanBangViewModel_1.Title;
                danhSachHocSinhTaoBangViewModel.TruongHoc = nhomTaoVanBangViewModel_1.TruongHoc;
                danhSachHocSinhTaoBangViewModel.CanDelete = nhomTaoVanBangViewModel_1.CanDelete;
                responseViewModel.Data = danhSachHocSinhTaoBangViewModel;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("AddThongTinVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddThongTinVanBang(AddThongTinVanBangViewModel addThongTinVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(addThongTinVanBangViewModel.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.ChoPhepTaoLai.HasValue && !nhomTaoVanBangViewModel.ChoPhepTaoLai.Value)
                {
                    return BadRequest("Bằng đã được in hoặc phát, không thể tạo lại!!!");
                }
                addThongTinVanBangViewModel.NgayTao = DateTime.Now;
                addThongTinVanBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                addThongTinVanBangViewModel.NgayCapNhat = DateTime.Now;
                addThongTinVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                addThongTinVanBangViewModel.DonViId = nguoiDung.DonViId.Value;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _thongTinVanBang.AddThongTinVanBang(addThongTinVanBangViewModel, nguoiDung.DonViId.Value, ip);
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

        [Route("TaoAnhVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult TaoAnhVanBang(TaoAnhVanBangViewModel taoAnhVanBang)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(taoAnhVanBang.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.ChoPhepTaoLai.HasValue && !nhomTaoVanBangViewModel.ChoPhepTaoLai.Value)
                {
                    return BadRequest("Bằng đã được in hoặc phát, không thể tạo lại!!!");
                }
                taoAnhVanBang.NgayTao = DateTime.Now;
                taoAnhVanBang.NguoiTao = nguoiDung.NguoiDungId;
                taoAnhVanBang.NgayCapNhat = DateTime.Now;
                taoAnhVanBang.NguoiCapNhat = nguoiDung.NguoiDungId;
                taoAnhVanBang.DonViId = nguoiDung.DonViId.Value;

                _thongTinVanBang.TaoAnhVanBang(taoAnhVanBang, nguoiDung.DonViId.Value);
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

        [Route("DeleteNhomTaoVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteNhomTaoVanBang(NhomTaoVanBangViewModel model)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(model.Id, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.CanDelete.HasValue && !nhomTaoVanBangViewModel.CanDelete.Value)
                {
                    Exception exception = new Exception("Không thể xóa nhóm tạo văn bằng");
                    throw exception;
                }
      
                _thongTinVanBang.DeleteNhomTaoVanBang(model.Id, nguoiDung.DonViId.Value);
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


        [Route("UpdateTrangThaiDaIn")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTrangThaiDaIn(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                hocSinhTaoVanBangViewModel.NgayTao = DateTime.Now;
                hocSinhTaoVanBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                hocSinhTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                hocSinhTaoVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                _thongTinVanBang.UpdateTrangThaiDaIn(hocSinhTaoVanBangViewModel, nguoiDung.DonViId.Value, ip);
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

        [Route("UpdateTrangThaiPhatBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTrangThaiPhatBang(PhatVanBangViewModel phatVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                phatVanBangViewModel.NgayCapNhat = DateTime.Now;
                phatVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;

                _thongTinVanBang.UpdateTrangThaiPhatBang(phatVanBangViewModel, nguoiDung.DonViId.Value, ip);
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

        [Route("ChiTietThongTinVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ChiTietThongTinVanBang(int bangId)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                responseViewModel.Data = _thongTinVanBang.ChiTietThongTinVanBang(bangId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("TaoBanSaoTuBangGoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult TaoBanSaoTuBangGoc(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                if (!hocSinhTaoVanBangViewModel.SoLuongBanSao.HasValue || hocSinhTaoVanBangViewModel.SoLuongBanSao.Value <= 0
                     || hocSinhTaoVanBangViewModel.SoLuongBanSao.Value > 10)
                {
                    return BadRequest("Số lượng bản sao phải là số nguyên từ 1 -> 10");
                }
                hocSinhTaoVanBangViewModel.NgayTao = DateTime.Now;
                hocSinhTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                hocSinhTaoVanBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                hocSinhTaoVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;

                _thongTinVanBang.TaoBanSaoTuBangGoc(hocSinhTaoVanBangViewModel, nguoiDung.DonViId.Value, ip);
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

        [Route("TaoAnhBanSao")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult TaoAnhBanSao(TaoAnhVanBangViewModel taoAnhVanBang)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(taoAnhVanBang.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);
                if (nhomTaoVanBangViewModel.ChoPhepTaoLai.HasValue && !nhomTaoVanBangViewModel.ChoPhepTaoLai.Value)
                {
                    return BadRequest("Bằng đã được in hoặc phát, không thể tạo lại!!!");
                }
                taoAnhVanBang.NgayTao = DateTime.Now;
                taoAnhVanBang.NguoiTao = nguoiDung.NguoiDungId;
                taoAnhVanBang.NgayCapNhat = DateTime.Now;
                taoAnhVanBang.NguoiCapNhat = nguoiDung.NguoiDungId;
                taoAnhVanBang.DonViId = nguoiDung.DonViId.Value;

                _thongTinVanBang.TaoAnhVanBang(taoAnhVanBang, nguoiDung.DonViId.Value);

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

        [Route("GetNhomTaoVanBangs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomTaoVanBangs(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, bool? isChungChi)
        {
            ResponseViewModel<NhomTaoVanBangsViewModel> responseViewModel = new ResponseViewModel<NhomTaoVanBangsViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                responseViewModel.Data = _thongTinVanBang.GetNhomTaoVanBangs(truongHocId, LoaiBangId, loaiNhomTaoVanBangId, trangThaiBangId, nam, nguoiDung.DonViId.Value, isChungChi);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetNhomTaoVanBangDaIns")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomTaoVanBangDaIns(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, bool? isChungChi, int currentPage)
        {
            ResponseViewModel<NhomTaoVanBangWithPaginationViewModel> responseViewModel = new ResponseViewModel<NhomTaoVanBangWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                responseViewModel.Data = _thongTinVanBang.GetNhomTaoVanBangDaIns(truongHocId, LoaiBangId, loaiNhomTaoVanBangId, trangThaiBangId, nam, isChungChi, nguoiDung.DonViId.Value, currentPage);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachBangChoTruong")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachBangChoTruong(int? trangThaiBangId, int? lopHocId, string hoVaTen, int? namTotNghiep, int currentPage)
        {
            ResponseViewModel<DanhSachBangViewModel> responseViewModel = new ResponseViewModel<DanhSachBangViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                responseViewModel.Data = _thongTinVanBang.GetDanhSachBangChoTruong(trangThaiBangId, lopHocId, hoVaTen, namTotNghiep, currentPage, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("ChuyenDonViSoIn")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> ChuyenDonViSoIn(ChuyenDonViInViewModel chuyenDonViIn)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                if (nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    return BadRequest("Có lỗi xảy ra");
                }
                _thongTinVanBang.ChuyenDonViSoIn(chuyenDonViIn, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = _thongTinVanBang.GetNhomTaoVanBang(chuyenDonViIn.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);
                List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(8, nhomTaoVanBangViewModel.DonViIn.Value);
                if (roomViewModels != null && roomViewModels.Count > 0)
                {
                    NotificationMessage notificationMessage = new NotificationMessage();
                    notificationMessage.Message = nguoiDung.DonVi.TenDonVi + " gửi yêu cầu in văn bằng";
                    notificationMessage.Url = "/danh-sach-nhom-tao-van-bang/chi-tiet-van-bang-nhap-so-hieu?idYeuCau=" + chuyenDonViIn.NhomTaoVanBangId;
                    notificationMessage.Code = "CHUYEN_DON_VI_SO_IN";
                    await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                    // add thong bao
                    List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                    foreach (RoomViewModel roomViewModel in roomViewModels)
                    {
                        ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                        thongBaoViewModel.NoiDung = notificationMessage.Message;
                        thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                        thongBaoViewModel.Title = "In văn bằng";
                        thongBaoViewModel.ThongBaoTypeId = 8;
                        thongBaoViewModel.NguoiGuiId = nguoiDung.NguoiDungId.ToString();
                        thongBaoViewModel.DonViGuiId = nguoiDung.DonViId.Value;
                        thongBaoViewModel.PhongBanGuiId = nguoiDung.PhongBan.PhongBanId;
                        thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                        thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                        thongBaoViewModel.DaDoc = false;
                        thongBaoViewModel.NgayTao = DateTime.Now;
                        thongBaoViewModel.Code = notificationMessage.Code;
                        thongBaoViewModel.Url = notificationMessage.Url;
                        thongBaoViewModels.Add(thongBaoViewModel);
                    }
                    _notificationProvider.AddThongBao(thongBaoViewModels, nguoiDung.DonViId.Value);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("ChuyenDonViPhongIn")]
        [ClaimRequirement("All")]
        [HttpPost]
        public async Task<IActionResult> ChuyenDonViPhongIn(ChuyenDonViInViewModel chuyenDonViIn)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                if (nguoiDung.DonVi.CapDonVi.Code != "SOGD")
                {
                    return BadRequest("Có lỗi xảy ra");
                }
                NhomTaoVanBangViewModel nhomTaoVanBang = _thongTinVanBang.GetNhomTaoVanBang(chuyenDonViIn.NhomTaoVanBangId.Value, nguoiDung.DonViId.Value);

                _thongTinVanBang.ChuyenDonViPhongIn(chuyenDonViIn, nguoiDung.DonViId.Value);
                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
                List<RoomViewModel> roomViewModels = _notificationProvider.GetPhongBansChoThongBaoType(9, nhomTaoVanBang.DonViId.Value);
                LoaiBangViewModel loaiBangViewModel = _loaiVanBang.GetLoaiBang(nhomTaoVanBang.LoaiBangId.Value, nguoiDung.DonViId.Value);
                if (roomViewModels != null && roomViewModels.Count > 0)
                {
                    NotificationMessage notificationMessage = new NotificationMessage();
                    notificationMessage.Message = nguoiDung.DonVi.TenDonVi + " hoàn tất In văn bằng";

                    if (loaiBangViewModel.IsChungChi.HasValue && loaiBangViewModel.IsChungChi.Value)
                    {
                        notificationMessage.Url = "/danh-sach-chung-chi-da-in/chi-tiet-nhom-chung-chi-da-in?idYeuCau=" + chuyenDonViIn.NhomTaoVanBangId;
                    }
                    else
                    {
                        notificationMessage.Url = "/danh-sach-van-bang-da-in/chi-tiet-nhom-van-bang-da-in?idYeuCau=" + chuyenDonViIn.NhomTaoVanBangId;
                    }
                    notificationMessage.Code = "CHUEYN_DON_VI_PHONG_IN";
                    await _chatHub.Clients.Groups(roomViewModels.Select(x => x.RoomName).Distinct().ToArray()).ReceiveMessage(notificationMessage);

                    // add thong bao
                    List<ThongBaoViewModel> thongBaoViewModels = new List<ThongBaoViewModel>();
                    foreach (RoomViewModel roomViewModel in roomViewModels)
                    {
                        ThongBaoViewModel thongBaoViewModel = new ThongBaoViewModel();
                        thongBaoViewModel.Id = roomViewModel.DonViId.Value + "-" + Guid.NewGuid().ToString();
                        thongBaoViewModel.NoiDung = notificationMessage.Message;
                        thongBaoViewModel.Title = loaiBangViewModel.IsChungChi.HasValue && loaiBangViewModel.IsChungChi.Value ? "Yêu cầu in chứng chỉ" : "Yêu cầu in văn bằng";
                        thongBaoViewModel.ThongBaoTypeId = 9;
                        thongBaoViewModel.NguoiGuiId = nguoiDung.NguoiDungId.ToString();
                        thongBaoViewModel.DonViGuiId = nguoiDung.DonViId.Value;
                        thongBaoViewModel.PhongBanGuiId = nguoiDung.PhongBan.PhongBanId;
                        thongBaoViewModel.DonViNhanId = roomViewModel.DonViId;
                        thongBaoViewModel.PhongBanNhanId = roomViewModel.PhongBanId;
                        thongBaoViewModel.DaDoc = false;
                        thongBaoViewModel.NgayTao = DateTime.Now;
                        thongBaoViewModel.Code = notificationMessage.Code;
                        thongBaoViewModel.Url = notificationMessage.Url;
                        thongBaoViewModels.Add(thongBaoViewModel);
                    }
                    _notificationProvider.AddThongBao(thongBaoViewModels, nguoiDung.DonViId.Value);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("CapNhatTrangThaiInBanSao")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult CapNhatTrangThaiInBanSao(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel)
        {
            ResponseViewModel<object> responseViewModel = new ResponseViewModel<object>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                hocSinhTaoVanBangViewModel.NgayTao = DateTime.Now;
                hocSinhTaoVanBangViewModel.NguoiTao = nguoiDung.NguoiDungId;
                hocSinhTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                hocSinhTaoVanBangViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                _thongTinVanBang.UpdateTrangThaiDaIn(hocSinhTaoVanBangViewModel, nguoiDung.DonViId.Value, ip);
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
    }
}
