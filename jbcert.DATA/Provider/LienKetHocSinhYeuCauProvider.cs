using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class LienKetHocSinhYeuCauProvider : ApplicationDbContext
    {
        public ResultModel InsertLienKet(List<LienKetHocSinhYeuCauViewModel> model)
        {
            try
            {
                var lst = new List<LienKetHocSinhYeuCau>();
                foreach (var item in model)
                {
                    var obj = new LienKetHocSinhYeuCau();
                    obj.YeuCauId = item.YeuCauId;
                    obj.HocSinhId = item.HocSinhId;
                    obj.TrangThaiCapPhatBang = 1;
                    obj.NgayTao = DateTime.Now;
                    obj.NguoiTao = item.NguoiTao;
                    lst.Add(obj);
                }
                DbContext.LienKetHocSinhYeuCaus.AddRange(lst);
                DbContext.SaveChanges();
                var objLog = new LogYeuCau();
                var user = new NguoiDungProvider().GetById(model.FirstOrDefault().NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm danh sách học sinh ";
                objLog.YeuCauId = model.FirstOrDefault().YeuCauId;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                new YeuCauProvider().InsertLog(objLog);
                return new ResultModel(true, "Thêm danh sách học sinh thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ResultModel DeleteLienKet(List<LienKetHocSinhYeuCauViewModel> model)
        {
            try
            {
                var lst = new List<LienKetHocSinhYeuCau>();
                foreach (var item in model)
                {
                    LienKetHocSinhYeuCau lienKetHocSinhYeuCau = DbContext.LienKetHocSinhYeuCaus.Where(x => x.YeuCauId == item.YeuCauId && x.HocSinhId == item.HocSinhId).FirstOrDefault();
                    if (lienKetHocSinhYeuCau != null)
                    {
                        lst.Add(lienKetHocSinhYeuCau);
                    }
                }
                DbContext.LienKetHocSinhYeuCaus.RemoveRange(lst);
                DbContext.SaveChanges();
                var objLog = new LogYeuCau();
                var user = new NguoiDungProvider().GetById(model.FirstOrDefault().NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã cập nhật danh sách học sinh ";
                objLog.YeuCauId = model.FirstOrDefault().YeuCauId;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                new YeuCauProvider().InsertLog(objLog);
                return new ResultModel(true, "Cập nhật danh sách học sinh thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
    }
}
