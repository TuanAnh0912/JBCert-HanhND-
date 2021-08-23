using jbcert.DATA.Helpers;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class NguoiDungProvider : ApplicationDbContext
    {
        public NguoiDung GetByTenDangNhap(string TenDangNhap)
        {
            try
            {
                NguoiDung nguoiDung = DbContext.NguoiDungs.Include("DonVi").Include("PhongBan").FirstOrDefault(n => n.TenDangNhap == TenDangNhap);
                if (nguoiDung != null)
                {
                    nguoiDung.DonVi.CapDonVi = DbContext.CapDonVis.Find(nguoiDung.DonVi.CapDonViId);
                }
                else
                {
                    return null;
                }
                return nguoiDung;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public NguoiDung GetByTenDangNhapMatKhau(string TenDangNhap, string MatKhau)
        {
            try
            {
                return DbContext.NguoiDungs.FirstOrDefault(a => a.TenDangNhap == TenDangNhap && a.MatKhau == MatKhau);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public NguoiDung GetById(Guid NguoiDungId)
        {
            try
            {
                return DbContext.NguoiDungs.FirstOrDefault(n => n.NguoiDungId == NguoiDungId);
            }
            catch (Exception e)
            {
                return
                    null;
            }
        }
        public ResultModel InsertUser(NguoiDungViewModel model)
        {
            try
            {
                // ko dung ham nay de insert nguoi dung
                if (CheckUserName(model.TenDangNhap))
                {
                    var obj = new NguoiDung();
                    obj.NguoiDungId = Guid.NewGuid();
                    obj.TenDangNhap = model.TenDangNhap;
                    obj.MatKhau = SecurityHelper.Hash(model.TenDangNhap + model.MatKhau);
                    obj.HoTen = model.HoTen;
                    obj.NhomNguoiDungId = model.NhomNguoiDungId;
                    obj.PhongBanId = model.PhongBanId;
                    obj.NgaySinh = model.NgaySinh;
                    obj.DiaChi = model.DiaChi;
                    obj.SoCanCuoc = model.SoCanCuoc;
                    obj.DienThoai = model.DienThoai;
                    obj.Email = model.Email;
                    obj.NgayTao = DateTime.Now;
                    obj.NguoiTao = model.NguoiTao;
                    obj.Active = true;
                    obj.IsDelete = false;
                    obj.GioiTinhId = model.GioiTinhId;
                    obj.DanTocId = model.DanTocId;
                    obj.DonViId = model.DonViId;
                    obj.LaGiaoVien = model.LaGiaoVien;
                    DbContext.NguoiDungs.Add(obj);
                    DbContext.SaveChanges();
                    return new ResultModel(true, "Thêm mới người dùng thành công");
                }
                else
                {
                    return new ResultModel(false, "Tên đăng nhập này đã tồn tại");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ListReturnViewModel GetAll(string keyword, int? DonViId)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = @"Select n.*, p.TenPhongBan, d.TenDonVi, nd.TenNhomNguoiDung from 
                                      NguoiDung as n 
                                      inner join PhongBan as p 
                                      on n.PhongBanId = p.PhongBanId 
                                      inner join DonVi as d 
                                      on p.DonViId = d.DonViId 
                                      inner join NhomNguoiDung as nd 
                                      on n.NhomNguoiDungId = nd.NhomNguoiDungId 
                                      inner join GioiTinh as g 
                                      on n.GioiTinhId = g.Id 
                                      inner join DanToc 
                                      as dt on n.DanTocId = dt.Id
                                      where n.HoTen Like N'%'+@HoTen+'%'";
                if (DonViId.HasValue)
                {
                    sqlString += " and p.DonViId = " + DonViId.Value;
                }
                sqlString += " Order by n.NgayTao DESC";
                List<NguoiDungViewModel> nguoiDungViewModels = new List<NguoiDungViewModel>();

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoTen", keyword));

                        using (var reader = command.ExecuteReader())
                        {
                            nguoiDungViewModels = MapDataHelper<NguoiDungViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                //if (!string.IsNullOrEmpty(keyword))
                //{
                //    nguoiDungViewModels = nguoiDungViewModels.Where(n => (Configuration.NonUnicode(n.HoTen).ToLower().Contains(Configuration.NonUnicode(keyword).ToLower())) || n.DienThoai.Contains(keyword)).ToList();
                //}
                result.Data = nguoiDungViewModels;
                result.Message = "Get data success";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Message = e.Message;
                result.Status = false;
                return
                    result;
            }
        }
        public ResultModel UpdateUser(NguoiDungViewModel model)
        {
            try
            {
                var obj = GetById(model.NguoiDungId);
                if (model.IsDelete.HasValue && model.IsDelete.Value) // delete
                {
                    obj.TenDangNhap = model.TenDangNhap;
                    obj.MatKhau = SecurityHelper.Hash(model.TenDangNhap + model.MatKhau);
                    obj.HoTen = model.HoTen;
                    obj.NhomNguoiDungId = model.NhomNguoiDungId;
                    obj.PhongBanId = model.PhongBanId;
                    obj.NgaySinh = model.NgaySinh;
                    obj.DiaChi = model.DiaChi;
                    obj.SoCanCuoc = model.SoCanCuoc;
                    obj.DienThoai = model.DienThoai;
                    obj.Email = "";
                    obj.NgayTao = DateTime.Now;
                    obj.NguoiTao = model.NguoiTao;
                    obj.Active = model.Active;
                    obj.IsDelete = model.IsDelete;
                    obj.GioiTinhId = model.GioiTinhId;
                    obj.DanTocId = model.DanTocId;
                    obj.DonViId = model.DonViId;
                    DbContext.SaveChanges();

                    string sqlString = @"Delete [AspNetUsers] Where Id = @Id";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@Id", model.NguoiDungId));
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
                    obj.TenDangNhap = model.TenDangNhap;
                    obj.MatKhau = DATA.Helpers.SecurityHelper.Hash(model.TenDangNhap + model.MatKhau);
                    obj.HoTen = model.HoTen;
                    obj.NhomNguoiDungId = model.NhomNguoiDungId;
                    obj.PhongBanId = model.PhongBanId;
                    obj.NgaySinh = model.NgaySinh;
                    obj.DiaChi = model.DiaChi;
                    obj.SoCanCuoc = model.SoCanCuoc;
                    obj.DienThoai = model.DienThoai;
                    obj.Email = model.Email;
                    obj.NgayTao = DateTime.Now;
                    obj.NguoiTao = model.NguoiTao;
                    obj.Active = model.Active;
                    obj.IsDelete = model.IsDelete;
                    obj.GioiTinhId = model.GioiTinhId;
                    obj.DanTocId = model.DanTocId;
                    obj.DonViId = model.DonViId;
                    DbContext.SaveChanges();
                }
                return new ResultModel(true, "Update success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public NguoiDungViewModel GetNguoiDungById(string NguoiDungId)
        {
            try
            {
                NguoiDungViewModel nguoiDungViewModel = new NguoiDungViewModel();
                string sqlString = @"select n.*, p.TenPhongBan, d.TenDonVi, nd.TenNhomNguoiDung from 
                                      NguoiDung as n 
                                      inner join PhongBan as p 
                                      on n.PhongBanId = p.PhongBanId 
                                      inner join DonVi as d 
                                      on p.DonViId = d.DonViId 
                                      inner join NhomNguoiDung as nd 
                                      on n.NhomNguoiDungId = nd.NhomNguoiDungId 
                                      inner join GioiTinh as g 
                                      on n.GioiTinhId = g.Id 
                                      inner join DanToc 
                                      as dt on n.DanTocId = dt.Id
                                      where n.NguoiDungId = @NguoiDungId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NguoiDungId", NguoiDungId));
                        using (var reader = command.ExecuteReader())
                        {
                            nguoiDungViewModel = MapDataHelper<NguoiDungViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return nguoiDungViewModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool CheckUserName(string TenDangNhap)
        {
            try
            {
                var user = GetByTenDangNhap(TenDangNhap);
                if (user != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public ResultModel DoiMatKhau(ChangePasswordViewModel model)
        {
            try
            {
                var user = GetById(model.NguoiDungId);
                if (user.MatKhau == DATA.Helpers.SecurityHelper.Hash(user.TenDangNhap + model.MatKhau))
                {
                    user.MatKhau = DATA.Helpers.SecurityHelper.Hash(user.TenDangNhap + model.MatKhauMoi);
                    DbContext.SaveChanges();
                    return new ResultModel(true, "Đổi mật khẩu thành công!");
                }
                else
                {
                    return new ResultModel(false, "Mật khẩu cũ không chính xác");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
    }
}
