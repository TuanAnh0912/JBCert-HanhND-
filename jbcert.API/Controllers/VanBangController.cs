using Google.Cloud.Vision.V1;
using jbcert.API.GoogleCredential;
using jbcert.API.Middleware;
using jbcert.API.Service;
using jbcert.DATA.IdentityModels;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.Providers;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Spire.Xls;
using Spire.Xls.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class VanBangController : ControllerBase
    {
        VanBangProvider _vanBanProvider = new VanBangProvider();
        NguoiDungProvider _nguoiDungProvider = new NguoiDungProvider();
        private ITuDien _tuDienProvider;
        private ILoaiVanBang _loaiBangProvider;
        private IDiaGioiHanhChinh _diaGioiHanhChinh;
        private IThongTinVanBang _thongTinVanBangProvider;
        private HocSinhProvider _hocSinhProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsSender _smsSender;
        private DonViProvider _donViProvider;
        public VanBangController(UserManager<ApplicationUser> userManager, ISmsSender smsSender)
        {
            _userManager = userManager;
            _smsSender = smsSender;
            _tuDienProvider = new TuDienProvider();
            _diaGioiHanhChinh = new DiaGioiHanhChinhProvider();
            _loaiBangProvider = new LoaiVanBangProvider();
            _thongTinVanBangProvider = new ThongTinVanBangProvider();
            _hocSinhProvider = new HocSinhProvider();
            _donViProvider = new DonViProvider();
        }
        [Route("InsertYeuCauPhatBang")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertYeuCauPhatBang(YeuCauPhatBangModel model)
        {
            try
            {
                var res = _vanBanProvider.InsertYeuCauPhatBang(model);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("GetChiTietPhatBang")]
        [HttpGet]
        public IActionResult GetChiTietPhatBang(int yeucauphatbangId)
        {
            var res = _vanBanProvider.GetChiTietYeuCauPhatBangByID(yeucauphatbangId);
            return Ok(res);
        }
        [Route("GetYeuCauPhatBang")]
        [HttpGet]
        public IActionResult GetYeuCauPhatBang(int trangthaiId)
        {
            // var res = new ResultModel();
            var nguoidung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var donviId = nguoidung.DonViId;
            var rs = _vanBanProvider.GetDanhSachCapBangByTrangThai(trangthaiId, donviId.Value);
            return Ok(rs);
        }
        [Route("InsertVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertVanBang(BangViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.TrangThaiBangId = 6;
            model.NguoiTao = nguoiDung.NguoiDungId;
            model.DonViId = nguoiDung.PhongBan.DonViId;
            var id = 0;
            var res = _vanBanProvider.InsertVanBang(model, ref id);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.VanBangId = id;
                obj.HanhDong = "Đã tải lên thông tin văn bằng";
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }

        [Route("GetVanBangSoHoa")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetVanBangSoHoa(int? LoaiBangId, string keyword, int? pageNum, int? pageSize)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var result = _vanBanProvider.GetVanBangSoHoa(LoaiBangId, keyword, nguoiDung.PhongBan.DonViId.Value);
            if (result != null)
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
        [Route("GetChiTietVanBang")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetChiTietVanBang(int VanBangId)
        {
            return Ok(_vanBanProvider.GetChiTietVanBang(VanBangId));
        }
        [Route("InsertTruongDuLieu2VanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertTruongDuLieu2VanBang(ThongTinVBViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiTao = nguoiDung.NguoiDungId;
            model.DonViId = nguoiDung.PhongBan.DonViId.Value;
            var res = _vanBanProvider.InsertTruongDuLieu2VanBang(model);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.VanBangId = model.BangId;
                obj.HanhDong = "Đã thêm trường dữ liệu: " + model.TenTruongDuLieu + " với giá trị: " + model.GiaTri;
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }
        [Route("DeleteTruongDuLieuFromVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteTruongDuLieuFromVanBang(ThongTinVBViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _vanBanProvider.DeleteTruongDuLieuFromVanBang(model);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.VanBangId = model.BangId;
                obj.HanhDong = "Đã xóa trường dữ liệu: " + model.TenTruongDuLieu;
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }
        [Route("UpdateTruongDuLieu")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTruongDuLieu(ThongTinVBViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiCapNhat = nguoiDung.NguoiDungId;
            var res = _vanBanProvider.UpdateTruongDuLieu(model);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.VanBangId = model.BangId;
                obj.HanhDong = "Đã cập nhật dữ liệu: " + model.TenTruongDuLieu + " thành: '" + model.GiaTri + "'";
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }
        [Route("UpdateVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateVanBang(BangViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            model.NguoiCapNhat = nguoiDung.NguoiDungId;
            var res = _vanBanProvider.UpdateVanBang(model);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.VanBangId = model.Id;
                obj.HanhDong = "Đã cập nhật loại bằng thành: '" + model.TenLoaiBang + "'";
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }
        [Route("DeleteAnhFromVanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteAnhFromVanBang(AnhVanBangViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _vanBanProvider.DeleteAnhFromVanBang(model);
            if (res.Status)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), model.Url);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = nguoiDung.NguoiDungId;
                obj.HanhDong = "Đã xóa file ảnh: '" + model.TenFile + "'";
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }

        [Route("InsertAnh2VanBang")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertAnh2VanBang(AnhVanBangViewModel model)
        {
            var nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var res = _vanBanProvider.InsertAnh2VanBang(model);
            if (res.Status)
            {
                var obj = new LogVanBangViewModel();
                obj.NguoiDungId = model.NguoiTao;
                obj.HanhDong = "Đã thêm file ảnh: '" + model.TenFile + "'";
                obj.ThoiGian = DateTime.Now;
                obj.HoTen = nguoiDung.HoTen;
                obj.Ip = HttpContext.Connection.RemoteIpAddress.ToString();
                _vanBanProvider.InsertLogVanBang(obj);
            }
            return Ok(res);
        }
        [Route("SoHoaByImage")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public IActionResult SoHoaByImage(SoHoaByAnhInput model)
        {
            AuthExplicit.CredentialRegistration();
            var rs = _vanBanProvider.DrawRectanglesOnImage(model.lstSoLieu,model.url);
            return Ok(rs);
        }
        [Route("InsertDataBySoHoa")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public IActionResult InsertDataBySoHoa(InputDuLieuBySoHoa model)
        {
            try
            {
                return Ok(_vanBanProvider.InsertBangBySoHoaAnh(model));
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        [Route("ImportFileVanBangSoHoa")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ImportFileVanBangSoHoa()
        {
            try
            {
                AuthExplicit.CredentialRegistration();

                var formFiles = Request.Form.Files;
                //var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<FileDinhKemYeuCauViewModel>();
                    List<EntityAnnotation> result = new List<EntityAnnotation>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/AnhSoHoa/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/AnhSoHoa/" + fileName;
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }
                        var image = System.Drawing.Image.FromFile(filePath);
                        var sizedImage = new Bitmap(image/*, new Size(520, 760)*/);
                        sizedImage.Save(fileName);

                        var readImage = Google.Cloud.Vision.V1.Image.FromFile(filePath);
                        var client = ImageAnnotatorClient.Create();
                        var response = client.DetectText(readImage);
                        result = response.Where(x => (x.BoundingPoly.Vertices[0].X >= 141)
                                                        && (x.BoundingPoly.Vertices[0].Y >= 114)
                                                        && (x.BoundingPoly.Vertices[2].X <= 261)
                                                        && (x.BoundingPoly.Vertices[2].Y <= 133)).ToList();
                    }
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetNhomSoHoas")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNhomSoHoas(int? loaiBangId, int? namTotNghiep, int currentPage, int pageSize)
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                return Ok(_vanBanProvider.GetNhomSoHoas(loaiBangId, namTotNghiep, user.DonViId.Value, currentPage, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDetailNhomSoHoa")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDetailNhomSoHoa(int soHoaId, string hoVaTen, int? truongHocId, int currentPage, int pageSize)
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                return Ok(_vanBanProvider.GetDetailNhomSoHoa(soHoaId, hoVaTen, truongHocId, user.DonViId.Value, currentPage, pageSize));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetDetailSoGocSoHoa")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDetailSoGocSoHoa(int bangId)
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                return Ok(_vanBanProvider.GetDetailSoGocSoHoa(bangId, user.DonViId.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdateSoGocSoHoa")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateSoGocSoHoa(UpdateSoGocSoHoaViewModel updateSoGocSoHoaViewModel)
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _vanBanProvider.UpdateSoGocSoHoa(updateSoGocSoHoaViewModel, user.DonViId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SoHoaByExcel")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult SoHoaByExcel(SoHoaByExcelViewModel model)
        {
            try
            {
                //model.Url = "/Upload/ImportHocSinhExcel/77/111.xlsx";
                //model.LoaiBangId = 59;
                //model.NamTotNghiep = 2014;
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                List<DanTocViewModel> danTocViewModels = _tuDienProvider.GetDanTocs();
                // read file
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(Directory.GetCurrentDirectory() + model.Url);
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
                LoaiBangViewModel loaiBangViewModel = _loaiBangProvider.GetLoaiBang(model.LoaiBangId, user.DonViId.Value);
                if (loaiBangViewModel == null)
                {
                    Exception exception = new Exception("Không tồn tại loại bằng");
                    throw exception;
                }
                else
                {
                    if (loaiBangViewModel.TruongDuLieuLoaiBangs == null || loaiBangViewModel.TruongDuLieuLoaiBangs.Count == 0)
                    {
                        Exception exception = new Exception("Không tìm thấy trường dữ liệu loại bằng");
                        throw exception;
                    }
                    bool same = rules.Where(x => x.CodeTruongDuLieu != "SOHIEU" && x.CodeTruongDuLieu != "KK" && x.CodeTruongDuLieu != "LAN-XET" && x.CodeTruongDuLieu != "XL" && x.CodeTruongDuLieu != "DIEM").All(x => loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => x.KieuDuLieu == 1).Any(k => x.CodeTruongDuLieu == k.TruongDuLieuCode));
                    if (!same)
                    {
                        Exception exception = new Exception("Thiếu mã trường dữ liệu loại bằng, vui lòng tải lại file mẫu và nhập lại thông tin");
                        throw exception;
                    }
                    else
                    {
                        foreach (var rule in rules.Where(x => loaiBangViewModel.TruongDuLieuLoaiBangs.Any(k => k.TruongDuLieuCode == x.CodeTruongDuLieu)))
                        {
                            var truongDuLieuLoaiBang = loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => x.TruongDuLieuCode == rule.CodeTruongDuLieu).FirstOrDefault();
                            rule.TenTruongDuLieu = truongDuLieuLoaiBang.TenTruongDuLieu;
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
                bangExcelFormatExcelViewModel.Bangs = new List<BangViewModel>();
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
                    BangViewModel bangViewModel = new BangViewModel();
                    bangViewModel.ThongTinVanBangs = new List<ThongTinVBViewModel>();

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
                        ThongTinVBViewModel thongTinVanBang = new ThongTinVBViewModel();

                        ThongTinTruongDuLieuHocSinhExcelViewModel thongTinTruongDuLieuHocSinhExcelViewModel = new ThongTinTruongDuLieuHocSinhExcelViewModel();
                        thongTinTruongDuLieuHocSinhExcelViewModel.CodeTruongDuLieu = rule.CodeTruongDuLieu;
                        thongTinTruongDuLieuHocSinhExcelViewModel.Value = cell.DisplayedText;
                        hocSinhExcelViewModel.ThongTinRiengs.Add(thongTinTruongDuLieuHocSinhExcelViewModel);

                        thongTinVanBang.TruongDuLieuCode = rule.CodeTruongDuLieu;
                        thongTinVanBang.GiaTri = cell.DisplayedText;
                        bangViewModel.ThongTinVanBangs.Add(thongTinVanBang);

                        if (!string.IsNullOrEmpty(rule.TenTruongDuLieu))
                        {
                            var info = (PropertyInfo)hashtable[rule.TenTruongDuLieu.ToUpper()];
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(info.PropertyType);
                            object propValue = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), cell.DisplayedText);
                            info.SetValue(hocSinhExcelViewModel.TTHS, propValue, null);
                        }

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
                        hocSinhExcelViewModel.TTHS.TrangThaiBangId = 2;
                        hocSinhExcelViewModel.TTHS.NienKhoa = model.NamTotNghiep.ToString();
                        hocSinhExcelViewModel.TTHS.NamTotNghiep = model.NamTotNghiep;
                        hocSinhExcelViewModel.TTHS.DaInBangGoc = false;
                        hocSinhExcelViewModel.TTHS.KQ = true;
                        hocSinhExcelViewModel.TTHS.CongNhanTotNghiep = true;
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

                        var diemCell = rules.Where(x => x.CodeTruongDuLieu == "DIEM").FirstOrDefault();
                        if (diemCell != null)
                        {
                            hocSinhExcelViewModel.TTHS.DiemThi = row[diemCell.Col + rowIndex].EnvalutedValue;
                        }

                        bangViewModel.HoVaTen = hocSinhExcelViewModel.TTHS.HoVaTen;
                        bangViewModel.HocSinhId = -1;
                        bangViewModel.SoVaoSo = hocSinhExcelViewModel.TTHS.SoVaoSo;
                        bangViewModel.LoaiBangId = model.LoaiBangId;
                        bangViewModel.YeuCauId = -1;
                        bangViewModel.TruongHoc = hocSinhExcelViewModel.TTHS.TruongHoc;
                        bangViewModel.TruongHocId = hocSinhExcelViewModel.TTHS.TruongHocId;
                        var soHieuCell = rules.Where(x => x.CodeTruongDuLieu == "SOHIEU").FirstOrDefault();
                        if (soHieuCell != null)
                        {
                            bangViewModel.SoHieu = row[soHieuCell.Col + rowIndex].Text;
                        }
                        bangViewModel.DonViId = user.DonViId.Value;
                        bangViewModel.NamTotNghiep = model.NamTotNghiep;
                        bangViewModel.DiemThi = hocSinhExcelViewModel.TTHS.DiemThi;
                        bangViewModel.IsChungChi = loaiBangViewModel.IsChungChi;
                        bangViewModel.IsDeleted = false;
                        bangViewModel.NgayTao = DateTime.Now;
                        bangViewModel.NguoiTao = user.NguoiDungId;
                        bangViewModel.DanToc = hocSinhExcelViewModel.TTHS.DanToc;
                        bangViewModel.NgaySinh = hocSinhExcelViewModel.TTHS.NgaySinh;
                        bangViewModel.TrangThaiBangId = 6;
                    }
                    else
                    {
                        Exception exception = new Exception("Không đúng định dạng cột trường học tại hàng " + rowIndex);
                        throw exception;
                    }


                    bangExcelFormatExcelViewModel.HocSinhs.Add(hocSinhExcelViewModel);
                    bangExcelFormatExcelViewModel.Bangs.Add(bangViewModel);
                SkipLoop: continue;
                }

                // add thong tin học sinh vào db, tạo nhóm in văn bằng
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                // add bang ko add hoc sinh (tam thoi)   
                SoHoaViewModel soHoaViewModel = new SoHoaViewModel();
                soHoaViewModel.LoaiBangId = model.LoaiBangId;
                soHoaViewModel.NamTotNghiep = model.NamTotNghiep;
                soHoaViewModel.TenLoaiBang = loaiBangViewModel.Ten;
                soHoaViewModel.DonViId = user.DonViId.Value;
                soHoaViewModel.NgayTao = DateTime.Now;
                soHoaViewModel.FileUrl = model.Url;

                var maTruongHoc = loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => x.TenTruongDuLieu == "TruongHoc").FirstOrDefault();
                if (maTruongHoc != null)
                {
                    foreach (int truongHocId in bangExcelFormatExcelViewModel.Bangs.GroupBy(x => x.TruongHocId).Select(x => x.Key))
                    {
                        DonViViewModel donViTruongHoc = _donViProvider.GetDonViById(truongHocId);
                        foreach (var bangViewModel in bangExcelFormatExcelViewModel.Bangs.Where(x => x.TruongHocId == truongHocId))
                        {
                            bangViewModel.TruongHoc = donViTruongHoc.TenDonVi;
                            var thongTinVanBang = bangViewModel.ThongTinVanBangs.Where(x => x.TruongDuLieuCode == maTruongHoc.TruongDuLieuCode).FirstOrDefault();
                            if (thongTinVanBang != null)
                            {
                                thongTinVanBang.GiaTri = donViTruongHoc.TenDonVi;
                            }
                        }
                    }
                }

                _vanBanProvider.SoHoaByExcel(model, soHoaViewModel, bangExcelFormatExcelViewModel, ip);

                return Ok(bangExcelFormatExcelViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTemplateSoHoaByExcel")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult GetTemplateSoHoaByExcel(GetTemplateHocSinhTotNghiepExcelFileViewModel model)
        {
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                LoaiBangViewModel loaiBangViewModel = _loaiBangProvider.GetLoaiBang(model.LoaiBangId, nguoiDung.DonViId.Value);
                string fileName = nguoiDung.DonViId.Value + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "TemplateExcel", fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                FileInfo file = new FileInfo(path);

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var tthsWS = package.Workbook.Worksheets.Add("TTHS");

                    var ruleWS = package.Workbook.Worksheets.Add("Quy tắc");
                    ruleWS.Cells[1, 1].Value = "Ô";
                    ruleWS.Cells[1, 2].Value = "Mã";
                    ruleWS.Cells[1, 3].Value = "Cách đọc dữ liệu";
                    ruleWS.Cells[1, 4].Value = "Mô tả";
                    ruleWS.Cells[1, 5].Value = "Chú thích";
                    ruleWS.Cells[1, 1, 1, 5].Style.Font.Bold = true;

                    int i = 2;
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => (x.KieuDuLieu == 1)).OrderByDescending(x => x.DungChung).ToList())
                    {
                        ruleWS.Cells[i, 2].Value = truongDuLieuLoaiBangViewModel.TruongDuLieuCode;
                        ruleWS.Cells[i, 3].Value = "2";
                        ruleWS.Cells[i, 4].Value = truongDuLieuLoaiBangViewModel.Ten;
                        i++;
                    }

                    ruleWS.Cells[i, 2].Value = "SOHIEU";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Số hiệu";
                    i++;

                    ruleWS.Cells[i, 2].Value = "DIEM";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Điểm thi";

                    ruleWS.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ruleWS.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ruleWS.Cells[1, 1, i, 4].AutoFitColumns();

                    var formatWS = package.Workbook.Worksheets.Add("Cách đọc dữ liệu");
                    formatWS.Cells[1, 1].Value = "Mã cách đọc dữ liệu";
                    formatWS.Cells[1, 2].Value = "Mô tả cách đọc dữ liệu";
                    formatWS.Cells[2, 1].Value = "1";
                    formatWS.Cells[2, 2].Value = "Đọc theo ô";
                    formatWS.Cells[3, 1].Value = "2";
                    formatWS.Cells[3, 2].Value = "Đọc theo cột";
                    formatWS.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                    formatWS.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    formatWS.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    formatWS.Cells[1, 1, 3, 2].AutoFitColumns();

                    var truongWS = package.Workbook.Worksheets.Add("Mã trường");
                    truongWS.Cells[1, 1].Value = "Mã trường";
                    truongWS.Cells[1, 2].Value = "Tên trường";
                    truongWS.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                    truongWS.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    truongWS.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    List<DonViViewModel> donViViewModels = _donViProvider.GetTruongHocsByDonViCha(nguoiDung.DonViId.Value);
                    int k = 2;
                    foreach (DonViViewModel donViViewModel in donViViewModels)
                    {
                        truongWS.Cells[k, 1].Value = donViViewModel.DonViId;
                        truongWS.Cells[k, 2].Value = donViViewModel.TenDonVi;
                        k++;
                    }
                    formatWS.Cells[1, 1, k, 2].AutoFitColumns();


                    package.Save();

                }
                return Ok(Path.Combine("Upload", "TemplateExcel", fileName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteNhomSoHoa")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteNhomSoHoa(DeleteNhomSoHoaViewModel model)
        {
            try
            {
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                _vanBanProvider.DeleteNhomSoHoa(model.SoHoaId, user.DonViId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("ConvertImageToExcel")]
        //[ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ConvertImageToExcel(ConvertImageToExcelViewModel model)
        {
            try
            {
                model.FileUrl = "Upload/1.jpg";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), model.FileUrl);
                //var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                AuthExplicit.CredentialRegistration();


                var readImage = Google.Cloud.Vision.V1.Image.FromFile(filePath);
                var client = ImageAnnotatorClient.Create();
                var response = client.DetectText(readImage);
                var result = response.ToList();
                var letters = result.Where(x => x.BoundingPoly.Vertices[0].X >= 44
                                                && x.BoundingPoly.Vertices[0].Y >= 73
                                                && x.BoundingPoly.Vertices[2].X <= 44 + 164
                                                && x.BoundingPoly.Vertices[2].Y <= 73 + 66).ToList();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("UploadFileSoHoa")]
       // [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UploadFileSoHoa()
        {
            try
            {
                var lstFile = new List<string>();
                var formFiles = Request.Form.Files;
             //   var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (formFiles != null && formFiles.Count() > 0)
                {
                    var files = new List<PhoiFileDinhKemViewModel>();
                    foreach (var formFile in formFiles)
                    {
                        string fileId = Guid.NewGuid().ToString();
                        string guid = DateTime.Now.ToString("yyyy-MM-dd") + "_" + fileId + "_";
                        var fileName = guid + formFile.FileName;
                        string url = "/Upload/SoGoc/" + fileName;
                        var filePath = Directory.GetCurrentDirectory() + "/Upload/SoGoc/" + fileName;
                        lstFile.Add(url);
                        string ext = Path.GetExtension(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            formFile.CopyTo(stream);
                        }

                        string path_e = GetVirtualPath(filePath);
                        #region
                        //var obj = new PhoiFileDinhKemViewModel();
                        //obj.FileId = fileId;
                        //obj.Url = url;
                        //obj.TenFile = formFile.FileName;
                        //obj.NguoiTao = user.NguoiDungId;
                        //obj.NgayTao = DateTime.Now;
                        //obj.DonViId = user.PhongBan.DonViId;
                        //obj.Ext = ext;
                        //switch (ext.ToLower())
                        //{
                        //    case ".jpg":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                        //        break;

                        //    case ".jpeg":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/jpg.png";
                        //        break;

                        //    case ".png":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/png.png";
                        //        break;

                        //    case ".pdf":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/pdf.png";
                        //        break;

                        //    case ".xlsx":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                        //        break;

                        //    case ".xls":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/excel.png";
                        //        break;

                        //    case ".doc":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                        //        break;

                        //    case ".docx":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/doc.png";
                        //        break;

                        //    case ".ppt":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/ppt.png";
                        //        break;

                        //    case ".txt":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/txt.png";
                        //        break;

                        //    case ".zip":
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/zip.png";
                        //        break;

                        //    default:
                        //        obj.IconFile = "https://apistaging.jbcert.vn/Upload/FileTypes/unknow.png";
                        //        break;
                        //}
                        //files.Add(obj);
                        #endregion
                    }
                    return Ok(lstFile.First());
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private string GetVirtualPath(string physicalPath)
        {
            string rootpath = Directory.GetCurrentDirectory();
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");
            return "/" + physicalPath;
        }
    }
}
