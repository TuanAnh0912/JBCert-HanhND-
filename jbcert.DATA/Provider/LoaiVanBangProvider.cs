using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace jbcert.DATA.Providers
{
    public class LoaiVanBangProvider : ApplicationDbContext, ILoaiVanBang
    {
        #region truong du lieu

        public void AddTruongDuLieu(TruongDuLieuViewModel truongDuLieu)
        {
            try
            {
                TruongDuLieu entity = new TruongDuLieu();
                entity.Code = truongDuLieu.Code;
                entity.Ten = truongDuLieu.Ten;
                entity.NgayTao = truongDuLieu.NgayTao;
                entity.NguoiTao = truongDuLieu.NguoiTao;
                entity.NgayCapNhat = truongDuLieu.NgayCapNhat;
                entity.NguoiCapNhat = truongDuLieu.NguoiCapNhat;
                entity.KieuDuLieu = truongDuLieu.KieuDuLieu;
                entity.DonViId = truongDuLieu.DonViId.Value;
                entity.IsChungChi = truongDuLieu.IsChungChi;
                entity.IsDeleted = false;
                DbContext.TruongDuLieus.Add(entity);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTruongDuLieu(TruongDuLieuViewModel truongDuLieu)
        {
            try
            {
                string sqlString = @"SELECT Count(*) as 'TotalRow' FROM [dbo].[ThongTinVanBang] Where (TruongDuLieuCode Like @TruongDuLieuCode)";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == System.Data.ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", truongDuLieu.Code));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow = Convert.ToInt32(reader[0]);
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
                    Exception exception = new Exception("Không thể xóa trường dữ liệu đang được sử dụng");
                    throw exception;
                }


                if (truongDuLieu.Code == truongDuLieu.DonViId + "-SOHIEU")
                {
                    Exception exception = new Exception("Không thể xóa trường số hiệu bản sao");
                    throw exception;
                }

                TruongDuLieu entity = DbContext.TruongDuLieus.Find(truongDuLieu.Code);
                entity.NgayCapNhat = truongDuLieu.NgayCapNhat;
                entity.NguoiCapNhat = truongDuLieu.NguoiCapNhat;
                entity.IsDeleted = truongDuLieu.IsDeleted;
                DbContext.TruongDuLieus.Remove(entity);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TruongDuLieuViewModel GetTruongDuLieu(string code, int donViId)
        {
            try
            {
                TruongDuLieuViewModel truongDuLieu = new TruongDuLieuViewModel();

                string sqlString = @"SELECT * FROM [dbo].[TruongDuLieu] Where (IsDeleted = 0) and (DonViId = @DonViId) and (Code = @Code)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == System.Data.ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Code", code));
                        using (var reader = command.ExecuteReader())
                        {
                            truongDuLieu = MapDataHelper<TruongDuLieuViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return truongDuLieu;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TruongDuLieuViewModel> GetTruongDuLieus(bool? isChungChi, int donViId)
        {
            try
            {
                List<TruongDuLieuViewModel> truongdulieuViewmodels = new List<TruongDuLieuViewModel>();
                string sqlString = @"SELECT * FROM [dbo].[TruongDuLieu] 
                                    Where (IsDeleted = 0) and (DonViId = @DonViId) and ((@IsChungChi is null) or (IsChungChi = @IsChungChi))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == System.Data.ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Code", donViId + "-SOHIEU"));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            truongdulieuViewmodels = MapDataHelper<TruongDuLieuViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return truongdulieuViewmodels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTruongDuLieu(TruongDuLieuViewModel truongDuLieu)
        {
            try
            {
                string sqlString = @"SELECT Count(*) as 'TotalRow' FROM [dbo].[ThongTinVanBang] Where (TruongDuLieuCode Like @TruongDuLieuCode)";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == System.Data.ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", truongDuLieu.Code));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow = Convert.ToInt32(reader[0]);
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
                    Exception exception = new Exception("Không thể update trường dữ liệu đang được sử dụng");
                    throw exception;
                }


                if (truongDuLieu.Code == truongDuLieu.DonViId + "-SOHIEU")
                {
                    Exception exception = new Exception("Không thể chỉnh sửa trường số hiệu bản sao");
                    throw exception;
                }

                TruongDuLieu entity = DbContext.TruongDuLieus.Find(truongDuLieu.Code);
                if (entity != null)
                {
                    entity.Ten = truongDuLieu.Ten;
                    entity.KieuDuLieu = truongDuLieu.KieuDuLieu;
                    entity.NgayCapNhat = truongDuLieu.NgayCapNhat;
                    entity.NguoiCapNhat = truongDuLieu.NguoiCapNhat;
                    DbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion truong du lieu

        #region loai bang

        public void AddLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId)
        {
            try
            {
                LoaiBang loaiBangEntity = new LoaiBang();
                loaiBangEntity.Ten = loaiBangViewModel.Ten;
                loaiBangEntity.HinhThucCapId = 1;
                loaiBangEntity.MaLoaiBang = "";
                loaiBangEntity.MaNoiIn = "";
                loaiBangEntity.ChiSo = 0;
                loaiBangEntity.Width = loaiBangViewModel.Width;
                loaiBangEntity.Height = loaiBangViewModel.Height;
                loaiBangEntity.IsChungChi = loaiBangViewModel.IsChungChi;
                loaiBangEntity.CodeCapDonVi = loaiBangViewModel.CodeCapDonVi;
                loaiBangEntity.NgayTao = loaiBangViewModel.NgayTao;
                loaiBangEntity.NguoiTao = loaiBangViewModel.NguoiTao;
                loaiBangEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                loaiBangEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                loaiBangEntity.DonViId = donViId;
                loaiBangEntity.IsDeleted = false;
                DbContext.LoaiBangs.Add(loaiBangEntity);
                DbContext.SaveChanges();

                LoaiBang loaiBangBanSaoEntity = new LoaiBang();
                loaiBangBanSaoEntity.Ten = loaiBangViewModel.Ten + " Bản sao";
                loaiBangBanSaoEntity.HinhThucCapId = 2;
                loaiBangBanSaoEntity.MaLoaiBang = "";
                loaiBangBanSaoEntity.MaNoiIn = "";
                loaiBangBanSaoEntity.ChiSo = 0;
                loaiBangBanSaoEntity.Width = loaiBangViewModel.Width;
                loaiBangBanSaoEntity.Height = loaiBangViewModel.Height;
                loaiBangBanSaoEntity.IsChungChi = loaiBangViewModel.IsChungChi;
                loaiBangBanSaoEntity.CodeCapDonVi = loaiBangViewModel.CodeCapDonVi;
                loaiBangBanSaoEntity.NgayTao = loaiBangViewModel.NgayTao;
                loaiBangBanSaoEntity.NguoiTao = loaiBangViewModel.NguoiTao;
                loaiBangBanSaoEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                loaiBangBanSaoEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                loaiBangBanSaoEntity.DonViId = donViId;
                loaiBangBanSaoEntity.LoaiBangGocId = loaiBangEntity.Id;
                loaiBangBanSaoEntity.IsDeleted = false;
                DbContext.LoaiBangs.Add(loaiBangBanSaoEntity);
                DbContext.SaveChanges();

                //List<TruongDuLieuLoaiBang> truongDuLieuLoaiBangEntities = new List<TruongDuLieuLoaiBang>();
                //foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBang in loaiBangViewModel.TruongDuLieuLoaiBangs)
                //{
                //    TruongDuLieuLoaiBang truongDuLieuLoaiEntity = new TruongDuLieuLoaiBang();
                //    truongDuLieuLoaiEntity.LoaiBangId = loaiBangEntity.Id;
                //    truongDuLieuLoaiEntity.TruongDuLieuCode = truongDuLieuLoaiBang.TruongDuLieuCode;
                //    truongDuLieuLoaiEntity.X = truongDuLieuLoaiBang.X;
                //    truongDuLieuLoaiEntity.Y = truongDuLieuLoaiBang.Y;
                //    truongDuLieuLoaiEntity.Format = truongDuLieuLoaiBang.Format;
                //    truongDuLieuLoaiEntity.Font = truongDuLieuLoaiBang.Font;
                //    truongDuLieuLoaiEntity.Color = truongDuLieuLoaiBang.Color;
                //    truongDuLieuLoaiEntity.Bold = truongDuLieuLoaiBang.Bold;
                //    truongDuLieuLoaiEntity.Italic = truongDuLieuLoaiBang.Italic;
                //    truongDuLieuLoaiEntity.Underline = truongDuLieuLoaiBang.Underline;
                //    truongDuLieuLoaiEntity.DungChung = truongDuLieuLoaiBang.DungChung;
                //    truongDuLieuLoaiEntity.Size = truongDuLieuLoaiBang.Size;
                //    truongDuLieuLoaiEntity.NgayTao = loaiBangViewModel.NgayTao;
                //    truongDuLieuLoaiEntity.NguoiTao = loaiBangViewModel.NguoiTao;
                //    truongDuLieuLoaiEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                //    truongDuLieuLoaiEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                //    truongDuLieuLoaiEntity.TenTruongDuLieu = truongDuLieuLoaiBang.TenTruongDuLieu;
                //    truongDuLieuLoaiEntity.KieuDuLieu = truongDuLieuLoaiBang.KieuDuLieu;
                //    if (truongDuLieuLoaiBang.KieuDuLieu == 2)
                //    {
                //        truongDuLieuLoaiEntity.Width = truongDuLieuLoaiBang.Width.Value;
                //        truongDuLieuLoaiEntity.Height = truongDuLieuLoaiBang.Height.Value;
                //    }
                //    truongDuLieuLoaiEntity.IsDeleted = false;
                //    truongDuLieuLoaiEntity.DonViId = donViId;
                //    truongDuLieuLoaiBangEntities.Add(truongDuLieuLoaiEntity);
                //}

                //DbContext.TruongDuLieuLoaiBangs.AddRange(truongDuLieuLoaiBangEntities);
                //DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId)
        {
            try
            {
                if (!CanUpdateLoaiBang(loaiBangViewModel.Id.Value))
                {
                    Exception exception = new Exception("Loại bằng này đang được sử dụng, không thể cập nhật hay xóa!");
                    throw exception;
                }

                string sqlString_2 = @"Select Count(*) as 'TotalRow' From Bang Where LoaiBangId = @LoaiBangId";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangViewModel.Id));
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
                if (totalRow > 0)
                {
                    Exception exception = new Exception("Loại bằng này đang được sử dụng, không thể cập nhật");
                    throw exception;
                }


                string sqlString = @"Select * From [dbo].[LoaiBang]
                                      Where [Id] = @Id and IsDeleted = 0";
                //DbContext.Database.GetDbConnection().Open();
                //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                //{
                //    command.CommandText = sqlString;
                //    command.CommandType = System.Data.CommandType.Text;
                //    command.Parameters.Add(new SqlParameter("@Id", loaiBangViewModel.Id));
                //    using(var reader= command.ExecuteReader())
                //    {
                //        LoaiBang loaiBang = MapDataHelper<LoaiBang>.Map(reader);
                //    }
                //}
                LoaiBang loaiBangEntity = DbContext.LoaiBangs.FromSqlRaw(sqlString, new SqlParameter("@Id", loaiBangViewModel.Id), new SqlParameter("@DonViId", donViId)).FirstOrDefault();
                loaiBangEntity.Ten = loaiBangViewModel.Ten;
                //loaiBangEntity.HinhThucCapId = loaiBangViewModel.HinhThucCapId;
                loaiBangEntity.CodeCapDonVi = loaiBangViewModel.CodeCapDonVi;
                loaiBangEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                loaiBangEntity.Width = loaiBangViewModel.Width;
                loaiBangEntity.IsChungChi = loaiBangViewModel.IsChungChi;
                loaiBangEntity.Height = loaiBangViewModel.Height;
                loaiBangEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                loaiBangEntity.DonViId = donViId;
                DbContext.SaveChanges();


                string sqlString_1 = @"Select * From [dbo].[LoaiBang]
                                      Where [LoaiBangGocId] = @Id and IsDeleted = 0";
                LoaiBang loaiBangBanSaoEntity = DbContext.LoaiBangs.FromSqlRaw(sqlString_1, new SqlParameter("@Id", loaiBangViewModel.Id), new SqlParameter("@DonViId", donViId)).FirstOrDefault();
                if (loaiBangBanSaoEntity != null)
                {
                    loaiBangBanSaoEntity.CodeCapDonVi = loaiBangViewModel.CodeCapDonVi;
                    loaiBangBanSaoEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                    loaiBangBanSaoEntity.IsChungChi = loaiBangViewModel.IsChungChi;
                    loaiBangBanSaoEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                    loaiBangBanSaoEntity.DonViId = donViId;
                    DbContext.SaveChanges();
                }

                //string sqlString_1 = @"Delete [dbo].[TruongDuLieuLoaiBang] Where [LoaiBangId] = @LoaiBangId";
                //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                //{
                //    bool wasOpen = command.Connection.State == ConnectionState.Open;
                //    if (!wasOpen) command.Connection.Open();

                //    try
                //    {
                //        command.CommandText = sqlString_1;
                //        command.CommandType = CommandType.Text;
                //        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangViewModel.Id));
                //        command.ExecuteNonQuery();
                //    }
                //    finally
                //    {
                //        command.Connection.Close();
                //    }
                //}

                //List<TruongDuLieuLoaiBang> truongDuLieuLoaiBangEntities = new List<TruongDuLieuLoaiBang>();
                //foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBang in loaiBangViewModel.TruongDuLieuLoaiBangs)
                //{
                //    TruongDuLieuLoaiBang truongDuLieuLoaiEntity = new TruongDuLieuLoaiBang();
                //    truongDuLieuLoaiEntity.LoaiBangId = loaiBangEntity.Id;
                //    truongDuLieuLoaiEntity.TruongDuLieuCode = truongDuLieuLoaiBang.TruongDuLieuCode;
                //    truongDuLieuLoaiEntity.X = truongDuLieuLoaiBang.X;
                //    truongDuLieuLoaiEntity.Y = truongDuLieuLoaiBang.Y;
                //    truongDuLieuLoaiEntity.Format = truongDuLieuLoaiBang.Format;
                //    truongDuLieuLoaiEntity.Font = truongDuLieuLoaiBang.Font;
                //    truongDuLieuLoaiEntity.Color = truongDuLieuLoaiBang.Color;
                //    truongDuLieuLoaiEntity.Bold = truongDuLieuLoaiBang.Bold;
                //    truongDuLieuLoaiEntity.Italic = truongDuLieuLoaiBang.Italic;
                //    truongDuLieuLoaiEntity.Underline = truongDuLieuLoaiBang.Underline;
                //    truongDuLieuLoaiEntity.DungChung = truongDuLieuLoaiBang.DungChung;
                //    truongDuLieuLoaiEntity.Size = truongDuLieuLoaiBang.Size;
                //    truongDuLieuLoaiEntity.KieuDuLieu = truongDuLieuLoaiBang.KieuDuLieu;
                //    if (truongDuLieuLoaiBang.KieuDuLieu == 2)
                //    {
                //        truongDuLieuLoaiEntity.Width = truongDuLieuLoaiBang.Width.Value;
                //        truongDuLieuLoaiEntity.Height = truongDuLieuLoaiBang.Height.Value;
                //    }
                //    truongDuLieuLoaiEntity.NgayTao = loaiBangViewModel.NgayTao;
                //    truongDuLieuLoaiEntity.NguoiTao = loaiBangViewModel.NguoiTao;
                //    truongDuLieuLoaiEntity.NgayCapNhat = loaiBangViewModel.NgayCapNhat;
                //    truongDuLieuLoaiEntity.NguoiCapNhat = loaiBangViewModel.NguoiCapNhat;
                //    truongDuLieuLoaiEntity.TenTruongDuLieu = truongDuLieuLoaiBang.TenTruongDuLieu;
                //    truongDuLieuLoaiEntity.IsDeleted = false;
                //    truongDuLieuLoaiEntity.DonViId = donViId;
                //    truongDuLieuLoaiBangEntities.Add(truongDuLieuLoaiEntity);
                //}

                //DbContext.TruongDuLieuLoaiBangs.AddRange(truongDuLieuLoaiBangEntities);
                //DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoaiBangViewModel GetLoaiBang(int loaiBangId, int donViId)
        {
            try
            {
                string sqlString = @"Select a.*,b.Ten as 'HinhThucCap', c.TenCapDonVi as 'CapDonVi' From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
                                    Left Join CapDonVi as c
                                    on a.CodeCapDonVi = c.Code
                                    Where a.[Id] = @Id and a.IsDeleted = 0";
                LoaiBangViewModel loaiBangViewModel = new LoaiBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBangViewModel = MapDataHelper<LoaiBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                if (loaiBangViewModel != null)
                {
                    string sqlString_1 = @"Select a.*, b.Ten From [dbo].[TruongDuLieuLoaiBang] as a
                                        Left Join [dbo].[TruongDuLieu] as b
                                        on a.TruongDuLieuCode = b.Code
                                        Where (a.[LoaiBangId] = @LoaiBangId) and (b.IsDeleted = 0) and (a.DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                loaiBangViewModel.TruongDuLieuLoaiBangs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return loaiBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteLoaiBang(LoaiBangViewModel loaiBangViewModel, int donViId)
        {
            try
            {

                if (!CanUpdateLoaiBang(loaiBangViewModel.Id.Value))
                {
                    Exception exception = new Exception("Loại bằng này đang được sử dụng, không thể cập nhật hay xóa!");
                    throw exception;
                }

                string sqlString = @"Update [dbo].[LoaiBang]
                                    Set IsDeleted = 1, NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat
                                      Where [Id] = @Id and [DonViId] = @DonViId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", loaiBangViewModel.Id));
                        command.Parameters.Add(new SqlParameter("@DonViId", loaiBangViewModel.DonViId));
                        command.Parameters.Add(new SqlParameter("@NgayCapNhat", loaiBangViewModel.NgayCapNhat));
                        command.Parameters.Add(new SqlParameter("@NguoiCapNhat", loaiBangViewModel.NguoiCapNhat));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoaiBangsWithPaginationViewModel GetLoaiBangs(string ten, int? hinhThucCapId, bool? isChungChi, int currentPage, int donViId)
        {
            try
            {
                LoaiBangsWithPaginationViewModel bangsWithPaginationViewModel = new LoaiBangsWithPaginationViewModel();
                string sqlString = @"Select a.*,b.Ten as 'HinhThucCap', c.TenCapDonVi as 'CapDonVi' From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
									Left Join [CapDonVi] as c
									on a.CodeCapDonVi = c.Code
                                    Where (a.IsDeleted = 0) and (a.Ten like N'%'+@Ten+'%') 
                                            and ( (@HinhThucCapId is null) or (a.HinhThucCapId  = @HinhThucCapId) )
                                            and ( (@IsChungChi is null) or (a.IsChungChi = @IsChungChi))
                                    Order By NgayCapNhat Desc
                                    OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Ten", ten));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HinhThucCapId", hinhThucCapId.HasValue ? hinhThucCapId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));
                        using (var reader = command.ExecuteReader())
                        {
                            bangsWithPaginationViewModel.LoaiBangs = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                string sqlString_1 = @"Select Count(*) 'TotalRow'From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
                                    Where (a.IsDeleted = 0) and (a.Ten like N'%'+@Ten+'%') 
                                         and ( (@HinhThucCapId is null) or (a.HinhThucCapId  = @HinhThucCapId) )
                                         and ( (@IsChungChi is null) or (a.IsChungChi = @IsChungChi))";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Ten", ten));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HinhThucCapId", hinhThucCapId.HasValue ? hinhThucCapId.Value : DBNull.Value));
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
                bangsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                bangsWithPaginationViewModel.CurrentPage = currentPage;

                return bangsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiBangViewModel> GetLoaiBangTheoCapDonVis(string ten, int? hinhThucCapId, int donViId)
        {
            try
            {
                string sqlString = @"Select b.Code From DonVi as a
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
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
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


                List<LoaiBangViewModel> loaiBangViewModels = new List<LoaiBangViewModel>();
                string sqlString_1 = "";
                if (codeCapDonVi == "BOGD" || codeCapDonVi == "SOGD" || codeCapDonVi == "PHONGGD")
                {
                     sqlString_1 = @"Select a.*,b.Ten as 'HinhThucCap', c.TenCapDonVi as 'CapDonVi' 
                                        From [dbo].[LoaiBang] as a
                                        Left Join [dbo].[HinhThucCap] as b
                                        on a.HinhThucCapId = b.Id
                                        Left Join [CapDonVi] as c
                                        on a.CodeCapDonVi = c.Code
                                        Where (a.IsDeleted = 0) and (a.Ten like N'%'+@Ten+'%') and ( (@HinhThucCapId is null) or (a.HinhThucCapId  = @HinhThucCapId) ) and (c.Level > (Select b.Level From DonVi as a
																											                                        Left Join CapDonVi as b
																											                                        on a.CapDonViId = b.CapDonViId
																											                                        Where a.DonViId = @DonViId))";
                }
                else
                {
                     sqlString_1 = @"Select a.*,b.Ten as 'HinhThucCap', c.TenCapDonVi as 'CapDonVi' 
                                        From [dbo].[LoaiBang] as a
                                        Left Join [dbo].[HinhThucCap] as b
                                        on a.HinhThucCapId = b.Id
                                        Left Join [CapDonVi] as c
                                        on a.CodeCapDonVi = c.Code
                                        Where (a.IsDeleted = 0) and (a.Ten like N'%'+@Ten+'%') and ( (@HinhThucCapId is null) or (a.HinhThucCapId  = @HinhThucCapId) ) and (a.CodeCapDonVi = @CodeCapDonVi)";
                }
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Ten", string.IsNullOrEmpty(ten) ? "" : ten));
                        command.Parameters.Add(new SqlParameter("@HinhThucCapId", hinhThucCapId.HasValue ? hinhThucCapId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@CodeCapDonVi", codeCapDonVi));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBangViewModels = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return loaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddAnhLoaiBang(List<LoaiBangFileDinhKemViewModel> model, int donViId)
        {
            try
            {
                if (model.Count != 0)
                {
                    string sqlString_1 = @"Delete FileDinhKemLoaiBangs Where LoaiBangId = @LoaiBangId";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", model.FirstOrDefault().LoaiBangId));
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


                    int count = model.Count();
                    var lst = new List<FileDinhKemLoaiBang>();
                    foreach (var item in model)
                    {
                        var obj = new FileDinhKemLoaiBang();
                        obj.FileId = item.FileId;
                        obj.LoaiBangId = item.LoaiBangId;
                        obj.Url = item.Url;
                        obj.TenFile = item.TenFile;
                        obj.NguoiTao = item.NguoiTao;
                        obj.IconFile = item.IconFile;
                        obj.Ext = item.Ext;
                        obj.NgayTao = item.NgayTao;
                        obj.DonViId = item.DonViId;
                        lst.Add(obj);
                    }
                    DbContext.FileDinhKemLoaiBangs.AddRange(lst);
                    DbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public LoaiBangFileDinhKemViewModel GetAnhLoaiBang(int loaiBangId, int donViId)
        {
            try
            {
                string sqlString = @"Select * From FileDinhKemLoaiBangs Where LoaiBangId = @LoaiBangId ";
                LoaiBangFileDinhKemViewModel loaiBangFileDinhKemViewModel = new LoaiBangFileDinhKemViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBangFileDinhKemViewModel = MapDataHelper<LoaiBangFileDinhKemViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return loaiBangFileDinhKemViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiBangViewModel> GetLoaiBangs()
        {
            try
            {
                string sqlString = @"Select a.*,b.Ten as 'HinhThucCap', c.[Level] From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
									Left Join CapDonVi as c
									on a.CodeCapDonVi = c.Code
                                    Where IsDeleted = 0";
                List<LoaiBangViewModel> lstLoaiBangViewModels = new List<LoaiBangViewModel>();
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
                            lstLoaiBangViewModels = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lstLoaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiBangViewModel> GetLoaiBangs(bool? IsChungChi)
        {
            try
            {
                string sqlString = @"Select a.*,b.Ten as 'HinhThucCap', c.[Level] From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
									Left Join CapDonVi as c
									on a.CodeCapDonVi = c.Code
                                    Where (IsDeleted = 0) and ((@IsChungChi is null) or (IsChungChi = @IsChungChi))";
                List<LoaiBangViewModel> lstLoaiBangViewModels = new List<LoaiBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@IsChungChi", IsChungChi.HasValue ? IsChungChi.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            lstLoaiBangViewModels = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lstLoaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTruongDuLieuLoaiBang(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangs, int donViId)
        {
            try
            {
                if (truongDuLieuLoaiBangs.Count() > 0)
                {
                    //if (!CanUpdateLoaiBang(truongDuLieuLoaiBangs.FirstOrDefault().LoaiBangId))
                    //{
                    //    Exception exception = new Exception("Loại bằng này đang được sử dụng, không thể cập nhật hay xóa!");
                    //    throw exception;
                    //}

                    string sqlString = @"Delete [TruongDuLieuLoaiBang] Where (LoaiBangId = @LoaiBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", truongDuLieuLoaiBangs.FirstOrDefault().LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    List<TruongDuLieuLoaiBang> truongDuLieuLoaiBangs_1 = new List<TruongDuLieuLoaiBang>();
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBang in truongDuLieuLoaiBangs)
                    {
                        TruongDuLieuLoaiBang truongDuLieuLoaiEntity = new TruongDuLieuLoaiBang();
                        truongDuLieuLoaiEntity.LoaiBangId = truongDuLieuLoaiBang.LoaiBangId;
                        truongDuLieuLoaiEntity.TruongDuLieuCode = truongDuLieuLoaiBang.TruongDuLieuCode;
                        truongDuLieuLoaiEntity.X = truongDuLieuLoaiBang.X;
                        truongDuLieuLoaiEntity.Y = truongDuLieuLoaiBang.Y;
                        truongDuLieuLoaiEntity.Format = truongDuLieuLoaiBang.Format;
                        truongDuLieuLoaiEntity.Font = truongDuLieuLoaiBang.Font;
                        truongDuLieuLoaiEntity.Color = truongDuLieuLoaiBang.Color;
                        truongDuLieuLoaiEntity.Bold = truongDuLieuLoaiBang.Bold;
                        truongDuLieuLoaiEntity.Italic = truongDuLieuLoaiBang.Italic;
                        truongDuLieuLoaiEntity.Underline = truongDuLieuLoaiBang.Underline;
                        truongDuLieuLoaiEntity.DungChung = truongDuLieuLoaiBang.DungChung;
                        truongDuLieuLoaiEntity.Size = truongDuLieuLoaiBang.Size;
                        truongDuLieuLoaiEntity.KieuDuLieu = truongDuLieuLoaiBang.KieuDuLieu;
                        if (truongDuLieuLoaiBang.KieuDuLieu == 2)
                        {
                            truongDuLieuLoaiEntity.Width = truongDuLieuLoaiBang.Width.Value;
                            truongDuLieuLoaiEntity.Height = truongDuLieuLoaiBang.Height.Value;
                        }
                        truongDuLieuLoaiEntity.NgayTao = truongDuLieuLoaiBang.NgayTao;
                        truongDuLieuLoaiEntity.NguoiTao = truongDuLieuLoaiBang.NguoiTao;
                        truongDuLieuLoaiEntity.NgayCapNhat = truongDuLieuLoaiBang.NgayCapNhat;
                        truongDuLieuLoaiEntity.NguoiCapNhat = truongDuLieuLoaiBang.NguoiCapNhat;
                        truongDuLieuLoaiEntity.TenTruongDuLieu = truongDuLieuLoaiBang.TenTruongDuLieu;
                        truongDuLieuLoaiEntity.IsDeleted = false;
                        truongDuLieuLoaiEntity.DonViId = donViId;
                        truongDuLieuLoaiBangs_1.Add(truongDuLieuLoaiEntity);
                    }

                    //TruongDuLieuLoaiBang truongDuLieuLoaiEntity_1 = new TruongDuLieuLoaiBang();
                    //truongDuLieuLoaiEntity_1.LoaiBangId = truongDuLieuLoaiBangs.FirstOrDefault().LoaiBangId;
                    //truongDuLieuLoaiEntity_1.TruongDuLieuCode = donViId + "-SOHIEU";
                    //truongDuLieuLoaiEntity_1.TenTruongDuLieu = "Số hiệu";
                    //truongDuLieuLoaiEntity_1.X = 30;
                    //truongDuLieuLoaiEntity_1.Y = 30;
                    //truongDuLieuLoaiEntity_1.Format = "";
                    //truongDuLieuLoaiEntity_1.Font = "Time new Roman";
                    //truongDuLieuLoaiEntity_1.Color = "#000000";
                    //truongDuLieuLoaiEntity_1.Bold = false;
                    //truongDuLieuLoaiEntity_1.Italic = false;
                    //truongDuLieuLoaiEntity_1.Underline = false;
                    //truongDuLieuLoaiEntity_1.DungChung = true;
                    //truongDuLieuLoaiEntity_1.Size = 12;
                    //truongDuLieuLoaiEntity_1.KieuDuLieu = 1;
                    //truongDuLieuLoaiEntity_1.IsDeleted = false;
                    //truongDuLieuLoaiEntity_1.DonViId = donViId;

                    DbContext.TruongDuLieuLoaiBangs.AddRange(truongDuLieuLoaiBangs_1);
                    DbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTruongDuLieuLoaiBangBanSaoTheoBangGoc(int id, int donViId)
        {
            try
            {
                LoaiBangViewModel loaiBangViewModel = new LoaiBangViewModel();
                string sqlString = @"Select * From LoaiBang Where Id = @Id";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", id));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBangViewModel = MapDataHelper<LoaiBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (loaiBangViewModel.Id != 0 && loaiBangViewModel.LoaiBangGocId.HasValue)
                {
                    // lay truong du lieu cua bang goc
                    List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangViewModels = new List<TruongDuLieuLoaiBangViewModel>();
                    string sqlString_1 = @"Select * From [TruongDuLieuLoaiBang] Where (LoaiBangId = @LoaiBangId) and (DonViId = @DonViid)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangViewModel.LoaiBangGocId.Value));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuLoaiBangViewModels = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // xoa truong du lieu bang ban sao
                    string sqlString_2 = @"Delete [TruongDuLieuLoaiBang] Where LoaiBangId = @LoaiBangId and (DonViId = @DonViId) ";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // add truong du lieu ban sao
                    List<TruongDuLieuLoaiBang> truongDuLieuLoaiBangs_1 = new List<TruongDuLieuLoaiBang>();
                    foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBang in truongDuLieuLoaiBangViewModels)
                    {
                        TruongDuLieuLoaiBang truongDuLieuLoaiEntity = new TruongDuLieuLoaiBang();
                        truongDuLieuLoaiEntity.LoaiBangId = loaiBangViewModel.Id.Value;
                        truongDuLieuLoaiEntity.TruongDuLieuCode = truongDuLieuLoaiBang.TruongDuLieuCode;
                        truongDuLieuLoaiEntity.X = truongDuLieuLoaiBang.X;
                        truongDuLieuLoaiEntity.Y = truongDuLieuLoaiBang.Y;
                        truongDuLieuLoaiEntity.Format = truongDuLieuLoaiBang.Format;
                        truongDuLieuLoaiEntity.Font = truongDuLieuLoaiBang.Font;
                        truongDuLieuLoaiEntity.Color = truongDuLieuLoaiBang.Color;
                        truongDuLieuLoaiEntity.Bold = truongDuLieuLoaiBang.Bold;
                        truongDuLieuLoaiEntity.Italic = truongDuLieuLoaiBang.Italic;
                        truongDuLieuLoaiEntity.Underline = truongDuLieuLoaiBang.Underline;
                        truongDuLieuLoaiEntity.DungChung = truongDuLieuLoaiBang.DungChung;
                        truongDuLieuLoaiEntity.Size = truongDuLieuLoaiBang.Size;
                        truongDuLieuLoaiEntity.KieuDuLieu = truongDuLieuLoaiBang.KieuDuLieu;
                        if (truongDuLieuLoaiBang.KieuDuLieu == 2)
                        {
                            truongDuLieuLoaiEntity.Width = truongDuLieuLoaiBang.Width.Value;
                            truongDuLieuLoaiEntity.Height = truongDuLieuLoaiBang.Height.Value;
                        }
                        truongDuLieuLoaiEntity.NgayTao = truongDuLieuLoaiBang.NgayTao;
                        truongDuLieuLoaiEntity.NguoiTao = truongDuLieuLoaiBang.NguoiTao;
                        truongDuLieuLoaiEntity.NgayCapNhat = truongDuLieuLoaiBang.NgayCapNhat;
                        truongDuLieuLoaiEntity.NguoiCapNhat = truongDuLieuLoaiBang.NguoiCapNhat;
                        truongDuLieuLoaiEntity.TenTruongDuLieu = truongDuLieuLoaiBang.TenTruongDuLieu;
                        truongDuLieuLoaiEntity.IsDeleted = false;
                        truongDuLieuLoaiEntity.DonViId = donViId;
                        truongDuLieuLoaiBangs_1.Add(truongDuLieuLoaiEntity);
                    }

                    // xoa truong du lieu bang ban sao
                    string sqlString_3 = @"Select Count(*) as 'TotalRow' From TruongDuLieu Where (Code Like @Code) and (DonViId = @DonViId) and (IsChungChi = @IsChungChi)";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@Code", donViId + "-" + (loaiBangViewModel.IsChungChi.Value ? "1" : "0") + "-SOHIEU"));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@IsChungChi", loaiBangViewModel.IsChungChi.Value));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    totalRow = Convert.ToInt32(reader[0]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // add truong du lieu so hieu cho don vi neu chua ton tai
                    if (totalRow == 0)
                    {
                        TruongDuLieu entity = new TruongDuLieu();
                        entity.Code = donViId + "-" + (loaiBangViewModel.IsChungChi.Value ? "1" : "0") + "-SOHIEU";
                        entity.Ten = "Số hiệu bản sao";
                        entity.KieuDuLieu = 1;
                        entity.DonViId = donViId;
                        entity.IsDeleted = false;
                        entity.IsChungChi = loaiBangViewModel.IsChungChi;
                        DbContext.TruongDuLieus.Add(entity);
                        DbContext.SaveChanges();
                    }

                    TruongDuLieuLoaiBang truongDuLieuLoaiEntity_1 = new TruongDuLieuLoaiBang();
                    truongDuLieuLoaiEntity_1.LoaiBangId = loaiBangViewModel.Id.Value;
                    truongDuLieuLoaiEntity_1.TruongDuLieuCode = donViId + "-" + (loaiBangViewModel.IsChungChi.Value ? "1" : "0") + "-SOHIEU";
                    truongDuLieuLoaiEntity_1.X = 30;
                    truongDuLieuLoaiEntity_1.Y = 30;
                    truongDuLieuLoaiEntity_1.Format = "";
                    truongDuLieuLoaiEntity_1.Font = "Time new Roman";
                    truongDuLieuLoaiEntity_1.Color = "#000000";
                    truongDuLieuLoaiEntity_1.Bold = false;
                    truongDuLieuLoaiEntity_1.Italic = false;
                    truongDuLieuLoaiEntity_1.Underline = false;
                    truongDuLieuLoaiEntity_1.DungChung = true;
                    truongDuLieuLoaiEntity_1.Size = 12;
                    truongDuLieuLoaiEntity_1.KieuDuLieu = 1;
                    truongDuLieuLoaiEntity_1.IsDeleted = false;
                    truongDuLieuLoaiEntity_1.DonViId = donViId;
                    truongDuLieuLoaiBangs_1.Add(truongDuLieuLoaiEntity_1);
                    DbContext.TruongDuLieuLoaiBangs.AddRange(truongDuLieuLoaiBangs_1);
                    DbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateTruongDuLieuLoaiBangBanSao(List<TruongDuLieuLoaiBangViewModel> truongDuLieuLoaiBangViewModels, int donViId)
        {
            try
            {
                foreach (TruongDuLieuLoaiBangViewModel truongDuLieuLoaiBangViewModel in truongDuLieuLoaiBangViewModels)
                {
                    string sqlString_1 = @"Update [TruongDuLieuLoaiBang]
                                           Set X = @X, Y = @Y
                                           Where (LoaiBangId = @LoaiBangId) and (TruongDuLieuCode = @TruongDuLieuCode) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@X", truongDuLieuLoaiBangViewModel.X));
                            command.Parameters.Add(new SqlParameter("@Y", truongDuLieuLoaiBangViewModel.Y));
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", truongDuLieuLoaiBangViewModel.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", truongDuLieuLoaiBangViewModel.TruongDuLieuCode));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion loai bang
        private bool CanUpdateLoaiBang(int loaiBangId)
        {
            try
            {
                string sqlString_1 = @"Select Count(*) as 'TotalRow' From Bang Where LoaiBangId = @LoaiBangId";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow += Convert.ToInt32(reader["TotalRow"]);
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
                    Exception exception = new Exception("Loại bằng này đang được sử dụng, không thể cập nhật");
                    throw exception;
                }

                string sqlString_2 = @"Select Count(*) as 'TotalRow' From YeuCau Where LoaiVanBangId = @LoaiBangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow += Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                string sqlString_3 = @"Select Count(*) as 'TotalRow' From Bang Where LoaiBangId = @LoaiBangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow += Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return totalRow == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddAnhLoaiBangCu(AttachFileViewModel attachFileViewModel, int donViId)
        {
            try
            {
                if (attachFileViewModel != null)
                {
                    List<AnhLoaiBang> anhLoaiBangs = DbContext.AnhLoaiBangs.Where(x => (x.DonViId == donViId) && (x.IsDeleted == false) && (x.ObjectId == attachFileViewModel.ObjectId)).ToList();
                    foreach (AnhLoaiBang item in anhLoaiBangs)
                    {
                        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), item.Url);
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                        DbContext.AnhLoaiBangs.Remove(item);
                    }

                    AnhLoaiBang anhLoaiBang = new AnhLoaiBang();
                    anhLoaiBang.Id = Guid.NewGuid().ToString();
                    anhLoaiBang.Url = "/Upload/AnhLoaiBang/" + anhLoaiBang.Id + "." + attachFileViewModel.Extension;
                    anhLoaiBang.ObjectId = attachFileViewModel.ObjectId;
                    anhLoaiBang.NguoiTao = attachFileViewModel.NguoiTao;
                    anhLoaiBang.NgayTao = attachFileViewModel.NgayTao;
                    anhLoaiBang.NguoiCapNhat = attachFileViewModel.NguoiCapNhat;
                    anhLoaiBang.NgayCapNhat = attachFileViewModel.NgayCapNhat;
                    anhLoaiBang.DonViId = donViId;
                    anhLoaiBang.IsDeleted = false;
                    DbContext.AnhLoaiBangs.Add(anhLoaiBang);
                    DbContext.SaveChanges();
                    Base64Helper.ConvertBase64ToFileAndSave(attachFileViewModel.Base64String, anhLoaiBang.Id + "." + attachFileViewModel.Extension, "AnhLoaiBang");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttachFileViewModel GetAnhLoaiBangCu(int loaiBangId, int donViId)
        {
            try
            {
                AttachFileViewModel attachFileViewModel = new AttachFileViewModel();
                string sqlString = @"Select * From [AnhLoaiBang] Where (IsDeleted = 0) and (ObjectId = @ObjectId) and (DonViId = @DonViId)";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@ObjectId", loaiBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            attachFileViewModel = MapDataHelper<AttachFileViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return attachFileViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiBangViewModel> GetLoaiBangsChoTruong(int donViId)
        {
            try
            {
                List<LoaiBangViewModel> loaiBangViewModels = new List<LoaiBangViewModel>();
                string sqlString = @"Select a.* From LoaiBang as a
                                    Left Join CapDonVi as b
                                    on a.CodeCapDonVi = b.Code
                                    Inner Join DonVi as c
                                    on c.CapDonViId = b.CapDonViId
                                    Where (c.DonViId = @DonViId) and (a.IsDeleted = 0) and (a.LoaiBangGocId is null)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            loaiBangViewModels = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return loaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}