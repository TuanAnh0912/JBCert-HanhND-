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
    public class NhomNguoiDungProvider : ApplicationDbContext
    {
        public ListReturnViewModel GetAll(int? donViId, int? phongBanId)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = @"Select * from NhomNguoiDung as n inner join PhongBan as p on n.PhongBanId = p.PhongBanId
                                    Where ((@DonViId is null) or (p.DonViId = @DonViId)) and ((@PhongBanId is null) or (p.PhongBanId = @PhongBanId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.HasValue ? donViId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@PhongBanId", phongBanId.HasValue ? phongBanId.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            result.Data = MapDataHelper<NhomNguoiDungViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

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
        public NhomNguoiDung GetById(int Id)
        {
            try
            {
                return DbContext.NhomNguoiDungs.FirstOrDefault(n => n.NhomNguoiDungId == Id);
            }
            catch (Exception)
            {
                return
                    null;
            }
        }
        public ResultModel Insert(NhomNguoiDungViewModel model)
        {
            try
            {
                var obj = new NhomNguoiDung();
                obj.TenNhomNguoiDung = model.TenNhomNguoiDung;
                obj.PhongBanId = model.PhongBanId;
                DbContext.NhomNguoiDungs.Add(obj);
                DbContext.SaveChanges();
                if (model.ChucNangs != null && model.ChucNangs.Count() > 0)
                {
                    var lst = new List<LienKetNhomNguoiDungChucNang>();
                    foreach (var item in model.ChucNangs)
                    {
                        var lk = new LienKetNhomNguoiDungChucNang();
                        lk.ChucNangid = item.ChucNangId;
                        lk.NhomNguoiDungId = obj.NhomNguoiDungId;
                        lst.Add(lk);
                    }
                    DbContext.LienKetNhomNguoiDungChucNangs.AddRange(lst);
                    DbContext.SaveChanges();
                }
                return new ResultModel(true, "Thêm mới nhóm người dùng thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel Update(NhomNguoiDungViewModel model)
        {
            try
            {
                var obj = GetById(model.NhomNguoiDungId.Value);
                obj.TenNhomNguoiDung = model.TenNhomNguoiDung;
                obj.PhongBanId = model.PhongBanId;
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật nhóm người dùng thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public NhomNguoiDungViewModel GetByNhomNguoiDungId(int id)
        {
            try
            {
                NhomNguoiDungViewModel nhomNguoiDungViewModel = new NhomNguoiDungViewModel();
                string sqlString = "select * from NhomNguoiDung as n inner join PhongBan as p on n.PhongBanId = p.PhongBanId where n.NhomNguoiDungId = " + id + "";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            nhomNguoiDungViewModel = MapDataHelper<NhomNguoiDungViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                nhomNguoiDungViewModel.ChucNangs = new ChucNangProvider().GetByNhomNguoiDungId(id);
                return nhomNguoiDungViewModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<NhomNguoiDungViewModel> GetByPhongBanId(int PhongBanId)
        {
            try
            {
                var sqlString = @"select * from NhomNguoiDung where PhongBanId = " + PhongBanId;
                var lst = new List<NhomNguoiDungViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                           lst = MapDataHelper<NhomNguoiDungViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lst;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
