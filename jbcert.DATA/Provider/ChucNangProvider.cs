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
using System.Threading.Tasks;
namespace jbcert.DATA.Provider
{
    public class ChucNangProvider: ApplicationDbContext
    {
        public List<ChucNangViewModel> GetByNhomNguoiDungId(int NhomNguoiDungId)
        {
            try
            {
                List<ChucNangViewModel> chucNangViewModels = new List<ChucNangViewModel>();
                string sqlString = @"select c.ChucNangId, c.KhoaChaId, c.TenChucNang, c.AuthCode, c.Icon, c.ShowOnMenu, c.Alias, c.DefaultAlias, c.Summary  
                                    from LienKetNhomNguoiDungChucNang as lk 
                                    inner join NhomNguoiDung as n 
                                    on lk.NhomNguoiDungId = n.NhomNguoiDungId 
                                    inner join ChucNang as c 
                                    on lk.ChucNangid = c.ChucNangId 
                                    where n.NhomNguoiDungId = @NhomNguoiDungId 
                                    ORDER BY c.[Order] ASC";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomNguoiDungId", NhomNguoiDungId));
                        using (var reader = command.ExecuteReader())
                        {
                            chucNangViewModels = MapDataHelper<ChucNangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return chucNangViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel InsertLienKetChucNangNhomNguoiDung(List<LienKetNhomNguoiDungChucNang> model)
        {
            try
            {
                DbContext.LienKetNhomNguoiDungChucNangs.AddRange(model);
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật quyền thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel DeleteLienKetNhomNguoiDungChucNang(int NhomNguoiDungId)
        {
            try
            {
                var lst = DbContext.LienKetNhomNguoiDungChucNangs.Where(l => l.NhomNguoiDungId == NhomNguoiDungId);
                if (lst.Count() > 0)
                {
                    DbContext.LienKetNhomNguoiDungChucNangs.RemoveRange(lst);
                    DbContext.SaveChanges();
                }
                return new ResultModel(true, "Delete success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel UpdateLienKetChucNangNhomNguoiDung(NhomNguoiDungViewModel model)
        {
            try
            {
                var checkDel = DeleteLienKetNhomNguoiDungChucNang(model.NhomNguoiDungId.Value);
                if (checkDel.Status)
                {
                    var lst = new List<LienKetNhomNguoiDungChucNang>();
                    foreach (var item in model.ChucNangs)
                    {
                        if (item.KhoaChaId != null)
                        {
                            ChucNang chucNangCha = DbContext.ChucNangs.Find(item.KhoaChaId);
                            if (!lst.Any(x => x.ChucNangid == chucNangCha.ChucNangId))
                            {
                                LienKetNhomNguoiDungChucNang lkCha = new LienKetNhomNguoiDungChucNang();
                                lkCha.NhomNguoiDungId = model.NhomNguoiDungId.Value;
                                lkCha.ChucNangid = chucNangCha.ChucNangId;
                                lst.Add(lkCha);
                            }
                            
                        }
                        LienKetNhomNguoiDungChucNang lk = new LienKetNhomNguoiDungChucNang();
                        if (lst.Any(x => x.ChucNangid == item.ChucNangId))
                        {
                            continue;
                        }
                        lk.NhomNguoiDungId = model.NhomNguoiDungId.Value;
                        lk.ChucNangid = item.ChucNangId;
                        lst.Add(lk);
                    }
                    return InsertLienKetChucNangNhomNguoiDung(lst);
                }
                else
                {
                    return checkDel;
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ListReturnViewModel GetAll(int? CapDonViId)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = "select * from LienKetCapDonViChucNang as lk inner join ChucNang as c on lk.ChucNangId = c.ChucNangId";
                if (CapDonViId.HasValue)
                {
                    sqlString += " where lk.CapDonViId = " + CapDonViId;
                }
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
                            result.Data = MapDataHelper<ChucNangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                result.Message = "Get data success";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Message = e.Message;
                result.Status = false;
                return result;
            }
        }

        public List<LienKetCapDonViChucNang> GetByCapDonDonVi(int CapDonViId)
        {
            try
            {
                return DbContext.LienKetCapDonViChucNangs.Where(l => l.CapDonViId == CapDonViId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
