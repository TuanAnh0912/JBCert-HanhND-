using jbcert.DATA.Helpers;
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
    public class GiaoVienProvider : ApplicationDbContext
    {
        public List<NguoiDungViewModel> GetGiaoViens(string keyword, int ? DonViId)
        {
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
                                      where n.LaGiaoVien = 1 and  n.HoTen Like N'%'+@HoTen+'%'";
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
                var giaoViens = new List<NguoiDungViewModel>();
                foreach(var item in nguoiDungViewModels)
                {
                    if (!DonViId.HasValue)
                    {
                        DonViId = 0;
                    }
                    item.LopHocs = new LopHocProvider().Gets(item.NguoiDungId, "", DonViId.Value);
                    giaoViens.Add(item);
                }
                return giaoViens; ;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
