using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
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
    public class PhoiProvider : ApplicationDbContext, IPhoi
    {
        public List<LichSuPhoiViewModel> GetLichSuPhoi(int phoiId)
        {
            try
            {
                string sqlString_1 = @"Select a.*, b.TenDonVi as 'TenDonViCap', c.TenDonVi as 'TenDonViNhan' From LogCapPhois as a
                                        Left Join DonVi as b
                                        on a.DonViCapId = b.DonViId
                                        Left Join DonVi as c
                                        on a.DonViNhanId = c.DonViId
                                        Where a.PhoiId = @PhoiId
                                        Order by a.NgayCap Desc";
                List<LichSuPhoiViewModel> lichSuPhoiViewModels = new List<LichSuPhoiViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhoiId", phoiId));
                        using (var reader = command.ExecuteReader())
                        {
                            lichSuPhoiViewModels = MapDataHelper<LichSuPhoiViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return lichSuPhoiViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LogPhoiViewModel> GetLogPhoi(int phoiId)
        {
            try
            {
                string sqlString_1 = @"Select * From LogPhois Where PhoiId = @PhoiId";
                List<LogPhoiViewModel> logPhoiViewModels = new List<LogPhoiViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@PhoiId", phoiId));
                        using (var reader = command.ExecuteReader())
                        {
                            logPhoiViewModels = MapDataHelper<LogPhoiViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return logPhoiViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddPhois(AddPhoiViewModel addPhoiViewModel, int donViId)
        {
            try
            {
                string sqlString = @"Select * From [dbo].[LoaiBang] Where ([Id] = @Id) and ([IsDeleted] = 0)";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@Id", addPhoiViewModel.LoaiBangId));
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                LoaiBang loaiBang = DbContext.LoaiBangs.FromSqlRaw(sqlString, sqlParameters.ToArray()).FirstOrDefault();
                if (loaiBang != null)
                {
                    List<PhoiVanBang> phois = new List<PhoiVanBang>();
                    for (int i = 1; i <= addPhoiViewModel.SoLuongPhoi; i++)
                    {
                        PhoiVanBang phoi = new PhoiVanBang();
                        phoi.LoaiBangId = addPhoiViewModel.LoaiBangId;
                        phoi.SoHieu = loaiBang.MaLoaiBang + @"\" + loaiBang.MaNoiIn + (loaiBang.ChiSo + i);
                        phoi.TrangThaiPhoiId = 1;
                        phoi.MoTaTrangThai = "";
                        phoi.NguoiTao = addPhoiViewModel.NguoiTao;
                        phoi.NgayTao = addPhoiViewModel.NgayTao;
                        phoi.IsDeleted = false;
                        phoi.DonViId = donViId;
                        phois.Add(phoi);
                    }
                    DbContext.Phois.AddRange(phois);
                    loaiBang.ChiSo += addPhoiViewModel.SoLuongPhoi;
                    DbContext.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResultModel InsertPhoi(PhoiVanBangViewModel model, string ip)
        {
            try
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

                    string sqlString_2 = @"Select Count(*) as 'TotalRow' From Phoi
                                            Where SoHieu Like @SoHieu";
                    int count = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoHieu", model.SoHieu));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    count = Convert.ToInt32(reader[0]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    if (count > 0)
                    {
                        Exception exception = new Exception("Đã tồn tại số hiệu");
                        throw exception;
                    }

                    // so nhap phoi
                    var obj = new PhoiVanBang();
                    obj.LoaiBangId = model.LoaiBangId;
                    obj.SoHieu = model.SoHieu;
                    obj.TrangThaiPhoiId = 1;
                    obj.NgayTao = DateTime.Now;
                    obj.NguoiTao = model.NguoiTao;
                    obj.IsDeleted = false;
                    obj.DonViId = model.DonViId;
                    obj.NhapPhoiId = model.NhapPhoiId;
                    obj.ChiSoCoDinh = model.ChiSoCoDinh;
                    obj.DaCap = false;
                    obj.ChiSoThayDoi = model.ChiSoThayDoi;
                    obj.DonViDaNhan = ";" + model.DonViId + ";";
                    DbContext.Phois.Add(obj);
                    DbContext.SaveChanges();

                    LogCapPhoi logCapPhoi = new LogCapPhoi();
                    logCapPhoi.NgayCap = DateTime.Now;
                    logCapPhoi.SoHieu = model.SoHieu;
                    logCapPhoi.PhoiId = obj.Id;
                    logCapPhoi.DonViNhanId = model.DonViId;
                    DbContext.LogCapPhois.Add(logCapPhoi);
                    DbContext.SaveChanges();

                    var objLog = new LogPhoi();
                    var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã thêm phôi có số hiệu " + obj.SoHieu;
                    objLog.PhoiId = model.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                }
                else
                {
                    // nhap phoi tu cap tren phat
                    string sqlString = @"Select * From Phoi Where (SoHieu Like @SoHieu) and (LoaiBangId Like @LoaiBangId) and (TrangThaiPhoiId = 1)
                                                                   and (DonViId = (Select khoachaId From DonVi WHere DonViId = @DonViId))";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    sqlParameters.Add(new SqlParameter("@SoHieu", model.SoHieu));
                    sqlParameters.Add(new SqlParameter("@LoaiBangId", model.LoaiBangId));
                    sqlParameters.Add(new SqlParameter("@DonViId", model.DonViId));
                    PhoiVanBang phoiVanBang = new PhoiVanBang();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            foreach (var parameter in sqlParameters)
                            {
                                command.Parameters.Add(parameter);
                            }
                            using (var reader = command.ExecuteReader())
                            {
                                phoiVanBang = MapDataHelper<PhoiVanBang>.Map(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    if (phoiVanBang != null && !string.IsNullOrEmpty(phoiVanBang.SoHieu))
                    {
                        string sqlString_2 = @"Update Phoi
                                                Set DonViId = @DonViId, DonViDaNhan = Concat(';',DonViDaNhan, @DonViId, ';'), NhapPhoiId = @NhapPhoiId,
                                                    DaCap = 1
                                                Where (SoHieu Like @SoHieu) and (LoaiBangId Like @LoaiBangId)
                                                        and (DonViId = (Select khoachaId From DonVi WHere DonViId = @DonViId))";
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
                                command.Parameters.Add(new SqlParameter("@SoHieu", model.SoHieu));
                                command.Parameters.Add(new SqlParameter("@NhapPhoiId", model.NhapPhoiId));
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

                        NhapPhoi_Phoi nhapPhoi_Phoi = new NhapPhoi_Phoi();
                        nhapPhoi_Phoi.NhapPhoiId = phoiVanBang.NhapPhoiId;
                        nhapPhoi_Phoi.PhoiId = phoiVanBang.Id;
                        DbContext.NhapPhoi_Phois.Add(nhapPhoi_Phoi);
                        DbContext.SaveChanges();

                        LogCapPhoi logCapPhoi = new LogCapPhoi();
                        logCapPhoi.NgayCap = DateTime.Now;
                        logCapPhoi.SoHieu = model.SoHieu;
                        logCapPhoi.PhoiId = phoiVanBang.Id;
                        logCapPhoi.DonViNhanId = model.DonViId;
                        logCapPhoi.DonViCapId = phoiVanBang.DonViId;
                        DbContext.LogCapPhois.Add(logCapPhoi);
                        DbContext.SaveChanges();

                        var objLog = new LogPhoi();
                        var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                        objLog.NguoiDungId = user.NguoiDungId;
                        objLog.HanhDong = tenDonViNhan + " nhận phôi có số hiệu " + phoiVanBang.SoHieu + " từ " + tenDonViGui;
                        objLog.PhoiId = phoiVanBang.Id;
                        objLog.ThoiGian = DateTime.Now;
                        objLog.HoTen = user.HoTen;
                        objLog.Ip = ip;
                        InsertLog(objLog);
                    }
                    else
                    {
                        Exception exception = new Exception("Số hiệu không đúng");
                        throw exception;
                    }


                }
                return new ResultModel(true, "Thêm số hiệu thành công");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ResultModel InsertPhois(List<PhoiVanBangViewModel> models, string ip)
        {
            try
            {
                if (models.Count() > 0)
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
                            command.Parameters.Add(new SqlParameter("@DonViId", models.FirstOrDefault().DonViId));
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
                        List<string> sqlParamteres_3 = new List<string>();
                        int i = 0;
                        foreach (var model in models)
                        {
                            sqlParamteres_3.Add("@_" + i.ToString());
                            i++;
                        }
                        int count = 0;
                        string sqlString_3 = @"Select Count(*) as 'TotalRow' From Phoi
                                            Where (SoHieu In ({0}))";
                        sqlString_3 = string.Format(sqlString_3, string.Join(",", sqlParamteres_3.ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_3;
                                command.CommandType = System.Data.CommandType.Text;
                                int k = 0;
                                foreach (string soHieu in models.Select(x => x.SoHieu))
                                {
                                    command.Parameters.Add(new SqlParameter("@_" + k, soHieu));
                                    k++;
                                }

                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        count = Convert.ToInt32(reader[0]);
                                    }
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }

                        if (count > 0)
                        {
                            Exception exception = new Exception("Đã tồn tại số hiệu");
                            throw exception;
                        }

                        List<PhoiVanBang> phoiVanBangs = new List<PhoiVanBang>();
                        foreach (var model in models)
                        {
                            var obj = new PhoiVanBang();
                            obj.LoaiBangId = model.LoaiBangId;
                            obj.SoHieu = model.SoHieu;
                            obj.TrangThaiPhoiId = 1;
                            obj.NgayTao = DateTime.Now;
                            obj.ChiSoCoDinh = model.ChiSoCoDinh;
                            obj.ChiSoThayDoi = model.ChiSoThayDoi;
                            obj.NguoiTao = model.NguoiTao;
                            obj.IsDeleted = false;
                            obj.DaCap = false;
                            obj.DonViId = model.DonViId;
                            obj.NhapPhoiId = model.NhapPhoiId;
                            obj.DonViDaNhan = ";" + model.DonViId + ";";
                            phoiVanBangs.Add(obj);
                        }
                        DbContext.Phois.AddRange(phoiVanBangs);
                        DbContext.SaveChanges();

                        List<LogCapPhoi> logCapPhois = new List<LogCapPhoi>();
                        foreach (PhoiVanBang phoiVanBang in phoiVanBangs)
                        {
                            LogCapPhoi logCapPhoi = new LogCapPhoi();
                            logCapPhoi.NgayCap = DateTime.Now;
                            logCapPhoi.SoHieu = phoiVanBang.SoHieu;
                            logCapPhoi.PhoiId = phoiVanBang.Id;
                            logCapPhoi.DonViNhanId = phoiVanBang.DonViId;
                            logCapPhois.Add(logCapPhoi);
                        }
                        DbContext.SaveChanges();


                        DbContext.LogCapPhois.AddRange(logCapPhois);
                        DbContext.SaveChanges();

                        List<LogPhoi> logPhois = new List<LogPhoi>();
                        foreach (var model in models)
                        {
                            var objLog = new LogPhoi();
                            var user = new NguoiDungProvider().GetById(models.FirstOrDefault().NguoiTao.Value);
                            objLog.NguoiDungId = user.NguoiDungId;
                            objLog.HanhDong = "Đã thêm phôi có số hiệu " + model.SoHieu;
                            objLog.ThoiGian = DateTime.Now;
                            objLog.PhoiId = model.Id;
                            objLog.HoTen = user.HoTen;
                            objLog.Ip = ip;
                            logPhois.Add(objLog);
                        }
                        DbContext.LogPhois.AddRange(logPhois);
                        DbContext.SaveChanges();
                    }
                    else
                    {
                        // nhap phoi tu cap tren phat
                        List<string> parameters = new List<string>();
                        int i = 0;
                        foreach (var soHieu in models.Select(x => x.SoHieu).ToList())
                        {
                            parameters.Add("@_" + i);
                            i++;
                        }
                        string sqlString = @"Select * From Phoi Where (ChiSoCoDinh Like @ChiSoCoDinh) and (LoaiBangId Like @LoaiBangId)
                                                                   and (TrangThaiPhoiId = 1)
                                                                   and (DonViId = (Select KhoaChaId From DonVi WHere DonViId = @DonViId))
                                                                    and (SoHieu in ({0}))";
                        sqlString = string.Format(sqlString, string.Join(",", parameters.ToArray()));
                        List<PhoiVanBang> phoiVanBangs_1 = new List<PhoiVanBang>();
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString;
                                command.CommandType = System.Data.CommandType.Text;
                                int k = 0;
                                foreach (var soHieu in models.Select(x => x.SoHieu).ToList())
                                {
                                    command.Parameters.Add(new SqlParameter("@_" + k, soHieu));
                                    k++;
                                }
                                command.Parameters.Add(new SqlParameter("@ChiSoCoDinh", models.FirstOrDefault().ChiSoCoDinh));
                                command.Parameters.Add(new SqlParameter("@LoaiBangId", models.FirstOrDefault().LoaiBangId));
                                command.Parameters.Add(new SqlParameter("@DonViId", models.FirstOrDefault().DonViId));
                                using (var reader = command.ExecuteReader())
                                {
                                    phoiVanBangs_1 = MapDataHelper<PhoiVanBang>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }

                        if (phoiVanBangs_1.Count == 0)
                        {
                            Exception exception = new Exception("Số hiệu không đúng");
                            throw exception;
                        }

                        List<string> parameters_2 = new List<string>();
                        int m = 0;
                        foreach (var soHieu in phoiVanBangs_1.Select(x => x.SoHieu).ToList())
                        {
                            parameters_2.Add("@_" + m);
                            m++;
                        }

                        string sqlString_2 = @"Update Phoi
                                             Set DonViId = @DonViId, DonViDaNhan = Concat(DonViDaNhan, @DonViId, ';'), NhapPhoiId = @NhapPhoiId,
                                                DaCap = 1
                                            Where (ChiSoCoDinh Like @ChiSoCoDinh) and (LoaiBangId Like @LoaiBangId)
                                                        and (DonViId = (Select KhoaChaId From DonVi WHere DonViId = @DonViId))
                                                        and (SoHieu in ({0}))";
                        sqlString_2 = string.Format(sqlString_2, string.Join(",", parameters_2.ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = System.Data.CommandType.Text;
                                int k = 0;
                                foreach (var soHieu in phoiVanBangs_1.Select(x => x.SoHieu).ToList())
                                {
                                    command.Parameters.Add(new SqlParameter("@_" + k, soHieu));
                                    k++;
                                }
                                command.Parameters.Add(new SqlParameter("@DonViId", models.FirstOrDefault().DonViId));
                                command.Parameters.Add(new SqlParameter("@LoaiBangId", models.FirstOrDefault().LoaiBangId));
                                command.Parameters.Add(new SqlParameter("@ChiSoCoDinh", models.FirstOrDefault().ChiSoCoDinh));
                                command.Parameters.Add(new SqlParameter("@NhapPhoiId", models.FirstOrDefault().NhapPhoiId));
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }

                        }

                        List<NhapPhoi_Phoi> nhapPhoi_Phois = new List<NhapPhoi_Phoi>();
                        foreach (PhoiVanBang phoiVanBang in phoiVanBangs_1)
                        {
                            NhapPhoi_Phoi nhapPhoi_Phoi = new NhapPhoi_Phoi();
                            nhapPhoi_Phoi.NhapPhoiId = phoiVanBang.NhapPhoiId;
                            nhapPhoi_Phoi.PhoiId = phoiVanBang.Id;

                            nhapPhoi_Phois.Add(nhapPhoi_Phoi);
                        }
                        DbContext.NhapPhoi_Phois.AddRange(nhapPhoi_Phois);
                        DbContext.SaveChanges();

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
                                command.Parameters.Add(new SqlParameter("@DonViId", models.FirstOrDefault().DonViId));
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

                        // log
                        List<LogCapPhoi> logCapPhois = new List<LogCapPhoi>();
                        List<LogPhoi> logPhois = new List<LogPhoi>();
                        foreach (var phoi in phoiVanBangs_1)
                        {
                            LogCapPhoi logCapPhoi = new LogCapPhoi();
                            logCapPhoi.NgayCap = DateTime.Now;
                            logCapPhoi.SoHieu = phoi.SoHieu;
                            logCapPhoi.PhoiId = phoi.Id;
                            logCapPhoi.DonViNhanId = models.FirstOrDefault().DonViId;
                            logCapPhoi.DonViCapId = phoi.DonViId;
                            logCapPhois.Add(logCapPhoi);

                            var objLog = new LogPhoi();
                            var user = new NguoiDungProvider().GetById(models.FirstOrDefault().NguoiTao.Value);
                            objLog.NguoiDungId = user.NguoiDungId;
                            objLog.HanhDong = tenDonViNhan + " nhận phôi có số hiệu " + phoi.SoHieu + " từ " + tenDonViGui;
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

                }

                return new ResultModel(true, "Thêm số hiệu thành công");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ResultModel UpdatePhoiV2(PhoiVanBangViewModel model, string ip)
        {
            try
            {
                string sqlString_1 = @"Select b.* From Phoi as a
                                    Left Join LoaiBang as b
                                    on a.LoaiBangId = b.Id
                                    Where a.Id = @Id";
                LoaiBang loaiBang = new LoaiBang();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", model.Id));
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

                if (loaiBang.LoaiBangGocId == null) // phoi bang goc
                {
                    string sqlString = @"Select Count(*) as 'TotalRow' From Phoi Where (SoHieu Like @SoHieu) and (Id != @Id)";
                    int count = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoHieu", model.SoHieu));
                            command.Parameters.Add(new SqlParameter("@Id", model.Id));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    count = Convert.ToInt32(reader[0]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                    if (count > 0)
                    {
                        Exception exception = new Exception("Đã tồn tại số hiệu");
                        throw exception;
                    }

                    string sqlString_2 = @"Update Phoi
                                            Set SoHieu = @SoHieu, ChiSoCoDinh = @ChiSoCoDinh, ChiSoThayDoi = @ChiSoThayDoi,
                                                TrangThaiPhoiId = @TrangThaiPhoiId, MoTaTrangThai = @MoTaTrangThai
                                            Where Id = @Id";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoHieu", model.SoHieu));
                            command.Parameters.Add(new SqlParameter("@ChiSoThayDoi", model.ChiSoThayDoi));
                            command.Parameters.Add(new SqlParameter("@ChiSoCoDinh", model.ChiSoCoDinh));
                            command.Parameters.Add(new SqlParameter("@TrangThaiPhoiId", model.TrangThaiPhoiId));
                            command.Parameters.Add(new SqlParameter("@MoTaTrangThai", string.IsNullOrEmpty(model.MoTaTrangThai) ? "" : model.MoTaTrangThai));
                            command.Parameters.Add(new SqlParameter("@Id", model.Id));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    var objLog = new LogPhoi();
                    var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã cập nhật phôi có số hiệu " + model.SoHieu;
                    objLog.PhoiId = model.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                    return new ResultModel(true, "Cập nhật thông tin phôi thành công");
                }
                else // phoi ban sao
                {
                    string sqlString_2 = @"Update Phoi
                                            Set TrangThaiPhoiId = @TrangThaiPhoiId, MoTaTrangThai = @MoTaTrangThai
                                            Where Id = @Id";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@TrangThaiPhoiId", model.TrangThaiPhoiId));
                            command.Parameters.Add(new SqlParameter("@MoTaTrangThai", string.IsNullOrEmpty(model.MoTaTrangThai) ? "" : model.MoTaTrangThai));
                            command.Parameters.Add(new SqlParameter("@Id", model.Id));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    var objLog = new LogPhoi();
                    var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã cập nhật phôi bản sao ";
                    objLog.PhoiId = model.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                    return new ResultModel(true, "Cập nhật thông tin phôi thành công");
                }
            }
            catch (Exception e)
            {
                return new ResultModel(true, e.Message);
            }
        }
        public void UpdatePhoi(PhoiViewModel phoiViewModel, int donViId)
        {
            try
            {
                string sqlString = @"Select * From [Phoi] Where (Id = @Id) and (DonViId = @DonViId) and (IsDeleted = 0)";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@Id", phoiViewModel.Id));
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                PhoiVanBang phoiEntity = DbContext.Phois.FromSqlRaw(sqlString, sqlParameters.ToArray()).FirstOrDefault();
                if (phoiEntity != null)
                {
                    phoiEntity.TrangThaiPhoiId = phoiViewModel.TrangThaiPhoiId;
                    phoiEntity.MoTaTrangThai = phoiViewModel.MoTa;
                    DbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddAnhPhoi(List<AttachFileViewModel> attachFileViewModels, int donViId)
        {
            try
            {
                foreach (AttachFileViewModel attachFileViewModel in attachFileViewModels)
                {
                    AnhPhoi anh = new AnhPhoi();
                    anh.Id = Guid.NewGuid().ToString();
                    anh.Url = "Upload/AnhPhoi/" + anh.Id + "." + attachFileViewModel.Extension;
                    anh.ObjectId = attachFileViewModel.ObjectId;
                    anh.NguoiTao = attachFileViewModel.NguoiTao;
                    anh.NgayTao = attachFileViewModel.NgayTao;
                    anh.NguoiCapNhat = attachFileViewModel.NguoiCapNhat;
                    anh.NgayCapNhat = attachFileViewModel.NgayCapNhat;
                    anh.DonViId = donViId;
                    anh.IsDeleted = false;
                    DbContext.AnhPhois.Add(anh);
                    DbContext.SaveChanges();
                    Base64Helper.ConvertBase64ToFileAndSave(attachFileViewModel.Base64String, anh.Id + "." + attachFileViewModel.Extension, "AnhPhoi");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteAnhPhoi(List<AttachFileViewModel> attachFileViewModels, int donViId)
        {
            try
            {
                int i = 0;
                string[] paraIds = attachFileViewModels.Select(x => "@Id_" + i++).ToArray();
                string sqlString = string.Format(@"Select * From [dbo].[AnhPhoi]
                                        Where ([DonViId] = @DonViId) and ([IsDeleted] = 0) and ([Id] In ({0}))", string.Join(",", paraIds));
                i = 0;
                List<SqlParameter> sqlParameters = attachFileViewModels.Select(x => new SqlParameter("@Id_" + i++, x.Id)).ToList();
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                List<AnhPhoi> anhPhois = DbContext.AnhPhois.FromSqlRaw(sqlString, sqlParameters.ToArray()).ToList();

                foreach (AnhPhoi anhPhoi in anhPhois)
                {
                    anhPhoi.IsDeleted = true;
                    anhPhoi.NgayCapNhat = attachFileViewModels.Where(x => x.Id == anhPhoi.Id).FirstOrDefault().NgayCapNhat;
                    anhPhoi.NguoiCapNhat = attachFileViewModels.Where(x => x.Id == anhPhoi.Id).FirstOrDefault().NguoiCapNhat;
                    DbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public PhoiViewModel GetPhoi(int phoiId, int donViId)
        {
            try
            {
                string sqlString = @"Select a.*, b.Ten as 'TrangThaiPhoi' From Phoi as a
                                Left Join TrangThaiPhoi as b
                                on a.TrangThaiPhoiId = b.Id
                                Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and (a.Id = @PhoiId)";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters.Add(new SqlParameter("@PhoiId", phoiId));
                PhoiViewModel phoiViewModel = new PhoiViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        foreach (var parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            phoiViewModel = MapDataHelper<PhoiViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (phoiViewModel != null)
                {
                    string sqlString_1 = @"Select * From [dbo].[AnhPhoi] Where ([ObjectId] = @ObjectId) and ([DonViId] = @DonViId) and ([IsDeleted] = 0)";
                    List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                    sqlParameters_1.Add(new SqlParameter("@ObjectId", phoiId));
                    sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));
                    ;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            foreach (var parameter in sqlParameters_1)
                            {
                                command.Parameters.Add(parameter);
                            }
                            using (var reader = command.ExecuteReader())
                            {
                                phoiViewModel.AnhPhois = MapDataHelper<AttachFileViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                }
                return phoiViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region dashboard phoi trong loai bang
        public PhoisWithPaginationViewModel GetPhoisTrongLoaiBang(int loaiBangId, int trangThaiPhoiId, int donViId, int currentPage)
        {
            try
            {
                PhoisWithPaginationViewModel phoisWithPaginationViewModel = new PhoisWithPaginationViewModel();
                string sqlString = @"Select a.*, b.Ten as 'TrangThaiPhoi' From Phoi as a
                                    Left Join TrangThaiPhoi as b
                                    on a.TrangThaiPhoiId = b.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and (a.LoaiBangId = @LoaiBangId) 
                                            and ((-1 = @TrangThaiPhoiId) or (a.TrangThaiPhoiId = @TrangThaiPhoiId))
                                    Order By a.NgayCapNhat Desc
                                    OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters.Add(new SqlParameter("@TrangThaiPhoiId", trangThaiPhoiId));
                sqlParameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                sqlParameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                sqlParameters.Add(new SqlParameter("@Next", 12));
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        foreach (var parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            phoisWithPaginationViewModel.Phois = MapDataHelper<PhoiViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                phoisWithPaginationViewModel.CurrentPage = currentPage; ;

                string sqlString_1 = @"Select Count(*) as 'TotalRow' From Phoi as a
                                    Left Join TrangThaiPhoi as b
                                    on a.TrangThaiPhoiId = b.Id
                                    Where (IsDeleted = 0) and (DonViId = @DonViId) and (LoaiBangId = @LoaiBangId) 
                                            and ((-1 = @TrangThaiPhoiId) or (a.TrangThaiPhoiId = @TrangThaiPhoiId))";
                List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters_1.Add(new SqlParameter("@TrangThaiPhoiId", trangThaiPhoiId));
                sqlParameters_1.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        foreach (var parameter in sqlParameters_1)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            totalRow = MapDataHelper<int>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                phoisWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                return phoisWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModel> GetTongSoPhoiTungTrangThaiPhoiTrongLoaiBang(int loaiBangId, int donViId)
        {
            try
            {
                List<TongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModel> tongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModels = new List<TongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModel>();
                string sqlString = @"Select b.Id, b.Ten as 'TrangThaiPhoi', Count(TrangThaiPhoiId) as 'TongSoPhoi' 
                                    From (Select * From Phoi Where LoaiBangId = @LoaiBangId and DonViId = @DonViId) as a
                                    Right Join TrangThaiPhoi as b
                                    on a.TrangThaiPhoiId = b.Id
                                    Group By b.Id, b.Ten";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        foreach (var parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            tongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModels = MapDataHelper<TongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return tongSoPhoiTungTrangThaiPhoiTrongLoaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SoPhoiNhanTheoTungThangViewModel> GetSoPhoiNhanTheoTungThang(int loaiBangId, int year, int month, int donViId)
        {
            try
            {
                List<SoPhoiNhanTheoTungThangViewModel> soPhoiNhanTheoTungThangViewModels = new List<SoPhoiNhanTheoTungThangViewModel>();
                string sqlString = @"Select MONTH(NgayTao) 'Thang', Count(*) as 'TongSoPhoiNhan' From Phoi 
                                    Where Year(NgayTao) = @Year and ( (-1 = @Month) or ( MONTH(NgayTao) = @Month)) and (IsDeleted = 0) and (DonViId = @DonViId) and (LoaiBangId = @LoaiBangId)
                                    Group By MONTH(NgayTao)";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@Year", year));
                sqlParameters.Add(new SqlParameter("@Month", month));
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        foreach (var parameter in sqlParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            soPhoiNhanTheoTungThangViewModels = MapDataHelper<SoPhoiNhanTheoTungThangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return soPhoiNhanTheoTungThangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        public void CapNhatTrangThaiDanhSachPhoiDeIn(int loaiBangId, int soLuongPhoi, int donViId, Guid nguoiDungId)
        {
            try
            {
                string sqlString = @"Select * From Phoi
                                     Where (LoaiBangId = @LoaiBangId) and (IsDeleted = 0) and (DonViId = @DonViId) and (TrangThaiPhoiId = 1) and ()";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters.Add(new SqlParameter("@SoLuongPhoi", soLuongPhoi));

                foreach (PhoiVanBang phoi in DbContext.Phois.FromSqlRaw(sqlString, sqlParameters.ToArray()).ToList())
                {
                    phoi.TrangThaiPhoiId = 2;
                }
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<AttachFileViewModel> GetAnhPhois(int phoiId, int donViId)
        {
            try
            {
                List<AttachFileViewModel> attachFileViewModels = new List<AttachFileViewModel>();
                string sqlString_1 = @"Select * From [dbo].[AnhPhoi] Where ([ObjectId] = @ObjectId) and ([DonViId] = @DonViId) and ([IsDeleted] = 0)";
                List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                sqlParameters_1.Add(new SqlParameter("@ObjectId", phoiId));
                sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        foreach (var parameter in sqlParameters_1)
                        {
                            command.Parameters.Add(parameter);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            attachFileViewModels = MapDataHelper<AttachFileViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return attachFileViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListPagingViewModel GetAllPhoiVanBang(string soHieu, int? trangThaiPhoiId, int? loaiBangId, int? nam, int pageNum, int pageSize, int DonViId)
        {
            try
            {
                ListPagingViewModel listPagingViewModel = new ListPagingViewModel();
                var sqlString = @"select p.*, l.Ten as TenLoaiBang, t.Ten as TenTrangThai, t.MaMau, t.Border, t.MauChu ,
                                    	 Case
		                                    When p.DonViId = @DonViId Then 1
		                                    Else 0
	                                    End as 'ChinhSua',
								        Case 
									        When l.LoaiBangGocId is null Then 0
									        Else 1
								        End as 'IsBanSao'
                            from Phoi as p 
                            inner join LoaiBang as l on p.LoaiBangId = l.Id 
                            inner join TrangThaiPhoi as t on p.TrangThaiPhoiId = t.Id 
                            where (p.DonViId = @DonViId) and ((@TrangThaiPhoiId is null) or (p.TrangThaiPhoiId = @TrangThaiPhoiId)) 
                                and ((@LoaiBangId is null) or (p.LoaiBangId = @LoaiBangId)) 
                                and ((@Nam is null) or (Year(p.NgayTao) = @Nam)) 
                                and (p.SoHieu Like '%' + @SoHieu + '%') 
                            Order by p.Id desc 
                            Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", DonViId));
                        command.Parameters.Add(new SqlParameter("@TrangThaiPhoiId", trangThaiPhoiId.HasValue ? trangThaiPhoiId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@SoHieu", string.IsNullOrEmpty(soHieu) ? "" : soHieu));
                        command.Parameters.Add(new SqlParameter("@Offset", (pageNum - 1) * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));
                        using (var reader = command.ExecuteReader())
                        {
                            listPagingViewModel.listOfObj = MapDataHelper<PhoiVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                var sqlString_1 = @"select Count(*) as 'TotalRow' from Phoi as p 
                            inner join LoaiBang as l on p.LoaiBangId = l.Id 
                            inner join TrangThaiPhoi as t on p.TrangThaiPhoiId = t.Id 
                            where (p.DonViId = @DonViId) and ((@TrangThaiPhoiId is null) or (p.TrangThaiPhoiId = @TrangThaiPhoiId)) and ((@LoaiBangId is null) or (p.LoaiBangId = @LoaiBangId))
									and ((@Nam is null) or (Year(p.NgayTao) = @Nam)) and (p.SoHieu Like '%' + @SoHieu + '%')";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", DonViId));
                        command.Parameters.Add(new SqlParameter("@TrangThaiPhoiId", trangThaiPhoiId.HasValue ? trangThaiPhoiId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@SoHieu", string.IsNullOrEmpty(soHieu) ? "" : soHieu));
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
                listPagingViewModel.numberOfPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                listPagingViewModel.Total = totalRow;
                return listPagingViewModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public PaginationViewModel GetByNhapPhoiId(int Id, int donViId, PaginationViewModel paginationViewModel)
        {
            try
            {
                PaginationViewModel result = new PaginationViewModel();
                var sql = @"select p.*, l.Ten as TenLoaiBang, t.Ten as TenTrangThai, t.MaMau, t.Border, t.MauChu, n.HoTen as TenNguoiNhap,
                                Case
		                            When p.DonViId = @DonViId Then 1
		                            Else 0
	                            End as 'ChinhSua',
								Case 
									When l.LoaiBangGocId is null Then 0
									Else 1
								End as 'IsBanSao'
                            from Phoi as p 
                            inner join LoaiBang as l on p.LoaiBangId = l.Id 
                            inner join TrangThaiPhoi as t on p.TrangThaiPhoiId = t.Id inner join NguoiDung as n 
                            on p.NguoiTao = n.NguoiDungId 
                            Left Join NhapPhoi_Phois as np
                            on np.PhoiId = p.Id
                            where (np.NhapPhoiId = @NhapPhoiId) or (p.NhapPhoiId = @NhapPhoiId)
                            Order by p.Id DESC
                            Offset @Offset Rows Fetch Next @Next Rows Only";
                var phois = new List<PhoiVanBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    try
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhapPhoiId", Id));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (paginationViewModel.CurrentPage - 1) * paginationViewModel.PageSize));
                        command.Parameters.Add(new SqlParameter("@Next", paginationViewModel.PageSize));

                        using (var reader = command.ExecuteReader())
                        {
                            phois = MapDataHelper<PhoiVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                foreach (PhoiVanBangViewModel phoi in phois)
                {
                    var sql_1 = @"Select * From [FileDinhKemPhois] Where PhoiId = @PhoiId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sql_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@PhoiId", phoi.Id));
                            using (var reader = command.ExecuteReader())
                            {
                                phoi.FileDinhKemPhois = MapDataHelper<PhoiFileDinhKemViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get totalPage
                var sql_2 = @"select Count(*) as 'TotalRow' from Phoi as p 
                            inner join LoaiBang as l on p.LoaiBangId = l.Id 
                            inner join TrangThaiPhoi as t on p.TrangThaiPhoiId = t.Id inner join NguoiDung as n 
                            on p.NguoiTao = n.NguoiDungId 
                            Left Join NhapPhoi_Phois as np
                            on np.PhoiId = p.Id
                            where (np.NhapPhoiId = @NhapPhoiId) or (p.NhapPhoiId = @NhapPhoiId)";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    try
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        command.CommandText = sql_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhapPhoiId", Id));

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
                result.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / paginationViewModel.PageSize.Value));
                result.Data = phois;
                result.CurrentPage = paginationViewModel.CurrentPage;
                result.PageSize = paginationViewModel.PageSize;
                result.Count = totalRow;
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region yeu cau mua phoi tu cap duoi
        public ThongSoPhoiNamCuViewModel ThongSoPhoiNamCu(int nam, int loaiBangId, int donViId)
        {
            try
            {
                ThongSoPhoiNamCuViewModel thongSoPhoiNamCuViewModel = new ThongSoPhoiNamCuViewModel();
                thongSoPhoiNamCuViewModel.Nam = nam;
                thongSoPhoiNamCuViewModel.NienKhoa = (nam - 1) + " - " + nam;
                // get code capdonvi
                string sqlString_6 = @"Select CodeCapDonVi, Ten From LoaiBang Where Id = @Id";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_6;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Id", loaiBangId));

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.CodeCapDonVi = Convert.ToString(reader["CodeCapDonVi"]);
                                thongSoPhoiNamCuViewModel.TenLoaiBang = Convert.ToString(reader["Ten"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // so phoi cap nam
                string sqlString = @"Select Count(*) as 'TotalRow' From Phoi Where (Year(NgayTao) = @Nam) and (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.SoPhoiDuocCap = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // so phoi con ton tu nam tuoc
                string sqlString_1 = @"Select Count(*) as 'TotalRow' From Phoi 
                                        Where (TrangThaiPhoiId = 1) and (Year(NgayTao) < @Nam) and (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_1;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.SoPhoiConTonTuNamTruoc = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // so phoi da hoan thien va da cap
                string sqlString_2 = @"Select Count(*) as 'TotalRow' from Phoi 
                                        Where (TrangThaiPhoiId = 2) and (Year(NgayTao) = @Nam) and (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_2;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.SoPhoiDaHoanThienVaDaCap = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // so phoi hong
                string sqlString_3 = @"Select Count(*) as 'TotalRow' from Phoi 
                                        Where ((TrangThaiPhoiId = 3) or (TrangThaiPhoiId = 4)) and (Year(NgayTao) = @Nam) and (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_3;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.SoPhoiHong = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                // Tong so phoi con
                string sqlString_5 = @"Select Count(*) as 'TotalRow' 
                                        From Phoi Where (TrangThaiPhoiId = 1) and (Year(NgayTao) <= @Nam) and (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_5;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoPhoiNamCuViewModel.TongSoPhoiCon = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return thongSoPhoiNamCuViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ThongSoDeNghiCapPhoiBangViewModel ThongSoDeNghiCapPhoiBang(int nam, int loaiBangId, int donViId)
        {
            try
            {
                ThongSoDeNghiCapPhoiBangViewModel thongSoDeNghiCapPhoiBangViewModel = new ThongSoDeNghiCapPhoiBangViewModel();
                thongSoDeNghiCapPhoiBangViewModel.Nam = nam;
                thongSoDeNghiCapPhoiBangViewModel.NienKhoa = (nam - 1) + " - " + nam;

                // get code capdonvi cua loai bang
                string sqlString_1 = @"Select CodeCapDonVi, Ten From LoaiBang Where Id = @Id";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString_1;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Id", loaiBangId));

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoDeNghiCapPhoiBangViewModel.CodeCapDonVi = Convert.ToString(reader["CodeCapDonVi"]);
                                thongSoDeNghiCapPhoiBangViewModel.TenLoaiBang = Convert.ToString(reader["Ten"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // so phoi cap nam
                string sqlString = @"Select Count(*) as 'TotalRow'
                                    From Bang
                                    where (TrangThaiBangId <= 3) and (DonViId = @DonViId) and (NamTotNghiep = @Nam) and (LoaiBangId = @LoaiBangId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    command.CommandText = sqlString;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@Nam", nam));
                    //command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                    command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongSoDeNghiCapPhoiBangViewModel.SoHocSinhTotNghiep = Convert.ToInt32(reader["TotalRow"]);
                                thongSoDeNghiCapPhoiBangViewModel.CapDuTheoSoHSTotNghiep = thongSoDeNghiCapPhoiBangViewModel.SoHocSinhTotNghiep;
                                thongSoDeNghiCapPhoiBangViewModel.CapDuPhong = 0;
                                thongSoDeNghiCapPhoiBangViewModel.TongSo = thongSoDeNghiCapPhoiBangViewModel.SoHocSinhTotNghiep;
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return thongSoDeNghiCapPhoiBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public ResultModel InsertFile(List<PhoiFileDinhKemViewModel> model, string ip)
        {
            try
            {
                if (model.Count() == 0)
                {
                    return new ResultModel(false, "Chưa upload bất cứ file nào");
                }
                int count = model.Count();
                var lst = new List<FileDinhKemPhoi>();
                foreach (var item in model)
                {
                    var obj = new FileDinhKemPhoi();
                    obj.FileId = item.FileId;
                    obj.PhoiId = item.PhoiId;
                    obj.Url = item.Url;
                    obj.TenFile = item.TenFile;
                    obj.NguoiTao = item.NguoiTao;
                    obj.IconFile = item.IconFile;
                    obj.Ext = item.Ext;
                    obj.NgayTao = item.NgayTao;
                    obj.DonViId = item.DonViId;
                    lst.Add(obj);
                }
                DbContext.FileDinhKemPhois.AddRange(lst);
                DbContext.SaveChanges();
                var objLog = new LogPhoi();
                var user = new NguoiDungProvider().GetById(model.FirstOrDefault().NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm " + count + " file trong phôi ";
                objLog.PhoiId = model.FirstOrDefault().PhoiId;
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

        public ResultModel DeleteFile(PhoiFileDinhKemViewModel model, string ip)
        {
            try
            {
                var obj = DbContext.FileDinhKemPhois.FirstOrDefault(f => f.FileId == model.FileId);
                DbContext.FileDinhKemPhois.Remove(obj);
                DbContext.SaveChanges();

                var objLog = new LogPhoi();
                var user = new NguoiDungProvider().GetById(model.NguoiTao.Value);
                objLog.NguoiDungId = user.NguoiTao;
                objLog.HanhDong = "Đã xóa file: " + model.TenFile;
                objLog.PhoiId = model.PhoiId;
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

        public ResultModel InsertLog(LogPhoi model)
        {
            try
            {
                DbContext.LogPhois.Add(model);
                DbContext.SaveChanges();
                return new ResultModel(true, "Log success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }

        public GetThongSoPhoiTheoNamVaLoaiBangViewModel GetThongSoPhoiTheoNamVaLoaiBang(int? loaiBangId, int? nam, int donViId)
        {
            try
            {
                GetThongSoPhoiTheoNamVaLoaiBangViewModel getThongSoPhoiTheoNamVaLoaiBangViewModel = new GetThongSoPhoiTheoNamVaLoaiBangViewModel();
                getThongSoPhoiTheoNamVaLoaiBangViewModel.LoaiBangId = loaiBangId;
                getThongSoPhoiTheoNamVaLoaiBangViewModel.Nam = nam;

                // Get tong so phoi da nhap
                string sqlString = @"Select Count(*) as 'TotalRow' From Phoi 
                                    Where (DonViId = @DonViId) and ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) and ((@NgayTao is null) or (Year(NgayTao) = @NgayTao))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NgayTao", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                getThongSoPhoiTheoNamVaLoaiBangViewModel.TongSoPhoiDaNhap = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                // Get tong so phoi da in
                string sqlString_1 = @"Select Count(*) as 'TotalRow' From Phoi 
                                    Where (TrangThaiPhoiId = 2) and (DonViId = @DonViId) and ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) and ((@NgayTao is null) or (Year(NgayTao) = @NgayTao))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NgayTao", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                getThongSoPhoiTheoNamVaLoaiBangViewModel.TongSoPhoiDaIn = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // Get tong so phoi con lai
                string sqlString_2 = @"Select Count(*) as 'TotalRow' From Phoi 
                                    Where (TrangThaiPhoiId = 1) and (DonViId = @DonViId) and ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) and ((@NgayTao is null) or (Year(NgayTao) = @NgayTao))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NgayTao", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                getThongSoPhoiTheoNamVaLoaiBangViewModel.TongSoPhoiConLai = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // Get tong so phoi loi
                string sqlString_3 = @"Select Count(*) as 'TotalRow' From Phoi 
                                    Where ((TrangThaiPhoiId = 3) or (TrangThaiPhoiId = 4)) and (DonViId = @DonViId) and ((@LoaiBangId is null) or (LoaiBangId = @LoaiBangId)) and ((@NgayTao is null) or (Year(NgayTao) = @NgayTao))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NgayTao", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                getThongSoPhoiTheoNamVaLoaiBangViewModel.TongSoPhoiLoi = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return getThongSoPhoiTheoNamVaLoaiBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
