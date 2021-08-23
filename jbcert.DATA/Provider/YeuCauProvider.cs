using jbcert.DATA.Helpers;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace jbcert.DATA.Provider
{
    public class YeuCauProvider : ApplicationDbContext
    {
        #region LOG
        public ResultModel InsertLog(LogYeuCau model)
        {
            try
            {
                //var obj = new LogYeuCau();
                //obj.NguoiDungId = model.NguoiDungId;
                //obj.HanhDong = model.HanhDong;
                //obj.YeuCauId = model.YeuCauId;
                //obj.ThoiGian = model.ThoiGian;
                //obj.HoTen = model.HoTen;
                DbContext.LogYeuCaus.Add(model);
                DbContext.SaveChanges();
                return new ResultModel(true, "Log success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }

        public bool IsTruyCapYeuCau(int yeuCauId, int currentDonViId, bool isNhan)
        {
            try
            {
                string sqlString = "";
                bool flag = false;
                if (isNhan)
                {
                    sqlString = @"Select DonViDichId From YeuCau Where Id = @YeuCauId";
                }
                else
                {
                    sqlString = @"Select DonViYeuCauId From YeuCau Where Id = @YeuCauId";
                }

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                flag = currentDonViId == Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return flag;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<LogYeuCauViewModel> GetLogByYeuCauId(int YeuCauId)
        {
            try
            {
                List<LogYeuCauViewModel> logYeuCauViewModels = new List<LogYeuCauViewModel>();
                string sqlString = "select * from LogYeuCau Where YeuCauId = @YeuCauId Order by ThoiGian DESC";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LogYeuCauViewModel logYeuCauViewModel = new LogYeuCauViewModel();
                                logYeuCauViewModel.LogId = Convert.ToInt32(reader["LogId"]);
                                logYeuCauViewModel.NguoiDungId = Guid.Parse(Convert.ToString(reader["NguoiDungId"]));
                                logYeuCauViewModel.HanhDong = Convert.ToString(reader["HanhDong"]);
                                logYeuCauViewModel.YeuCauId = Convert.ToInt32(reader["YeuCauId"]);
                                logYeuCauViewModel.ThoiGian = Convert.ToDateTime(reader["ThoiGian"]);
                                logYeuCauViewModel.HoTen = Convert.ToString(reader["HoTen"]);

                                logYeuCauViewModels.Add(logYeuCauViewModel);
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return logYeuCauViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<LogYeuCauViewModel> GetByYeuCauId(int YeuCauId)
        {
            try
            {
                List<LogYeuCauViewModel> logYeuCauViewModels = new List<LogYeuCauViewModel>();
                string sqlString = "select * from LogYeuCau where YeuCauId = @YeuCauId";
                DbContext.Database.GetDbConnection().Open();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LogYeuCauViewModel logYeuCauViewModel = new LogYeuCauViewModel();
                                logYeuCauViewModel.LogId = Convert.ToInt32(reader["LogId"]);
                                logYeuCauViewModel.NguoiDungId = Guid.Parse(Convert.ToString(reader["NguoiDungId"]));
                                logYeuCauViewModel.HanhDong = Convert.ToString(reader["HanhDong"]);
                                logYeuCauViewModel.YeuCauId = Convert.ToInt32(reader["YeuCauId"]);
                                logYeuCauViewModel.ThoiGian = Convert.ToDateTime(reader["ThoiGian"]);
                                logYeuCauViewModel.HoTen = Convert.ToString(reader["HoTen"]);

                                logYeuCauViewModels.Add(logYeuCauViewModel);
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return logYeuCauViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion
        #region Student
        public List<HocSinhViewModel> GetHocSinhByYeuCau(int YeuCauId)
        {
            try
            {
                List<HocSinhViewModel> hocSinhViewModels = new List<HocSinhViewModel>();
                var sqlString = @"Select lk.LienKetId, lk.YeuCauId, a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', 
		                                lk.TrangThaiCapPhatBang as 'TrangThaiBangId', f.Ten as 'TrangThaiBang', f.MaMauTrangThai as 'MaMauTrangThaiBang' From LienKetHocSinhYeuCau as lk 
                                inner join HocSinh as a 
                                on lk.HocSinhId = a.Id
                                Left Join [dbo].[DonVi] as b
                                on a.TruongHocId = b.DonViId
                                Left Join [dbo].[DanToc] as c
                                on a.DanTocId = c.Id
                                Left Join [dbo].[GioiTinh] as d
                                on a.GioiTinhId = d.Id 
                                Left Join TrangThaiBang as f
                                on lk.TrangThaiCapPhatBang = f.id
                                where lk.YeuCauId = @YeuCauId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhViewModels = MapDataHelper<HocSinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return hocSinhViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion
        #region File
        public ResultModel InsertFile(List<FileDinhKemYeuCauViewModel> model, int YeuCauId, string ip)
        {
            try
            {
                int count = model.Count();
                var lst = new List<FileDinhKemYeuCau>();
                foreach (var item in model)
                {
                    var obj = new FileDinhKemYeuCau();
                    obj.FileId = item.FileId;
                    obj.YeuCauId = YeuCauId;
                    obj.Url = item.Url;
                    obj.TenFile = item.TenFile;
                    obj.NguoiTao = item.NguoiTao;
                    obj.IconFile = item.IconFile;
                    obj.Ext = item.Ext;
                    obj.NgayTao = item.NgayTao;
                    obj.DonViId = item.DonViId;
                    lst.Add(obj);
                }
                DbContext.FileDinhKemYeuCaus.AddRange(lst);
                DbContext.SaveChanges();
                var objLog = new LogYeuCau();
                var user = new NguoiDungProvider().GetById(model.FirstOrDefault().NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm " + count + " file trong yêu cầu ";
                objLog.YeuCauId = YeuCauId;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                InsertLog(objLog);
                return new ResultModel(true, "Thêm file vào yêu cầu thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public List<FileDinhKemYeuCauViewModel> GetFileByYeuCauId(int YeuCauId)
        {
            try
            {
                List<FileDinhKemYeuCauViewModel> fileDinhKemYeuCauViewModels = new List<FileDinhKemYeuCauViewModel>();
                var sqlString = "select * from FileDinhKemYeuCau where YeuCauId = @YeuCauId";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            fileDinhKemYeuCauViewModels = MapDataHelper<FileDinhKemYeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return fileDinhKemYeuCauViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel DeleteFile(FileDinhKemYeuCauViewModel model, string ip)
        {
            try
            {
                var obj = DbContext.FileDinhKemYeuCaus.FirstOrDefault(f => f.FileId == model.FileId);
                DbContext.FileDinhKemYeuCaus.Remove(obj);
                DbContext.SaveChanges();
                var objLog = new LogYeuCau();

                var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiTao;
                objLog.HanhDong = "Đã xóa file: " + model.TenFile;
                objLog.YeuCauId = model.YeuCauId;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                InsertLog(objLog);
                return new ResultModel(true, "Xóa file thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        #endregion
        public YeuCau GetById(int YeuCauId)
        {
            try
            {
                return DbContext.YeuCaus.Include("MaTrangThaiYeuCauNavigation").FirstOrDefault(y => y.Id == YeuCauId);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel Insert(YeuCauViewModel model, string ip)
        {
            try
            {
                var obj = new YeuCau();
                obj.NoiDung = model.NoiDung;
                obj.NguoiTaoYeuCau = model.NguoiTaoYeuCau;
                obj.DonViYeuCauId = model.DonViYeuCauId;
                obj.LoaiYeuCauId = model.LoaiYeuCauId;
                obj.DonViDichId = model.DonViDichId;
                obj.NgayTao = DateTime.Now;
                obj.NguoiTao = model.NguoiTao;
                obj.IsDeleted = false;
                obj.DonViId = model.DonViId;
                obj.MaTrangThaiYeuCau = "init";
                obj.GhiChu = model.GhiChu;
                obj.MaYeuCau = Configuration.RandomString(10);
                obj.TenYeuCau = model.TenYeuCau;
                obj.LoaiVanBangId = model.LoaiVanBangId;
                DbContext.YeuCaus.Add(obj);
                DbContext.SaveChanges();
                var objLog = new LogYeuCau();
                var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm mới yêu cầu " + obj.MaYeuCau;
                objLog.YeuCauId = obj.Id;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                InsertLog(objLog);

                return new ResultModel(true, "Thêm mới yêu cầu thành công", obj.Id.ToString());
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }

        public int InsertYeuCauGuiThongTinHocSinhCongNhanTotNghiep(YeuCauViewModel model, int donViId, string ip)
        {
            try
            {

                var yeuCau = new YeuCau();
                yeuCau.NoiDung = model.NoiDung;
                yeuCau.NguoiTaoYeuCau = model.NguoiTaoYeuCau;
                yeuCau.DonViYeuCauId = model.DonViYeuCauId;
                yeuCau.LoaiYeuCauId = model.LoaiYeuCauId;
                yeuCau.DonViDichId = model.DonViDichId;
                yeuCau.NgayTao = DateTime.Now;
                yeuCau.NguoiTao = model.NguoiTao;
                yeuCau.NgayGuiYeuCau = model.NgayTao;
                yeuCau.IsDeleted = false;
                yeuCau.DonViId = donViId;
                yeuCau.MaTrangThaiYeuCau = "waiting";
                yeuCau.GhiChu = model.GhiChu;
                yeuCau.MaYeuCau = Configuration.RandomString(10);
                yeuCau.TenYeuCau = model.TenYeuCau;
                yeuCau.LoaiVanBangId = model.LoaiVanBangId;
                DbContext.YeuCaus.Add(yeuCau);
                DbContext.SaveChanges();
                var logYeuCau = new LogYeuCau();
                logYeuCau.NguoiDungId = model.NguoiTao;
                logYeuCau.HanhDong = "Đã thêm mới yêu cầu " + yeuCau.MaYeuCau;
                logYeuCau.YeuCauId = yeuCau.Id;
                logYeuCau.ThoiGian = DateTime.Now;
                logYeuCau.HoTen = model.NguoiTaoYeuCau;
                logYeuCau.Ip = ip;
                InsertLog(logYeuCau);
                DbContext.SaveChanges();

                // insert file
                int count = model.Files.Count();
                var fileDinhKemYeuCaus = new List<FileDinhKemYeuCau>();
                foreach (var item in model.Files)
                {
                    var fileDinhKemYeuCau = new FileDinhKemYeuCau();
                    fileDinhKemYeuCau.FileId = item.FileId;
                    fileDinhKemYeuCau.YeuCauId = yeuCau.Id;
                    fileDinhKemYeuCau.Url = item.Url;
                    fileDinhKemYeuCau.TenFile = item.TenFile;
                    fileDinhKemYeuCau.NguoiTao = item.NguoiTao;
                    fileDinhKemYeuCau.IconFile = item.IconFile;
                    fileDinhKemYeuCau.Ext = item.Ext;
                    fileDinhKemYeuCau.NgayTao = item.NgayTao;
                    fileDinhKemYeuCau.DonViId = item.DonViId;
                    fileDinhKemYeuCaus.Add(fileDinhKemYeuCau);
                }
                DbContext.FileDinhKemYeuCaus.AddRange(fileDinhKemYeuCaus);
                DbContext.SaveChanges();
                var logYeuCau_1 = new LogYeuCau();
                logYeuCau_1.NguoiDungId = model.NguoiTao;
                logYeuCau_1.HanhDong = "Đã thêm " + count + " file trong yêu cầu ";
                logYeuCau_1.YeuCauId = yeuCau.Id;
                logYeuCau_1.ThoiGian = DateTime.Now;
                logYeuCau_1.HoTen = model.NguoiTaoYeuCau;
                logYeuCau_1.Ip = ip;
                InsertLog(logYeuCau_1);
                DbContext.SaveChanges();

                // insert hocsinh
                var lienKetHocSinhYeuCaus = new List<LienKetHocSinhYeuCau>();
                foreach (var item in model.HocSinhs)
                {
                    var lienKetHocSinhYeuCau = new LienKetHocSinhYeuCau();
                    lienKetHocSinhYeuCau.YeuCauId = yeuCau.Id;
                    lienKetHocSinhYeuCau.HocSinhId = item.Id;
                    lienKetHocSinhYeuCau.TrangThaiCapPhatBang = 1;
                    lienKetHocSinhYeuCau.NgayTao = DateTime.Now;
                    lienKetHocSinhYeuCau.NguoiTao = model.NguoiTao;
                    lienKetHocSinhYeuCaus.Add(lienKetHocSinhYeuCau);
                }
                DbContext.LienKetHocSinhYeuCaus.AddRange(lienKetHocSinhYeuCaus);
                DbContext.SaveChanges();
                var logYeuCau_2 = new LogYeuCau();
                logYeuCau_2.NguoiDungId = model.NguoiTao;
                logYeuCau_2.HanhDong = "Đã thêm danh sách học sinh ";
                logYeuCau_2.YeuCauId = yeuCau.Id;
                logYeuCau_2.ThoiGian = DateTime.Now;
                logYeuCau_2.HoTen = model.NguoiTaoYeuCau;
                logYeuCau_2.Ip = ip;
                new YeuCauProvider().InsertLog(logYeuCau_2);

                return yeuCau.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int InsertYeuCauXetDuyet(YeuCauViewModel model, int donViId, string ip)
        {
            try
            {

                var yeuCau = new YeuCau();
                yeuCau.NoiDung = model.NoiDung;
                yeuCau.NguoiTaoYeuCau = model.NguoiTaoYeuCau;
                yeuCau.DonViYeuCauId = model.DonViYeuCauId;
                yeuCau.LoaiYeuCauId = model.LoaiYeuCauId;
                yeuCau.DonViDichId = model.DonViDichId;
                yeuCau.NgayTao = DateTime.Now;
                yeuCau.NguoiTao = model.NguoiTao;
                yeuCau.NgayGuiYeuCau = model.NgayTao;
                yeuCau.IsDeleted = false;
                yeuCau.DonViId = donViId;
                yeuCau.MaTrangThaiYeuCau = "waiting";
                yeuCau.GhiChu = model.GhiChu;
                yeuCau.MaYeuCau = Configuration.RandomString(10);
                yeuCau.TenYeuCau = model.TenYeuCau;
                yeuCau.LoaiVanBangId = model.LoaiVanBangId;
                DbContext.YeuCaus.Add(yeuCau);
                DbContext.SaveChanges();
                var logYeuCau = new LogYeuCau();
                logYeuCau.NguoiDungId = model.NguoiTao;
                logYeuCau.HanhDong = "Đã thêm mới yêu cầu " + yeuCau.MaYeuCau;
                logYeuCau.YeuCauId = yeuCau.Id;
                logYeuCau.ThoiGian = DateTime.Now;
                logYeuCau.HoTen = model.NguoiTaoYeuCau;
                logYeuCau.Ip = ip;
                InsertLog(logYeuCau);
                DbContext.SaveChanges();

                // insert file
                int count = model.Files.Count();
                var fileDinhKemYeuCaus = new List<FileDinhKemYeuCau>();
                foreach (var item in model.Files)
                {
                    var fileDinhKemYeuCau = new FileDinhKemYeuCau();
                    fileDinhKemYeuCau.FileId = item.FileId;
                    fileDinhKemYeuCau.YeuCauId = yeuCau.Id;
                    fileDinhKemYeuCau.Url = item.Url;
                    fileDinhKemYeuCau.TenFile = item.TenFile;
                    fileDinhKemYeuCau.NguoiTao = item.NguoiTao;
                    fileDinhKemYeuCau.IconFile = item.IconFile;
                    fileDinhKemYeuCau.Ext = item.Ext;
                    fileDinhKemYeuCau.NgayTao = item.NgayTao;
                    fileDinhKemYeuCau.DonViId = item.DonViId;
                    fileDinhKemYeuCaus.Add(fileDinhKemYeuCau);
                }
                DbContext.FileDinhKemYeuCaus.AddRange(fileDinhKemYeuCaus);
                DbContext.SaveChanges();
                var logYeuCau_1 = new LogYeuCau();
                logYeuCau_1.NguoiDungId = model.NguoiTao;
                logYeuCau_1.HanhDong = "Đã thêm " + count + " file trong yêu cầu ";
                logYeuCau_1.YeuCauId = yeuCau.Id;
                logYeuCau_1.ThoiGian = DateTime.Now;
                logYeuCau_1.HoTen = model.NguoiTaoYeuCau;
                logYeuCau_1.Ip = ip;
                InsertLog(logYeuCau_1);
                DbContext.SaveChanges();

                // insert hocsinh
                //var lienKetHocSinhYeuCaus = new List<LienKetHocSinhYeuCau>();
                //foreach (var item in model.HocSinhs)
                //{
                //    var lienKetHocSinhYeuCau = new LienKetHocSinhYeuCau();
                //    lienKetHocSinhYeuCau.YeuCauId = yeuCau.Id;
                //    lienKetHocSinhYeuCau.HocSinhId = item.Id;
                //    lienKetHocSinhYeuCau.TrangThaiCapPhatBang = 1;
                //    lienKetHocSinhYeuCau.NgayTao = DateTime.Now;
                //    lienKetHocSinhYeuCau.NguoiTao = model.NguoiTao;
                //    lienKetHocSinhYeuCaus.Add(lienKetHocSinhYeuCau);
                //}
                //DbContext.LienKetHocSinhYeuCaus.AddRange(lienKetHocSinhYeuCaus);
                //DbContext.SaveChanges();

                // add file hoc sinh excel
                List<FileHocSinhYeuCau> fileHocSinhYeuCaus = new List<FileHocSinhYeuCau>();
                if (model.FileHocSinhYeuCauDuXetTotNghieps != null && model.FileHocSinhYeuCauDuXetTotNghieps.Count > 0)
                {
                    foreach (var item in model.FileHocSinhYeuCauDuXetTotNghieps)
                    {
                        FileHocSinhYeuCau fileHocSinhYeuCau = new FileHocSinhYeuCau();
                        fileHocSinhYeuCau.YeuCauId = yeuCau.Id;
                        fileHocSinhYeuCau.Url = item.Url;
                        fileHocSinhYeuCau.TenFile = item.TenFile;
                        fileHocSinhYeuCau.NguoiTao = item.NguoiTao;
                        fileHocSinhYeuCau.IconFile = item.IconFile;
                        fileHocSinhYeuCau.Ext = item.Ext;
                        fileHocSinhYeuCau.NgayTao = item.NgayTao;
                        fileHocSinhYeuCau.DonViId = item.DonViId;
                        fileHocSinhYeuCaus.Add(fileHocSinhYeuCau);
                    }
                }
                DbContext.FileHocSinhYeuCaus.AddRange(fileHocSinhYeuCaus);
                DbContext.SaveChanges();

                var logYeuCau_2 = new LogYeuCau();
                logYeuCau_2.NguoiDungId = model.NguoiTao;
                logYeuCau_2.HanhDong = "Đã thêm file danh sách học sinh ";
                logYeuCau_2.YeuCauId = yeuCau.Id;
                logYeuCau_2.ThoiGian = DateTime.Now;
                logYeuCau_2.HoTen = model.NguoiTaoYeuCau;
                new YeuCauProvider().InsertLog(logYeuCau_2);

                return yeuCau.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ResultModel Update(YeuCauViewModel model, string ip)
        {
            try
            {
                var obj = GetById(model.Id);
                var flag = false;
                var mess = "";
                switch (obj.MaTrangThaiYeuCauNavigation.Code)
                {
                    case "waiting":
                        mess = "Yêu cầu đang chờ phê duyệt. Thu hồi yêu cầu để có thể cập nhật!";
                        break;
                    case "approved":
                        mess = "Bạn không thể cập nhật, yêu cầu đã được phê duyệt!";
                        break;
                    case "rejected":
                        mess = "Bạn không thể cập nhật, yêu cầu đã bị từ chối!";
                        break;
                    default:
                        flag = true;
                        break;
                }
                if (flag)
                {
                    obj.NoiDung = model.NoiDung;
                    //obj.LoaiYeuCauId = model.LoaiYeuCauId;
                    //obj.DonViDichId = model.DonViDichId;
                    obj.NgayCapNhat = model.NgayCapNhat;
                    obj.NguoiCapNhat = model.NguoiCapNhat;
                    //obj.IsDeleted = obj.IsDeleted;
                    //obj.MaTrangThaiYeuCau = model.MaTrangThaiYeuCau;
                    //obj.GhiChu = model.GhiChu;
                    obj.TenYeuCau = model.TenYeuCau;
                    obj.LoaiVanBangId = model.LoaiVanBangId;
                    DbContext.SaveChanges();
                    var objLog = new LogYeuCau();
                    var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã cập nhật thông tin yêu cầu";
                    objLog.YeuCauId = obj.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                    return new ResultModel(true, "Cập nhật yêu cầu thành công");
                }
                else
                {
                    return new ResultModel(flag, mess);
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ListReturnViewModel GetAllLoaiYeuCau()
        {
            var result = new ListReturnViewModel();
            try
            {
                var sqlString = "select * from LoaiYeuCau where IsDeleted = 0";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            var lst = MapDataHelper<LoaiYeuCauViewModel>.MapList(reader);
                            result.Data = lst;
                            result.Message = "Get data success!";
                            result.Status = true;
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public YeuCauViewModel GetYeuCauById(int YeuCauId)
        {
            try
            {
                YeuCauViewModel model = new YeuCauViewModel();
                var sqlString = @"select y.Id, y.NoiDung, y.NguoiTaoYeuCau, y.MaTrangThaiYeuCau, y.DonViYeuCauId, y.LoaiYeuCauId, 
                    y.DonViDichId, y.NgayTao, y.NguoiTao, y.NgayCapNhat, y.NguoiCapNhat, y.TenYeuCau, y.LoaiVanBangId,
                    d.TenDonVi, d1.TenDonVi as TenDonViYeuCau, d2.TenDonVi as TenDonViDich, lb.Ten as TenLoaiBang,
					t.TenTrangThai, t.MaMau, t.MauChu, t.Border, l.TenLoaiYeuCau, y.GhiChu, d2.DiaGioiHanhChinh as DiaGioiHanhChinhDonViDich
					from YeuCau as y inner join DonVi as d on y.DonViId = d.DonViId
                    inner join DonVi as d1 on y.DonViYeuCauId = d1.DonViId 
					inner join DonVi as d2 on y.DonViDichId = d2.DonViId
					inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code
					inner join LoaiYeuCau as l on y.LoaiYeuCauId = l.Id 
					left join LoaiBang as lb on y.LoaiVanBangId = lb.Id where y.Id = @YeuCauId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            model = MapDataHelper<YeuCauViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }

                }
             
                model.Logs = GetLogByYeuCauId(YeuCauId);
                model.Files = GetFileByYeuCauId(YeuCauId);
                model.HocSinhs = GetHocSinhByYeuCau(YeuCauId);
                model.FileHocSinhYeuCauDuXetTotNghieps = GetFileHocSinhYeuCauDuXetTotNghiep(YeuCauId);
                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<FileHocSinhYeuCauDuXetTotNghiepViewModel> GetFileHocSinhYeuCauDuXetTotNghiep(int YeuCauId)
        {
            List<FileHocSinhYeuCauDuXetTotNghiepViewModel> fileHocSinhYeuCauDuXetTotNghiepViewModels = new List<FileHocSinhYeuCauDuXetTotNghiepViewModel>();
            string sqlString = @"Select * From FileHocSinhYeuCaus
                                 Where YeuCauId = @YeuCauId";
            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
            {
                bool wasOpen = command.Connection.State == ConnectionState.Open;
                if (!wasOpen) command.Connection.Open();
                try
                {
                    command.CommandText = sqlString;
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@YeuCauId", YeuCauId));
                    using (var reader = command.ExecuteReader())
                    {
                        fileHocSinhYeuCauDuXetTotNghiepViewModels = MapDataHelper<FileHocSinhYeuCauDuXetTotNghiepViewModel>.MapList(reader);
                    }
                }
                finally
                {
                    if (!wasOpen) command.Connection.Close();
                }
            }

            return fileHocSinhYeuCauDuXetTotNghiepViewModels;
        }

        public ListReturnViewModel GetYeuCauDaGui(int DonViId, string MaTrangThai)
        {
            var result = new ListReturnViewModel();
            try
            {
                var sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViDich, t.MaMau, t.MauChu, t.Border 
                                from YeuCau as y inner join TrangThaiYeuCau as t 
                                on y.MaTrangThaiYeuCau = t.Code inner join DonVi as d on y.DonViDichId = d.DonViId 
                                where y.IsDeleted = 0 and y.DonViYeuCauId = " + DonViId;
                if (!string.IsNullOrEmpty(MaTrangThai))
                {
                    sqlString += " and y.MaTrangThaiYeuCau = '" + MaTrangThai + "'";
                }
                sqlString += " ORDER BY y.NgayTao DESC";

                List<YeuCauViewModel> lst;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }

                }

                if (lst == null) lst = new List<YeuCauViewModel>();

                foreach (YeuCauViewModel yeuCauViewModel in lst)
                {
                    string sqlString_1 = @"Select b.* From Bang as a
                                          Left Join TrangThaiBang as b
                                          on a.TrangThaiBangId = b.Id
                                          Where YeuCauId = @YeuCauId
                                          Group By b.Ten, b.Id, b.MaMauTrangThai";
                    TrangThaiBang trangThaiBang;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauViewModel.Id));
                            using (var reader = command.ExecuteReader())
                            {
                                trangThaiBang = MapDataHelper<TrangThaiBang>.Map(reader);
                            }
                        }
                        finally
                        {
                            if (!wasOpen) command.Connection.Close();
                        }
                    }


                    if (trangThaiBang == null)
                    {
                        trangThaiBang = DbContext.TrangThaiBangs.Find(1);
                    }
                    else
                    {
                        trangThaiBang = DbContext.TrangThaiBangs.Find(2);
                    }
                    yeuCauViewModel.TrangThaiBangId = trangThaiBang.Id;
                    yeuCauViewModel.TrangThaiBang = trangThaiBang.Ten;
                    yeuCauViewModel.MaMauTrangThai = trangThaiBang.MaMauTrangThai;
                }

                result.Data = lst;
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public ListReturnViewModel GetYeuCauPheDuyet(DonVi donVi, string MaTrangThai)
        {
            var result = new ListReturnViewModel();
            try
            {
                var lst = new List<YeuCauViewModel>();
                switch (donVi.CapDonVi.Code)
                {
                    case "PHONGGD":
                        if (MaTrangThai == "waiting")
                        {
                            string sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViYeuCauId = d.DonViId  
                                where y.IsDeleted = 0 and y.MaTrangThaiYeuCau = 'waiting' and y.DonViDichId = " + donVi.DonViId;

                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                        }
                        if (MaTrangThai == "approved")
                        {
                            string sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViYeuCauId = d.DonViId  
                                where y.IsDeleted = 0 and (y.MaTrangThaiYeuCau = 'approved' 
                                or y.MaTrangThaiYeuCau = 'forward') and y.DonViChuyenTiepId = " + donVi.DonViId;
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }
                        }
                        foreach (YeuCauViewModel yeuCauViewModel in lst)
                        {
                            string sqlString = @"Select b.* From Bang as a
                                          Left Join TrangThaiBang as b
                                          on a.TrangThaiBangId = b.Id
                                          Where YeuCauId = @YeuCauId
                                          Group By b.Ten, b.Id, b.MaMauTrangThai";
                            TrangThaiBang trangThaiBang = new TrangThaiBang();
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauViewModel.Id));
                                    using (var reader = command.ExecuteReader())
                                    {
                                        trangThaiBang = MapDataHelper<TrangThaiBang>.Map(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                            if (trangThaiBang == null)
                            {
                                trangThaiBang = DbContext.TrangThaiBangs.Find(1);
                            }
                            else
                            {
                                trangThaiBang = DbContext.TrangThaiBangs.Find(2);
                            }
                            yeuCauViewModel.TrangThaiBangId = trangThaiBang.Id;
                            yeuCauViewModel.TrangThaiBang = trangThaiBang.Ten;
                            yeuCauViewModel.MaMauTrangThai = trangThaiBang.MaMauTrangThai;
                        }
                        break;
                    case "SOGD":
                        if (MaTrangThai == "waiting")
                        {
                            List<YeuCauViewModel> lst1 = new List<YeuCauViewModel>();
                            List<YeuCauViewModel> lst2 = new List<YeuCauViewModel>();
                            string sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViYeuCauId = d.DonViId  
                                where y.IsDeleted = 0 and y.MaTrangThaiYeuCau = 'waiting' and y.DonViDichId = " + donVi.DonViId;
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst1 = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                            string sqlString_1 = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViChuyenTiepId = d.DonViId  
                                where y.IsDeleted = 0 and y.MaTrangThaiYeuCau = 'forward' and y.DonViDichId = " + donVi.DonViId;

                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString_1;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst1 = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                            lst = lst1.Concat(lst2).ToList();
                        }
                        if (MaTrangThai == "approved")
                        {
                            List<YeuCauViewModel> lst1 = new List<YeuCauViewModel>();
                            List<YeuCauViewModel> lst2 = new List<YeuCauViewModel>();
                            string sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViYeuCauId = d.DonViId  
                                where y.IsDeleted = 0 and y.DonViChuyenTiepId is null and y.MaTrangThaiYeuCau = 'approved' and y.DonViDichId = " + donVi.DonViId;

                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst1 = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                            string sqlString_1 = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViChuyenTiepId = d.DonViId  
                                where y.IsDeleted = 0 and y.DonViChuyenTiepId is not null and y.MaTrangThaiYeuCau = 'approved' and y.DonViDichId = " + donVi.DonViId;
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString_1;
                                    command.CommandType = System.Data.CommandType.Text;
                                    using (var reader = command.ExecuteReader())
                                    {
                                        lst1 = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }

                            lst = lst1.Concat(lst2).ToList();
                        }
                        foreach (YeuCauViewModel yeuCauViewModel in lst)
                        {
                            string sqlString = @"Select b.* From Bang as a
                                          Left Join TrangThaiBang as b
                                          on a.TrangThaiBangId = b.Id
                                          Where YeuCauId = @YeuCauId
                                          Group By b.Ten, b.Id, b.MaMauTrangThai";
                            TrangThaiBang trangThaiBang = new TrangThaiBang();
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();
                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauViewModel.Id));
                                    using (var reader = command.ExecuteReader())
                                    {
                                        trangThaiBang = MapDataHelper<TrangThaiBang>.Map(reader);
                                    }
                                }
                                finally
                                {
                                    if (!wasOpen) command.Connection.Close();
                                }
                            }
                            if (trangThaiBang == null)
                            {
                                trangThaiBang = DbContext.TrangThaiBangs.Find(1);
                            }

                            yeuCauViewModel.TrangThaiBangId = trangThaiBang.Id;
                            yeuCauViewModel.TrangThaiBang = trangThaiBang.Ten;
                            yeuCauViewModel.MaMauTrangThai = trangThaiBang.MaMauTrangThai;
                        }
                        break;
                    default:
                        string sqlString_2 = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                        t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                        inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                        inner join DonVi as d on y.DonViYeuCauId = d.DonViId  
                                        where y.IsDeleted = 0 and y.DonViDichId = " + donVi.DonViId;
                        if (!string.IsNullOrEmpty(MaTrangThai))
                        {
                            sqlString_2 += " and y.MaTrangThaiYeuCau = '" + MaTrangThai + "'";
                        }
                        else
                        {
                            sqlString_2 += " and(y.MaTrangThaiYeuCau = 'waiting' or y.MaTrangThaiYeuCau = 'approved')";
                        }
                        sqlString_2 += " ORDER BY y.NgayGuiYeuCau DESC";

                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();
                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = System.Data.CommandType.Text;
                                using (var reader = command.ExecuteReader())
                                {
                                    lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                if (!wasOpen) command.Connection.Close();
                            }
                        }
                        break;
                }
                result.Data = lst.OrderByDescending(x => x.NgayGuiYeuCau).ToList();
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public ListReturnViewModel GetYeuCauDaPheDuyetVaDaTaoVanBang(int DonViId, string MaTrangThai)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = @"select y.Id, y.MaYeuCau, y.DonViYeuCauId, y.MaTrangThaiYeuCau, y.TenYeuCau, y.NgayTao, 
                                t.TenTrangThai, y.NgayGuiYeuCau, d.TenDonVi as TenDonViYeuCau, t.MaMau, t.MauChu, t.Border from YeuCau as y 
                                inner join TrangThaiYeuCau as t on y.MaTrangThaiYeuCau = t.Code 
                                inner join DonVi as d on y.DonViYeuCauId = d.DonViId 
                                where y.IsDeleted = 0 and y.DonViDichId = " + DonViId;
                if (!string.IsNullOrEmpty(MaTrangThai))
                {
                    sqlString += " and y.MaTrangThaiYeuCau = '" + MaTrangThai + "'";
                }
                sqlString += " ORDER BY y.NgayTao DESC";
                List<YeuCauViewModel> lst = new List<YeuCauViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }


                foreach (YeuCauViewModel yeuCauViewModel in lst)
                {
                    string sqlString_1 = @"Select b.* From Bang as a
                                          Left Join TrangThaiBang as b
                                          on a.TrangThaiBangId = b.Id
                                          Where YeuCauId = @YeuCauId
                                          Group By b.Ten, b.Id, b.MaMauTrangThai";
                    TrangThaiBang trangThaiBang = new TrangThaiBang();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauViewModel.Id));
                            using (var reader = command.ExecuteReader())
                            {
                                lst = MapDataHelper<YeuCauViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            if (!wasOpen) command.Connection.Close();
                        }
                    }
                    if (trangThaiBang == null)
                    {
                        trangThaiBang = DbContext.TrangThaiBangs.Find(1);
                    }

                    yeuCauViewModel.TrangThaiBangId = trangThaiBang.Id;
                    yeuCauViewModel.TrangThaiBang = trangThaiBang.Ten;
                    yeuCauViewModel.MaMauTrangThai = trangThaiBang.MaMauTrangThai;
                }

                result.Data = lst.Where(x => x.TrangThaiBangId >= 2).OrderByDescending(x => x.NgayGuiYeuCau).ToList();
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public List<LoaiYeuCauViewModel> GetLoaiYeuCauByCapDonViId(int CapDonViId)
        {
            try
            {
                List<LoaiYeuCauViewModel> loaiYeuCauViewModels = new List<LoaiYeuCauViewModel>();
                string sqlString = "select * from LienKetLoaiYeuCauCapDonVi as lk inner join LoaiYeuCau as l on lk.LoaiYeuCauId = l.Id where CapDonViId =" + CapDonViId;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            loaiYeuCauViewModels = MapDataHelper<LoaiYeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return loaiYeuCauViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #region Trang thai
        public void SendRequest(YeuCauViewModel model, Guid NguoiDungId, string ip)
        {
            try
            {
                var obj = GetById(model.Id);
                var flag = false;
                var mess = "";
                switch (obj.MaTrangThaiYeuCauNavigation.Code)
                {
                    case "waiting":
                        mess = "Yêu cầu đã được gửi đi rồi!";
                        break;
                    case "approved":
                        mess = "Bạn không thể gửi, yêu cầu đã được phê duyệt!";
                        break;
                    case "rejected":
                        mess = "Bạn không thể gửi, yêu cầu đã bị từ chối!";
                        break;
                    default:
                        flag = true;
                        break;
                }
                if (flag)
                {
                    string sqlString = @"Select Count(*) as 'TotalRow' from LienKetHocSinhYeuCau as a
                                        Left Join YeuCau as b 
                                        on a.YeuCauId = b.Id
                                        where (YeuCauId = @YeuCauId) and (b.DonViId = @DonViId) and a.HocSinhId in (Select a.HocSinhId from LienKetHocSinhYeuCau as a
																                                        Left Join YeuCau as b 
																                                        on a.YeuCauId = b.Id
																                                        Where ((b.MaTrangThaiYeuCau like 'approved') or (b.MaTrangThaiYeuCau like 'waiting'))
																                                         and (b.DonViYeuCauId = @DonViId) and (b.Id != @YeuCauId))";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
                            command.Parameters.Add(new SqlParameter("@YeuCauId", model.Id));
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
                            if (!wasOpen) command.Connection.Close();
                        }
                    }

                    if(totalRow > 0)
                    {
                        Exception exception = new Exception("Tồn tại học sinh nằm trong yêu cầu khác đã được gửi");
                        throw exception;
                    }

                    obj.MaTrangThaiYeuCau = "waiting";
                    obj.NgayGuiYeuCau = DateTime.Now;
                    DbContext.SaveChanges();
                    var objLog = new LogYeuCau();
                    var user = new NguoiDungProvider().GetById(NguoiDungId);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã gửi yêu cầu";
                    objLog.YeuCauId = obj.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.Ip = ip;
                    objLog.HoTen = user.HoTen;
                    InsertLog(objLog);
                }
                else
                {
                    Exception exception = new Exception(mess);
                    throw exception;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ResultModel ForwardRequest(YeuCauViewModel model, Guid NguoiDungId, string ip)
        {
            try
            {
                var obj = GetById(model.Id);
                var flag = false;
                var mess = "";
                switch (obj.MaTrangThaiYeuCauNavigation.Code)
                {
                    case "forward":
                        mess = "Bạn không thể chuyển tiếp, yêu cầu đã được chuyển đi rồi!";
                        break;
                    case "approved":
                        mess = "Bạn không thể chuyển tiếp, yêu cầu đã được phê duyệt!";
                        break;
                    case "rejected":
                        mess = "Bạn không thể chuyển tiếp, yêu cầu đã bị từ chối!";
                        break;
                    default:
                        flag = true;
                        break;
                }
                if (flag)
                {
                    obj.MaTrangThaiYeuCau = "forward";
                    obj.DonViChuyenTiepId = model.DonViChuyenTiepId;
                    obj.DonViDichId = model.DonViDichId;
                    DbContext.SaveChanges();
                    var objLog = new LogYeuCau();
                    var user = new NguoiDungProvider().GetById(NguoiDungId);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã 'Chuyển tiếp' yêu cầu";
                    objLog.YeuCauId = obj.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                    return new ResultModel(true, "Cập nhật trạng thái yêu cầu thành công");
                }
                else
                {
                    return new ResultModel(flag, mess);
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public void RetrievalOrAbort(YeuCauViewModel model, Guid NguoiDungId, string ip)
        {
            try
            {
                var obj = GetById(model.Id);
                var flag = false;
                var mess = "";
                switch (obj.MaTrangThaiYeuCauNavigation.Code)
                {
                    case "approved":
                        mess = "Bạn không thể cập nhật, yêu cầu đã được phê duyệt!";
                        break;
                    default:
                        flag = true;
                        break;
                }
                if (flag)
                {
                    obj.MaTrangThaiYeuCau = model.MaTrangThaiYeuCau;
                    DbContext.SaveChanges();
                    var objLog = new LogYeuCau();
                    var user = new NguoiDungProvider().GetById(NguoiDungId);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã chuyển trạng thái '" + GetById(model.Id).MaTrangThaiYeuCauNavigation.TenTrangThai + "'";
                    objLog.YeuCauId = obj.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                }
                else
                {
                    Exception exception = new Exception(mess);
                    throw exception;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void ApproveOrReject(YeuCauViewModel model, Guid NguoiDungId, string ip)
        {
            try
            {
                var obj = GetById(model.Id);
                if (obj.LoaiYeuCauId == 6 && model.MaTrangThaiYeuCau == "approved")
                {
                    string sqlString = @"Select Count(*) as 'TotalRow' From TruongDuLieuLoaiBang 
                                        Where (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", obj.LoaiVanBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
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
                            if (!wasOpen) command.Connection.Close();
                        }
                    }
                    if (totalRow == 0)
                    {
                        Exception exception = new Exception("Chưa tạo trường dữ liệu trong bằng");
                        throw exception;
                    }
                }
                var flag = false;
                var mess = "";
                switch (obj.MaTrangThaiYeuCauNavigation.Code)
                {
                    case "abort":
                        mess = "Bạn không thể cập nhật, yêu cầu đã bị hủy";
                        break;
                    case "retrieval":
                        mess = "Bạn không thể cập nhật, yêu cầu đã được thu hồi";
                        break;
                    default:
                        flag = true;
                        break;
                }
                if (flag)
                {
                    obj.MaTrangThaiYeuCau = model.MaTrangThaiYeuCau;

                    DbContext.SaveChanges();

                    var objLog = new LogYeuCau();
                    var user = new NguoiDungProvider().GetById(NguoiDungId);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã chuyển trạng thái'" + GetById(model.Id).MaTrangThaiYeuCauNavigation.TenTrangThai + "'";
                    objLog.YeuCauId = obj.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                }
                else
                {
                    Exception exception = new Exception(mess);
                    throw exception;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<TrangThaiYeuCauViewModel> GetAllTrangThaiYeuCau()
        {
            try
            {
                List<TrangThaiYeuCauViewModel> trangThaiYeuCauViewModels = new List<TrangThaiYeuCauViewModel>();
                string sqlString = "select * from TrangThaiYeuCau";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            trangThaiYeuCauViewModels = MapDataHelper<TrangThaiYeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return trangThaiYeuCauViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public KetQuaKiemTraPheDuyetViewModel KiemTraPheDuyet(int yeuCauId, int donViId)
        {
            try
            {
                string sqlString_3 = @"Select * From YeuCau Where ([Id] = @YeuCauId) and ([DonViDichId] = @DonViId)";
                YeuCau yeuCau = new YeuCau();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            yeuCau = MapDataHelper<YeuCau>.Map(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                if (yeuCau == null)
                {
                    return null;
                }
                int loaiBangId = yeuCau.LoaiVanBangId.Value;
                string sqlString_4 = @"Select * From [LienKetHocSinhYeuCau] Where YeuCauId = @YeuCauId";

                List<LienKetHocSinhYeuCauViewModel> lienKetHocSinhYeuCauViewModels = new List<LienKetHocSinhYeuCauViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            lienKetHocSinhYeuCauViewModels = MapDataHelper<LienKetHocSinhYeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                KetQuaKiemTraPheDuyetViewModel ketQuaKiemTraPheDuyetViewModel = new KetQuaKiemTraPheDuyetViewModel();
                ketQuaKiemTraPheDuyetViewModel.TuChoi = 0;
                ketQuaKiemTraPheDuyetViewModel.PheDuyet = 0;
                // get 

                string sqlString_2 = @"Select * From TruongDuLieuLoaiBang
                                        Where LoaiBangId = @LoaiBangId and DungChung = 0";

                List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            truongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }


                foreach (LienKetHocSinhYeuCauViewModel ketHocSinhYeuCauViewModel in lienKetHocSinhYeuCauViewModels)
                {
                    string sqlString = @"Select * From [HocSinh] Where (Id = @Id)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    HocSinh hocSinh = new HocSinh();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@Id", ketHocSinhYeuCauViewModel.HocSinhId));
                            using (var reader = command.ExecuteReader())
                            {
                                hocSinh = MapDataHelper<HocSinh>.Map(reader);
                            }
                        }
                        finally
                        {
                            if (!wasOpen) command.Connection.Close();
                        }
                    }

                    if (hocSinh != null)
                    {
                        string sqlString_1 = @"Select * From [LienKetHocSinhYeuCau] Where (YeuCauId = @YeuCauId) and (HocSinhId = @HocSinhId)";
                        LienKetHocSinhYeuCau lienKetHocSinhYeuCau = new LienKetHocSinhYeuCau();
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();
                            try
                            {
                                command.CommandText = sqlString_1;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@YeuCauId", ketHocSinhYeuCauViewModel.YeuCauId));
                                command.Parameters.Add(new SqlParameter("@HocSinhId", ketHocSinhYeuCauViewModel.HocSinhId));
                                using (var reader = command.ExecuteReader())
                                {
                                    lienKetHocSinhYeuCau = MapDataHelper<LienKetHocSinhYeuCau>.Map(reader);
                                }
                            }
                            finally
                            {
                                if (!wasOpen) command.Connection.Close();
                            }
                        }


                        bool isPassed = true;
                        Type myType = hocSinh.GetType();
                        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                        foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBang in truongDuLieuTrongBangs)
                        {
                            PropertyInfo propertyInfo = props.Where(x => x.Name == truongDuLieuTrongBang.TenTruongDuLieu).FirstOrDefault();
                            if (string.IsNullOrEmpty(Convert.ToString(propertyInfo.GetValue(hocSinh, null))))
                            {
                                isPassed = false;
                                break;
                            }
                        }
                        if (isPassed)
                        {
                            ketQuaKiemTraPheDuyetViewModel.PheDuyet += 1;
                        }
                        else
                        {
                            ketQuaKiemTraPheDuyetViewModel.TuChoi += 1;
                        }
                        lienKetHocSinhYeuCau.TrangThaiKiemTraTaoHoSo = isPassed;
                        DbContext.SaveChanges();
                    }

                }

                return ketQuaKiemTraPheDuyetViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region yeu cau phe duyet
        public ThongTinDonViGuiYeuCauXetDuyetViewModel ThongTinDonViGuiYeuCauXetDuyetViewModel(int donViGuiId)
        {
            try
            {
                ThongTinDonViGuiYeuCauXetDuyetViewModel thongTinDonViGuiYeuCauXetDuyetViewModel = new ThongTinDonViGuiYeuCauXetDuyetViewModel();
                thongTinDonViGuiYeuCauXetDuyetViewModel.LoaiBangs = new List<LoaiBangViewModel>();
                string sqlString = @"Select a.DonViId as 'DonViGuiId', a.TenDonVi as 'DonViGui',b.DonViId as 'DonViNhanId', b.TenDonVi as 'DonViNhan', d.Id as 'LoaiBangId', d.Ten as 'TenLoaiBang',  a.CapDonViId
                                    From DonVi as a
                                    Left Join DonVi as b
                                    on a.KhoaChaId = b.DonViId
									Left Join CapDonVi as c
									on a.CapDonViId = c.CapDonViId
                                    Left Join LoaiBang as d
                                    on c.Code = d.CodeCapDonVi
									Where (a.DonViId = @DonViGuiId)";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@donViGuiId", donViGuiId));
                        using (var reader = command.ExecuteReader())
                        {
                            var result = MapDataHelper<RowThongTinDonViGuiYeuCaXetDuyetViewModel>.MapList(reader);
                            foreach (var item in result)
                            {
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViGuiId = item.DonViGuiId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViGui = item.DonViGui;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViNhanId = item.DonViNhanId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViNhan = item.DonViNhan;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.CapDonViId = item.CapDonViId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.TenYeuCau = "Phê duyệt DS dự xét TN " + item.DonViGui;
                                if ((item.LoaiBangId != null) && (item.TenLoaiBang != null))
                                {
                                    thongTinDonViGuiYeuCauXetDuyetViewModel.LoaiBangs.Add(new LoaiBangViewModel()
                                    {
                                        Id = item.LoaiBangId.Value,
                                        Ten = item.TenLoaiBang
                                    });
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return thongTinDonViGuiYeuCauXetDuyetViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ThongTinDonViGuiYeuCauXetDuyetViewModel ThongTinDonViGuiYeuCauPheDuyetTotNghiep(int donViGuiId)
        {
            try
            {
                ThongTinDonViGuiYeuCauXetDuyetViewModel thongTinDonViGuiYeuCauXetDuyetViewModel = new ThongTinDonViGuiYeuCauXetDuyetViewModel();
                thongTinDonViGuiYeuCauXetDuyetViewModel.LoaiBangs = new List<LoaiBangViewModel>();
                string sqlString = @"Select a.DonViId as 'DonViGuiId', a.TenDonVi as 'DonViGui',b.DonViId as 'DonViNhanId', b.TenDonVi as 'DonViNhan', d.Id as 'LoaiBangId', d.Ten as 'TenLoaiBang',  a.CapDonViId
                                    From DonVi as a
                                    Left Join DonVi as b
                                    on a.KhoaChaId = b.DonViId
									Left Join CapDonVi as c
									on a.CapDonViId = c.CapDonViId
                                    Left Join LoaiBang as d
                                    on c.Code = d.CodeCapDonVi
									Where (a.DonViId = @DonViGuiId) and (d.HinhThucCapId = 1)";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@donViGuiId", donViGuiId));
                        using (var reader = command.ExecuteReader())
                        {
                            var result = MapDataHelper<RowThongTinDonViGuiYeuCaXetDuyetViewModel>.MapList(reader);
                            foreach (var item in result)
                            {
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViGuiId = item.DonViGuiId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViGui = item.DonViGui;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViNhanId = item.DonViNhanId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.DonViNhan = item.DonViNhan;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.CapDonViId = item.CapDonViId;
                                thongTinDonViGuiYeuCauXetDuyetViewModel.TenYeuCau = "Phê duyệt DS dự xét TN " + item.DonViGui;
                                if ((item.LoaiBangId != null) && (item.TenLoaiBang != null))
                                {
                                    thongTinDonViGuiYeuCauXetDuyetViewModel.LoaiBangs.Add(new LoaiBangViewModel()
                                    {
                                        Id = item.LoaiBangId.Value,
                                        Ten = item.TenLoaiBang
                                    });
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return thongTinDonViGuiYeuCauXetDuyetViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YeuCauWithPaginationViewModel GetYeuCauXetDuyets(int? truongHocId, int? nam, string maTrangThai, int currentPage, int? donViId)
        {
            try
            {
                YeuCauWithPaginationViewModel yeuCauWithPaginationViewModel = new YeuCauWithPaginationViewModel();
                string sqlString = @"Select a.Id, a.MaYeuCau, a.DonViYeuCauId, a.MaTrangThaiYeuCau, a.TenYeuCau, a.NgayTao, a.NgayGuiYeuCau, 
		                            b.TenTrangThai, b.MaMau, b.MauChu, b.Border,
		                            c.TenDonVi as 'TenDonViYeuCau', a.DonViDichId, d.TenDonVi as 'TenDonViDich'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Left Join DonVi as d
                                    on a.DonViDichId = d.DonViId
                                    Where (IsDeleted = 0) and (LoaiYeuCauId = 6) and ((@DonViDichId is null) or ((DonViDichId = @DonViDichId) ))
                                            and ((@MaTrangThai is null) or(a.MaTrangThaiYeuCau like @MaTrangThai))
                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(NgayTao) = @Nam))
                                    Order by NgayTao Desc
                                    Offset @Offset ROws Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            yeuCauWithPaginationViewModel.YeuCaus = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                int totalRow = 0;
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Where (IsDeleted = 0) and (LoaiYeuCauId = 6) and ((@DonViDichId is null) or ((DonViDichId = @DonViDichId) ))
                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(NgayTao) = @Nam))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));

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
                }

                yeuCauWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                yeuCauWithPaginationViewModel.CurrentPage = currentPage;
                return yeuCauWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YeuCauWithPaginationViewModel GetYeuCauXinCapPhoisCuaConGui(int? nam, string maTrangThai, int? loaiYeuCau, int? loaiBangId, int currentPage, int? donViId)
        {
            try
            {
                YeuCauWithPaginationViewModel yeuCauWithPaginationViewModel = new YeuCauWithPaginationViewModel();
                string sqlString = @"Select a.Id, a.MaYeuCau, a.DonViYeuCauId, a.MaTrangThaiYeuCau, a.TenYeuCau, a.NgayTao, a.NgayGuiYeuCau, 
		                                            b.TenTrangThai, b.MaMau, b.MauChu, b.Border,
		                                            c.TenDonVi as 'TenDonViYeuCau', a.DonViDichId, d.TenDonVi as 'TenDonViDich',
									                a.LoaiVanBangId, e.Ten as 'TenLoaiBang', f.TenLoaiYeuCau, a.LoaiYeuCauId
                                                    From YeuCau as a
                                                    Left Join TrangThaiYeuCau as b
                                                    on a.MaTrangThaiYeuCau = b.Code
                                                    Left Join DonVi as c
                                                    on a.DonViYeuCauId = c.DonViId
                                                    Left Join DonVi as d
                                                    on a.DonViDichId = d.DonViId
									                Left Join LoaiBang as e
									                on a.LoaiVanBangId = e.Id
                                                    Left Join LoaiYeuCau as f
									                on a.LoaiYeuCauId = f.Id
                                                    Where (a.IsDeleted = 0) and ( ((@LoaiYeuCauId is null) and ((a.LoaiYeuCauId = 2) or (a.LoaiYeuCauId = 7)) ) or (a.LoaiYeuCauId = @LoaiYeuCauId) ) 
                                                            and ((@MaTrangThai is null) or(a.MaTrangThaiYeuCau like @MaTrangThai)) and ((@Nam is null) or (YEAR(a.NgayTao) = @Nam))
                                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) and (DonViYeuCauId = @DonViId)
                                                    Order by NgayTao Desc
                                                    Offset @Offset ROws Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiYeuCauId", loaiYeuCau.HasValue ? loaiYeuCau.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            yeuCauWithPaginationViewModel.YeuCaus = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                int totalRow = 0;
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Where (a.IsDeleted = 0) and ( ((@LoaiYeuCauId is null) and ((a.LoaiYeuCauId = 2) or (a.LoaiYeuCauId = 7)) ) or (a.LoaiYeuCauId = @LoaiYeuCauId) ) 
                                                            and ((@MaTrangThai is null) or(a.MaTrangThaiYeuCau like @MaTrangThai)) and ((@Nam is null) or (YEAR(a.NgayTao) = @Nam))
                                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) and (DonViYeuCauId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiYeuCauId", loaiYeuCau.HasValue ? loaiYeuCau.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));

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
                }

                yeuCauWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                yeuCauWithPaginationViewModel.CurrentPage = currentPage;
                return yeuCauWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YeuCauWithPaginationViewModel GetYeuCauXinCapPhoisCuaChaNhan(int? donViGuiId, int? nam, string maTrangThai, int? loaiYeuCau, int? loaiBangId, int currentPage, int? donViId)
        {
            try
            {
                YeuCauWithPaginationViewModel yeuCauWithPaginationViewModel = new YeuCauWithPaginationViewModel();
                string sqlString = @"Select a.Id, a.MaYeuCau, a.DonViYeuCauId, a.MaTrangThaiYeuCau, a.TenYeuCau, a.NgayTao, a.NgayGuiYeuCau, 
		                                            b.TenTrangThai, b.MaMau, b.MauChu, b.Border,
		                                            c.TenDonVi as 'TenDonViYeuCau', a.DonViDichId, d.TenDonVi as 'TenDonViDich',
									                a.LoaiVanBangId, e.Ten as 'TenLoaiBang', f.TenLoaiYeuCau, a.LoaiYeuCauId
                                                    From YeuCau as a
                                                    Left Join TrangThaiYeuCau as b
                                                    on a.MaTrangThaiYeuCau = b.Code
                                                    Left Join DonVi as c
                                                    on a.DonViYeuCauId = c.DonViId
                                                    Left Join DonVi as d
                                                    on a.DonViDichId = d.DonViId
									                Left Join LoaiBang as e
									                on a.LoaiVanBangId = e.Id
                                                    Left Join LoaiYeuCau as f
									                on a.LoaiYeuCauId = f.Id
                                                    Where (a.IsDeleted = 0) and ( ((@LoaiYeuCauId is null) and ((a.LoaiYeuCauId = 2) or (a.LoaiYeuCauId = 7)) ) or (a.LoaiYeuCauId = @LoaiYeuCauId) ) 
                                                            and ( ((@MaTrangThai is null) and (a.MaTrangThaiYeuCau in ('waiting','approved','rejected'))) or(a.MaTrangThaiYeuCau like @MaTrangThai)) and ((@Nam is null) or (YEAR(a.NgayTao) = @Nam))
                                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) 
                                                            and (DonViDichId = @DonViId) and ((@DonViGuiId is null) or (DonViYeuCauId = @DonViGuiId))
                                                    Order by NgayTao Desc
                                                    Offset @Offset ROws Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiYeuCauId", loaiYeuCau.HasValue ? loaiYeuCau.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.Value));
                        command.Parameters.Add(new SqlParameter("@DonViGuiId", donViGuiId.HasValue ? donViGuiId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            yeuCauWithPaginationViewModel.YeuCaus = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                int totalRow = 0;
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Where (a.IsDeleted = 0) and ( ((@LoaiYeuCauId is null) and ((a.LoaiYeuCauId = 2) or (a.LoaiYeuCauId = 7)) ) or (a.LoaiYeuCauId = @LoaiYeuCauId) ) 
                                                            and (((@MaTrangThai is null) and (a.MaTrangThaiYeuCau in ('waiting','approved','rejected'))) or(a.MaTrangThaiYeuCau like @MaTrangThai)) and ((@Nam is null) or (YEAR(a.NgayTao) = @Nam))
                                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) 
                                                            and (DonViDichId = @DonViId) and ((@DonViGuiId is null) or (DonViYeuCauId = @DonViGuiId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiYeuCauId", loaiYeuCau.HasValue ? loaiYeuCau.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.Value));
                        command.Parameters.Add(new SqlParameter("@DonViGuiId", donViGuiId.HasValue ? donViGuiId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));

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
                }

                yeuCauWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                yeuCauWithPaginationViewModel.CurrentPage = currentPage;
                return yeuCauWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YeuCauWithPaginationViewModel GetYeuCauXinCapPhoiBanSaos(int? phongGiaoDucId, int? nam, string maTrangThai, int? loaiBangId, int currentPage, int? donViId)
        {
            try
            {
                string expand = @"";
                if (donViId.HasValue && string.IsNullOrEmpty(maTrangThai))
                {
                    expand = " and a.MaTrangThaiYeuCau In ('approved', 'rejected', 'waiting')";
                }

                YeuCauWithPaginationViewModel yeuCauWithPaginationViewModel = new YeuCauWithPaginationViewModel();
                string sqlString = string.Format(@"Select a.Id, a.MaYeuCau, a.DonViYeuCauId, a.MaTrangThaiYeuCau, a.TenYeuCau, a.NgayTao, a.NgayGuiYeuCau, 
		                                            b.TenTrangThai, b.MaMau, b.MauChu, b.Border,
		                                            c.TenDonVi as 'TenDonViYeuCau', a.DonViDichId, d.TenDonVi as 'TenDonViDich',
									                a.LoaiVanBangId, e.Ten as 'TenLoaiBang'
                                                    From YeuCau as a
                                                    Left Join TrangThaiYeuCau as b
                                                    on a.MaTrangThaiYeuCau = b.Code
                                                    Left Join DonVi as c
                                                    on a.DonViYeuCauId = c.DonViId
                                                    Left Join DonVi as d
                                                    on a.DonViDichId = d.DonViId
									                Left Join LoaiBang as e
									                on a.LoaiVanBangId = e.Id
                                                    Where (a.IsDeleted = 0) and (LoaiYeuCauId = 7) and ((@DonViDichId is null) or (DonViDichId = @DonViDichId))
                                                            and ((@MaTrangThai is null) or(a.MaTrangThaiYeuCau like @MaTrangThai))
                                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(a.NgayTao) = @Nam))
                                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) {0}
                                                    Order by NgayTao Desc
                                                    Offset @Offset ROws Fetch Next @Next Rows Only", expand);
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", phongGiaoDucId.HasValue ? phongGiaoDucId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            yeuCauWithPaginationViewModel.YeuCaus = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                int totalRow = 0;
                string sqlString_1 = string.Format(@"Select Count(*) as 'TotalRow'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Where (IsDeleted = 0) and (LoaiYeuCauId = 7) and ((@DonViDichId is null) or ((DonViDichId = @DonViDichId) ))
                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(NgayTao) = @Nam))
                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) {0}", expand);
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", phongGiaoDucId.HasValue ? phongGiaoDucId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));

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
                }

                yeuCauWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                yeuCauWithPaginationViewModel.CurrentPage = currentPage;
                return yeuCauWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public YeuCauWithPaginationViewModel GetYeuCauCapBangs(int? phongGiaoDucId, int? nam, string maTrangThai, int? loaiBangId, int currentPage, int? donViId)
        {
            try
            {
                string expand = @"";
                if (donViId.HasValue && string.IsNullOrEmpty(maTrangThai))
                {
                    expand = " and a.MaTrangThaiYeuCau In ('approved', 'rejected', 'waiting')";
                }
                YeuCauWithPaginationViewModel yeuCauWithPaginationViewModel = new YeuCauWithPaginationViewModel();
                string sqlString = string.Format(@"Select a.Id, a.MaYeuCau, a.DonViYeuCauId, a.MaTrangThaiYeuCau, a.TenYeuCau, a.NgayTao, a.NgayGuiYeuCau, 
		                            b.TenTrangThai, b.MaMau, b.MauChu, b.Border,
		                            c.TenDonVi as 'TenDonViYeuCau', a.DonViDichId, d.TenDonVi as 'TenDonViDich'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Left Join DonVi as d
                                    on a.DonViDichId = d.DonViId
                                    Where (IsDeleted = 0) and (LoaiYeuCauId = 1) and ((@DonViDichId is null) or (DonViDichId = @DonViDichId))
                                            and ((@MaTrangThai is null) or(a.MaTrangThaiYeuCau like @MaTrangThai))
                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(NgayTao) = @Nam))
                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) {0}
                                    Order by NgayTao Desc
                                    Offset @Offset ROws Fetch Next @Next Rows Only", expand);
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@MaTrangThai", string.IsNullOrEmpty(maTrangThai) ? DBNull.Value : maTrangThai));
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", phongGiaoDucId.HasValue ? phongGiaoDucId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            yeuCauWithPaginationViewModel.YeuCaus = MapDataHelper<YeuCauViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                int totalRow = 0;
                string sqlString_1 = string.Format(@"Select Count(*) as 'TotalRow'
                                    From YeuCau as a
                                    Left Join TrangThaiYeuCau as b
                                    on a.MaTrangThaiYeuCau = b.Code
                                    Left Join DonVi as c
                                    on a.DonViYeuCauId = c.DonViId
                                    Where (IsDeleted = 0) and (LoaiYeuCauId = 1) and ((@DonViDichId is null) or ((DonViDichId = @DonViDichId) ))
                                            and ((@DonViYeuCau is null) or (DonViYeuCauId = @DonViYeuCau)) and ((@Nam is null) or (YEAR(NgayTao) = @Nam))
                                            and ((@LoaiBangId is null) or (a.LoaiVanBangId = @LoaiBangId)) {0}", expand);
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViDichId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViYeuCau", phongGiaoDucId.HasValue ? phongGiaoDucId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));

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
                }

                yeuCauWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                yeuCauWithPaginationViewModel.CurrentPage = currentPage;
                return yeuCauWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
