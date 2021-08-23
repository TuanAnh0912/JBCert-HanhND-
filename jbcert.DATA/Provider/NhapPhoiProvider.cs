using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using jbcert.DATA.Models;
using Microsoft.EntityFrameworkCore;
using jbcert.DATA.Helpers;
using Microsoft.Data.SqlClient;

namespace jbcert.DATA.Provider
{
    public class NhapPhoiProvider : ApplicationDbContext
    {
        public ResultModel InsertNhapPhoi(NhapPhoiViewModel model, string ip)
        {
            try
            {
                var nhapPhoi = new NhapPhoi();
                nhapPhoi.Ma = Configuration.RandomString(6);
                nhapPhoi.NgayTao = DateTime.Now;
                nhapPhoi.NguoiTao = model.NguoiTao;
                nhapPhoi.SoLuong = model.SoLuong;
                nhapPhoi.DiaDiemNhan = model.DiaDiemNhan;
                nhapPhoi.LoaiBangId = model.LoaiBangId;
                nhapPhoi.SoDienThoaiNguoiTao = model.SoDienThoaiNguoiTao;
                nhapPhoi.DonViId = model.DonViId;
                DbContext.NhapPhois.Add(nhapPhoi);
                DbContext.SaveChanges();

                string sqlString = @"Select * From LoaiBang Where Id = @Id";
                LoaiBang loaiBang = new LoaiBang();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", model.LoaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBang = MapDataHelper<LoaiBang>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (loaiBang.LoaiBangGocId.HasValue)
                {

                    // get codedonvi
                    string sqlString_1 = @"Select b.Code From DonVi as a
							                Left Join CapDonVi as b
							                on a.CapDonViId = b.CapDonViId
                                            Where a.DonViId = @DonViId";
                    string codeCapDonVi = "";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    codeCapDonVi = Convert.ToString(reader[0]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    if (codeCapDonVi == "SOGD")
                    {
                        int soLuong = 1;
                        List<PhoiVanBang> phoiVanBangs = new List<PhoiVanBang>();
                        while (soLuong <= model.SoLuong)
                        {
                            var phoiVanBang = new PhoiVanBang();
                            phoiVanBang.LoaiBangId = model.LoaiBangId;
                            phoiVanBang.SoHieu = "";
                            phoiVanBang.TrangThaiPhoiId = 1;
                            phoiVanBang.NgayTao = DateTime.Now;
                            phoiVanBang.NguoiTao = model.NguoiTao;
                            phoiVanBang.IsDeleted = false;
                            phoiVanBang.DonViId = model.DonViId;
                            phoiVanBang.NhapPhoiId = model.NhapPhoiId;
                            phoiVanBang.ChiSoCoDinh = "";
                            phoiVanBang.DaCap = false;
                            phoiVanBang.ChiSoThayDoi = "";
                            phoiVanBang.DonViDaNhan = ";" + model.DonViId + ";";

                            phoiVanBangs.Add(phoiVanBang);
                            soLuong++;
                        }
                        DbContext.Phois.AddRange(phoiVanBangs);
                        DbContext.SaveChanges();

                        List<NhapPhoi_Phoi> nhapPhoi_Phois = new List<NhapPhoi_Phoi>();
                        List<LogCapPhoi> logCapPhois = new List<LogCapPhoi>();
                        List<LogPhoi> logPhois = new List<LogPhoi>();

                        foreach (PhoiVanBang phoiVanBang_1 in phoiVanBangs)
                        {
                            NhapPhoi_Phoi nhapPhoi_Phoi = new NhapPhoi_Phoi();
                            nhapPhoi_Phoi.NhapPhoiId = nhapPhoi.NhapPhoiId;
                            nhapPhoi_Phoi.PhoiId = phoiVanBang_1.Id;
                            nhapPhoi_Phois.Add(nhapPhoi_Phoi);

                            LogCapPhoi logCapPhoi = new LogCapPhoi();
                            logCapPhoi.NgayCap = DateTime.Now;
                            logCapPhoi.SoHieu = "";
                            logCapPhoi.PhoiId = phoiVanBang_1.Id;
                            logCapPhoi.DonViNhanId = model.DonViId;
                            logCapPhois.Add(logCapPhoi);

                            var logPhoi = new LogPhoi();
                            var user = new NguoiDungProvider().GetById(model.NguoiTao);
                            logPhoi.NguoiDungId = user.NguoiDungId;
                            logPhoi.HanhDong = "Đã thêm bản sao";
                            logPhoi.PhoiId = phoiVanBang_1.Id;
                            logPhoi.ThoiGian = DateTime.Now;
                            logPhoi.HoTen = user.HoTen;
                            logPhoi.Ip = ip;
                            logPhois.Add(logPhoi);
                        }
                        DbContext.NhapPhoi_Phois.AddRange(nhapPhoi_Phois);
                        DbContext.SaveChanges();

                        DbContext.LogCapPhois.AddRange(logCapPhois);
                        DbContext.SaveChanges();

                        DbContext.LogPhois.AddRange(logPhois);
                        DbContext.SaveChanges();
                    }
                    else
                    {
                        // nhap phoi tu cap tren phat
                        string sqlString_4 = @"Select Top (@SoLuong) * From Phoi Where (LoaiBangId Like @LoaiBangId) and (TrangThaiPhoiId = 1)
                                                                   and (DonViId = (Select khoachaId From DonVi Where DonViId = @DonViId))";
                        List<SqlParameter> sqlParameters = new List<SqlParameter>();
                        sqlParameters.Add(new SqlParameter("@LoaiBangId", model.LoaiBangId));
                        sqlParameters.Add(new SqlParameter("@DonViId", model.DonViId));
                        sqlParameters.Add(new SqlParameter("@SoLuong", model.SoLuong));
                        List<PhoiVanBang> phoiVanBangs = new List<PhoiVanBang>();
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_4;
                                command.CommandType = System.Data.CommandType.Text;
                                foreach (var parameter in sqlParameters)
                                {
                                    command.Parameters.Add(parameter);
                                }
                                using (var reader = command.ExecuteReader())
                                {
                                    phoiVanBangs = MapDataHelper<PhoiVanBang>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }

                        // update phoi ve don vi nhan
                        if (phoiVanBangs != null && phoiVanBangs.Count != 0)
                        {
                            if (phoiVanBangs.Count < model.SoLuong)
                            {
                                Exception exception = new Exception("Không đủ số phôi bằng bản sao");
                                throw exception;
                            }

                            int i = 0;
                            List<string> sqlParameters_1 = phoiVanBangs.Select(x => "@_" + i++).ToList();
                            string sqlString_2 = string.Format(@"Update Phoi
                                                Set DonViId = @DonViId, DonViDaNhan = Concat(';',DonViDaNhan, @DonViId, ';'), NhapPhoiId = @NhapPhoiId,
                                                    DaCap = 1
                                                Where (Id in ({0})) and (LoaiBangId Like @LoaiBangId)
                                                        and (DonViId = (Select khoachaId From DonVi WHere DonViId = @DonViId))", string.Join(",", sqlParameters_1.ToArray()));
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_2;
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
                                    command.Parameters.Add(new SqlParameter("@LoaiBangId", model.LoaiBangId));
                                    command.Parameters.Add(new SqlParameter("@NhapPhoiId", model.NhapPhoiId));
                                    int k = 0;
                                    foreach (PhoiVanBang phoiVanBang in phoiVanBangs)
                                    {
                                        command.Parameters.Add(new SqlParameter("@_" + k++, phoiVanBang.Id));
                                    }
                                    command.ExecuteNonQuery();
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }

                            // get thong tin don vi gui don vi nhan
                            string sqlString_3 = @"Select a.TenDonVi as 'TenDonViNhan', b.TenDonVi as 'TenDonViGui' From DonVi as a
                                                Left Join DonVi as b
                                                on a.KhoaChaId = b.DonViId
                                                Where a.DonViId = @DonViId";
                            string tenDonViNhan = "";
                            string tenDonViGui = "";
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_3;
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@DonViId", model.DonViId));
                                    using (var reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            tenDonViNhan = Convert.ToString(reader["TenDonViNhan"]);
                                            tenDonViGui = Convert.ToString(reader["TenDonViGui"]);
                                        }
                                    }
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }

                            }

                            List<NhapPhoi_Phoi> nhapPhoi_Phois = new List<NhapPhoi_Phoi>();
                            foreach (PhoiVanBang phoiVanBang in phoiVanBangs)
                            {
                                NhapPhoi_Phoi nhapPhoi_Phoi = new NhapPhoi_Phoi();
                                nhapPhoi_Phoi.NhapPhoiId = nhapPhoi.NhapPhoiId;
                                nhapPhoi_Phoi.PhoiId = phoiVanBang.Id;
                                nhapPhoi_Phois.Add(nhapPhoi_Phoi);
                            }
                            DbContext.NhapPhoi_Phois.AddRange(nhapPhoi_Phois);
                            DbContext.SaveChanges();

                            // log
                            List<LogCapPhoi> logCapPhois = new List<LogCapPhoi>();
                            List<LogPhoi> logPhois = new List<LogPhoi>();
                            foreach (var phoi in phoiVanBangs)
                            {
                                LogCapPhoi logCapPhoi = new LogCapPhoi();
                                logCapPhoi.NgayCap = DateTime.Now;
                                logCapPhoi.SoHieu = phoi.SoHieu;
                                logCapPhoi.PhoiId = phoi.Id;
                                logCapPhoi.DonViNhanId = model.DonViId;
                                logCapPhoi.DonViCapId = phoi.DonViId;
                                logCapPhois.Add(logCapPhoi);

                                var objLog = new LogPhoi();
                                var user = new NguoiDungProvider().GetById(model.NguoiTao);
                                objLog.NguoiDungId = user.NguoiDungId;
                                objLog.HanhDong = tenDonViNhan + " nhận phôi bằng bản sao từ " + tenDonViGui;
                                objLog.ThoiGian = DateTime.Now;
                                objLog.HoTen = user.HoTen;
                                objLog.PhoiId = phoi.Id;
                                objLog.Ip = ip;
                                logPhois.Add(objLog);
                            }

                            DbContext.LogCapPhois.AddRange(logCapPhois);
                            DbContext.SaveChanges();

                            DbContext.LogPhois.AddRange(logPhois);
                            DbContext.SaveChanges();
                        }
                        else
                        {
                            Exception exception = new Exception("Không tìm thấy phôi bằng bản sao");
                            throw exception;
                        }



                    }
                }

