using Google.Cloud.Vision.V1;
using jbcert.DATA.Helpers;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Provider
{
    public class VanBangProvider : ApplicationDbContext
    {
        public ResultModel InsertVanBang(BangViewModel model, ref int id)
        {
            using var transaction = DbContext.Database.BeginTransaction();
            try
            {
                var obj = new Bang();
                obj.LoaiBangId = model.LoaiBangId;
                obj.HocSinhId = -1;
                obj.HoVaTen = model.HoVaTen;
                obj.NamTotNghiep = model.NamTotNghiep;
                obj.SoHieu = model.SoHieu;
                obj.SoVaoSo = model.SoVaoSo;
                obj.TruongHocId = model.TruongHocId;
                obj.TruongHoc = model.TruongHoc;
                obj.NguoiTao = model.NguoiTao;
                obj.NgayTao = DateTime.Now;
                obj.DonViId = model.DonViId;
                obj.TrangThaiBangId = 6;
                obj.SoLanCaiChinh = 0;
                DbContext.Bangs.Add(obj);
                DbContext.SaveChanges();
                id = obj.Id;
                var lst = new List<ThongTinVanBang>();
                foreach (var item in model.ThongTinVanBangs)
                {
                    var o = new ThongTinVanBang();
                    o.BangId = obj.Id;
                    o.NguoiTao = model.NguoiTao;
                    o.NgayTao = DateTime.Now;
                    o.TruongDuLieuCode = item.TruongDuLieuCode;
                    o.GiaTri = item.GiaTri;
                    o.DonViId = model.DonViId.Value;
                    lst.Add(o);
                }
                var lstImg = new List<AnhVanBang>();
                if (model.AnhVanBangs != null)
                {
                    foreach (var item in model.AnhVanBangs)
                    {
                        var o = new AnhVanBang();
                        o.FileId = item.FileId;
                        o.Url = item.Url;
                        o.TenFile = item.TenFile;
                        o.BangId = obj.Id;
                        o.NgayTao = item.NgayTao;
                        o.NguoiTao = item.NguoiTao;
                        o.Ext = item.Ext;
                        o.IconFile = item.IconFile;
                        o.DonViId = item.DonViId;
                        lstImg.Add(o);
                    }
                    DbContext.AnhVanBangs.AddRange(lstImg);
                }
                DbContext.ThongTinVanBangs.AddRange(lst);
                DbContext.SaveChanges();

                transaction.Commit();
                return new ResultModel(true, "Tải lên thông tin văn bằng thành công");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel UpdateTrangThaiBang(int yeucauId, int trangthaiyeucauId)
        {
            try
            {
                // todo
                var model = DbContext.YeuCauPhatBangs.Where(x => x.YeuCauPhatBangId == yeucauId).FirstOrDefault();
                model.TrangThaiYeuCauPhatBangId = trangthaiyeucauId;
                return new ResultModel(true, "Cập nhật trạng thái yêu cầu thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, "Cập nhật trạng thái yêu cầu thất bại");
            }
        }
        //public ThongKeTrangThaiBangChungChi
        public List<ChiTietYeuCauPhatBangViewModel> GetChiTietYeuCauPhatBangByID(int yeucauphatbangId)
        {
            try
            {
                string sql = @"select a.Mota,b.DuongDanAnh from YeuCauPhatBangs as a
                                inner join AnhYeuCauPhatBangs as b on a.YeuCauPhatBangId=b.YeuCauPhatBangId";
                var lst = new List<ChiTietYeuCauPhatBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        lst = MapDataHelper<ChiTietYeuCauPhatBangViewModel>.MapList(reader);
                    }
                }
                return lst;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<DanhSachPhatBangViewModel> GetDanhSachCapBangByTrangThai(int trangthaiId, int donviId)
        {
            try
            {
                int capdonviId;
                string sqldv = "select CapDonViId from DonVi where DonViId=@donviId";
                var lstDScapbang = new List<DanhSachPhatBangViewModel>();
                //lấy cấp đơn vị
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqldv;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@donviId", donviId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            capdonviId = MapDataHelper<int>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }

                }
                var lst = new List<int>();
                string lstDonViDuocxem;
                //lấy list danh sách các đơn vị được phép xem
                if (capdonviId == 5)
                {
                    lstDonViDuocxem = @"select DonViId from DonVi as a
                                            left join CapDonVi as b on a.CapDonViId=b.CapDonViId
                                            where (b.Level >=@capdonviId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        command.CommandText = lstDonViDuocxem;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@capdonviId", capdonviId));
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                lst = MapDataHelper<int>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }

                    }
                }
                else if (capdonviId == 4)
                {
                    lstDonViDuocxem = @"select DonViId from DonVi as a
                                        left join CapDonVi as b on a.CapDonViId=b.CapDonViId
                                        where (b.Level >=@capdonviId) and (b.Code!='THPT')";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        command.CommandText = lstDonViDuocxem;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@capdonviId", capdonviId));
                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                lst = MapDataHelper<int>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }

                    }
                }

                else lst.Add(donviId);
                int m = 0;
                List<string> sqlParams = lst.Select(x => "@Id_" + m++).ToList();
                string sql = @"select d.TenDonVi,a.Mota,b.TenTrangThaiYeuCauPhatBang,a.NgayTaoYeuCau from YeuCauPhatBangs as a
                            left join TrangThaiYeuCauPhatBangs as b on a.TrangThaiYeuCauPhatBangId=b.TrangThaiYeuCauPhatBangId
                            left join DonVi as d on d.DonViId=a.DonViId
                            where (a.TrangThaiYeuCauPhatBangId=@trangthaiId ) and (a.DonViId in {0})
							order by a.NgayTaoYeuCau DESC";
                sql = string.Format(sql, string.Join(",", sqlParams.ToArray()));

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@trangthaiId", trangthaiId));
                    int k = 0;
                    foreach (var a in lst)
                    {
                        command.Parameters.Add(new SqlParameter("@Id_" + k++, a));
                    }

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            lstDScapbang = MapDataHelper<DanhSachPhatBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }

                }
                return lstDScapbang;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel InsertYeuCauPhatBang(YeuCauPhatBangModel yeuCauModel)
        {
            using var transaction = DbContext.Database.BeginTransaction();
            try
            {

                if (yeuCauModel.LstDuongDanAnh == null)
                {
                    return new ResultModel(false, "Lỗi,Chưa có ảnh yêu cầu");
                }
                var model = new YeuCauPhatBang();
                model.BangId = yeuCauModel.BangId;
                model.NgayTaoYeuCau = DateTime.Now;
                model.TrangThaiYeuCauPhatBangId = 1;
                model.DonViId = GetDonViByBangId(yeuCauModel.BangId);//nếu bằng không có đơn vị trả ra -1
                DbContext.YeuCauPhatBangs.Add(model);
                DbContext.SaveChanges();
                var lstAnhYc = new List<AnhYeuCauPhatBang>();
                foreach (var x in yeuCauModel.LstDuongDanAnh)
                {
                    var anhYeuCau = new AnhYeuCauPhatBang();
                    anhYeuCau.DuongDanAnh = x;
                    anhYeuCau.YeuCauPhatBangId = model.YeuCauPhatBangId;
                    lstAnhYc.Add(anhYeuCau);
                }
                DbContext.AnhYeuCauPhatBangs.AddRange(lstAnhYc);
                DbContext.SaveChanges();

                transaction.Commit();
                return new ResultModel(true, "Thêm yêu cầu thành công");

            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResultModel(false, e.Message);
            }
        }
        public int GetDonViByBangId(int bangId)
        {
            try
            {
                int rs;
                string sql = @"select DonViId from Bang where Id=@bangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@bangId", bangId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            rs = MapDataHelper<int>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }

                }
                return rs;
            }
            catch
            {
                return -1;
            }

        }
        public List<BangViewModel> GetVanBangSoHoa(int? LoaiBangId, string keyword, int DonViId)
        {
            try
            {
                string sql = @" Select a.*, l.Ten as TenLoaiBang From Bang as a 
                                inner join LoaiBang as l on a.LoaiBangId = l.Id 
                                Where a.Id in (Select a.Id From Bang as a
                                Left Join [ThongTinVanBang] as b
                                on a.Id = b.BangId Where a.HocSinhId = -1 and a.DonViId = " + DonViId + " and ((@keyword is null) or (b.GiaTri Like N'%'+@keyword+'%')) Group by a.Id)";
                if (LoaiBangId.HasValue)
                {
                    sql += " and a.LoaiBangId = " + LoaiBangId.Value;
                }
                var lstVanBangs = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    var lst = new List<BangViewModel>();
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword));
                        using (var reader = command.ExecuteReader())
                        {
                            lst = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                        foreach (var item in lst)
                        {
                            command.CommandText = "select a.*, b.Ten as TenTruongDuLieu, b.KieuDuLieu from ThongTinVanBang as a inner join TruongDuLieu as b on a.TruongDuLieuCode = b.Code where a.DonViId = " + DonViId + " and a.BangId = " + item.Id;
                            using (var reader2 = command.ExecuteReader())
                            {
                                item.ThongTinVanBangs = MapDataHelper<ThongTinVBViewModel>.MapList(reader2);
                            }

                            command.CommandText = "select * from AnhVanBangs where BangId = " + item.Id;
                            using (var reader3 = command.ExecuteReader())
                            {
                                item.AnhVanBangs = MapDataHelper<AnhVanBangViewModel>.MapList(reader3);
                            }
                            lstVanBangs.Add(item);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lstVanBangs;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public BangViewModel GetChiTietVanBang(int VanBangId)
        {
            try
            {
                string sql = @"  Select a.*, l.Ten as TenLoaiBang From Bang as a 
                                inner join LoaiBang as l on a.LoaiBangId = l.Id 
                                Where a.Id = @vanBangId";
                var vanBang = new BangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@vanBangId", VanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            vanBang = MapDataHelper<BangViewModel>.Map(reader);
                        }

                        command.CommandText = "select a.*, b.Ten as TenTruongDuLieu, b.KieuDuLieu from ThongTinVanBang as a inner join TruongDuLieu as b on a.TruongDuLieuCode = b.Code where a.DonViId = " + vanBang.DonViId + " and a.BangId = @vanBangId1";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@vanBangId1", VanBangId));
                        using (var reader2 = command.ExecuteReader())
                        {
                            vanBang.ThongTinVanBangs = MapDataHelper<ThongTinVBViewModel>.MapList(reader2);
                        }

                        command.CommandText = "select * from AnhVanBangs where BangId = @vanBangId2";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@vanBangId2", VanBangId));
                        using (var reader3 = command.ExecuteReader())
                        {
                            vanBang.AnhVanBangs = MapDataHelper<AnhVanBangViewModel>.MapList(reader3);
                        }

                        command.CommandText = "select * from LogVanBang where VanBangId = @vanBangId3 ORDER BY ThoiGian DESC";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@vanBangId3", VanBangId));
                        using (var reader3 = command.ExecuteReader())
                        {
                            vanBang.Logs = MapDataHelper<LogVanBangViewModel>.MapList(reader3);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return vanBang;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel InsertTruongDuLieu2VanBang(ThongTinVBViewModel model)
        {
            try
            {
                var o = new ThongTinVanBang();
                o.NguoiTao = model.NguoiTao;
                o.TruongDuLieuCode = model.TruongDuLieuCode;
                o.BangId = model.BangId;
                o.GiaTri = model.GiaTri;
                o.NgayTao = DateTime.Now;
                o.NguoiTao = model.NguoiTao;
                o.DonViId = model.DonViId;
                DbContext.ThongTinVanBangs.Add(o);
                DbContext.SaveChanges();
                return new ResultModel(true, "Thêm mới dữ liệu thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel DeleteTruongDuLieuFromVanBang(ThongTinVBViewModel model)
        {
            try
            {
                ThongTinVanBang obj;
                if (model.Id != 0)
                {
                    string sql = @"Delete ThongTinVanBang Where Id = @Id";
                    var vanBang = new BangViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sql;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@Id", model.Id));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else
                {
                    obj = DbContext.ThongTinVanBangs.FirstOrDefault(o => o.TruongDuLieuCode == model.TruongDuLieuCode && o.BangId == model.BangId);
                    DbContext.ThongTinVanBangs.Remove(obj);
                    DbContext.SaveChanges();
                }
                return new ResultModel(true, "Cập nhật dữ liệu thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel UpdateTruongDuLieu(ThongTinVBViewModel model)
        {
            try
            {
                var obj = DbContext.ThongTinVanBangs.FirstOrDefault(o => o.TruongDuLieuCode == model.TruongDuLieuCode && o.BangId == model.BangId);
                obj.GiaTri = model.GiaTri;
                obj.NguoiCapNhat = model.NguoiCapNhat;
                obj.NgayCapNhat = DateTime.Now;
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật dữ liệu thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public void InsertLogVanBang(LogVanBangViewModel model)
        {
            var obj = new LogVanBang();
            obj.NguoiDungId = model.NguoiDungId;
            obj.VanBangId = model.VanBangId;
            obj.HanhDong = model.HanhDong;
            obj.ThoiGian = model.ThoiGian;
            obj.HoTen = model.HoTen;
            obj.Ip = model.Ip;
            DbContext.LogVanBangs.Add(obj);
            DbContext.SaveChanges();
        }
        public ResultModel UpdateVanBang(BangViewModel model)
        {
            try
            {
                var obj = DbContext.Bangs.FirstOrDefault(b => b.Id == model.Id);
                obj.LoaiBangId = model.LoaiBangId;
                obj.NgayCapNhat = DateTime.Now;
                obj.NguoiCapNhat = model.NguoiCapNhat;
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật thông tin văn bằng thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel DeleteAnhFromVanBang(AnhVanBangViewModel model)
        {
            try
            {
                var obj = DbContext.AnhVanBangs.FirstOrDefault(o => o.FileId == model.FileId);
                DbContext.AnhVanBangs.Remove(obj);
                DbContext.SaveChanges();
                return new ResultModel(true, "Xóa ảnh thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel InsertAnh2VanBang(AnhVanBangViewModel model)
        {
            try
            {
                var obj = new AnhVanBang();
                obj.FileId = model.FileId;
                obj.Url = model.Url;
                obj.TenFile = model.TenFile;
                obj.BangId = model.BangId;
                obj.NgayTao = model.NgayTao;
                obj.NguoiTao = model.NguoiTao;
                obj.Ext = model.Ext;
                obj.IconFile = model.IconFile;
                obj.DonViId = model.DonViId;
                DbContext.AnhVanBangs.Add(obj);
                DbContext.SaveChanges();
                return new ResultModel(true, "Thêm ảnh thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public List<IDictionary<string, string>> DrawRectanglesOnImage(List<DuLieuSoHoaTuAnhModel> model,string url)
        {
            try
            {
                var rs = new ResultDataSoHoaByAnh();
                url = url.Substring(1);
                url=url.Replace("/","//");
                string file = (Path.Combine(Directory.GetCurrentDirectory(), url));
                var image = System.Drawing.Image.FromFile(file);
                var thumbnailBitmap = image;
                var thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
                thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                #region
                // Brush red = new SolidBrush(Color.Red);
                // brush red = new solidbrush(color.red);
                // Pen redpen = new Pen(red, 1);
                #endregion
                #region
                //foreach (var x in model)
                //{
                //    double adheigh = x.ToaDo.First().TopLeftY, w = 167, he, mar = 0;
                //    he = Math.Sqrt(Math.Pow((x.ToaDo.First().TopLeftX - x.ToaDo.First().BotLeftX), 2) + Math.Pow((x.ToaDo.First().TopLeftY - x.ToaDo.First().BotLeftY), 2));
                //    w = Math.Sqrt(Math.Pow(x.ToaDo.First().TopLeftX - x.ToaDo.First().TopRightX, 2) - Math.Pow(x.ToaDo.First().TopLeftY - x.ToaDo.First().TopRightY, 2));
                //    while (adheigh <= image.Height)
                //    {
                //        var test = Convert.ToInt32(adheigh);
                //        var tesstt = Convert.ToInt32(w);
                //        thumbnailGraph.DrawRectangle(redpen, Convert.ToInt32(x.ToaDo.First().TopLeftX), Convert.ToInt32(adheigh), Convert.ToInt32(w), Convert.ToInt32(he));
                //        adheigh += mar + he;
                //    }
                //}
                #endregion
                // Lấy toạ độ các ô tương ứng của các trường phía dưới
                foreach (var t in model)
                {
                    t.GiaTri = new List<string>();
                    var w = Math.Sqrt(Math.Pow(t.ToaDo.TopLeftX - t.ToaDo.TopRightX, 2) + Math.Pow(t.ToaDo.TopLeftY - t.ToaDo.TopRightY, 2));
                    var h = Math.Sqrt(Math.Pow((t.ToaDo.TopLeftX - t.ToaDo.BotLeftX), 2) + Math.Pow((t.ToaDo.TopLeftY - t.ToaDo.BotLeftY), 2));
                    Rectangle cropRect = new Rectangle(Convert.ToInt32(t.ToaDo.TopLeftX), Convert.ToInt32(t.ToaDo.TopLeftY), Convert.ToInt32(w), Convert.ToInt32(h));
                    Bitmap newImage = new Bitmap(cropRect.Width, cropRect.Height);
                    Graphics g = Graphics.FromImage(newImage);
                    g.DrawImage(image, new Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel);
                    g.Dispose();
                    string fileout = Path.Combine(Directory.GetCurrentDirectory(), @"Upload\SoGoc", "CropImage.jpg");
                    newImage.Save(fileout);
                    #region
                    var luu = new ToaDoModel();
                    luu = t.ToaDo;
                    double y = luu.BotLeftY;
                    w = Math.Sqrt(Math.Pow(luu.TopLeftX - luu.TopRightX, 2) - Math.Pow(luu.TopLeftY - luu.TopRightY, 2));
                    var check = Math.Pow((luu.TopLeftX - luu.BotLeftX), 2) + Math.Pow((luu.TopLeftY - luu.BotLeftY), 2);
                    //var h = Math.Sqrt(Math.Pow((luu.TopLeftX - luu.BotLeftX), 2) + Math.Pow((luu.TopLeftY - luu.BotLeftY), 2));
                    #endregion
                    var readImage = Google.Cloud.Vision.V1.Image.FromFile(fileout);
                    var client = ImageAnnotatorClient.Create();
                    var response = client.DetectText(readImage);
                    var lstString = new List<string>();
                    t.GiaTri = response.First().Description.Split('\n').ToList();
                    #region
                    //var result = response.Where(x => (x.BoundingPoly.Vertices[0].X >= t.ToaDo.TopLeftX)
                    //                                     && (x.BoundingPoly.Vertices[0].Y >= t.ToaDo.TopLeftY)
                    //                                     && (x.BoundingPoly.Vertices[1].X <= t.ToaDo.TopRightX)
                    //                                     && (x.BoundingPoly.Vertices[1].Y >= t.ToaDo.TopRightY)
                    //                                     && (x.BoundingPoly.Vertices[2].X <= t.ToaDo.BotRightX)
                    //                                     && (x.BoundingPoly.Vertices[2].Y <= t.ToaDo.BotRightY)
                    //                                     && (x.BoundingPoly.Vertices[3].X >= t.ToaDo.BotLeftX)
                    //                                     && (x.BoundingPoly.Vertices[3].Y <= t.ToaDo.BotLeftY)) &&
                    //                                     (x.Description.Split('\n')).ToList();
                    while (y <= image.Height)
                    {

                        var new_ToaDo = new ToaDoModel();
                        //toạ độ TopLeft
                        new_ToaDo.TopLeftX = luu.TopLeftX;
                        new_ToaDo.TopLeftY = luu.BotLeftY + t.Margin;
                        //toạ độ TopRight
                        new_ToaDo.TopRightX = luu.TopRightX;
                        new_ToaDo.TopRightY = luu.BotLeftY + t.Margin;
                        //toạ độ BotLeft
                        new_ToaDo.BotLeftX = luu.TopLeftX;
                        new_ToaDo.BotLeftY = luu.BotLeftY + t.Margin + h;
                        //toạ độ BotRight
                        new_ToaDo.BotRightX = luu.TopRightX;
                        new_ToaDo.BotRightY = luu.BotLeftY + t.Margin + h;
                        luu = new_ToaDo;
                        if (y > image.Height) break;
                        t.ToaDo.Add(new_ToaDo);
                        y += t.Margin + h;

                    }
                    foreach (var i in t.ToaDo)
                    {
                        var yMoi = i.BotLeftY + t.Margin + h / 2;
                        var xMoiLeft = i.TopLeftX - 10;
                        var xMoiRight = i.TopRightX + 10;
                        var result = response.Where(x => (x.BoundingPoly.Vertices[0].X >= xMoiLeft)
                                                         && (x.BoundingPoly.Vertices[0].Y >= i.TopLeftY)
                                                         && (x.BoundingPoly.Vertices[1].X <= xMoiRight)
                                                         && (x.BoundingPoly.Vertices[1].Y >= i.TopRightY)
                                                         && (x.BoundingPoly.Vertices[2].X <= xMoiRight)
                                                         && (x.BoundingPoly.Vertices[2].Y <= yMoi)
                                                         && (x.BoundingPoly.Vertices[3].X >= xMoiLeft)
                                                         && (x.BoundingPoly.Vertices[3].Y <= yMoi)).ToList();
                        string GiaTri = "";
                        foreach (var x in result) GiaTri += x.Description + " ";
                        t.GiaTri.Add(GiaTri);
                    }
                    #endregion
                }
                // Nhóm các trường dữ liệu lấy được thành thông tin của các học sinh
                var lstDataHocSinhTuAnh = new DanhSachHocSinhSoHoaTuAnhViewModel();
                lstDataHocSinhTuAnh.DanhSachHocSinhSoHoa = new List<List<DataSoHoaModel>>();
                int k = 0;
                int j = 0;
                int check1 = 0;
                // var LstdataSoHoa = new List<DataSoHoaModel>();
                var lstData = new List<IDictionary<string, string>>();
                int max = model[0].GiaTri.Count - 1;
                foreach (var x in model)
                {
                    if (x.GiaTri.Count > max)
                    {
                        max = x.GiaTri.Count;
                    }
                }
                var lstDataSoH0a = new List<IDictionary<string, string>>();
                IDictionary<string, string> numberNames = new Dictionary<string, string>();
                while (k < max)
                {

                    //var CodeGt = new DataSoHoaModel();
                    if (j > model.Count - 1)
                    {
                        j = 0;
                        if (check1 != model.Count)
                        {
                            //lstDataHocSinhTuAnh.DanhSachHocSinhSoHoa.Add(LstdataSoHoa);
                            lstDataSoH0a.Add(numberNames);
                            check1 = 0;
                        }
                        // LstdataSoHoa = new List<DataSoHoaModel>();
                        lstData = new List<IDictionary<string, string>>();
                        numberNames = new Dictionary<string, string>();
                        k++;
                        continue;
                    }
                    if ((model[j].TruongDuLieuCode != "") && (model[j].GiaTri[k] != ""))
                    {

                        numberNames.Add(model[j].TruongDuLieuCode, model[j].GiaTri[k]);
                    }
                    //CodeGt.GiaTri = model[j].GiaTri[k];
                    //if (model[j].GiaTri[k] == "") check1++;
                    //CodeGt.TruongDuLieuCode = model[j].TruongDuLieuCode;
                    //LstdataSoHoa.Add(CodeGt);
                    j++;
                }
                var lstDataParse = new List<string>();
                foreach (var x in lstDataSoH0a)
                {
                    string json = JsonConvert.SerializeObject(x, Formatting.Indented);
                    lstDataParse.Add(json);
                }
                #region
                //Map dữ liệu vào HocSinhModel
                //Type businessEntityType = typeof(HocSinhViewModel);
                //Hashtable hashtable = new Hashtable();
                //PropertyInfo[] properties = businessEntityType.GetProperties();
                //foreach (PropertyInfo info in properties)
                //{
                //    hashtable[info.Name.ToUpper()] = info;
                //}
                //var lstHocSinh = new List<HocSinhViewModel>();
                //foreach (var x in lstDataHocSinhTuAnh.DanhSachHocSinhSoHoa)
                //{
                //    var hs = new HocSinhViewModel();
                //    foreach (var y in x)
                //    {
                //        var TenTruongDuLieu = GetTenTruongDuLieuByCode(y.TruongDuLieuCode, model.First().loaiBangId);
                //        var info = (PropertyInfo)hashtable[TenTruongDuLieu.ToUpper()];
                //        TypeConverter typeConverter = TypeDescriptor.GetConverter(info.PropertyType);
                //        object propValue = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), y.GiaTri);
                //        info.SetValue(hs, propValue, null);
                //    }
                //    lstHocSinh.Add(hs);
                //}
                #endregion
                thumbnailGraph.Dispose();
                thumbnailBitmap.Dispose();
                image.Dispose();
                return lstDataSoH0a;
            }
            catch (Exception e)
            {

                return new List<IDictionary<string, string>>();
            }
        }
        public string GetTenTruongDuLieuByCode(string code, int loaiBangId)
        {
            string rs = "";
            string sql = @"select  TenTruongDuLieu from TruongDuLieuLoaiBang where TruongDuLieuCode=@code and LoaiBangId=@loaiBangId";
            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
            {
                bool wasOpen = command.Connection.State == ConnectionState.Open;
                if (!wasOpen) command.Connection.Open();
                try
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@code", code));
                    command.Parameters.Add(new SqlParameter("@loaiBangId", loaiBangId));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rs = reader["TenTruongDuLieu"].ToString();
                        }
                    }
                }
                catch (Exception e)
                {

                    throw;
                }
            }
            return rs;
        }

        public void DeleteNhomSoHoa(int soHoaId, int donViId)
        {
            try
            {
                string sqlString = @"Delete LogHocSinh Where HocSinhId In (Select HocSinhId From Bang Where (SoHoaId = @SoHoaId) and (DonViId = @DonViId));
                                    Delete HocSinh Where Id In (Select HocSinhId From Bang Where (SoHoaId = @SoHoaId) and (DonViId = @DonViId));
                                    Delete LogVanBang Where VanBangId In (Select Id From Bang Where (SoHoaId = @SoHoaId) and (DonViId = @DonViId));
                                    Delete ThongTinVanBang Where BangId In (Select Id From Bang Where (SoHoaId = @SoHoaId) and (DonViId = @DonViId));
                                    Delete Bang Where  (SoHoaId = @SoHoaId) and (DonViId = @DonViId);
                                    Delete SoHoas Where (Id = @SoHoaId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@SoHoaId", soHoaId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SoHoaByExcel(SoHoaByExcelViewModel soHoaByExcelViewModel, SoHoaViewModel soHoaViewModel, BangExcelFormatExcelViewModel bangExcelFormatExcelViewModel, string ip)
        {
            try
            {
                SoHoa soHoa = new SoHoa();
                soHoa.LoaiBangId = soHoaViewModel.LoaiBangId;
                soHoa.NamTotNghiep = soHoaViewModel.NamTotNghiep;
                soHoa.NgayTao = soHoaViewModel.NgayTao;
                soHoa.TenLoaiBang = soHoaViewModel.TenLoaiBang;
                soHoa.FileUrl = soHoaViewModel.FileUrl;
                soHoa.DonViId = soHoaViewModel.DonViId;
                DbContext.SoHoas.Add(soHoa);
                DbContext.SaveChanges();

                var lst = new List<HocSinhFileDinhKem>();
                SoGocFileDinhKem soGocFileDinhKem = new SoGocFileDinhKem();
                soGocFileDinhKem.FileId = soHoaByExcelViewModel.FileId;
                soGocFileDinhKem.Url = soHoaByExcelViewModel.Url;
                soGocFileDinhKem.TenFile = soHoaByExcelViewModel.TenFile;
                soGocFileDinhKem.SoHoaId = soHoa.Id;
                soGocFileDinhKem.NgayTao = soHoaViewModel.NgayTao;
                soGocFileDinhKem.DonViId = soHoaViewModel.DonViId;
                soGocFileDinhKem.IconFile = soHoaByExcelViewModel.IconFile;
                soGocFileDinhKem.Ext = soHoaByExcelViewModel.Ext;

                DbContext.SoGocFileDinhKem.Add(soGocFileDinhKem);
                DbContext.SaveChanges();

                List<HocSinh> hocSinhs = new List<HocSinh>();
                List<LogHocSinh> logHocSinhs = new List<LogHocSinh>();

                foreach (HocSinhViewModel hocSinhViewModel in bangExcelFormatExcelViewModel.HocSinhs.Select(x => x.TTHS))
                {
                    HocSinh hocSinh = new HocSinh();
                    hocSinh.HoVaTen = string.IsNullOrEmpty(hocSinhViewModel.HoVaTen) ? "" : hocSinhViewModel.HoVaTen.Trim();
                    hocSinh.NgaySinh = hocSinhViewModel.NgaySinh.HasValue ? hocSinhViewModel.NgaySinh : DateTime.Now;
                    hocSinh.NoiSinh = string.IsNullOrEmpty(hocSinhViewModel.NoiSinh) ? "" : hocSinhViewModel.NoiSinh.Trim();
                    hocSinh.HoKhauThuongTru = string.IsNullOrEmpty(hocSinhViewModel.HoKhauThuongTru) ? "" : hocSinhViewModel.HoKhauThuongTru.Trim();
                    hocSinh.TruongHocId = hocSinhViewModel.TruongHocId;
                    hocSinh.TruongHoc = hocSinhViewModel.TruongHoc;
                    hocSinh.LopHoc = hocSinhViewModel.LopHoc;
                    hocSinh.LopHocId = hocSinhViewModel.LopHocId;
                    hocSinh.DanTocId = hocSinhViewModel.DanTocId;
                    hocSinh.GioiTinhId = hocSinhViewModel.GioiTinhId;
                    hocSinh.NamTotNghiep = hocSinhViewModel.NamTotNghiep;
                    hocSinh.XepLoaiTotNghiep = hocSinhViewModel.XepLoaiTotNghiep;
                    hocSinh.HinhThucDaoTao = hocSinhViewModel.HinhThucDaoTao;
                    hocSinh.GioiTinh = hocSinhViewModel.GioiTinh;
                    hocSinh.DanToc = hocSinhViewModel.DanToc;
                    hocSinh.SoVaoSo = hocSinhViewModel.SoVaoSo;
                    hocSinh.XetHK = hocSinhViewModel.XetHK;
                    hocSinh.UT = hocSinhViewModel.UT;
                    hocSinh.KK = hocSinhViewModel.KK;
                    hocSinh.HL = hocSinhViewModel.HL;
                    hocSinh.HK = hocSinhViewModel.HK;
                    hocSinh.KQ = hocSinhViewModel.KQ;
                    hocSinh.TT = hocSinhViewModel.TT;
                    hocSinh.CongNhanTotNghiep = hocSinhViewModel.CongNhanTotNghiep;
                    hocSinh.DaInBangGoc = false;
                    hocSinh.NgayTao = hocSinhViewModel.NgayTao;
                    hocSinh.NgayCapNhat = hocSinhViewModel.NgayCapNhat;
                    hocSinh.NguoiTao = hocSinhViewModel.NguoiTao;
                    hocSinh.NguoiCapNhat = hocSinhViewModel.NguoiCapNhat;
                    hocSinh.IsDeleted = false;
                    hocSinh.DonViId = hocSinhViewModel.TruongHocId;
                    hocSinh.SoLanXet = hocSinh.SoLanXet;

                    hocSinhs.Add(hocSinh);
                }
                DbContext.HocSinhs.AddRange(hocSinhs);
                DbContext.SaveChanges();

                foreach (HocSinh hocSinh in hocSinhs)
                {
                    var objLog = new LogHocSinh();
                    var user = new NguoiDungProvider().GetById(hocSinh.NguoiCapNhat.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã thêm thêm mới thông tin học sinh " + hocSinh.HoVaTen;
                    objLog.HocSinhId = hocSinh.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    logHocSinhs.Add(objLog);
                }

                DbContext.LogHocSinhs.AddRange(logHocSinhs);
                DbContext.SaveChanges();
                int k = 0;
                List<Bang> bangs = new List<Bang>();
                foreach (BangViewModel bangViewModel in bangExcelFormatExcelViewModel.Bangs)
                {
                    Bang bang = new Bang();
                    bang.HoVaTen = bangViewModel.HoVaTen;
                    bang.SoVaoSo = bangViewModel.SoVaoSo;
                    bang.TruongHoc = bangViewModel.TruongHoc;
                    bang.LoaiBangId = bangViewModel.LoaiBangId;
                    bang.IsChungChi = bangViewModel.IsChungChi;
                    bang.TrangThaiBangId = bangViewModel.TrangThaiBangId;
                    bang.YeuCauId = -1;
                    bang.TruongHocId = bangViewModel.TruongHocId;
                    bang.HocSinhId = hocSinhs[k++].Id;
                    bang.SoHieu = bangViewModel.SoHieu;
                    bang.DonViId = bangViewModel.DonViId;
                    bang.NamTotNghiep = bangViewModel.NamTotNghiep;
                    bang.DiemThi = bangViewModel.DiemThi;
                    bang.IsChungChi = bangViewModel.IsChungChi;
                    bang.IsDeleted = false;
                    bang.NgayTao = DateTime.Now;
                    bang.NguoiTao = bangViewModel.NguoiTao;
                    bang.SoHoaId = soHoa.Id;
                    bangs.Add(bang);
                }

                DbContext.Bangs.AddRange(bangs);
                DbContext.SaveChanges();

                List<ThongTinVanBang> thongTinVanBangs = new List<ThongTinVanBang>();
                List<LogVanBang> logVanBangs = new List<LogVanBang>();
                for (int i = 0; i < bangExcelFormatExcelViewModel.Bangs.Count; i++)
                {
                    foreach (ThongTinVBViewModel thongTinVBViewModel in bangExcelFormatExcelViewModel.Bangs[i].ThongTinVanBangs)
                    {
                        ThongTinVanBang thongTinVanBang = new ThongTinVanBang();
                        thongTinVanBang.GiaTri = thongTinVBViewModel.GiaTri;
                        thongTinVanBang.DonViId = soHoaViewModel.DonViId.Value;
                        thongTinVanBang.BangId = bangs[i].Id;
                        thongTinVanBang.TruongDuLieuCode = thongTinVBViewModel.TruongDuLieuCode;
                        thongTinVanBangs.Add(thongTinVanBang);
                    }

                    var objLog = new LogVanBang();
                    var user = new NguoiDungProvider().GetById(bangExcelFormatExcelViewModel.Bangs[i].NguoiTao.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Số hóa sổ gốc cho học sinh " + bangExcelFormatExcelViewModel.Bangs[i].HoVaTen;
                    objLog.VanBangId = bangs[i].Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    logVanBangs.Add(objLog);
                }

                DbContext.ThongTinVanBangs.AddRange(thongTinVanBangs);
                DbContext.SaveChanges();

                // add log
                DbContext.LogVanBangs.AddRange(logVanBangs);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public NhomSoHoaWithPaginationViewModel GetNhomSoHoas(int? loaiBangId, int? namTotNghiep, int donViId, int currentPage, int pageSize)
        {
            try
            {
                NhomSoHoaWithPaginationViewModel nhomSoHoaWithPaginationViewModel = new NhomSoHoaWithPaginationViewModel();
                nhomSoHoaWithPaginationViewModel.SoHoas = new List<SoHoaViewModel>();
                string sqlString = @"Select * From SoHoas 
                                Where ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) 
                                        and (DonViId = @DonViId) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep))
                                Order by NgayTao Desc
                                Offset @Offset Rows 
                                Fetch Next @Next Row Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * pageSize));
                    command.Parameters.Add(new SqlParameter("@Next", pageSize));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            nhomSoHoaWithPaginationViewModel.SoHoas = MapDataHelper<SoHoaViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }

                }

                int totalRow = 0;
                string sqlString_1 = @"Select Count(*) From SoHoas
                                Where ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) 
                                    and (DonViId = @DonViId) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_1;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * pageSize));
                    command.Parameters.Add(new SqlParameter("@Next", pageSize));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }

                }

                nhomSoHoaWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / pageSize));
                return nhomSoHoaWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DetailNhomSoHoaViewModel GetDetailNhomSoHoa(int soHoaId, string hoVaTen, int? truongHocId, int donViId, int currentPage, int pageSize)
        {
            try
            {
                DetailNhomSoHoaViewModel detailNhomSoHoaViewModel = new DetailNhomSoHoaViewModel();
                detailNhomSoHoaViewModel.SoGocs = new List<BangViewModel>();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.Ten as 'TrangThaiBang', b.MaMauTrangThai as 'MaMauTrangThaiBang'
                                    From Bang as a
                                    Left Join TrangThaiBang as b
                                    on a.TrangThaiBangId = b.Id
                                    Where ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
                                    and (a.SoHoaId = @SoHoaId) and (a.DonViId = @DonViId) and (a.BangGocId is null)
                                    and ((a.TrangThaiBangId >= 4) or (a.SoLanCaiChinh > 0))
                                    Order By RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@SoHoaId", soHoaId));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));

                        using (var reader = command.ExecuteReader())
                        {
                            detailNhomSoHoaViewModel.SoGocs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get tong so row
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                    From Bang as a
                                    Left Join TrangThaiBang as b
                                    on a.TrangThaiBangId = b.Id
                                    Where ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
                                    and (a.SoHoaId = @SoHoaId) and (a.DonViId = @DonViId) and (a.BangGocId is null)
                                    and ((a.TrangThaiBangId >= 4) or (a.SoLanCaiChinh > 0))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@SoHoaId", soHoaId));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                    detailNhomSoHoaViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    detailNhomSoHoaViewModel.CurrentPage = currentPage;
                }


                // get file
                var sqlString_2 = "select * from SoGocFileDinhKem where SoHoaId = @SoHoaId";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@SoHoaId", soHoaId));
                        using (var reader = command.ExecuteReader())
                        {
                            detailNhomSoHoaViewModel.SoHoaFiles = MapDataHelper<SoHoaFileDinhKemViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return detailNhomSoHoaViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DetailSoGocSoHoaViewModel GetDetailSoGocSoHoa(int bangId, int donViId)
        {
            try
            {
                DetailSoGocSoHoaViewModel detailSoGocSoHoaViewModel = new DetailSoGocSoHoaViewModel();
                string sqlString = @"Select a.*, b.*, c.Ten as 'TenTruongDuLieu' From ThongTinVanBang as a
                                    Left Join Bang as b
                                    on b.Id = a.BangId
                                    Left Join TruongDuLieu as c
                                    on a.TruongDuLieuCode = c.Code
                                    Where (a.BangId = @BangId) and (b.DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            detailSoGocSoHoaViewModel.ThongTinVanBang = MapDataHelper<ThongTinVBViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                string sqlString_1 = @"Select a.*, b.DanToc, b.GioiTinh, b.HK, b.HL, b.HinhThucDaoTao,
		                                    b.HoKhauThuongTru, b.KK, b.KQ, b.LopHoc, 
		                                    b.NoiSinh, b.XepLoaiTotNghiep, b.UT, b.XetHK, b.NgaySinh,
		                                    c.Ten as 'TrangThaiBang', c.MaMauTrangThai as 'MaMauTrangThaiBang'
                                    From Bang as a
                                    Left Join HocSinh as b
                                    on a.HocSinhId = b.Id
                                    Left Join TrangThaiBang as c
                                    on a.TrangThaiBangId = c.Id
                                    Where (a.Id = @BangId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            detailSoGocSoHoaViewModel.Bang = MapDataHelper<BangViewModel>.Map(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return detailSoGocSoHoaViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSoGocSoHoa(UpdateSoGocSoHoaViewModel updateSoGocSoHoaViewModel, int donViId)
        {
            try
            {
                BangViewModel bangViewModel = new BangViewModel();
                string sqlString = @"Select * From Bang Where (Id = @Id) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Id", updateSoGocSoHoaViewModel.BangId));

                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModel = MapDataHelper<BangViewModel>.Map(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                TruongDuLieuLoaiBang truongDuLieuLoaiBang = new TruongDuLieuLoaiBang();
                string sqlString_1 = @"Select * From TruongDuLieuLoaiBang
                                        Where (TruongDuLieuCode = @TruongDuLieuCode) and (LoaiBangId = @LoaiBangId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", updateSoGocSoHoaViewModel.TruongDuLieuCode));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", bangViewModel.LoaiBangId));

                        using (var reader = command.ExecuteReader())
                        {
                            truongDuLieuLoaiBang = MapDataHelper<TruongDuLieuLoaiBang>.Map(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (!truongDuLieuLoaiBang.DungChung.Value)
                {
                    // update hoc sinh
                    Type businessEntityHocSinhType = typeof(HocSinh);
                    Hashtable hashtableHocSinh = new Hashtable();
                    foreach (PropertyInfo item in businessEntityHocSinhType.GetProperties())
                    {
                        hashtableHocSinh[item.Name.ToUpper()] = item;
                    }
                    var propertyInfoHocSinh = (PropertyInfo)hashtableHocSinh[truongDuLieuLoaiBang.TenTruongDuLieu.ToUpper()];
                    if (propertyInfoHocSinh != null)
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyInfoHocSinh.PropertyType);
                        try
                        {
                            object propValue = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), updateSoGocSoHoaViewModel.GiaTri);
                            string sqlString_2 = @"Update HocSinh
                                                    Set {0} = @GiaTri
                                                    Where Id = @Id";
                            sqlString_2 = string.Format(sqlString_2, truongDuLieuLoaiBang.TenTruongDuLieu);
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_2;
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@GiaTri", propValue));
                                    command.Parameters.Add(new SqlParameter("@Id", bangViewModel.HocSinhId));
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }

                            if (truongDuLieuLoaiBang.TenTruongDuLieu.ToUpper() == "DANTOC")
                            {
                                DanToc danToc = new DanToc();
                                string sqlString_4 = @"Select * From DanToc Where Ten = @Ten";
                                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                                {
                                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                                    if (!wasOpen) command.Connection.Open();

                                    try
                                    {
                                        command.CommandText = sqlString_4;
                                        command.CommandType = CommandType.Text;
                                        command.Parameters.Add(new SqlParameter("@Ten", donViId));

                                        using (var reader = command.ExecuteReader())
                                        {
                                            danToc = MapDataHelper<DanToc>.Map(reader);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    finally
                                    {
                                        command.Connection.Close();
                                    }
                                }
                                if (danToc != null && !string.IsNullOrEmpty(danToc.Ten))
                                {
                                    string sqlString_5 = @"Update HocSinh
                                                            Set DanTocId = @DanTocId
                                                            Where Id = @Id";
                                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                                    {
                                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                                        if (!wasOpen) command.Connection.Open();

                                        try
                                        {
                                            command.CommandText = sqlString_5;
                                            command.CommandType = CommandType.Text;
                                            command.Parameters.Add(new SqlParameter("@DanTocId", danToc.Id));
                                            command.Parameters.Add(new SqlParameter("@Id", bangViewModel.Id));

                                            using (var reader = command.ExecuteReader())
                                            {
                                                danToc = MapDataHelper<DanToc>.Map(reader);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        finally
                                        {
                                            command.Connection.Close();
                                        }
                                    }
                                }
                            }

                            if (truongDuLieuLoaiBang.TenTruongDuLieu.ToUpper() == "GIOITINH")
                            {
                                string sqlString_4 = @"Update HocSinh
                                                        Set GioiTinhId = @GioiTinhId
                                                        Where Id = @Id";
                                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                                {
                                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                                    if (!wasOpen) command.Connection.Open();

                                    try
                                    {
                                        command.CommandText = sqlString_4;
                                        command.CommandType = CommandType.Text;
                                        command.Parameters.Add(new SqlParameter("@GioiTinhId", (!string.IsNullOrEmpty(updateSoGocSoHoaViewModel.GiaTri) && (updateSoGocSoHoaViewModel.GiaTri.ToUpper().Trim() == "NAM")) ? 1 : 2));
                                        command.Parameters.Add(new SqlParameter("@Id", bangViewModel.HocSinhId));
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    finally
                                    {
                                        command.Connection.Close();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception("Sai định dạng dữ liệu");
                            throw exception;
                        }

                    }

                    // update bang
                    Type businessEntityBangType = typeof(Bang);
                    Hashtable hashtableBang = new Hashtable();
                    foreach (PropertyInfo item in businessEntityBangType.GetProperties())
                    {
                        hashtableBang[item.Name.ToUpper()] = item;
                    }

                    var propertyInfoBang = (PropertyInfo)hashtableBang[truongDuLieuLoaiBang.TenTruongDuLieu.ToUpper()];
                    if (propertyInfoBang != null)
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyInfoHocSinh.PropertyType);
                        try
                        {
                            object propValue = typeConverter.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), updateSoGocSoHoaViewModel.GiaTri);
                            string sqlString_2 = @"Update Bang
                                                    Set {0} = @GiaTri
                                                    Where Id = @Id";
                            sqlString_2 = string.Format(sqlString_2, truongDuLieuLoaiBang.TenTruongDuLieu);
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_2;
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@GiaTri", propValue));
                                    command.Parameters.Add(new SqlParameter("@Id", bangViewModel.Id));
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Exception exception = new Exception("Sai định dạng dữ liệu");
                            throw exception;
                        }

                    }
                }

                string sqlString_3 = @"Update ThongTinVanBang
                                           Set GiaTri = @GiaTri
                                           Where (TruongDuLieuCode = @TruongDuLieuCode) and (BangId = @BangId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@GiaTri", updateSoGocSoHoaViewModel.GiaTri));
                        command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", updateSoGocSoHoaViewModel.TruongDuLieuCode));
                        command.Parameters.Add(new SqlParameter("@BangId", updateSoGocSoHoaViewModel.BangId));
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResultModel InsertBangBySoHoaAnh(InputDuLieuBySoHoa model)
        {
            try
            {
                var lstHocSinh = new List<HocSinhViewModel>();
                var lstBang = new List<BangViewModel>();
                foreach (var x in model.lstData)
                {
                    // Map dữ liệu vào HocSinhModel và bangViewModel
                    #region
                    Type businessEntityType = typeof(HocSinhViewModel);
                    Hashtable hashtable = new Hashtable();
                    PropertyInfo[] properties = businessEntityType.GetProperties();
                    foreach (PropertyInfo info in properties)
                    {
                        hashtable[info.Name.ToUpper()] = info;
                    }
                    Type businessEntityType_bang = typeof(BangViewModel);
                    Hashtable hashtable_bang = new Hashtable();
                    PropertyInfo[] properties_bang = businessEntityType_bang.GetProperties();
                    foreach (PropertyInfo info in properties_bang)
                    {
                        hashtable_bang[info.Name.ToUpper()] = info;
                    }
                    #endregion
                    
                    var hs = new HocSinhViewModel();
                    var bang = new BangViewModel();
                    foreach (var y in x)
                    {
                        var TenTruongDuLieu = GetTenTruongDuLieuByCode(y.Key, model.loaiBangId);
                        var info_HocSinh= (PropertyInfo)hashtable[TenTruongDuLieu.ToUpper()];
                        var info_Bang= (PropertyInfo)hashtable_bang[TenTruongDuLieu.ToUpper()];
                        TypeConverter typeConverter_hs = TypeDescriptor.GetConverter(info_HocSinh.PropertyType);
                        TypeConverter typeConverter_bang = TypeDescriptor.GetConverter(info_Bang.PropertyType);
                        object propValue_hs = typeConverter_hs.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), y.Value);
                        object propValue_bang = typeConverter_bang.ConvertFromString(null, CultureInfo.GetCultureInfo("vi-VN"), y.Value);
                        info_HocSinh.SetValue(hs, propValue_hs, null);
                        info_Bang.SetValue(bang, propValue_bang, null);
                    }
                    lstHocSinh.Add(hs);
                    lstBang.Add(bang);
                }
                var check= 0;
                return new ResultModel(true, "Thêm dữ liệu thành công");
            }
            catch (Exception e)
            {

                return new ResultModel(true, e.Message);
            }
        }
    }
}