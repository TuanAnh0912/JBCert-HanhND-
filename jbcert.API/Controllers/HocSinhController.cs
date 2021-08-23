using ClosedXML.Excel;
using jbcert.API.Middleware;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.Provider;
using jbcert.DATA.Providers;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Spire.Xls;
using Spire.Xls.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jbcert.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientIpCheckActionFilter))]
    public class HocSinhController : ControllerBase
    {
        private IHocSinh _hocSinh;
        private NguoiDungProvider _nguoiDungProvider;
        private HocSinhProvider _hocSinhProvider;
        private DonViProvider _donViProvider;
        private ITuDien _tuDienProvider;
        private ILoaiVanBang _loaiBangProvider;
        private IDiaGioiHanhChinh _diaGioiHanhChinh;
        private IThongTinVanBang _thongTinVanBangProvider;


        public HocSinhController()
        {
            _hocSinhProvider = new HocSinhProvider(); ;
            _hocSinh = new HocSinhProvider();
            _nguoiDungProvider = new NguoiDungProvider();
            _donViProvider = new DonViProvider();
            _tuDienProvider = new TuDienProvider();
            _diaGioiHanhChinh = new DiaGioiHanhChinhProvider();
            _loaiBangProvider = new LoaiVanBangProvider();
            _thongTinVanBangProvider = new ThongTinVanBangProvider();
        }

        [Route("ImportHocSinhTotNghiepExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult ImportHocSinhTotNghiepExcelFile(ImportHocSinhTotNghiepExcelFileViewModel model)
        {
            try
            {
                //model.UrlFile = "/Upload/ImportHocSinhExcel/77/79_26-04-2021.xlsx";
                //model.LoaiBangId = 59;
                //model.NamTotNghiep = DateTime.Now.Year;
                var user = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                List<DanTocViewModel> danTocViewModels = _tuDienProvider.GetDanTocs();
                // read file
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(Directory.GetCurrentDirectory() + model.UrlFile);
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

                    foreach (var rule in rules.Where(x => x.CodeTruongDuLieu != "HK" && x.CodeTruongDuLieu != "UT" && x.CodeTruongDuLieu != "KK"
                                                    && x.CodeTruongDuLieu != "LAN-XET" && x.CodeTruongDuLieu != "XL" && x.CodeTruongDuLieu != "HDT" && x.CodeTruongDuLieu != "DIEM"
                                                    && x.Format == "2" && loaiBangViewModel.TruongDuLieuLoaiBangs.Any(k => k.TruongDuLieuCode == x.CodeTruongDuLieu)))
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
                            catch(Exception ex)
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
                            
                            try
                            {
                                hocSinhExcelViewModel.TTHS.SoLanXet = Convert.ToInt32(row[soLanXetCell.Col + rowIndex].EnvalutedValue);
                            }
                            catch (Exception ex)
                            {
                                hocSinhExcelViewModel.TTHS.SoLanXet = null;
                            }
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
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                List<NhomTaoVanBangViewModel> nhomTaoVanBangViewModels = new List<NhomTaoVanBangViewModel>();

                foreach (int truongHocId in bangExcelFormatExcelViewModel.HocSinhs.GroupBy(x => x.TruongHocId).Select(x => x.Key))
                {
                    int tt = _hocSinhProvider.GetSTTHocSinhDuocXet(model.NamTotNghiep, truongHocId);
                    DonViViewModel donViSo = _donViProvider.GetDonViSo(truongHocId);
                    DonViViewModel donViTruongHoc = _donViProvider.GetDonViById(truongHocId);
                    foreach (var hocSinh in bangExcelFormatExcelViewModel.HocSinhs.Where(x => x.TruongHocId == truongHocId))
                    {
                        hocSinh.TTHS.TT = tt;
                        hocSinh.TTHS.TruongHoc = donViTruongHoc.TenDonVi;
                        hocSinh.TTHS.SoVaoSo = string.Format("{0}/{1}/{2}/{3}", donViSo.MaDonVi, donViTruongHoc.MaDonVi, tt, model.NamTotNghiep);
                        tt++;
                    }

                    NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                    nhomTaoVanBangViewModel.Title = "Nhóm in văn bằng trường " + donViTruongHoc.TenDonVi + " năm " + model.NamTotNghiep;
                    nhomTaoVanBangViewModel.TruongHocId = truongHocId;
                    nhomTaoVanBangViewModel.DonViId = user.DonViId.Value;
                    nhomTaoVanBangViewModel.LoaiBangId = model.LoaiBangId;
                    nhomTaoVanBangViewModel.NgayTao = DateTime.Now;
                    nhomTaoVanBangViewModel.NguoiTao = user.NguoiDungId;
                    nhomTaoVanBangViewModel.NgayCapNhat = DateTime.Now;
                    nhomTaoVanBangViewModel.NguoiCapNhat = user.NguoiDungId;
                    nhomTaoVanBangViewModel.IsDeleted = false;
                    nhomTaoVanBangViewModel.CanDelete = true;
                    nhomTaoVanBangViewModel.AddedByImport = true;
                    nhomTaoVanBangViewModel.TrangThaiBangId = 1;
                    nhomTaoVanBangViewModel.ChoPhepTaoLai = true;
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
                            NamTotNghiep = hocSinh.NamTotNghiep,

                        });
                    }
                    nhomTaoVanBangViewModel.TongSohocSinh = nhomTaoVanBangViewModel.HocSinhs.Count();
                    nhomTaoVanBangViewModels.Add(nhomTaoVanBangViewModel);

                }
                foreach (NhomTaoVanBangViewModel nhomTaoVanBangViewModel in nhomTaoVanBangViewModels)
                {
                    //_thongTinVanBangProvider.AddhomTaoVanBang(nhomTaoVanBangViewModel, user.DonViId.Value);
                }

                return Ok(loaiBangViewModel.IsChungChi);
            }
            catch (Exception ex)
            {
                if (ex.GetType().ToString() == "System.FormatException")
                {
                    BadRequest("Sai định dạng dữ liệu");
                }
                return BadRequest(ex.Message);
            }
        }

        [Route("GetTemplateHocSinhTotNghiepExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult GetTemplateHocSinhTotNghiepExcelFile(GetTemplateHocSinhTotNghiepExcelFileViewModel model)
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
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => !x.DungChung && (x.KieuDuLieu == 1) && (x.TenTruongDuLieu != "SoVaoSo")).OrderByDescending(x => x.DungChung).ToList())
                    {
                        ruleWS.Cells[i, 2].Value = truongDuLieuLoaiBangViewModel.TruongDuLieuCode;
                        ruleWS.Cells[i, 2].Style.Font.Color.SetColor(Color.Red);
                        ruleWS.Cells[i, 3].Value = truongDuLieuLoaiBangViewModel.DungChung ? "1" : "2";
                        ruleWS.Cells[i, 4].Value = truongDuLieuLoaiBangViewModel.Ten;
                        i++;
                    }
                    ruleWS.Cells[i, 2].Value = "HK";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Hạnh kiểm";
                    i++;

                    ruleWS.Cells[i, 2].Value = "UT";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Ưu tiên";
                    i++;

                    ruleWS.Cells[i, 2].Value = "KK";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Diện khuyến khích";
                    i++;

                    ruleWS.Cells[i, 2].Value = "LAN-XET";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Số lần đã xét TN";
                    i++;

                    ruleWS.Cells[i, 2].Value = "XL";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Xếp loại";
                    i++;

                    ruleWS.Cells[i, 2].Value = "HDT";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Hệ đào tạo";
                    i++;

                    ruleWS.Cells[i, 2].Value = "Diem";
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

        [Route("GetTemplateHocSinhYeuCauXetDuyetTotNghiepExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult GetTemplateHocSinhYeuCauXetDuyetTotNghiepExcelFile(GetTemplateHocSinhYeuCauXetDuyetTotNghiepExcelFileViewModel model)
        {
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                DonViViewModel donVi = _donViProvider.GetDonViById(nguoiDung.DonViId.Value);
                LoaiBangViewModel loaiBangViewModel = _loaiBangProvider.GetLoaiBang(model.LoaiBangId, donVi.KhoaChaId.Value);
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
                    ruleWS.Cells[1, 1, 1, 4].Style.Font.Bold = true;

                    int i = 2;
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in loaiBangViewModel.TruongDuLieuLoaiBangs.Where(x => !x.DungChung && (x.KieuDuLieu == 1) && (x.TenTruongDuLieu != "SoVaoSo")).OrderByDescending(x => x.DungChung).ToList())
                    {
                        ruleWS.Cells[i, 2].Value = truongDuLieuLoaiBangViewModel.TruongDuLieuCode;
                        ruleWS.Cells[i, 2].Style.Font.Color.SetColor(Color.Red);
                        ruleWS.Cells[i, 3].Value = truongDuLieuLoaiBangViewModel.DungChung ? "1" : "2";
                        ruleWS.Cells[i, 4].Value = truongDuLieuLoaiBangViewModel.Ten;
                        i++;
                    }

                    ruleWS.Cells[i, 2].Value = "HK";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Hạnh kiểm";
                    i++;
                    ruleWS.Cells[i, 2].Value = "UT";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Ưu tiên";
                    i++;

                    ruleWS.Cells[i, 2].Value = "KK";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Diện khuyến khích";
                    i++;

                    ruleWS.Cells[i, 2].Value = "LAN-XET";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Số lần đã xét TN";
                    i++;

                    ruleWS.Cells[i, 2].Value = "XL";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Xếp loại";
                    i++;

                    ruleWS.Cells[i, 2].Value = "HDT";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Hệ đào tạo";
                    i++;
                    ruleWS.Cells[i, 2].Value = "Diem";
                    ruleWS.Cells[i, 3].Value = "2";
                    ruleWS.Cells[i, 4].Value = "Điểm thi";

                    ruleWS.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ruleWS.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ruleWS.Cells[1, 1, i, 5].AutoFitColumns();

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

                    int k = 2;
                    truongWS.Cells[k, 1].Value = donVi.DonViId;
                    truongWS.Cells[k, 2].Value = donVi.TenDonVi;
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

        [Route("GetTemplateHocSinhYeuCauXetDuyetTotNghiepCoDuLieuExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult GetTemplateHocSinhYeuCauXetDuyetTotNghiepCoDuLieuExcelFile()
        {
            try
            {
                string path = Path.Combine("Upload", "TemplateExcel", "MauDuLieuHocSinhYeuCauXetDuyet.xlsx");
                return Ok(path);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("GetTemplateCoDuLieuHocSinhTotNghiepExcelFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult GetTemplateCoDuLieuHocSinhTotNghiepExcelFile()
        {
            try
            {
                string path = Path.Combine("Upload", "TemplateExcel", "MauDuLieuYeuCauXetDuyetTotNghiep.xlsx");
                return Ok(path);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[Route("ImportDanhSachDaXetDuyet")]
        //[ClaimRequirement("All")]
        //[HttpPost]
        //public IActionResult ImportDanhSachDaXetDuyet()
        //{
        //    ResponseViewModel<List<string>> responseViewModel = new ResponseViewModel<List<string>>();
        //    try
        //    {
        //        NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
        //        var formFiles = Request.Form.Files;
        //        foreach (var formFile in formFiles)
        //        {
        //            string fileName = Guid.NewGuid() + "_" + formFile.FileName;
        //            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "ImportSoGoc", fileName);
        //            string ext = Path.GetExtension(filePath);
        //            if (ext.ToLower() == ".xls" || ext.ToLower() == ".xlsx")
        //            {
        //                using (var stream = System.IO.File.Create(filePath))
        //                {
        //                    formFile.CopyTo(stream);
        //                }

        //                // Read data to Excel file
        //                ExcelEngine excelEngine = new ExcelEngine();
        //                IApplication application = excelEngine.Excel;
        //                application.DefaultVersion = ExcelVersion.Excel2016;
        //                IWorkbook workbook = application.Workbooks.Open(System.IO.File.OpenRead(filePath));
        //                IWorksheet worksheet = workbook.Worksheets[0];
        //                IRange usedRange = worksheet.UsedRange;
        //                int lastRow = usedRange.LastRow;
        //                int lastColumn = usedRange.LastColumn;
        //                List<string> lst = new List<string>();
        //                lst.Add(worksheet[7,1].Value);
        //            }
        //        }
        //        //_hocSinh.ImportSoGoc(, nguoiDung.DonViId.Value);
        //        responseViewModel.Status = true;
        //        responseViewModel.Message = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    return Ok(responseViewModel);
        //}

        [Route("GetHocSinh")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetHocSinh(int hocSinhId)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinh.GetHocSinh(hocSinhId, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetHocSinhs")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetHocSinhs(int lopHocId, int namTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinh.GetHocSinhs(lopHocId, namTotNghiep, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachCanPheDuyet")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachCanPheDuyet(int lopHocId, int namTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinh.GetDanhSachCanPheDuyet(lopHocId, namTotNghiep, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachDaXetDuyet")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachDaXetDuyet(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                string nienKhoa = namTotNghiep.HasValue ? namTotNghiep.ToString() : null;

                responseViewModel.Data = _hocSinh.GetDanhSachDaXetDuyet(hoVaTen, namSinh, gioiTinhId, lopHocId, nienKhoa, danTocId, hL, hK, kQ, xepLoaiTotNghiep, congNhanTotNghiep, currentPage, nguoiDung.DonViId.Value);
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

        [Route("GetDanhSachDuXetCongNhanTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachDuXetCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, string nienKhoa, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinh.GetDanhSachDuXetCongNhanTotNghiep(hoVaTen, namSinh, gioiTinhId, lopHocId, nienKhoa, danTocId, hL, hK, kQ, xepLoaiTotNghiep, congNhanTotNghiep, currentPage, nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetDanhSachDaCongNhanTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT" || nguoiDung.DonVi.CapDonVi.Code == "TTGD")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaCongNhanTotNghiep(hoVaTen, namSinh, gioiTinhId, nguoiDung.DonViId.Value, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, currentPage, nguoiDung.DonViId.Value);
                    responseViewModel.Data.IsTruong = true;
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaCongNhanTotNghiep(hoVaTen, namSinh, gioiTinhId, truongHocId, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, currentPage, nguoiDung.DonViId.Value);
                    responseViewModel.Data.IsTruong = false;
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

        [Route("GetDanhSachDaCongNhanTotNghiepTheoDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT" || nguoiDung.DonVi.CapDonVi.Code == "TTGD")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaCongNhanTotNghiepTheoDonVi(hoVaTen, namSinh, lopHocId, gioiTinhId, nguoiDung.DonViId.Value, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, currentPage, nguoiDung.DonViId.Value);
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD" || nguoiDung.DonVi.CapDonVi.Code == "BOGD")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaCongNhanTotNghiepTheoDonVi(hoVaTen, namSinh, lopHocId, gioiTinhId, truongHocId, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, currentPage, nguoiDung.DonViId.Value);
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

        [Route("ExportDanhSachDaCongNhanTotNghiepTheoDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ExportDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT" || nguoiDung.DonVi.CapDonVi.Code == "TTGD")
                {
                    responseViewModel.Data = _hocSinh.ExportDanhSachDaCongNhanTotNghiepTheoDonVi(hoVaTen, namSinh, lopHocId, gioiTinhId, nguoiDung.DonViId.Value, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, nguoiDung.DonViId.Value);
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD" || nguoiDung.DonVi.CapDonVi.Code == "BOGD")
                {
                    responseViewModel.Data = _hocSinh.ExportDanhSachDaCongNhanTotNghiepTheoDonVi(hoVaTen, namSinh, lopHocId, gioiTinhId, truongHocId, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, nguoiDung.DonViId.Value);
                }

                foreach (var hocsinh in responseViewModel.Data.HocSinhs)
                {
                    hocsinh.Ho = hocsinh.HoVaTen.Trim().Substring(0, hocsinh.HoVaTen.Trim().LastIndexOf(" "));
                    hocsinh.Ten = hocsinh.HoVaTen.Trim().Split(" ").Last();
                }

                if (truongHocId.HasValue)
                {
                    DonViViewModel donViViewModel = _donViProvider.GetDonViById(truongHocId.Value);
                    responseViewModel.Data.TenTruong = donViViewModel.TenDonVi;
                }
                else
                {
                    responseViewModel.Data.TenTruong = nguoiDung.DonVi.TenDonVi;
                }
                if (nguoiDung.DonVi.DiaGioiHanhChinh.Length == 4)
                {
                    responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetHuyenByXa(nguoiDung.DonVi.DiaGioiHanhChinh.Substring(0, 4)).Ten;
                }
                else if (nguoiDung.DonVi.DiaGioiHanhChinh.Length == 2)
                {
                    responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetTinhById(nguoiDung.DonVi.DiaGioiHanhChinh).Ten;
                }
                int namTotNghiep = Convert.ToInt32(nienKhoa);
                DateTime now = DateTime.Now;
                responseViewModel.Data.Ngay = now.Day;
                responseViewModel.Data.Thang = now.Month;
                responseViewModel.Data.Nam = now.Year;
                responseViewModel.Data.NienKhoa = "NĂM HOC: " + (namTotNghiep - 1) + " - " + namTotNghiep;

                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }


        [Route("GetDanhSachDaTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc, int currentPage)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaTotNghiep(hoVaTen, namSinh, gioiTinhId, nguoiDung.DonViId.Value, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, daInBangGoc, currentPage, nguoiDung.DonViId.Value);
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _hocSinh.GetDanhSachDaTotNghiep(hoVaTen, namSinh, gioiTinhId, truongHocId, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, daInBangGoc, currentPage, nguoiDung.DonViId.Value);
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

        [Route("GetNienKhoas")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetNienKhoas()
        {
            ResponseViewModel<List<NienKhoaViewModel>> responseViewModel = new ResponseViewModel<List<NienKhoaViewModel>>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinh.GetNienKhoas(nguoiDung.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("ExportDanhSachDaXetDuyet")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ExportDanhSachDaXetDuyet(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var danhSachHocSinh = _hocSinh.ExportDanhSachDaXetDuyetTotNghiep(hoVaTen, namSinh, gioiTinhId, lopHocId, namTotNghiep, danTocId, hL, hK, kQ, xepLoaiTotNghiep, congNhanTotNghiep, nguoiDung.DonViId.Value);
                foreach (var hocsinh in danhSachHocSinh.HocSinhs)
                {
                    hocsinh.Ho = hocsinh.HoVaTen.Substring(0, hocsinh.HoVaTen.LastIndexOf(" "));
                    hocsinh.Ten = hocsinh.HoVaTen.Split(" ").Last();
                }
                responseViewModel.Data = danhSachHocSinh;
                responseViewModel.Data.TenTruong = nguoiDung.DonVi.TenDonVi;
                responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetHuyenByXa(nguoiDung.DonVi.DiaGioiHanhChinh.Substring(0, 4)).Ten;
                DateTime now = DateTime.Now;
                responseViewModel.Data.Ngay = now.Day;
                responseViewModel.Data.Thang = now.Month;
                responseViewModel.Data.Nam = now.Year;
                responseViewModel.Data.NienKhoa = namTotNghiep.HasValue ? "NĂM HOC: " + (namTotNghiep - 1) + " - " + namTotNghiep : "";

                switch (nguoiDung.DonVi.CapDonVi.Code)
                {
                    case "TIEUHOC":
                        responseViewModel.Data.CapTruong = "TIỂU HỌC";
                        break;
                    case "THCS":
                        responseViewModel.Data.CapTruong = "TRUNG HỌC CƠ SỞ";
                        break;
                    case "THPT":
                        responseViewModel.Data.CapTruong = "TRUNG HỌC PHỔ THÔNG";
                        break;
                    default:
                        responseViewModel.Data.CapTruong = "UNKNOWN";
                        break;
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

        [Route("GetDanhSachHocSinhDuXetCapTruong")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetDanhSachHocSinhDuXetCapTruong(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var danhSachHocSinh = _hocSinh.GetDanhSachHocSinhDuXetCapTruong(hoVaTen, namSinh, gioiTinhId, lopHocId, namTotNghiep, danTocId, hL, hK, kQ, xepLoaiTotNghiep, congNhanTotNghiep, nguoiDung.DonViId.Value);
                foreach (var hocsinh in danhSachHocSinh.HocSinhs)
                {
                    hocsinh.Ho = hocsinh.HoVaTen.Substring(0, hocsinh.HoVaTen.LastIndexOf(" "));
                    hocsinh.Ten = hocsinh.HoVaTen.Split(" ").Last();
                }
                responseViewModel.Data = danhSachHocSinh;
                responseViewModel.Data.TenTruong = nguoiDung.DonVi.TenDonVi;
                responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetHuyenByXa(nguoiDung.DonVi.DiaGioiHanhChinh.Substring(0, 4)).Ten;
                DateTime now = DateTime.Now;
                responseViewModel.Data.Ngay = now.Day;
                responseViewModel.Data.Thang = now.Month;
                responseViewModel.Data.Nam = now.Year;
                responseViewModel.Data.NienKhoa = namTotNghiep.HasValue ? "NĂM HOC: " + (namTotNghiep - 1) + " - " + namTotNghiep : "";

                switch (nguoiDung.DonVi.CapDonVi.Code)
                {
                    case "TIEUHOC":
                        responseViewModel.Data.CapTruong = "TIỂU HỌC";
                        break;
                    case "THCS":
                        responseViewModel.Data.CapTruong = "TRUNG HỌC CƠ SỞ";
                        break;
                    case "THPT":
                        responseViewModel.Data.CapTruong = "TRUNG HỌC CƠ SỞ";
                        break;
                    default:
                        responseViewModel.Data.CapTruong = "UNKNOWN";
                        break;
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

        [Route("ExportDanhSachDaCongNhanTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ExportDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, int? namTotNghiep, int? danTocId, string hL, string hK, string xepLoaiTotNghiep)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                var danhSachHocSinh = _hocSinh.ExportDanhSachDaCongNhanTotNghiep(hoVaTen, namSinh, gioiTinhId, truongHocId, namTotNghiep, danTocId, hL, hK, xepLoaiTotNghiep, nguoiDung.DonViId.Value);
                foreach (var hocsinh in danhSachHocSinh.HocSinhs)
                {
                    hocsinh.Ho = hocsinh.HoVaTen.Trim().Substring(0, hocsinh.HoVaTen.Trim().LastIndexOf(" "));
                    hocsinh.Ten = hocsinh.HoVaTen.Trim().Split(" ").Last();
                }

                responseViewModel.Data = danhSachHocSinh;
                if (truongHocId.HasValue)
                {
                    DonViViewModel donViViewModel = _donViProvider.GetDonViById(truongHocId.Value);
                    responseViewModel.Data.TenTruong = donViViewModel.TenDonVi;
                }
                else
                {
                    responseViewModel.Data.TenTruong = nguoiDung.DonVi.TenDonVi;
                }
                if (nguoiDung.DonVi.DiaGioiHanhChinh.Length == 4)
                {
                    responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetHuyenByXa(nguoiDung.DonVi.DiaGioiHanhChinh.Substring(0, 4)).Ten;
                }
                else if (nguoiDung.DonVi.DiaGioiHanhChinh.Length == 2)
                {
                    responseViewModel.Data.TenHuyen = _diaGioiHanhChinh.GetTinhById(nguoiDung.DonVi.DiaGioiHanhChinh).Ten;
                }
                DateTime now = DateTime.Now;
                responseViewModel.Data.Ngay = now.Day;
                responseViewModel.Data.Thang = now.Month;
                responseViewModel.Data.Nam = now.Year;
                responseViewModel.Data.NienKhoa = namTotNghiep.HasValue ? "NĂM HOC: " + (namTotNghiep - 1) + " - " + namTotNghiep : "";

                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("ExportDanhSachDaTotNghiep")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult ExportDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc)
        {
            ResponseViewModel<HocSinhsWithPaginationViewModel> responseViewModel = new ResponseViewModel<HocSinhsWithPaginationViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                if (nguoiDung.DonVi.CapDonVi.Code == "THCS" || nguoiDung.DonVi.CapDonVi.Code == "TIEUHOC" || nguoiDung.DonVi.CapDonVi.Code == "THPT")
                {
                    responseViewModel.Data = _hocSinh.ExportDanhSachDaTotNghiep(hoVaTen, namSinh, gioiTinhId, nguoiDung.DonViId.Value, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, daInBangGoc, nguoiDung.DonViId.Value);
                }
                else if (nguoiDung.DonVi.CapDonVi.Code == "PHONGGD" || nguoiDung.DonVi.CapDonVi.Code == "SOGD")
                {
                    responseViewModel.Data = _hocSinh.ExportDanhSachDaTotNghiep(hoVaTen, namSinh, gioiTinhId, truongHocId, nienKhoa, danTocId, hL, hK, xepLoaiTotNghiep, daInBangGoc, nguoiDung.DonViId.Value);
                }
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

        [Route("AddHocSinh")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddHocSinh(HocSinhViewModel hocSinhViewModel)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                if (!hocSinhViewModel.HoVaTen.Trim().Contains(" "))
                {
                    responseViewModel.Data = null;
                    responseViewModel.Status = false;
                    responseViewModel.Message = "Phải nhập đúng họ và tên!!!";

                    return BadRequest("Phải nhập đúng họ và tên!!!");
                }

                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                LopHocViewModel lopHocViewModel = _hocSinhProvider.GetLopHoc(hocSinhViewModel.LopHocId.Value, nguoiDung.DonViId.Value);
                hocSinhViewModel.DanToc = _tuDienProvider.GetDanTocs().Where(x => x.Id == hocSinhViewModel.DanTocId).FirstOrDefault().Ten;
                hocSinhViewModel.GioiTinh = _tuDienProvider.GetGioiTinhs().Where(x => x.Id == hocSinhViewModel.GioiTinhId).FirstOrDefault().Ten;
                hocSinhViewModel.LopHoc = lopHocViewModel.TenLop;
                hocSinhViewModel.TruongHocId = nguoiDung.DonViId.Value;
                hocSinhViewModel.TruongHoc = _donViProvider.GetById(nguoiDung.DonViId.Value).TenDonVi;
                hocSinhViewModel.NgayTao = DateTime.Now;
                hocSinhViewModel.NguoiTao = nguoiDung.NguoiDungId;
                hocSinhViewModel.NgayCapNhat = DateTime.Now;
                hocSinhViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                hocSinhViewModel.IsDeleted = false;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _hocSinh.AddHocSinh(hocSinhViewModel, nguoiDung.DonViId.Value, ip);

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

        [Route("UpdateHocSinh")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateHocSinh(HocSinhViewModel hocSinhViewModel)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                if (!hocSinhViewModel.HoVaTen.Trim().Contains(" "))
                {
                    responseViewModel.Data = null;
                    responseViewModel.Status = false;
                    responseViewModel.Message = "Phải nhập đúng họ và tên!!!";

                    return BadRequest("Phải nhập đúng họ và tên!!!");
                }

                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                LopHocViewModel lopHocViewModel = _hocSinhProvider.GetLopHoc(hocSinhViewModel.LopHocId.Value, nguoiDung.DonViId.Value);
                hocSinhViewModel.DanToc = _tuDienProvider.GetDanTocs().Where(x => x.Id == hocSinhViewModel.DanTocId).FirstOrDefault().Ten;
                hocSinhViewModel.GioiTinh = _tuDienProvider.GetGioiTinhs().Where(x => x.Id == hocSinhViewModel.GioiTinhId).FirstOrDefault().Ten;
                hocSinhViewModel.LopHoc = lopHocViewModel.TenLop;
                hocSinhViewModel.NgayCapNhat = DateTime.Now;
                hocSinhViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                hocSinhViewModel.IsDeleted = false;

                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _hocSinh.UpdateHocSinh(hocSinhViewModel, nguoiDung.DonViId.Value, ip);

                responseViewModel.Data = null;
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                responseViewModel.Data = null;
                responseViewModel.Status = false;
                responseViewModel.Message = ex.Message;
                return BadRequest(responseViewModel);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateTrangThaiXetDuyet")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateTrangThaiXetDuyet(HocSinhViewModel hocSinhViewModel)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                //if (hocSinhViewModel.KQ.Value)
                //{
                //    DonViViewModel donViViewModel = _donViProvider.GetDonViSo(nguoiDung.DonViId.Value);
                //    int stt = _hocSinh.GetSTTHocSinhDuocXet(hocSinhViewModel.Id, hocSinhViewModel.NamTotNghiep.Value, nguoiDung.DonViId.Value);
                //    hocSinhViewModel.SoVaoSo = donViViewModel.MaDonVi + "/" + nguoiDung.DonVi.MaDonVi + "/" + stt + "/" + DateTime.Now.Year;
                //}

                hocSinhViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                hocSinhViewModel.NgayCapNhat = DateTime.Now;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _hocSinh.UpdateTrangThaiXetDuyet(hocSinhViewModel, nguoiDung.DonViId.Value, ip);

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

        [Route("DeleteHocSinhs")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteHocSinhs(List<HocSinhViewModel> hocSinhViewModels)
        {
            ResponseViewModel<HocSinhViewModel> responseViewModel = new ResponseViewModel<HocSinhViewModel>();
            try
            {
                NguoiDung nguoiDung = _nguoiDungProvider.GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                foreach (HocSinhViewModel hocSinhViewModel in hocSinhViewModels)
                {
                    hocSinhViewModel.NgayCapNhat = DateTime.Now;
                    hocSinhViewModel.NguoiCapNhat = nguoiDung.NguoiDungId;
                }
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();

                _hocSinh.DeleteHocSinhs(hocSinhViewModels, nguoiDung.DonViId.Value, ip);

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

        /// <summary>
        /// Thêm file đính kèm thông tin học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddDinhKemFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddDinhKemFile(HocSinhViewModel model)
        {
            return Ok(_hocSinhProvider.AddAttachFile(model.Files, model.Id));
        }

        /// <summary>
        /// Xóa file đính kèm của học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteDinhKemFile")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteFileDinhKemHocSinh(HocSinhFileDinhKemViewModel model)
        {
            var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
            var result = _hocSinhProvider.DeleteAttachFile(model, user);
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


        [Route("AddLopHoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult AddLopHoc(LopHocViewModel model)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                _hocSinhProvider.AddLopHoc(model, user.DonViId.Value);
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("UpdateLopHoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult UpdateLopHoc(LopHocViewModel model)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                _hocSinhProvider.AddLopHoc(model, user.DonViId.Value);
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("DeleteLopHoc")]
        [ClaimRequirement("All")]
        [HttpPost]
        public IActionResult DeleteLopHoc(LopHocViewModel model)
        {
            ResponseViewModel<Object> responseViewModel = new ResponseViewModel<Object>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                _hocSinhProvider.DeleteLopHoc(model.Id);
                responseViewModel.Status = true;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetLopHocsByGiaoVien")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetLopHocsByGiaoVien()
        {
            ResponseViewModel<List<LopHocViewModel>> responseViewModel = new ResponseViewModel<List<LopHocViewModel>>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _hocSinhProvider.GetLopHocByGiaoVien(user.NguoiDungId, user.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetAllLopHocTrongTruong")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetAllLopHocTrongTruong()
        {
            ResponseViewModel<List<LopHocViewModel>> responseViewModel = new ResponseViewModel<List<LopHocViewModel>>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));
                responseViewModel.Data = _hocSinhProvider.GetAllLopHocTrongTruong(user.DonViId.Value);
                responseViewModel.Status = true;
                responseViewModel.Message = "";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(responseViewModel);
        }

        [Route("GetMonHocsByDonVi")]
        [ClaimRequirement("All")]
        [HttpGet]
        public IActionResult GetMonHocsByDonVi()
        {
            ResponseViewModel<List<MonHocViewModel>> responseViewModel = new ResponseViewModel<List<MonHocViewModel>>();
            try
            {
                var user = new NguoiDungProvider().GetByTenDangNhap(User.FindFirstValue(ClaimTypes.Name));

                responseViewModel.Data = _hocSinhProvider.GetMonHocsByCapDonVi(user.DonVi.CapDonVi.Code);
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