                return new ResultModel(true, "Nhập thành công", nhapPhoi.NhapPhoiId.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<NhapPhoiViewModel> GetAll(int? LoaiBangId, int DonViId)
        {
            try
            {
                string sqlString = @"select n.*, l.Ten as TenLoaiBang, u.HoTen as TenNguoiTao from NhapPhois as n 
                                inner join LoaiBang as l on n.LoaiBangId= l.Id 
                                inner join NguoiDung as u on n.NguoiTao = u.NguoiDungId where n.DonViId = " + DonViId;
                if (LoaiBangId.HasValue)
                {
                    sqlString += " and n.LoaiBangId = " + LoaiBangId.Value;
                }

                sqlString += " Order by NgayTao Desc";
                var nhapPhois = new List<NhapPhoiViewModel>();
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
                            nhapPhois = MapDataHelper<NhapPhoiViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return nhapPhois;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public NhapPhoi GetById(int id)
        {
            try
            {
                return DbContext.NhapPhois.FirstOrDefault(n => n.NhapPhoiId == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel UpdateNhapPhoi(NhapPhoiViewModel model)
        {
            try
            {
                var obj = GetById(model.NhapPhoiId);
                obj.SoLuong = model.SoLuong;
                obj.DiaDiemNhan = model.DiaDiemNhan;
                obj.LoaiBangId = model.LoaiBangId;
                obj.SoDienThoaiNguoiTao = model.SoDienThoaiNguoiTao;
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật thông tin thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public NhapPhoiViewModel GetChiTietNhapPhoi(int Id, int donViId, int PageSize, int CurrentPage)
        {
            try
            {
                string sqlString = @"select n.*, l.Ten as TenLoaiBang, u.HoTen as TenNguoiTao ,
								    Case 
									    When l.LoaiBangGocId is null Then 0
									    Else 1
								    End as 'IsBanSao'
                                    from NhapPhois as n 
                                    inner join LoaiBang as l on n.LoaiBangId= l.Id 
                                    inner join NguoiDung as u on n.NguoiTao = u.NguoiDungId 
                                    where (n.NhapPhoiId = @Id) and (n.DonViId = @DonViId)";
                var chiTiets = new NhapPhoiViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", Id));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            chiTiets = MapDataHelper<NhapPhoiViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                chiTiets.Pagination = new PaginationViewModel();
                chiTiets.Pagination.PageSize = PageSize;
                chiTiets.Pagination.CurrentPage = CurrentPage;
                chiTiets.Pagination = new PhoiProvider().GetByNhapPhoiId(Id, donViId, chiTiets.Pagination);
                chiTiets.SoLuongDaNhap = chiTiets.Pagination.Count.Value;
                if (chiTiets.Pagination == null)
                {
                    chiTiets.Pagination = new PaginationViewModel();
                }
                return chiTiets;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
