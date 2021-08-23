using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
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
    public class TuDienProvider : ApplicationDbContext, ITuDien
    {
        public List<CapDonViViewModel> GetAllCapDonVis()
        {
            try
            {
                string sqlString = "select * from CapDonVi Order by CapDonViId ASC";
                List<CapDonViViewModel> capDonViViewModels = new List<CapDonViViewModel>();
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
                            capDonViViewModels = MapDataHelper<CapDonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                CapDonViViewModel codeAll = capDonViViewModels.Where(x => x.Code.ToLower() == "all").FirstOrDefault();
                if (codeAll != null)
                {
                    capDonViViewModels.Remove(codeAll);

                }
                return capDonViViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DanTocViewModel> GetDanTocs()
        {
            try
            {
                List<DanTocViewModel> danTocViewModels = new List<DanTocViewModel>();
                string sqlString = @"Select * From [dbo].[DanToc]";

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
                            danTocViewModels = MapDataHelper<DanTocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return danTocViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GioiTinhViewModel> GetGioiTinhs()
        {
            try
            {
                List<GioiTinhViewModel> gioiTinhViewModels = new List<GioiTinhViewModel>();
                string sqlString = @"Select * From [dbo].[GioiTinh]";
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
                            gioiTinhViewModels = MapDataHelper<GioiTinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return gioiTinhViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<HinhThucCapViewModel> GetHinhThucCaps()
        {
            try
            {
                List<HinhThucCapViewModel> hinhThucCapViewModels = new List<HinhThucCapViewModel>();
                string sqlString = @"Select * From [dbo].[HinhThucCap]";

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
                            hinhThucCapViewModels = MapDataHelper<HinhThucCapViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return hinhThucCapViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiBangViewModel> GetLoaiBangTheoHinhThuc(int? hinhThuCapId)
        {
            try
            {
                List<LoaiBangViewModel> loaiBangViewModels = new List<LoaiBangViewModel>();
                string sqlString = @"Select * From LoaiBang Where ((@HinhThucCapId is null) or (HinhThucCapId = @HinhThucCapId)) and (IsDeleted = 0)";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HinhThucCapId", hinhThuCapId.HasValue ? hinhThuCapId.Value : DBNull.Value));
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

        public List<LoaiYeuCauViewModel> GetLoaiYeuCauMuaPhoi()
        {
            try
            {
                List<LoaiYeuCauViewModel> loaiYeuCauViewModels = new List<LoaiYeuCauViewModel>();
                string sqlString = @"Select a.*, b.Id as 'MauCongVanId', b.NoiDung, b.TenCongVan From [LoaiYeuCau] as a
                                    Left Join [MauCongVans] as b
                                    on a.Id = b.LoaiYeuCauId
                                    Where (a.Id = 2) or (a.Id = 7)";
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
                            var result = MapDataHelper<LoaiYeuCauViewModel>.MapList(reader);
                            foreach (var item in result)
                            {
                                if (loaiYeuCauViewModels.Any(x => x.Id == item.Id))
                                {
                                    var loaiYeuCauViewModel = loaiYeuCauViewModels.Where(x => x.Id == item.Id).FirstOrDefault();
                                    loaiYeuCauViewModel.MauCongVans.Add(new MauCongVanViewModel()
                                    {
                                        Id = item.MauCongVanId.Value,
                                        NoiDung = item.NoiDung,
                                        TenCongVan = item.TenCongVan
                                    });

                                }
                                else
                                {
                                    LoaiYeuCauViewModel loaiYeuCauViewModel = new LoaiYeuCauViewModel();
                                    loaiYeuCauViewModel.Id = item.Id;
                                    loaiYeuCauViewModel.TenLoaiYeuCau = item.TenLoaiYeuCau;
                                    loaiYeuCauViewModel.MauCongVans = new List<MauCongVanViewModel>();
                                    loaiYeuCauViewModel.LoaiBangs = new List<LoaiBangViewModel>();
                                    loaiYeuCauViewModel.MauCongVans.Add(new MauCongVanViewModel()
                                    {
                                        Id = item.MauCongVanId.Value,
                                        NoiDung = item.NoiDung,
                                        TenCongVan = item.TenCongVan
                                    });
                                    loaiYeuCauViewModels.Add(loaiYeuCauViewModel);
                                }
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                foreach (LoaiYeuCauViewModel loaiYeuCauViewModel_1 in loaiYeuCauViewModels)
                {
                    string sqlString_1 = @" Select a.*,b.Ten as 'HinhThucCap', c.[Level] From [dbo].[LoaiBang] as a
                                    Left Join [dbo].[HinhThucCap] as b
                                    on a.HinhThucCapId = b.Id
									Left Join CapDonVi as c
									on a.CodeCapDonVi = c.Code
                                    Where (IsDeleted = 0) and (HinhThucCapId = @HinhThucCapId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@HinhThucCapId", loaiYeuCauViewModel_1.Id == 2 ? 1 : 2));
                            using (var reader = command.ExecuteReader())
                            {
                                loaiYeuCauViewModel_1.LoaiBangs = MapDataHelper<LoaiBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return loaiYeuCauViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MauCongVanViewModel> GetMauCongVan(int loaiYeuCauId)
        {
            try
            {
                List<MauCongVanViewModel> mauCongVanViewModels = new List<MauCongVanViewModel>();
                string sqlString = @"Select * From MauCongVans Where LoaiYeuCauId = @LoaiYeuCauId";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiYeuCauId", loaiYeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            mauCongVanViewModels = MapDataHelper<MauCongVanViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return mauCongVanViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TrangThaiBangViewModel> GetTrangThaiBangs()
        {
            try
            {
                List<TrangThaiBangViewModel> trangThaiBangViewModels = new List<TrangThaiBangViewModel>();
                string sqlString = @"Select * From [dbo].[TrangThaiBang]";

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
                            trangThaiBangViewModels = MapDataHelper<TrangThaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return trangThaiBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TrangThaiPhoiViewModel> GetTrangThaiPhois()
        {
            try
            {
                List<TrangThaiPhoiViewModel> trangThaiPhoiViewModels = new List<TrangThaiPhoiViewModel>();
                string sqlString = @"Select * From [dbo].[TrangThaiPhoi]";

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
                            trangThaiPhoiViewModels = MapDataHelper<TrangThaiPhoiViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return trangThaiPhoiViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> GetTruongDuLieusThongTinHocSinh()
        {
            try
            {
                List<string> hocSinhCols = new List<string>();
                string sqlString = @"Select COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
                                        WHERE TABLE_NAME = N'HocSinh'";

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
                            hocSinhCols = MapDataHelper<Object>.MapListString(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                hocSinhCols.Remove("Id");
                hocSinhCols.Remove("NgayTao");
                hocSinhCols.Remove("NguoiTao");
                hocSinhCols.Remove("NgayCapNhat");
                hocSinhCols.Remove("NguoiCapNhat");
                hocSinhCols.Remove("IsDeleted");
                hocSinhCols.Remove("DonViId");
                hocSinhCols.Remove("TruongHocId");
                hocSinhCols.Remove("LopHocId");
                hocSinhCols.Remove("DanTocId");
                hocSinhCols.Remove("GioiTinhId");
                hocSinhCols.Remove("HK");
                hocSinhCols.Remove("KK");
                hocSinhCols.Remove("KQ");
                hocSinhCols.Remove("UT");
                hocSinhCols.Remove("HL");
                hocSinhCols.Remove("XetHK");
                hocSinhCols.Remove("SoLanXet");
                hocSinhCols.Remove("CongNhanTotNghiep");
                hocSinhCols.Remove("CodeGroupMonHoc");
                return hocSinhCols;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<XepLoaiTotNghiepViewModel> GetXepLoaiTotNghiepByCapDonVi(string codeCapDonVi)
        {
            try
            {
                List<XepLoaiTotNghiepViewModel> xepLoaiTotNghiepViewModels = new List<XepLoaiTotNghiepViewModel>();
                string sqlString = @"Select * From XepLoaiTotNghieps Where CodeCapDonVi Like @CodeCapDonVi";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@CodeCapDonVi", codeCapDonVi));
                        using (var reader = command.ExecuteReader())
                        {
                            xepLoaiTotNghiepViewModels = MapDataHelper<XepLoaiTotNghiepViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return xepLoaiTotNghiepViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> GetNamTotNghiepsByTruong(int truongHocId)
        {
            try
            {
                List<int> namTotNghieps = new List<int>();
                string sqlString = @"Select NamTotNghiep From Bang 
                                    Where TruongHocId = @TruongHocId
                                    Group by NamTotNghiep";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                namTotNghieps.Add(Convert.ToInt32(reader["NamTotNghiep"]));
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return namTotNghieps;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LoaiNhomTaoVanBangViewModel> GetLoaiNhomVanBangs()
        {
            try
            {
                List<LoaiNhomTaoVanBangViewModel> loaiNhomTaoVanBangViewModels = new List<LoaiNhomTaoVanBangViewModel>();
                string sqlString = @"Select * From LoaiNhomTaoVanBangs";
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
                            loaiNhomTaoVanBangViewModels = MapDataHelper<LoaiNhomTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return loaiNhomTaoVanBangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LopHocViewModel> GetLopHocByNam(string nam, int donViId)
        {
            try
            {
                List<LopHocViewModel> lopHocs = new List<LopHocViewModel>();
                string sqlString = @"Select * From LopHocs Where (DonViId = @DonViId) and (NienKhoa like @NienKhoa)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", nam));
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DonViViewModel> GetDonViByDonViCha(int donViChaId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.*
                                      From DonVi as a
                                      Left Join CapDonVi as b
                                      on a.CapDonViId = b.CapDonViId
                                      Where (KhoaChaId = @KhoaChaId) and (b.Code in ('SOGD', 'PHONGGD'))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@KhoaChaId", donViChaId));
                        using (var reader = command.ExecuteReader())
                        {
                            donViViewModels = MapDataHelper<DonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return donViViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DonViViewModel> GetAllDonViByDonViCha(int donViChaId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.*, b.Code
                                      From DonVi as a
                                      Left Join CapDonVi as b
                                      on a.CapDonViId = b.CapDonViId
                                    Where KhoaChaId = @KhoaChaId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@KhoaChaId", donViChaId));
                        using (var reader = command.ExecuteReader())
                        {
                            donViViewModels = MapDataHelper<DonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return donViViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DonViViewModel> GetTruongHocByDonViChaVaNhomTaoVanBang(int donViChaId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.*, b.Code
                                    from DonVi as a
                                    Left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    where (a.KhoaChaId = @DonViChaId) and (b.Code in ('THCS', 'THPT', 'TDH', 'TIEUHOC'))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViChaId", donViChaId));
                        using (var reader = command.ExecuteReader())
                        {
                            var result = MapDataHelper<DonViViewModel>.MapList(reader);
                            if (result != null && result.Count() > 0 && result.FirstOrDefault().DonViId != 0)
                            {
                                donViViewModels.AddRange(result);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get truong hoc trong nhom tao vao bang ma donViIn bang donviid
                string sqlString_1 = @"Select Distinct c.*
                                        From DonVi as a
                                        Left Join NhomTaoVanBangs as b
                                        on a.DonViId = b.DonViIn
                                        Left Join DonVi as c
                                        on b.TruongHocId = c.DonViId
                                        where (a.DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViChaId));
                        using (var reader = command.ExecuteReader())
                        {
                            var result = MapDataHelper<DonViViewModel>.MapList(reader);
                            if (result != null && result.Count() > 0 && result.FirstOrDefault().DonViId != 0)
                            {
                                donViViewModels.AddRange(result);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                return donViViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ThongBaoTypeViewModel> GetThongBaoTypes(int donViId)
        {
            try
            {
                List<ThongBaoTypeViewModel> thongBaoTypeViewModels = new List<ThongBaoTypeViewModel>();
                string sqlString = @"Select a.* From ThongBaoTypes as a
                                    Left Join ThongBaoTypeCapDonVis as b
                                    on a.Id = b.ThongBaoTypeId
                                    Where (b.CodeCapDonVi Like (Select b.Code From DonVi as a
							                                    Left Join CapDonVi as b
							                                    on a.CapDonViId = b.CapDonViId
							                                    Where a.DonViId = @DonViId))
	                                    or (b.CodeCapDonVi is null)";
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
                            thongBaoTypeViewModels = MapDataHelper<ThongBaoTypeViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return thongBaoTypeViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCodeCapDonViByNguoiDung(int donViId)
        {
            try
            {
                string codeCapDonVi = "";
                string sqlString = @"Select b.* From DonVi as a
                                    Left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    Where a.DonViId = @DonViId";
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
                                codeCapDonVi = Convert.ToString(reader["Code"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return codeCapDonVi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GetNamTotNghiepByDonViViewModel GetNamTotNghiepByDonVi(int donViId)
        {
            try
            {
                GetNamTotNghiepByDonViViewModel getNamTotNghiepByDonViViewModel = new GetNamTotNghiepByDonViViewModel();
                getNamTotNghiepByDonViViewModel.NamTotNghieps = new List<int>();
                // get code cap don vi
                string codeCapDonVi = "";
                string sqlString = @"Select b.* From DonVi as a
                                    Left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    Where a.DonViId = @DonViId";
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
                                codeCapDonVi = Convert.ToString(reader["Code"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get nam tot nghiep
                if (codeCapDonVi == "BOGD" || codeCapDonVi == "PHONGGD" || codeCapDonVi == "SOGD")
                {
                    string sqlString_1 = @"Select NamTotNghiep From Bang Where TruongHocId in (Select a.DonViId From DonVi as a
										Left Join CapDonVi as b
										on a.CapDonViId = b.CapDonViId
										Where b.Level > (Select b.Level From DonVi as a
														Left Join CapDonVi as b
														on a.CapDonViId = b.CapDonViId
														Where a.DonViId = @DonViId) )
                                        Group by NamTotNghiep";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    getNamTotNghiepByDonViViewModel.NamTotNghieps.Add(Convert.ToInt32(reader["NamTotNghiep"]));
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                getNamTotNghiepByDonViViewModel.CodeCapDonVi = codeCapDonVi;

                return getNamTotNghiepByDonViViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LopHocViewModel> GetLopHocByNamVaDonVi(int nam, int truongHocId, int donViId)
        {
            try
            {
                List<LopHocViewModel> lopHocViewModels = new List<LopHocViewModel>();
                string sqlString = @"Select * From LopHocs
                                    Where (DonViId = @TruongHocId) and (NienKhoa Like @NienKhoa)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", nam.ToString()));
                        using (var reader = command.ExecuteReader())
                        {
                            lopHocViewModels = MapDataHelper<LopHocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return lopHocViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DonViViewModel GetCodeCapDonViByDonViId(int donViId)
        {
            try
            {
                DonViViewModel donViViewModel = new DonViViewModel();
                string sqlString = @"Select a.DonViId, a.TenDonVi, b.Code From DonVi as a
                                    Left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    Where a.DonViId = @DonViId";
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
                            donViViewModel = MapDataHelper<DonViViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return donViViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CapDonViViewModel> GetCapDonVisForUpdating(int? currentDonViId, int donViId)
        {
            try
            {
                List<CapDonViViewModel> capDonViViewModels = new List<CapDonViViewModel>();
                if (!currentDonViId.HasValue)
                {
                    string sqlString = @"Select * From CapDonVi where level = (Select b.Level + 1 From DonVi as a
                                        Left Join CapDonVi as b
                                        on a.CapDonViId = b.CapDonViId
                                        where a.DonViId = @CurrentDonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@CurrentDonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                capDonViViewModels = MapDataHelper<CapDonViViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else if( currentDonViId == donViId)
                {
                    string sqlString = @"Select b.* From DonVi as a
                                            Left Join CapDonVi as b
                                            on a.CapDonViId = b.CapDonViId
                                            where a.DonViId = @DonViId";
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
                                capDonViViewModels = MapDataHelper<CapDonViViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else
                {
                    string sqlString = @"Select * From CapDonVi where level = (Select b.Level + 1 From DonVi as a
                                        Left Join CapDonVi as b
                                        on a.CapDonViId = b.CapDonViId
                                        where a.DonViId = @CurrentDonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@CurrentDonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                capDonViViewModels = MapDataHelper<CapDonViViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return capDonViViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DonViViewModel> GetCurrentDonVi(int? currentDonViId, int donViId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();

                if (!currentDonViId.HasValue || (currentDonViId.Value == donViId))
                {
                    string sqlString = @"Select * From DonVi Where DonViId = (Select KhoaChaId From DonVi Where DonViId = @DonViId)";
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
                                donViViewModels = MapDataHelper<DonViViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else
                {
                    string sqlString = @"Select * From DonVi Where DonViId = @DonViId";
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
                                donViViewModels = MapDataHelper<DonViViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                return donViViewModels;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
