using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class LogNguoiDungProvider : ApplicationDbContext
    {
        public ResultModel InsertLog(LogNguoiDungViewModel model)
        {
            try
            {
                var obj = new LogNguoiDung();
                obj.NguoiDungId = model.NguoiDungId;
                obj.HanhDong = model.HanhDong;
                obj.ThoiGian = DateTime.Now;
                obj.Ip = model.IP;
                DbContext.LogNguoiDungs.Add(obj);
                DbContext.SaveChanges();
                return new ResultModel(true, "Insert Log success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
    }
}
