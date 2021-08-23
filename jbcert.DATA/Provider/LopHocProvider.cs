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
    public class LopHocProvider : ApplicationDbContext
    {
        public List<LopHocViewModel> Gets(Guid ? GiaoVienId, string nienKhoa, int DonViId)
        {
            try
            {
                if (string.IsNullOrEmpty(nienKhoa))
                {
                    nienKhoa = "";
                }
                string sqlString = @"select l.*, n.HoTen as TenGiaoVien, n.DienThoai as DienThoaiGiaoVien  
                                    from LopHocs as l 
                                    inner join NguoiDung as n 
                                    on l.GiaoVien = n.NguoiDungId 
                                    where l.DonViId = @DonViId and ((@NienKhoa is null) or (l.NienKhoa Like '%'+@NienKhoa+'%'))";
                if (GiaoVienId.HasValue)
                {
                    sqlString += " and l.GiaoVien = '" + GiaoVienId.Value +"'";
                }
                sqlString += " Order by NgayTao DESC";
                List<LopHocViewModel> lopHocs = new List<LopHocViewModel>();

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ?  DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@DonViId", DonViId));

                        using (var reader = command.ExecuteReader())
                        {
                            lopHocs = MapDataHelper<LopHocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lopHocs;
            }
            catch(Exception e)
            {
                return null;
            }
        }
        public ResultModel Insert(LopHocViewModel model)
        {
            try
            {
                string sqlString = string.Format(@"Select Count(*) as 'TotalRow' 
                                                From LopHocs Where (TenLop Like N'{0}') and (NienKhoa like @NienKhoa) and (DonViId = @DonViId)", model.TenLop);
                List<LopHocViewModel> lopHocs = new List<LopHocViewModel>();
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NienKhoa", model.NienKhoa));
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
                        command.Connection.Close();
                    }
                }

                if(totalRow > 0)
                {
                    return new ResultModel(false, "Đã tồn tại lớp học " + model.TenLop + " năm " + model.NienKhoa );
                }

                var obj = new LopHoc();
                obj.TenLop = model.TenLop;
                obj.GiaoVien = model.GiaoVien;
                obj.DonViId = model.DonViId;
                obj.NienKhoa = model.NienKhoa;
                obj.NgayTao = DateTime.Now;
                DbContext.LopHocs.Add(obj);
                DbContext.SaveChanges();
                return new ResultModel(true, "Thêm mới lớp học " + obj.TenLop + " thành công!");
            }
            catch(Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public LopHoc GetById(int id)
        {
            try
            {
                return DbContext.LopHocs.FirstOrDefault(l => l.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel Update(LopHocViewModel model)
        {
            try
            {
                string sqlString = string.Format(@"Select Count(*) as 'TotalRow' 
                                                From LopHocs Where (TenLop Like N'{0}') and (NienKhoa like @NienKhoa) and (DonViId = @DonViId) and (Id != @Id)", model.TenLop);
                List<LopHocViewModel> lopHocs = new List<LopHocViewModel>();
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NienKhoa", model.NienKhoa));
                        command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
                        command.Parameters.Add(new SqlParameter("@Id", model.Id));

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

                if (totalRow > 0)
                {
                    return new ResultModel(false, "Đã tồn tại lớp học " + model.TenLop + " năm " + model.NienKhoa);
                }

                var obj = GetById(model.Id);
                if(obj != null)
                {
                    obj.TenLop = model.TenLop;
                    obj.NienKhoa = model.NienKhoa;
                    obj.GiaoVien = model.GiaoVien;
                    DbContext.SaveChanges();
                    return new ResultModel(true, "Cập nhật lớp " + obj.TenLop + " thành công!");
                }
                else
                {
                    return new ResultModel(false, "Không tìm thấy lớp học trong cơ sở dữ liệu");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
    }
}
