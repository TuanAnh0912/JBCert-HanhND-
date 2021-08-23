using jbcert.DATA.Helpers;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class PhongBanProvider : ApplicationDbContext
    {
        public ListReturnViewModel GetAll(int? DonViId)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = @"select a.TenDangNhap, a.HoTen, p.*, d.*  from PhongBan as p inner join DonVi as d on p.DonViId = d.DonViId left join NguoiDung as a on a.PhongBanId = p.PhongBanId";
                if (DonViId.HasValue)
                {
                    sqlString += " where p.DonViId = " + DonViId.Value;
                }
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
                            var lst = MapDataHelper<NguoiDungPhongBanViewModel>.MapList(reader);
                            List<PhongBanViewModel> phongBanViewModels = new List<PhongBanViewModel>();
                            foreach (var item in lst)
                            {
                                if (phongBanViewModels.Any(x => x.PhongBanId == item.PhongBanId))
                                {
                                    if (!string.IsNullOrEmpty(item.TenDangNhap))
                                    {
                                        PhongBanViewModel phongBanViewModel = phongBanViewModels.Where(x => x.PhongBanId == item.PhongBanId).FirstOrDefault();
                                        NguoiDungViewModel nguoiDungViewModel = new NguoiDungViewModel();
                                        nguoiDungViewModel.HoTen = item.HoTen;
                                        phongBanViewModel.nguoiDungs.Add(nguoiDungViewModel);
                                    }
                                }
                                else
                                {
                                    PhongBanViewModel phongBanViewModel = new PhongBanViewModel();
                                    if (!string.IsNullOrEmpty(item.TenDangNhap))
                                    {
                                        phongBanViewModel.nguoiDungs = new List<NguoiDungViewModel>();
                                        NguoiDungViewModel nguoiDungViewModel = new NguoiDungViewModel();
                                        nguoiDungViewModel.HoTen = item.HoTen;
                                        phongBanViewModel.nguoiDungs.Add(nguoiDungViewModel);
                                    }

                                    phongBanViewModel.TenPhongBan = item.TenPhongBan;
                                    phongBanViewModel.Mota = item.Mota;
                                    phongBanViewModel.DonViId = item.DonViId;
                                    phongBanViewModel.PhongBanId = item.PhongBanId.Value;
                                    phongBanViewModels.Add(phongBanViewModel);
                                }
                            }
                            result.Data = phongBanViewModels;
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
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
        public PhongBan GetById(int PhongBanId)
        {
            try
            {
                return DbContext.PhongBans.FirstOrDefault(p => p.PhongBanId == PhongBanId);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel Insert(PhongBanViewModel model)
        {
            try
            {
                var obj = new PhongBan();
                obj.TenPhongBan = model.TenPhongBan;
                obj.DonViId = model.DonViId;
                obj.Mota = model.Mota;
                DbContext.PhongBans.Add(obj);
                DbContext.SaveChanges();
                return new ResultModel(true, "Thêm mới phòng ban thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel Update(PhongBanViewModel model)
        {
            try
            {
                var obj = GetById(model.PhongBanId);
                if (obj != null)
                {
                    obj.TenPhongBan = model.TenPhongBan;
                    obj.DonViId = model.DonViId;
                    obj.Mota = model.Mota;
                    obj.IsDelete = model.IsDelete;
                    DbContext.SaveChanges();
                    return new ResultModel(true, "Cập nhật phòng ban thành công");
                }
                else
                {
                    return new ResultModel(false, "Không tìm thấy phòng ban");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public List<PhongBanViewModel> GetByDonViId(int DonViId)
        {
            try
            {
                var sqlString = @"select * from PhongBan where DonViId = " + DonViId;
                var lst = new List<PhongBanViewModel>();
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
                            lst = MapDataHelper<PhongBanViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
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
