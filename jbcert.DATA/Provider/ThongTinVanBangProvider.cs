using jbcert.DATA.Helpers;
using jbcert.DATA.Interfaces;
using jbcert.DATA.Models;
using jbcert.DATA.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace jbcert.DATA.Provider
{
    public class ThongTinVanBangProvider : ApplicationDbContext, IThongTinVanBang
    {
        private void DrawText(Graphics drawing, string text, System.Drawing.Font font, Color textColor, int x, int y)
        {
            text = string.IsNullOrEmpty(text) ? "" : text;
            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, x, y);
            textBrush.Dispose();
        }

        public DetailThongTinBangViewModel DetailThongTinBang(int bangId, int DonViId)
        {
            try
            {
                DetailThongTinBangViewModel detailThongTinBangViewModel = new DetailThongTinBangViewModel();
                detailThongTinBangViewModel.ThongTinVanBangs = new List<ThongTinVBViewModel>();
                string sqlString = @"Select a.*, b.Ten as 'TenLoaiBang' From Bang as a
                                    Left Join LoaiBang as b
                                    on a.LoaiBangId = b.Id
                                    Where (a.Id = @BangId) and (a.BangGocId Is Null) and (a.DonViId = @DonViId)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", DonViId));
                        using (var reader = command.ExecuteReader())
                        {
                            detailThongTinBangViewModel = MapDataHelper<DetailThongTinBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                detailThongTinBangViewModel.ThongTinVanBangs = new List<ThongTinVBViewModel>();

                string sqlString_1 = @"Select c.*, Case
				                                        When b.TenTruongDuLieu = 'SoVaoSo' Then 0
				                                        Else 1
			                                        End as 'CanUpDate'
                                        From ThongTinVanBang as a
                                        Left Join TruongDuLieuLoaiBang as b
                                        Left Join LoaiBang as c
                                        on b.LoaiBangId = c.Id
                                        on a.TruongDuLieuCode = b.TruongDuLieuCode
                                        Where (a.BangId  = @BangId) and (b.KieuDuLieu = 1) and (c.Id = @LoaiBangId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", detailThongTinBangViewModel.LoaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            detailThongTinBangViewModel.ThongTinVanBangs = MapDataHelper<ThongTinVBViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return detailThongTinBangViewModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateThongTinVanBang(List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels, int donViId)
        {
            try
            {
                foreach (TruongDuLieuTrongBangViewModel truongDuLieuLoaiBangViewModel in truongDuLieuTrongBangViewModels)
                {
                    string sqlString = @"Select * From [ThongTinVanBang] Where (TruongDuLieuCode = @TruongDuLieuCode) and (BangId = @BangId)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    sqlParameters.Add(new SqlParameter("@TruongDuLieuCode", truongDuLieuLoaiBangViewModel.TruongDuLieuCode));
                    sqlParameters.Add(new SqlParameter("@BangId", truongDuLieuLoaiBangViewModel.BangId));
                    ThongTinVanBang thongTinVanBang = DbContext.ThongTinVanBangs.FromSqlRaw(sqlString, sqlParameters).FirstOrDefault();


                    thongTinVanBang.GiaTri = truongDuLieuLoaiBangViewModel.GiaTri;
                    thongTinVanBang.NgayCapNhat = truongDuLieuLoaiBangViewModel.NgayCapNhat;
                    thongTinVanBang.NguoiCapNhat = truongDuLieuLoaiBangViewModel.NguoiCapNhat;
                    DbContext.SaveChanges();
                }

                string sqlString_1 = @"Select * From Bang Where (Id = @Id) and (DonViId = @DonViId)";
                List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                sqlParameters_1.Add(new SqlParameter("@Id", truongDuLieuTrongBangViewModels.FirstOrDefault().LoaiBangId));
                sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));
                Bang bang = DbContext.Bangs.FromSqlRaw(sqlString_1, sqlParameters_1).FirstOrDefault();
                bang.NgayCapNhat = truongDuLieuTrongBangViewModels.FirstOrDefault().NgayCapNhat;
                bang.NguoiCapNhat = truongDuLieuTrongBangViewModels.FirstOrDefault().NguoiCapNhat;
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<int> CapNhatTrangThaiPhoiVaBangSauKhiIn(List<TrangThaiPhoiVaBangSauKhiInViewModel> trangThaiPhoiVaBangSauKhiInViewModels)
        {
            try
            {
                List<int> bangKhongDuocCapNhats = new List<int>();
                foreach (TrangThaiPhoiVaBangSauKhiInViewModel trangThaiPhoiVaBangSauKhiInViewModel in trangThaiPhoiVaBangSauKhiInViewModels)
                {
                    string sqlString = @"Select * From Bang Where Id = @Id";
                    Bang bang = DbContext.Bangs.FromSqlRaw(sqlString, new SqlParameter("@Id", trangThaiPhoiVaBangSauKhiInViewModel.BangId)).FirstOrDefault();
                    string sqlString_1 = @"Select * From Phoi Where (SoHieu Like @SoHieu) and (LoaiBangId = @LoaiBangId)";
                    PhoiVanBang phoi = DbContext.Phois.FromSqlRaw(sqlString_1, new SqlParameter("@SoHieu", trangThaiPhoiVaBangSauKhiInViewModel.SoHieu), new SqlParameter("@LoaiBangId", bang.LoaiBangId)).FirstOrDefault();
                    if (phoi == null)
                    {
                        Exception exception = new Exception("Phôi loại bằng này không tồn tại!!!");
                        throw exception;
                    }
                    else if (phoi.TrangThaiPhoiId == 2)
                    {
                        Exception exception = new Exception("Phôi này đã được sử dụng!!!");
                        throw exception;
                    }
                    else if (phoi.TrangThaiPhoiId == 3)
                    {
                        Exception exception = new Exception("Phôi này đang lỗi!!!");
                        throw exception;
                    }
                    else if (phoi.TrangThaiPhoiId == 4)
                    {
                        Exception exception = new Exception("Phôi này đã được thu hồi!!!");
                        throw exception;
                    }

                    if (bang != null && !string.IsNullOrEmpty(bang.DuongDanFileAnh) && (phoi != null))
                    {
                        bang.TrangThaiBangId = 4;

                        string sqlString_2 = @"Select * From [LienKetHocSinhYeuCau] Where (YeuCauId = @YeuCauId) and (HocSinhId = @HocSinhId)";
                        List<SqlParameter> sqlParameters_2 = new List<SqlParameter>();
                        sqlParameters_2.Add(new SqlParameter("@YeuCauId", bang.YeuCauId));
                        sqlParameters_2.Add(new SqlParameter("@HocSinhId", bang.HocSinhId));
                        LienKetHocSinhYeuCau lienKetHocSinhYeuCau = DbContext.LienKetHocSinhYeuCaus.FromSqlRaw(sqlString_2, sqlParameters_2.ToArray()).FirstOrDefault();
                        if (lienKetHocSinhYeuCau != null)
                        {
                            lienKetHocSinhYeuCau.TrangThaiCapPhatBang = 4;
                        }
                        bang.PhoiId = phoi.Id;
                        phoi.TrangThaiPhoiId = 2;
                        DbContext.SaveChanges();
                    }
                    //else
                    //{
                    //    //bangKhongDuocCapNhats.Add(trangThaiPhoiVaBangSauKhiInViewModel.BangId);
                    //}

                }

                return bangKhongDuocCapNhats;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BangViewModel> DownLoadFileAnh(TaoAnhVanBangViewModel taoAnhVanBangViewModel)
        {
            try
            {
                // get nhom van bang
                string sqlString_4 = @"Select * From NhomTaoVanBangs Where (Id = @NhomTaoVanBangId) and ((DonViId = @DonViId) or (DonViIn = @DonViId))";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", taoAnhVanBangViewModel.DonViId.Value));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                if (nhomTaoVanBangViewModel.Id == 0)
                {
                    Exception exception = new Exception("Không tìm thấy dữ liệu");
                    throw exception;
                }

                string sqlString = @"Select c.*, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang',
                                            f.Width as 'AnhLoaiBangWidth', f.Height as 'AnhLoaiBangHeight'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Left Join LoaiBang as f
                                      on c.LoaiBangId = f.Id
                                      Where (b.Id = @NhomTaoVanBangId) and ((b.DonViId = @DonViId) or (b.DonViIn = @DonViId)) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId = 3)
                                    Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                List<BangViewModel> bangViewModels = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", taoAnhVanBangViewModel.DonViId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModels = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return bangViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachVanBangViewModel DanhSachBang(string hoVaTen, string soVaoSo, string lopHoc, int loaiBangId, List<int> truongIds, int donViId, int currentPage)
        {
            try
            {
                DanhSachVanBangViewModel danhSachVanBangViewModel = new DanhSachVanBangViewModel();
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@HoVaTen", hoVaTen));
                sqlParameters.Add(new SqlParameter("@SoVaoSo", soVaoSo));
                sqlParameters.Add(new SqlParameter("@LopHoc", lopHoc));
                sqlParameters.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                sqlParameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                sqlParameters.Add(new SqlParameter("@Next", 12));
                string extendString = "";
                if (truongIds == null || truongIds.Count() == 0) // cap so
                {
                    extendString = @" and (a.DonViId = @DonViId)";
                    sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                }
                else if (donViId != -1)// cap phong, thcs, thpt
                {
                    string[] paramTruongIds = truongIds.Select(x => "@TruongId_" + x).ToArray();
                    extendString = string.Format(@" and ( b.TruongHocId in ({0}) )", string.Join(",", paramTruongIds));
                    foreach (int paramTruongId in truongIds)
                    {
                        sqlParameters.Add(new SqlParameter("@TruongId_" + paramTruongId, paramTruongId));
                    }
                }
                else if (donViId == -1) // nha phat trien ALL
                {
                    extendString = "";
                }

                List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                sqlParameters_1.Add(new SqlParameter("@HoVaTen", hoVaTen));
                sqlParameters_1.Add(new SqlParameter("@SoVaoSo", soVaoSo));
                sqlParameters_1.Add(new SqlParameter("@LopHoc", lopHoc));
                sqlParameters_1.Add(new SqlParameter("@LoaiBangId", loaiBangId));
                if (truongIds == null || truongIds.Count() == 0) // cap so
                {
                    extendString = @" and (a.DonViId = @DonViId)";
                    sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));
                }
                else if (donViId != -1)// cap phong, thcs, thpt
                {
                    string[] paramTruongIds = truongIds.Select(x => "@TruongId_" + x).ToArray();
                    extendString = string.Format(@" and ( b.TruongHocId in ({0}) )", string.Join(",", paramTruongIds));
                    foreach (int paramTruongId in truongIds)
                    {
                        sqlParameters_1.Add(new SqlParameter("@TruongId_" + paramTruongId, paramTruongId));
                    }
                }
                else if (donViId == -1) // nha phat trien ALL
                {
                    extendString = "";
                }

                string sqlString = string.Format(@"Select a.Id as 'BangId', b.HoVaTen, b.NgaySinh, b.NoiSinh, b.HoKhauThuongTru, b.TruongHoc, b.LopHoc, b.DanToc, b.SoVaoSo,b.GioiTinh, b.NamTotNghiep, 
                                                    b.XepLoaiTotNghiep, b.HinhThucDaoTao, b.DiemThi, b.Id, f.Id as 'LoaiBangId', f.Ten as 'LoaiBang', a.CMTNguoiLayBang, a.SoDienThoaiNguoiLayBang,
	                                                a.QuanHeVoiNguoiDuocCapBang, a.TrangThaiBangId, c.Ten as 'TrangThaiBang', c.MaMauTrangThai ,a.YeuCauId , d.TenYeuCau,e.TenDonVi as 'DonViYeuCau', a.NgayCapNhat, a.DonViId
                                                From Bang as a
                                                Left Join HocSinh as b
                                                on a.HocSinhId = b.Id
                                                Left Join TrangThaiBang as c
                                                on a.TrangThaiBangId = c.Id
                                                Left Join YeuCau as d
                                                on a.YeuCauId = d.Id
                                                Left Join DonVi as e
                                                on d.DonViYeuCauId = e.DonViId
                                                Left Join LoaiBang as f
                                                on a.LoaiBangId = f.Id
                                                Where (b.HoVaTen Like N'%'+@HoVaTen+'%') and (b.LopHoc Like N'%'+@LopHoc+'%') and (b.SoVaoSo Like '%'+@SoVaoSo+'%') and ((-1 = @LoaiBangId) or (a.LoaiBangId = @LoaiBangId))
                                                        {0}
                                                Order By a.YeuCauId
                                                Offset @Offset Rows Fetch Next @Next Rows Only", extendString);


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
                            danhSachVanBangViewModel.VanBangs = MapDataHelper<HocSinhTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                string sqlString_1 = string.Format(@"Select Count(*) as 'Total'
                                                    From Bang as a
                                                    Left Join HocSinh as b
                                                    on a.HocSinhId = b.Id
                                                    Left Join TrangThaiBang as c
                                                    on a.TrangThaiBangId = c.Id
                                                    Left Join YeuCau as d
                                                    on a.YeuCauId = d.Id
                                                    Left Join DonVi as e
                                                    on d.DonViYeuCauId = e.DonViId
                                                    Where (b.HoVaTen Like N'%'+@HoVaTen+'%') and (b.LopHoc Like N'%'+@LopHoc+'%') and (b.SoVaoSo Like '%'+@SoVaoSo+'%') and ((-1 = @LoaiBangId) or (a.LoaiBangId = @LoaiBangId))
                                                            {0}", extendString);

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
                            danhSachVanBangViewModel.VanBangs = MapDataHelper<HocSinhTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                danhSachVanBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                danhSachVanBangViewModel.CurrentPage = currentPage;

                return danhSachVanBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void PhatVanBang(PhatVanBangViewModel phatVanBangViewModel, int donViId)
        {
            try
            {
                // get bang
                string sqlString = @"Select * From Bang Where (Id = @BangId) and (HocSinhId = @HocSinhId)";
                Bang bang = DbContext.Bangs.FromSqlRaw(sqlString, new SqlParameter("@BangId", phatVanBangViewModel.BangId), new SqlParameter("@HocSinhId", phatVanBangViewModel.HocSinhId)).FirstOrDefault();
                if (bang.TrangThaiBangId <= 3)
                {
                    Exception exception = new Exception("Bằng phải được in mới được cấp phát");
                    throw exception;
                }
                bang.CmtnguoiLayBang = phatVanBangViewModel.CMTNguoiLayBang;
                bang.SoDienThoaiNguoiLayBang = phatVanBangViewModel.SoDienThoaiNguoiLayBang;
                bang.QuanHeVoiNguoiDuocCapBang = phatVanBangViewModel.QuanHeVoiNguoiDuocCapBang;
                bang.NgayCapNhat = phatVanBangViewModel.NgayCapNhat;
                bang.NguoiCapNhat = phatVanBangViewModel.NguoiCapNhat;
                bang.TrangThaiBangId = 6; // da phat
                DbContext.SaveChanges();

                // update trang thai da phat bang cua hoc sinh trong nhom tao van bang
                string sqlString_1 = @"Update HocSinhTrongNhomTaoVanBangs
                                       Set TrangThaiBangId = 6
                                       Where (BangId = @BangId) and (NhomTaoVanBangId = @NhomTaoVanBangId) and (TrangThaiBangId >= 4)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", phatVanBangViewModel.BangId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // update trang thai da phat bang cua hoc sinh trong nhom tao van bang
                string sqlString_2 = @"Select Count(*) as 'TotalRow'  From HocSinhTrongNhomTaoVanBangs
                                       Where (TrangThaiBangId < 6) and (NhomTaoVanBangId = @NhomTaoVanBangId) and (TrangThaiBangId >= 4)";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                totalRow = Convert.ToInt32(reader["TotalRow"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (totalRow == 0)
                {
                    string sqlString_3 = @"Update NhomTaoVanBangs
                                           Set TrangThaiBangId = 6
                                           Where (Id = @NhomTaoVanBangId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
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

        public ChiTietSoGocViewModel GetChiTietSoGoc(int bangId, int donViId)
        {
            try
            {
                ChiTietSoGocViewModel chiTietSoGocViewModel = new ChiTietSoGocViewModel();
                // Get Chi Tiet So Goc
                string sqlString = @"Select a.*, b.Ten as 'TrangThaiBang', b.MaMauTrangThai as 'MaMauTrangThaiBang', c.[Url] as 'AnhLoaiBang', d.Width as 'AnhLoaiBangWidth', d.Height as 'AnhLoaiBangHeight'
                                    From Bang as a
                                    Left Join TrangThaiBang as b
                                    on a.TrangThaiBangId = b.Id
                                    Left Join AnhLoaiBang as c
                                    on a.LoaiBangId = c.ObjectId
                                    Left Join LoaiBang as d
                                    on a.LoaiBangId = d.Id
                                    Where (a.Id = @BangId) and (a.DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                        using (var reader = command.ExecuteReader())
                        {
                            chiTietSoGocViewModel.SoGoc = MapDataHelper<BangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get truong du lieu trong bang
                string sqlString_1 = @"Select c.*, a.GiaTri
                                        From ThongTinVanBang as a
                                        Left Join Bang as b
                                        on a.BangId = b.Id
                                        Left Join TruongDuLieuLoaiBang as c
                                        on a.TruongDuLieuCode = c.TruongDuLieuCode and b.LoaiBangId = c.LoaiBangId
                                        Where (b.Id = @BangId) and (b.DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                        using (var reader = command.ExecuteReader())
                        {
                            chiTietSoGocViewModel.SoGoc.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get log
                string sqlString_2 = @"Select * From LogVanBang
                                        Where VanBangId = @BangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            chiTietSoGocViewModel.LogVanBangs = MapDataHelper<LogVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return chiTietSoGocViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachSoGocViewModel GetSoGocs(int? truongHocId, string hoVaTen, int? namTotNghiep, int? donViId, int currentPage)
        {
            try
            {
                DanhSachSoGocViewModel danhSachSoGocViewModel = new DanhSachSoGocViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.Ten as 'TrangThaiBang', b.MaMauTrangThai as 'MaMauTrangThaiBang'
                                    From Bang as a
                                    Left Join TrangThaiBang as b
                                    on a.TrangThaiBangId = b.Id
                                    Where ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
                                    and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep)) and (DonViId = @DonViId) and (a.BangGocId is null)
                                    and ((a.TrangThaiBangId >= 4) or (a.SoLanCaiChinh > 0))
                                    Order By a.Id
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId.Value));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            danhSachSoGocViewModel.SoGocs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get tong so row
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                    From Bang as a
                                    Left Join TrangThaiBang as b
                                    on a.TrangThaiBangId = b.Id
                                    Where ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
                                    and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep)) and (a.BangGocId is null)
                                    and ((a.TrangThaiBangId >= 4) or (a.SoLanCaiChinh > 0))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));

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
                    danhSachSoGocViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    danhSachSoGocViewModel.CurrentPage = currentPage;
                }

                return danhSachSoGocViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachVanBangViewModel GetVanBangs(int? truongHocId, int? loaiBangId, string hoVaTen, string nienKhoa, int? trangThaiBangId, int? donViId, int currentPage)
        {
            try
            {
                DanhSachVanBangViewModel danhSachVanBangViewModel = new DanhSachVanBangViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa ,
                                    g.DuongDanFileAnh, g.DuongDanFileDeIn as 'DuongDanFileAnhDeIn', g.id as 'BangId', g.LoaiBangId as 'LoaiBangId', g.TrangThaiBangId, 
                                    h.Ten as 'TrangThaiBang', h.MaMauTrangThai as 'MaMauTrangThaiBang', i.SoHieu, g.CMTNguoiLayBang, g.SoDienThoaiNguoiLayBang, g.HinhThucNhan,
                                    g.QuanHeVoiNguoiDuocCapBang
                                    From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Left Join DonVi as f
                                    on a.TruongHocId = f.DonViId
                                    Left Join Bang as g
                                    on a.Id = g.HocSinhId
                                    Left Join TrangThaiBang as h
                                    on g.TrangThaiBangId = h.Id
                                    Left Join Phoi as i
                                    on g.PhoiId = i.Id
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TrangThaiBangId is null) or (a.DaInBangGoc = @DaInBangGoc))
                                            and ((@LoaiBangId is null) or (g.LoaiBangId = @LoaiBangId)) and (g.BangGocId is null)
		                                    and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@NienKhoa is null) or (e.NienKhoa like N'%'+@NienKhoa+'%')) 
		                                    and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
		                                    and ((@TrangThaiBangId is null) or ((@TrangThaiBangId = 1) and (g.HocSinhId not in (Select HocSinhId From Bang)) ) or (g.TrangThaiBangId = @TrangThaiBangId)) 
                                    Order By e.NienKhoa Desc , a.LopHocId, RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DaInBangGoc", trangThaiBangId.HasValue && (trangThaiBangId.Value >= 4) ? 1 : 0));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            danhSachVanBangViewModel.VanBangs = MapDataHelper<HocSinhTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get tong so row
                string sqlString_1 = @"Select Count(*) as 'TotalRow' 
                                    From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Left Join DonVi as f
                                    on a.TruongHocId = f.DonViId
                                    Left Join Bang as g
                                    on a.Id = g.HocSinhId
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TrangThaiBangId is null) or (a.DaInBangGoc = @DaInBangGoc))
                                            and ((@LoaiBangId is null) or (g.LoaiBangId = @LoaiBangId))
		                                    and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) 
		                                    and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) 
		                                    and ((@TrangThaiBangId is null) or ((@TrangThaiBangId = 1) and (g.HocSinhId not in (Select HocSinhId From Bang)) ) or (g.TrangThaiBangId = @TrangThaiBangId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaiBangId.HasValue ? loaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DaInBangGoc", trangThaiBangId.HasValue && (trangThaiBangId.Value >= 4) ? 1 : 0));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));

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
                    danhSachVanBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    danhSachVanBangViewModel.CurrentPage = currentPage;
                }

                return danhSachVanBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachBangViewModel GetDanhSachHocSinhDeTaoAnhVanBang(int nhomTaoVanBangId, string hoVaTen, int? donViId, int currentPage)
        {
            try
            {
                DanhSachBangViewModel danhSachBangViewModel = new DanhSachBangViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select c.*, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and ((b.DonViIn = @DonViId)or(b.DonViId = @DonViId)) and (b.IsDeleted = 0) and
		                                    ((b.TrangThaiBangId = 2) or (b.TrangThaiBangId = 3)) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%'))
                                    Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            danhSachBangViewModel.VanBangs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (danhSachBangViewModel.VanBangs == null || danhSachBangViewModel.VanBangs.Count == 0)
                {
                    return danhSachBangViewModel = new DanhSachBangViewModel();
                }

                // get nhomtaovanbang
                string sqlString_4 = @"Select * From NhomTaoVanBangs Where (Id = @Id)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", nhomTaoVanBangId));

                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // check neu in nho
                if (nhomTaoVanBangViewModel.DonViIn != null) // get truong du lieu trong bang
                {
                    // get truong du lieu theo nhom tao van bang
                    string sqlString_3 = @"Select a.TruongDuLieuCode, b.Ten as 'TenTruongDuLieu', b.KieuDuLieu
                                    From TruongDuLieuLoaiBang as a
                                    Left Join TruongDuLieu as b
                                    on a.TruongDuLieuCode = b.Code
									Where LoaiBangId = @LoaiBangId 
                                            and a.TruongDuLieuCode in (Select TruongDuLieuCode 
                                                                       From ThongTinVanBang Where BangId = @BangId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", danhSachBangViewModel.VanBangs.FirstOrDefault().Id));
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", danhSachBangViewModel.VanBangs.FirstOrDefault().LoaiBangId.Value));

                            using (var reader = command.ExecuteReader())
                            {
                                danhSachBangViewModel.TruongDuLieuLoaiBangs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else
                {
                    // get truong du lieu trong bang
                    string sqlString_3 = @"Select a.TruongDuLieuCode, b.Ten as 'TenTruongDuLieu', b.KieuDuLieu
                                    From TruongDuLieuLoaiBang as a
                                    Left Join TruongDuLieu as b
                                    on a.TruongDuLieuCode = b.Code
                                    Where (a.DonViId = @DonViId) and (a.LoaiBangId = @LoaiBangId) and (b.DonViId = @DonViId) and (a.DungChung = 0)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", danhSachBangViewModel.VanBangs.FirstOrDefault().LoaiBangId.Value));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                danhSachBangViewModel.TruongDuLieuLoaiBangs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get gia tri truong du lieu trong bang cho tung bang
                foreach (BangViewModel bangViewModel in danhSachBangViewModel.VanBangs)
                {
                    string sqlString_2 = @"Select * From ThongTinVanBang Where (BangId = @BangId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));

                            using (var reader = command.ExecuteReader())
                            {
                                bangViewModel.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }



                // get tong so row
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    ((b.TrangThaiBangId = 2) or (b.TrangThaiBangId = 3)) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%'))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));

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
                    danhSachBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    danhSachBangViewModel.CurrentPage = currentPage;
                }

                return danhSachBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachBangViewModel GetDanhSachHocSinhDeCapNhatSoHieu(int nhomTaoVanBangId, string hoVaTen, int? trangThaiBangId, int? donViId, int currentPage)
        {
            try
            {
                DanhSachBangViewModel danhSachBangViewModel = new DanhSachBangViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select c.*,a.BangCuId, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang',
								            Case 
									            When f.LoaiBangGocId is null Then 0
									            Else 1
								            End as 'IsBanSao'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Left Join LoaiBang as f
                                      on f.Id = b.LoaiBangId
                                      Where (b.Id = @NhomTaoVanBangId) and ((b.DonViIn = @DonViId)or(b.DonViId = @DonViId)) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId >= 3) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%'))
                                    Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            danhSachBangViewModel.VanBangs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (danhSachBangViewModel.VanBangs == null || danhSachBangViewModel.VanBangs.Count == 0)
                {
                    return danhSachBangViewModel = new DanhSachBangViewModel();
                }


                List<int> removeIds = new List<int>();
                List<BangViewModel> bangCuViewModels = new List<BangViewModel>();
                foreach (BangViewModel bangViewModel in danhSachBangViewModel.VanBangs.Where(x => x.BangCuId != null))
                {
                    string sqlString_2 = @"Select c.*,a.BangCuId, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang',
                                            Case 
									            When f.LoaiBangGocId is null Then 0
									            Else 1
								            End as 'IsBanSao'
                                      From HocSinhTrongNhomTaoVanBangs as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join BangCus as c 
                                      on a.BangCuId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Left Join LoaiBang as f
                                      on f.Id = b.LoaiBangId
                                      Where c.Id = @BangCuId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangCuId", bangViewModel.BangCuId));

                            using (var reader = command.ExecuteReader())
                            {
                                var item = MapDataHelper<BangViewModel>.Map(reader);
                                if (item != null && item.Id != 0)
                                {
                                    bangCuViewModels.Add(item);
                                }
                            }
                            removeIds.Add(bangViewModel.Id);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                foreach (int id in removeIds)
                {
                    danhSachBangViewModel.VanBangs.Remove(danhSachBangViewModel.VanBangs.Where(x => x.Id == id).FirstOrDefault());
                }

                danhSachBangViewModel.VanBangs.AddRange(bangCuViewModels);

                // get gia tri truong du lieu trong bang cho tung bang
                foreach (BangViewModel bangViewModel in danhSachBangViewModel.VanBangs)
                {
                    if (bangViewModel.BangCuId == null)
                    {
                        string sqlString_2 = @"Select * From ThongTinVanBang Where (BangId = @BangId)";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                                using (var reader = command.ExecuteReader())
                                {
                                    bangViewModel.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                    else
                    {
                        string sqlString_2 = @"Select * From ThongTinVanBangCus Where (BangId = @BangCuId)";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@BangCuId", bangViewModel.Id));

                                using (var reader = command.ExecuteReader())
                                {
                                    bangViewModel.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                }

                // get nhomtaovanbang
                string sqlString_4 = @"Select a.*,
                                        Case 
									        When b.LoaiBangGocId is null Then 0
									        Else 1
								        End as 'IsBanSao'
                                        From NhomTaoVanBangs as a
                                        Left Join LoaiBang as b
                                        on a.LoaiBangId = b.Id
                                        Where (a.Id = @Id)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", nhomTaoVanBangId));

                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // check neu in nho
                if (nhomTaoVanBangViewModel.DonViIn != null) // get truong du lieu trong bang
                {
                    // get truong du lieu theo nhom tao van bang
                    string sqlString_3 = @"Select a.TruongDuLieuCode, b.Ten as 'TenTruongDuLieu', b.KieuDuLieu
                                    From TruongDuLieuLoaiBang as a
                                    Left Join TruongDuLieu as b
                                    on a.TruongDuLieuCode = b.Code
									Where LoaiBangId = @LoaiBangId 
                                            and a.TruongDuLieuCode in (Select TruongDuLieuCode 
                                                                       From ThongTinVanBang Where BangId = @BangId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", danhSachBangViewModel.VanBangs.FirstOrDefault().Id));
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", danhSachBangViewModel.VanBangs.FirstOrDefault().LoaiBangId));

                            using (var reader = command.ExecuteReader())
                            {
                                danhSachBangViewModel.TruongDuLieuLoaiBangs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                else
                {
                    // get truong du lieu loai bang va donvi
                    string sqlString_3 = @"Select a.TruongDuLieuCode, b.Ten as 'TenTruongDuLieu', b.KieuDuLieu
                                    From TruongDuLieuLoaiBang as a
                                    Left Join TruongDuLieu as b
                                    on a.TruongDuLieuCode = b.Code
                                    Where (a.DonViId = @DonViId) and (a.LoaiBangId = @LoaiBangId) and (b.DonViId = @DonViId) and (a.DungChung = 0)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", danhSachBangViewModel.VanBangs.FirstOrDefault().LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                danhSachBangViewModel.TruongDuLieuLoaiBangs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                }


                // get tong so row
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and ((b.DonViIn = @DonViId)or(b.DonViId = @DonViId)) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId >= 3) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%'))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));

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
                    danhSachBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                }

                danhSachBangViewModel.CurrentPage = currentPage;

                return danhSachBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DanhSachHocSinhTaoVanBangViewModel GetHocSinhTaoVanBang(int nhomTaoVanBangId, string hoVaTen, int donViId, int currentPage)
        {
            try
            {
                DanhSachHocSinhTaoVanBangViewModel danhSachHocSinhTaoBangViewModel = new DanhSachHocSinhTaoVanBangViewModel();
                // get nhomtaovanbang
                string sqlString_6 = @"Select * From NhomTaoVanBangs Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_6;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (nhomTaoVanBangViewModel.Id == 0)
                {
                    Exception exception = new Exception("Không tìm thấy dữ liệu!!!");
                    throw exception;
                }


                // get truong ko dung chung
                string sqlString = @"Select a.*, b.Ten From [TruongDuLieuLoaiBang] as a
                                      Left Join TruongDuLieu as b
                                      On a.TruongDuLieuCode = b.Code
                                      Where (a.LoaiBangId = @LoaiBangId) and (a.DungChung = 0) and (b.DonViId = @DonViId) ";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            danhSachHocSinhTaoBangViewModel.TruongKhongDungChungs = MapDataHelper<TruongDuLieuLoaiBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get danh sach hoc sinh can in bang
                string sqlString_1 = @"Select c.*, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId <= 3) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%'))
                                    Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc
                                    Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));
                        using (var reader = command.ExecuteReader())
                        {
                            danhSachHocSinhTaoBangViewModel.HocSinhs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (danhSachHocSinhTaoBangViewModel.HocSinhs == null || danhSachHocSinhTaoBangViewModel.HocSinhs.Count == 0)
                {
                    return danhSachHocSinhTaoBangViewModel = new DanhSachHocSinhTaoVanBangViewModel();
                }

                // get truong du lieu trong bang va map du lieu vao hoc sinh
                string sqlString_2 = @"Select c.Ten as 'Ten', b.*  From LoaiBang as a
                                            Left Join TruongDuLieuLoaiBang as b
                                            on a.Id = b.LoaiBangId
                                            left Join TruongDuLieu as c
                                            on b.TruongDuLieuCode = c.Code
                                            Where (a.Id = @LoaiBangId) and (b.DonViId = @DonViId)";
                List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            truongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (danhSachHocSinhTaoBangViewModel.HocSinhs != null)
                {
                    foreach (BangViewModel bangViewModel in danhSachHocSinhTaoBangViewModel.HocSinhs)
                    {
                        string sqlString_5 = @"Select * From Bang as a 
                                                Left Join ThongTinVanBang as b
                                                on a.Id = b.BangId
                                                Left Join TruongDuLieuLoaiBang as c
                                                on  b.TruongDuLieuCode = c.TruongDuLieuCode and a.LoaiBangId = c.LoaiBangId
                                                where (a.Id = @BangId) and (a.DonViId = @DonViId) and (c.DonViId = @DonViId)";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_5;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                                using (var reader = command.ExecuteReader())
                                {
                                    bangViewModel.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }

                        if (bangViewModel.TruongDuLieuTrongBangs != null && bangViewModel.TruongDuLieuTrongBangs.Count != 0)
                        {
                            continue;
                        }

                    }
                }

                // get total page
                int totalRow = 0;
                string sqlString_4 = @"Select Count(*) as 'TotalRow'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId <= 3) and 
                                            ((@HoVaTen is null) or (c.HoVaTen like N'%'+@HoVaTen+'%')) ";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
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

                if (danhSachHocSinhTaoBangViewModel.HocSinhs != null && danhSachHocSinhTaoBangViewModel.HocSinhs.Count != 0)
                {
                    foreach (var truongDuLieuTrongBang in danhSachHocSinhTaoBangViewModel.HocSinhs.FirstOrDefault().TruongDuLieuTrongBangs.Where(x => x.DungChung.Value))
                    {
                        var truongDuLieu = truongDuLieuTrongBangs.Where(x => (x.DungChung.Value) && (x.TruongDuLieuCode == truongDuLieuTrongBang.TruongDuLieuCode)).FirstOrDefault();
                        if (truongDuLieu != null)
                        {
                            truongDuLieu.GiaTri = truongDuLieuTrongBang.GiaTri;
                        }
                    }
                }
                danhSachHocSinhTaoBangViewModel.TruongDungChungs = truongDuLieuTrongBangs.Where(x => x.DungChung == true).ToList();
                danhSachHocSinhTaoBangViewModel.CurrentPage = currentPage;
                danhSachHocSinhTaoBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(1.0 * totalRow / 12));
                danhSachHocSinhTaoBangViewModel.TotalRow = totalRow;
                danhSachHocSinhTaoBangViewModel.LoaiBangId = nhomTaoVanBangViewModel.LoaiBangId.Value;
                return danhSachHocSinhTaoBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddThongTinVanBang(AddThongTinVanBangViewModel addThongTinVanBangViewModel, int donViId, string ip)
        {
            try
            {
                // get nhom van bang
                string sqlString_4 = @"Select * From NhomTaoVanBangs Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // check 2 folder anh 
                if (string.IsNullOrEmpty(nhomTaoVanBangViewModel.DuongDanFileAnh) || !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnh)))
                {
                    nhomTaoVanBangViewModel.DuongDanFileAnh = Path.Combine("Upload", "ThongTinVanBang", nhomTaoVanBangViewModel.Id + "_" +  DateTime.Now.ToString("dd_MM_yyyy"));
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnh));
                    string sqlString_5 = @"Update NhomTaoVanBangs
                                        Set DuongDanFileAnh = @DuongDanFileAnh
                                        Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnh", nhomTaoVanBangViewModel.DuongDanFileAnh));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                if (string.IsNullOrEmpty(nhomTaoVanBangViewModel.DuongDanFileAnhDeIn) || !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnhDeIn)))
                {
                    nhomTaoVanBangViewModel.DuongDanFileAnhDeIn = Path.Combine("Upload", "ThongTinVanBangDeIn", nhomTaoVanBangViewModel.Id + "_" + DateTime.Now.ToString("dd_MM_yyyy"));
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnhDeIn));
                    string sqlString_5 = @"Update NhomTaoVanBangs
                                        Set DuongDanFileAnhDeIn = @DuongDanFileAnhDeIn
                                        Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnhDeIn", nhomTaoVanBangViewModel.DuongDanFileAnhDeIn));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }



                // get danh sach hoc sinh can in bang
                string sqlString = @"Select c.*, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    (b.TrangThaiBangId <= 3)
                                      Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                List<BangViewModel> hocSinhTaoVanBangViewModels = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhTaoVanBangViewModels = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }



                // check ban sao hay bang goc
                string sqlString_8 = "Select Count(*) as 'TotalRow' From LoaiBang Where (Id = @LoaiBangId) and (LoaiBangGocId is not null)";
                int isBanSao = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_8;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isBanSao = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                string soHieuBanSao = "";
                // lay so hieu ban sao neu la ban sao
                if (isBanSao == 1)
                {
                    var truongDuLieuSoHieuBanSao = addThongTinVanBangViewModel.TruongDulLieuDungChungs.Where(x => x.TruongDuLieuCode.Contains("-SOHIEU")).FirstOrDefault();
                    if (truongDuLieuSoHieuBanSao == null)
                    {
                        Exception exception = new Exception("Không tìm thấy số hiệu bản sao");
                        throw exception;
                    }
                    else
                    {
                        soHieuBanSao = truongDuLieuSoHieuBanSao.GiaTri;
                        int totalRow = 0;
                        // Check trung so hieu bản sao
                        int i = 0;
                        List<string> sqlParams = hocSinhTaoVanBangViewModels.Select(x => "@Id_" + i++).ToList();
                        string sqlString_9 = string.Format(@"Select Count(*) as 'TotalRow' From Bang Where (SoHieu Like @SoHieu) and (Id not in ({0}))", string.Join(",", sqlParams.ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_9;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@SoHieu", soHieuBanSao));
                                i = 0;
                                foreach (var hocSinhTaoVanBangViewModel in hocSinhTaoVanBangViewModels)
                                {
                                    command.Parameters.Add(new SqlParameter("@Id_" + i++, hocSinhTaoVanBangViewModel.Id != 0 ? hocSinhTaoVanBangViewModel.Id : DBNull.Value));
                                }
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        totalRow = Convert.ToInt32(reader[0]);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }

                        if (totalRow > 0)
                        {
                            Exception exception = new Exception("Đã tồn tại số hiệu bản sao");
                            throw exception;
                        }

                    }


                }
                int sizeOfSet = 2000;
                // xóa hết dữ liệu cũ (tối da param truyền vào là 2100 params nên phải tách ra nhiều set để thực hiện)
                if (addThongTinVanBangViewModel.TruongDulLieuDungChungs != null && addThongTinVanBangViewModel.TruongDulLieuDungChungs.Count > 0)
                {
                    int numberOfSet = (int)Math.Ceiling(addThongTinVanBangViewModel.TruongDulLieuDungChungs.Count * 1.0 / sizeOfSet);
                    for (int k = 0; k < numberOfSet; k++)
                    {
                        int i = 0;
                        var set = hocSinhTaoVanBangViewModels.Skip(sizeOfSet * i).Take(sizeOfSet);
                        List<string> sqlParams = set.Select(x => "@Id_" + i++).ToList();
                        string sqlString_1 = string.Format(@"Delete From ThongTinVanBang 
                                        Where (BangId In ({0})) and  (TruongDuLieuCode in ({1}))",
                                            string.Join(",", sqlParams.ToArray()),
                                            string.Join(",", addThongTinVanBangViewModel.TruongDulLieuDungChungs.Select(x => "'" + x.TruongDuLieuCode + "'").ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                i = 0;
                                command.CommandText = sqlString_1;
                                command.CommandType = System.Data.CommandType.Text;
                                foreach (var hocSinhTaoVanBangViewModel in set)
                                {
                                    command.Parameters.Add(new SqlParameter("@Id_" + i++, hocSinhTaoVanBangViewModel.Id != 0 ? hocSinhTaoVanBangViewModel.Id : DBNull.Value));
                                }
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                }

                // add vao bang bang
                List<Bang> bangs = new List<Bang>();
                foreach (BangViewModel hocSinhTaoVanBangViewModel in hocSinhTaoVanBangViewModels)
                {
                    Bang bang = new Bang();
                    bang.Id = hocSinhTaoVanBangViewModel.Id;
                    bang.HocSinhId = hocSinhTaoVanBangViewModel.HocSinhId;
                    bang.TruongHocId = hocSinhTaoVanBangViewModel.TruongHocId;
                    bang.YeuCauId = -1;
                    bang.LoaiBangId = nhomTaoVanBangViewModel.LoaiBangId;
                    bang.DonViId = donViId;
                    bang.TrangThaiBangId = 2;
                    bang.SoHieu = hocSinhTaoVanBangViewModel.SoHieu;
                    bang.SoVaoSo = hocSinhTaoVanBangViewModel.SoVaoSo;
                    bang.HoVaTen = hocSinhTaoVanBangViewModel.HoVaTen;
                    bang.NgayTao = addThongTinVanBangViewModel.NgayTao;
                    bang.NgayCapNhat = addThongTinVanBangViewModel.NgayCapNhat;
                    bang.NguoiTao = addThongTinVanBangViewModel.NguoiTao;
                    bang.NguoiCapNhat = addThongTinVanBangViewModel.NguoiCapNhat;
                    bang.IsDeleted = false;
                    bangs.Add(bang);

                    // ban sao
                    if (isBanSao == 1)
                    {
                        string sqlString_2 = @"Update Bang 
                                            Set NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat, TrangThaiBangId = 2, SoHieu = @SoHieu
                                            Where Id = @Id";

                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NgayCapNhat", addThongTinVanBangViewModel.NgayCapNhat));
                                command.Parameters.Add(new SqlParameter("@NguoiCapNhat", addThongTinVanBangViewModel.NguoiCapNhat));
                                command.Parameters.Add(new SqlParameter("@Id", hocSinhTaoVanBangViewModel.Id));
                                command.Parameters.Add(new SqlParameter("@SoHieu", soHieuBanSao));
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                }

                if (isBanSao != 1)// bang goc
                {
                    //(tối da param truyền vào là 2100 params nên phải tách ra nhiều set để thực hiện)
                    int numberOfSet = (int)Math.Ceiling(addThongTinVanBangViewModel.TruongDulLieuDungChungs.Count * 1.0 / sizeOfSet);
                    for (int k = 0; k < numberOfSet; k++)
                    {
                        int i = 0;
                        var set = hocSinhTaoVanBangViewModels.Skip(sizeOfSet * k).Take(sizeOfSet);
                        List<string> sqlParams = set.Select(x => "@Id_" + i++).ToList();

                        string sqlString_2 = string.Format(@"Update Bang 
                                            Set NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat, TrangThaiBangId = 2
                                            Where Id in ({0})", string.Join(",", sqlParams.ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                i = 0;
                                command.CommandText = sqlString_2;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NgayCapNhat", addThongTinVanBangViewModel.NgayCapNhat));
                                command.Parameters.Add(new SqlParameter("@NguoiCapNhat", addThongTinVanBangViewModel.NguoiCapNhat));
                                foreach (var hocSinhTaoVanBangViewModel in set)
                                {
                                    command.Parameters.Add(new SqlParameter("@Id_" + i++, hocSinhTaoVanBangViewModel.Id != 0 ? hocSinhTaoVanBangViewModel.Id : DBNull.Value));
                                }
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                }

                // update hoc sinh trong nhom tao van bang
                //(tối da param truyền vào là 2100 params nên phải tách ra nhiều set để thực hiện)
                {
                    int numberOfSet = (int)Math.Ceiling(addThongTinVanBangViewModel.TruongDulLieuDungChungs.Count * 1.0 / sizeOfSet);
                    for (int k = 0; k < numberOfSet; k++)
                    {
                        int i = 0;
                        var set = bangs.Skip(sizeOfSet * k).Take(sizeOfSet);
                        List<string> sqlParams = set.Select(x => "@Id_" + i++).ToList();

                        string sqlString_5 = string.Format(@"Update [HocSinhTrongNhomTaoVanBangs]
                                                        Set TrangThaiBangId = 2
                                                        Where (BangId in ({0})) and (NhomTaoVanBangId = @NhomTaoVanBangId)", string.Join(",", sqlParams.ToArray()));
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();
                            i = 0;
                            try
                            {
                                command.CommandText = sqlString_5;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                                foreach (var bang in set)
                                {
                                    command.Parameters.Add(new SqlParameter("@Id_" + i++, bang.Id));
                                }
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                }

                // update nhom tao van bang
                string sqlString_6 = string.Format(@"Update NhomTaoVanBangs
                                                        Set TrangThaiBangId = 2, NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat,
                                                             CanDelete = Case 
					                                                        When (AddedByImport = 1) and (DonViIn Is Null ) and (TrangThaiBangId <= 3) Then 1
					                                                        Else 0
				                                                         End
                                                        Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)");
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_6;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", addThongTinVanBangViewModel.NhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@NgayCapNhat", addThongTinVanBangViewModel.NgayCapNhat));
                        command.Parameters.Add(new SqlParameter("@NguoiCapNhat", addThongTinVanBangViewModel.NguoiCapNhat));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // add log
                List<LogVanBang> logVanBangs = new List<LogVanBang>();
                foreach (Bang bang in bangs)
                {
                    var objLog = new LogVanBang();
                    var user = new NguoiDungProvider().GetById(addThongTinVanBangViewModel.NguoiCapNhat.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã tạo thông tin văn bằng cho học sinh " + bang.HoVaTen;
                    objLog.VanBangId = bang.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    logVanBangs.Add(objLog);
                }
                DbContext.LogVanBangs.AddRange(logVanBangs);
                DbContext.SaveChanges();

                if (true)
                {
                    // get truong du lieu QRCode va sovaoso neu cos
                    string sqlString_3 = @"Select c.Ten as 'Ten', b.*  From LoaiBang as a
                                            Left Join TruongDuLieuLoaiBang as b
                                            on a.Id = b.LoaiBangId
                                            left Join TruongDuLieu as c
                                            on b.TruongDuLieuCode = c.Code
                                            Where (a.Id = @LoaiBangId) and (b.DonViId = @DonViId) and (b.TenTruongDuLieu like 'SoVaoSo')";
                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    if (truongDuLieuTrongBangs.Any(x => x.KieuDuLieu == 2) && truongDuLieuTrongBangs.Count() >= 2)
                    {
                        foreach (Bang bang in bangs)
                        {
                            string soVaoSoCode = truongDuLieuTrongBangs.Where(x => x.KieuDuLieu == 1).FirstOrDefault().TruongDuLieuCode;
                            string sqlString_5 = @"Select * From ThongTinVanBang 
                                                    Where (BangId = @BangId) and (TruongDuLieuCode Like @SoVaoSo)";
                            TruongDuLieuTrongBangViewModel qrCodeViewModel = new TruongDuLieuTrongBangViewModel();
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_5;
                                    command.CommandType = System.Data.CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@BangId", bang.Id));
                                    command.Parameters.Add(new SqlParameter("@SoVaoSo", soVaoSoCode));
                                    using (var reader = command.ExecuteReader())
                                    {
                                        qrCodeViewModel = MapDataHelper<TruongDuLieuTrongBangViewModel>.Map(reader);
                                    }
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }

                            if (qrCodeViewModel.BangId == 0)
                            {
                                Exception exception = new Exception("Không tìm thấy số vào sổ");
                                throw exception;
                            }

                            string soVaoSo = qrCodeViewModel.GiaTri;

                            foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangs.Where(x => x.KieuDuLieu == 2))
                            {
                                string sqlString_7 = @"Update ThongTinVanBang 
                                                    Set GiaTri = @GiaTri
                                                    Where (TruongDuLieuCode like @TruongDuLieuCode) and (BangId = @BangId)";
                                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                                {
                                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                                    if (!wasOpen) command.Connection.Open();

                                    try
                                    {
                                        command.CommandText = sqlString_7;
                                        command.CommandType = System.Data.CommandType.Text;
                                        command.Parameters.Add(new SqlParameter("@BangId", bang.Id));
                                        command.Parameters.Add(new SqlParameter("@GiaTri", QRHelper.QRGenerator(soVaoSo + "&bangId=" + bang.Id)));
                                        command.Parameters.Add(new SqlParameter("@TruongDuLieuCode", truongDuLieuTrongBangViewModel.TruongDuLieuCode));
                                        command.ExecuteNonQuery();
                                    }
                                    finally
                                    {
                                        command.Connection.Close();
                                    }
                                }
                            }
                        }
                    }
                }

                // add thong tin van bang
                if (hocSinhTaoVanBangViewModels != null)
                {
                    List<ThongTinVanBang> thongTinVanBangs = new List<ThongTinVanBang>();
                    foreach (Bang bang in bangs)
                    {
                        foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBang in addThongTinVanBangViewModel.TruongDulLieuDungChungs)
                        {
                            ThongTinVanBang thongTinVanBang = new ThongTinVanBang();
                            thongTinVanBang.TruongDuLieuCode = truongDuLieuTrongBang.TruongDuLieuCode;
                            thongTinVanBang.BangId = bang.Id;
                            thongTinVanBang.GiaTri = truongDuLieuTrongBang.GiaTri;
                            thongTinVanBang.NgayTao = addThongTinVanBangViewModel.NgayTao;
                            thongTinVanBang.NguoiTao = addThongTinVanBangViewModel.NguoiTao;
                            thongTinVanBang.NgayCapNhat = addThongTinVanBangViewModel.NgayCapNhat;
                            thongTinVanBang.NguoiCapNhat = addThongTinVanBangViewModel.NguoiCapNhat;
                            thongTinVanBang.DonViId = donViId;
                            thongTinVanBangs.Add(thongTinVanBang);
                        }
                    }

                    DbContext.ThongTinVanBangs.AddRange(thongTinVanBangs);
                    DbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void TaoAnhVanBangTheoYeuCau(List<TaoAnhVanBangTheoYeuCauViewModel> taoAnhVanBangTheoYeuCauViewModels, int donViId)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBang", "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString());
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string folderPath_1 = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBangDeIn", "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString());
                if (!Directory.Exists(folderPath_1))
                {
                    Directory.CreateDirectory(folderPath_1);
                }

                foreach (TaoAnhVanBangTheoYeuCauViewModel taoAnhVanBangTheoYeuCauViewModel in taoAnhVanBangTheoYeuCauViewModels)
                {
                    //Phoi phoi = DbContext.Phois.Find(taoAnhVanBangTheoYeuCauViewModel.PhoiId);
                    Bang bang = DbContext.Bangs.Find(taoAnhVanBangTheoYeuCauViewModel.BangId);
                    HocSinh hocSinh = DbContext.HocSinhs.Find(bang.HocSinhId);
                    LoaiBang loaiBang = DbContext.LoaiBangs.Find(bang.LoaiBangId);
                    //if (phoi == null || bang == null)
                    //{
                    //    continue;
                    //}
                    string sqlString = @"Select a.*, b.X, b.Y, b.[Format], b.Font, b.Color, b.Bold, b.Italic, b.Underline, b.Size From ThongTinVanBang as a
                                                Left Join TruongDuLieuLoaiBang as b
                                                on b.TruongDuLieuCode = a.TruongDuLieuCode
                                                Where (b.LoaiBangId = @LoaiBangId) and (a.BangId = @BangId) and (b.DonViId = @DonViId)";

                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", bang.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@BangId", bang.Id));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuTrongBangViewModels = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }


                    int width = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Width.Value));
                    int height = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Height.Value));

                    // drawing text trong thong tin van bang  co them background phoi bang
                    string sqlString_2 = @"Select * From [AnhLoaiBang] Where ObjectId = @LoaiBangId";
                    AnhLoaiBang anhLoaiBang = DbContext.AnhLoaiBangs.FromSqlRaw(sqlString_2, new SqlParameter("@LoaiBangId", loaiBang.Id)).FirstOrDefault();
                    Image img = null;
                    if (anhLoaiBang != null)
                    {
                        string backgroundPath = Path.Combine("~", anhLoaiBang.Url);
                        img = new Bitmap(Image.FromFile(backgroundPath), width, height);
                    }
                    else
                    {
                        img = new Bitmap(width, height);
                    }
                    Graphics drawing = Graphics.FromImage(img);
                    //drawing.Clear(Color.FromArgb(255, 255, 255));
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = truongDuLieuTrongBangViewModel.X.Value;
                        int y = truongDuLieuTrongBangViewModel.Y.Value;
                        DrawText(drawing, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                    }
                    drawing.Save();
                    drawing.Dispose();

                    // drawing text trong thong tin van bang de in
                    Image img_1 = new Bitmap(width, height);
                    Graphics drawing_1 = Graphics.FromImage(img_1);
                    drawing_1.Clear(Color.FromArgb(255, 255, 255));
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = /*Convert.ToInt32(Math.Ceiling(3.779527559 * truongDuLieuTrongBangViewModel.X));*/ truongDuLieuTrongBangViewModel.X.Value;
                        int y = /*Convert.ToInt32(Math.Ceiling(3.779527559 * truongDuLieuTrongBangViewModel.Y));*/ truongDuLieuTrongBangViewModel.Y.Value;
                        DrawText(drawing_1, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                    }
                    drawing_1.Save();
                    drawing_1.Dispose();

                    string fileName = bang.Id + "_" + hocSinh.SoVaoSo + ".png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBang", "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString(), fileName);
                    string path_1 = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBangDeIn", "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString(), fileName);
                    bang.TrangThaiBangId = 3; // Đã xuất ra ảnh, chờ in
                    bang.DuongDanFileAnh = "/Upload/ThongTinVanBang/" + "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString() + "/" + fileName;
                    bang.DuongDanFileDeIn = "/Upload/ThongTinVanBangDeIn/" + "YeuCau_" + taoAnhVanBangTheoYeuCauViewModels.FirstOrDefault().YeuCauId.ToString() + "/" + fileName;

                    // update trang thai bang cua hoc sinh trong lien ket yeu cau
                    string sqlString_1 = @"Select * From [LienKetHocSinhYeuCau] Where (YeuCauId = @YeuCauId) and (HocSinhId = @HocSinhId)";
                    List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                    sqlParameters_1.Add(new SqlParameter("@YeuCauId", bang.YeuCauId));
                    sqlParameters_1.Add(new SqlParameter("@HocSinhId", bang.HocSinhId));
                    LienKetHocSinhYeuCau lienKetHocSinhYeuCau = DbContext.LienKetHocSinhYeuCaus.FromSqlRaw(sqlString_1, sqlParameters_1.ToArray()).FirstOrDefault();
                    if (lienKetHocSinhYeuCau != null)
                    {
                        lienKetHocSinhYeuCau.TrangThaiCapPhatBang = 3;
                    }
                    DbContext.SaveChanges();

                    img.Save(path, ImageFormat.Png);
                    img_1.Save(path_1, ImageFormat.Png);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateTrangThaiDaIn(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel, int donViId, string ip)
        {
            try
            {
                // get nhomtaovanbang lay loai bang
                string sqlString_8 = @"Select * From NhomTaoVanBangs Where Id = @NhomTaoVanBangId";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_8;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get bang
                string sqlString_3 = @"Select *
                                        From Bang
                                        Where (Id = @BangId)";
                BangViewModel bangViewModel = new BangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = System.Data.CommandType.Text;
                        //command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@BangId", hocSinhTaoVanBangViewModel.Id));
                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModel = MapDataHelper<BangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (bangViewModel.Id == 0)
                {
                    Exception exception = new Exception("Không tìm thấy bằng");
                    throw exception;
                }

                // update trang thai cho bang / phoi / hoc sinh trong nhom tao van bang
                if (bangViewModel.BangGocId == null)// bang goc
                {
                    string sqlString = @"Select * From Phoi Where (SoHieu Like @SoHieu) and (DonViId = @DonViId) and (LoaiBangId = @LoaiBangId)";
                    PhoiViewModel phoiViewModel = new PhoiViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoHieu", hocSinhTaoVanBangViewModel.SoHieu));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
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

                    if (phoiViewModel.Id == 0)
                    {
                        Exception exception = new Exception("Không tìm thấy số hiệu!!!");
                        throw exception;
                    }
                    else if (phoiViewModel.TrangThaiPhoiId == 2)
                    {
                        Exception exception = new Exception("Số hiệu này đã được sử dụng!!!");
                        throw exception;
                    }
                    else if (phoiViewModel.TrangThaiPhoiId == 3)
                    {
                        Exception exception = new Exception("Số hiệu này đang lỗi!!!");
                        throw exception;
                    }
                    else if (phoiViewModel.TrangThaiPhoiId == 4)
                    {
                        Exception exception = new Exception("Số hiệu này đã được thu hồi!!!");
                        throw exception;
                    }

                    // xoa bo phoi cu trong bang
                    if (bangViewModel.PhoiId != null && bangViewModel.PhoiId != phoiViewModel.Id)
                    {
                        string sqlString_4 = @"Update Phoi
                                           Set TrangThaiPhoiId = 1
                                           Where (Id Like @PhoiId) and (DonViId = @DonViId);
                                           Update Bang
                                           Set TrangThaiBangId = 3, PhoiId = null
                                           Where (Id = @BangId); 
                                           Update HocSinh
                                           Set DaInBangGoc = 0
                                           Where (Id = @HocSinhId);";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_4;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@PhoiId", bangViewModel.PhoiId));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                                command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                                command.Parameters.Add(new SqlParameter("@HocSinhId", bangViewModel.HocSinhId));
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }


                    }

                    string soHieu = phoiViewModel.SoHieu;
                    string soVaoSo = bangViewModel.SoVaoSo;
                    string truongHoc = bangViewModel.TruongHoc;
                    string hoVaTen_1 = bangViewModel.HoVaTen;
                    string sqlString_7 = @"Update Phoi
                                           Set TrangThaiPhoiId = 2
                                           Where (SoHieu Like @SoHieu) and (DonViId = @DonViId);
                                           Update Bang
                                           Set TrangThaiBangId = 4, PhoiId = @PhoiId, NgayInBang = @NgayInBang,
                                                SoHieu = @SoHieu, SoVaoSo = @SoVaoSo, TruongHoc = @TruongHoc, HoVaTen = @HoVaTen, NamTotNghiep = @NamTotNghiep
                                           Where (Id = @BangId);
                                           Update HocSinh
                                           Set DaInBangGoc = 1
                                           Where (Id = @HocSinhId);
                                           Update HocSinhTrongNhomTaoVanBangs
                                           Set TrangThaiBangId = 4
                                           Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (BangId = @BangId);
                                           Update NhomTaoVanBangs
                                           Set ChoPhepTaoLai = 0
                                           Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_7;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@PhoiId", phoiViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@HocSinhId", bangViewModel.HocSinhId));
                            command.Parameters.Add(new SqlParameter("@SoHieu", soHieu));
                            command.Parameters.Add(new SqlParameter("@SoVaoSo", soVaoSo));
                            command.Parameters.Add(new SqlParameter("@TruongHoc", truongHoc));
                            command.Parameters.Add(new SqlParameter("@HoVaTen", hoVaTen_1));
                            command.Parameters.Add(new SqlParameter("@NamTotNghiep", bangViewModel.NamTotNghiep));
                            command.Parameters.Add(new SqlParameter("@NgayInBang", DateTime.Now));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // chuyen sang trang thai van bang 4 neu nhu da in toan bo danh sach
                    string sqlString_5 = @"Select Count(*) as 'TotalRow' From HocSinhTrongNhomTaoVanBangs
                                           Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (TrangThaiBangId <= 3)";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
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

                    // update trang thai nhom tao van bang
                    if (totalRow == 0)
                    {
                        string sqlString_6 = @"Update NhomTaoVanBangs
                                               Set TrangThaiBangId = 4
                                               Where (Id = @NhomTaoVanBangId) and ((DonViId = @DonViId) or (DonViIn = @DonViId))";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_6;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));
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
                    }

                }
                else // ban sao
                {
                    // get ngau nhien phoi ban sao
                    string sqlString_1 = @"Select top 1 Id From Phoi Where (DonViId = @DonViId) and (LoaiBangId = @LoaiBangId) and (TrangThaiPhoiId = 1)";
                    int? phoiId = null;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    phoiId = Convert.ToInt32(reader[0]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                    if (phoiId == null)
                    {
                        Exception exception = new Exception("Không đủ phôi bản sao để in");
                        throw exception;
                    }

                    // update nhomtaovanbang, bang, phoi ban sao
                    string sqlString_2 = @"Update Bang
                                           Set TrangThaiBangId = 4, PhoiId = @PhoiId, NgayInBang = @NgayInBang
                                           Where (Id = @BangId);
                                           Update HocSinhTrongNhomTaoVanBangs
                                           Set TrangThaiBangId = 4
                                           Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (BangId = @BangId);
                                           Update NhomTaoVanBangs
                                           Set ChoPhepTaoLai = 0
                                           Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId);
                                           Update Phoi
                                           Set TrangThaiPhoiId = 2, SoHieu = @SoHieu Where Id = @PhoiId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@PhoiId", phoiId.Value));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@SoHieu", bangViewModel.SoHieu));
                            command.Parameters.Add(new SqlParameter("@NgayInBang", DateTime.Now));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // chuyen sang trang thai van bang 4 neu nhu da in toan bo danh sach
                    string sqlString_4 = @"Select Count(*) as 'TotalRow' From HocSinhTrongNhomTaoVanBangs
                                           Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (TrangThaiBangId <= 3)";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_4;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
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

                    // update trang thai nhom tao van bang
                    if (totalRow == 0)
                    {
                        string sqlString_5 = @"Update NhomTaoVanBangs
                                               Set TrangThaiBangId = 4
                                               Where (Id = @NhomTaoVanBangId) and ((DonViId = @DonViId) or (DonViIn = @DonViId))";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_5;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", hocSinhTaoVanBangViewModel.NhomTaoVanBangId));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));
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
                    }

                }

                // add log
                var objLog = new LogVanBang();
                var user = new NguoiDungProvider().GetById(hocSinhTaoVanBangViewModel.NguoiCapNhat);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = (bangViewModel.BangGocId.HasValue ? "Đã in bản sao cho học sinh " : "Đã in bằng cho học sinh ") + bangViewModel.HoVaTen;
                objLog.VanBangId = hocSinhTaoVanBangViewModel.Id;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                DbContext.LogVanBangs.Add(objLog);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateTrangThaiPhatBang(PhatVanBangViewModel phatVanBangViewModel, int donViId, string ip)
        {
            try
            {
                // get van bang
                BangViewModel bangViewModel = new BangViewModel();
                string sqlString = @"Select a.*, b.HoVaTen From Bang as a
                                      Left Join HocSinh as b
                                      on a. HocSinhId = b.Id
                                      Where a.id = @BangId";

                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", phatVanBangViewModel.BangId));
                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModel = MapDataHelper<BangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // update trang thai
                string sqlString_1 = @"Update Bang
                                       Set TrangThaiBangId = 6, CMTNguoiLayBang = @CMTNguoiLayBang, QuanHeVoiNguoiDuocCapBang = @QuanHeVoiNguoiDuocCapBang,
                                            SoDienThoaiNguoiLayBang = @SoDienThoaiNguoiLayBang, HinhThucNhan = @HinhThucNhan,
                                            NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat, NgayPhatBang = @NgayPhatBang
                                       Where id = @BangId;
                                       Update HocSinhTrongNhomTaoVanBangs
                                       Set TrangThaiBangId = 6
                                       Where BangId = @BangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@CMTNguoiLayBang", phatVanBangViewModel.CMTNguoiLayBang));
                        command.Parameters.Add(new SqlParameter("@QuanHeVoiNguoiDuocCapBang", string.IsNullOrEmpty(phatVanBangViewModel.QuanHeVoiNguoiDuocCapBang) ? "" : phatVanBangViewModel.QuanHeVoiNguoiDuocCapBang));
                        command.Parameters.Add(new SqlParameter("@SoDienThoaiNguoiLayBang", phatVanBangViewModel.SoDienThoaiNguoiLayBang));
                        command.Parameters.Add(new SqlParameter("@HinhThucNhan", phatVanBangViewModel.HinhThucNhan));
                        command.Parameters.Add(new SqlParameter("@NgayCapNhat", phatVanBangViewModel.NgayCapNhat));
                        command.Parameters.Add(new SqlParameter("@NguoiCapNhat", phatVanBangViewModel.NguoiCapNhat));
                        command.Parameters.Add(new SqlParameter("@NgayPhatBang", phatVanBangViewModel.NgayCapNhat));
                        command.Parameters.Add(new SqlParameter("@BangId", phatVanBangViewModel.BangId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // update nhom tao van bang
                if (phatVanBangViewModel.NhomTaoVanBangId.HasValue)
                {
                    string sqlString_2 = @"Update HocSinhTrongNhomTaoVanBangs
                                            Set TrangThaiBangId = 6
                                            Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (BangId = @BangId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", phatVanBangViewModel.BangId));
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    string sqlString_3 = @"Select Count(*) as 'TotalRow'
                                            From HocSinhTrongNhomTaoVanBangs
                                            Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (TrangThaiBangId < 6)";
                    int totalRow = -1;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
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

                    if (totalRow == 0)
                    {
                        string sqlString_4 = @"Update NhomTaoVanBangs
                                            Set TrangThaiBangId = 6, NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat
                                            Where (Id = @NhomTaoVanBangId) ";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_4;
                                command.CommandType = System.Data.CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", phatVanBangViewModel.NhomTaoVanBangId));
                                command.Parameters.Add(new SqlParameter("@NgayCapNhat", phatVanBangViewModel.NgayCapNhat));
                                command.Parameters.Add(new SqlParameter("@NguoiCapNhat", phatVanBangViewModel.NguoiCapNhat));
                                command.ExecuteNonQuery();
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                }

                // add log
                var objLog = new LogVanBang();
                var user = new NguoiDungProvider().GetById(bangViewModel.NguoiCapNhat.Value);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã phát bằng cho học sinh " + bangViewModel.HoVaTen;
                objLog.VanBangId = bangViewModel.Id;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                DbContext.LogVanBangs.Add(objLog);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ChiTietThongTinVanBangViewModel<BangViewModel, LogVanBangViewModel, LogThongTinCaiChinhViewModel> ChiTietThongTinVanBang(int bangId, int donViId)
        {
            try
            {
                ChiTietThongTinVanBangViewModel<BangViewModel, LogVanBangViewModel, LogThongTinCaiChinhViewModel> dataWithLogViewModel = new ChiTietThongTinVanBangViewModel<BangViewModel, LogVanBangViewModel, LogThongTinCaiChinhViewModel>();
                string sqlString = @"Select a.*, b.DanToc, b.GioiTinh, b.HK, b.HL, b.HinhThucDaoTao,
		                                    b.HoKhauThuongTru, b.KK, b.KQ, b.LopHoc, 
		                                    b.NoiSinh, b.XepLoaiTotNghiep, b.UT, b.XetHK, b.NgaySinh,
		                                    c.Ten as 'TrangThaiBang', c.MaMauTrangThai as 'MaMauTrangThaiBang'
                                    From Bang as a
                                    Left Join HocSinh as b
                                    on a.HocSinhId = b.Id
                                    Left Join TrangThaiBang as c
                                    on a.TrangThaiBangId = c.Id
                                    Where (a.Id = @BangId) or (a.BangGocId = @BangId)
                                    Order by a.NgayTao DESC";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            dataWithLogViewModel.ListOfData = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (dataWithLogViewModel.ListOfData != null || dataWithLogViewModel.ListOfData.Count != 0)
                {
                    // get gia tri truong du lieu trong bang cho tung bang
                    foreach (BangViewModel bangViewModel in dataWithLogViewModel.ListOfData)
                    {
                        string sqlString_3 = @"Select a.*, b.Ten as 'TenTruongDuLieu', b.KieuDuLieu
                                                From ThongTinVanBang as a
                                                Left Join TruongDuLieu as b
                                                on a.TruongDuLieuCode =b.Code
                                                Where (a.DonViId = @DonViId) and (a.BangId = @BangId) and (b.DonViId = @DonViId)";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_3;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                                command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                                using (var reader = command.ExecuteReader())
                                {
                                    bangViewModel.TruongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                }

                // get log
                string sqlString_1 = @"Select a.* From LogVanBang as a
                                      Left Join Bang as b
                                      on a.VanBangId = b.id
                                      Where (b.Id = @BangId ) or (b.BangGocId = @BangId)
                                        Order by ThoiGian DESC";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            dataWithLogViewModel.ListOfLog = MapDataHelper<LogVanBangViewModel>.MapList(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get cai chinh
                string sqlString_2 = @"Select a.LanCaiChinh, a.LyDoCaiChinh, a.ThoiGianThucHien, b.* 
                                       From CaiChinhs as a
                                       Left Join ThongTinCaiChinhs as b
                                       on a.CaiChinhId = b.CaiChinhId
                                        Where a.BangId = @BangId";
                dataWithLogViewModel.ListOfCaiChinh = new List<LogThongTinCaiChinhViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            var items = MapDataHelper<LogThongTinCaiChinhViewModel>.MapList(reader);
                            foreach (LogThongTinCaiChinhViewModel item in items)
                            {
                                if (dataWithLogViewModel.ListOfCaiChinh.Any(x => x.CaiChinhId == item.CaiChinhId))
                                {
                                    var logCaiChinh = dataWithLogViewModel.ListOfCaiChinh.Where(x => x.CaiChinhId == item.CaiChinhId).First();
                                    logCaiChinh.ChiTietThongTinCaiChinhs.Add(new ChiTietThongTinCaiChinhViewModel()
                                    {
                                        CaiChinhId = item.CaiChinhId,
                                        ThongTinCaiChinhId = item.ThongTinCaiChinhId,
                                        ThongTinThayDoi = item.ThongTinThayDoi,
                                        ThongTinCu = item.ThongTinCu,
                                        ThongTinMoi = item.ThongTinMoi,
                                    });
                                }
                                else
                                {
                                    LogThongTinCaiChinhViewModel logCaiChinh = new LogThongTinCaiChinhViewModel();
                                    logCaiChinh.CaiChinhId = item.CaiChinhId;
                                    logCaiChinh.LanCaiChinh = item.LanCaiChinh;
                                    logCaiChinh.LyDoCaiChinh = item.LyDoCaiChinh;
                                    logCaiChinh.ThoiGianThucHien = item.ThoiGianThucHien;
                                    logCaiChinh.ChiTietThongTinCaiChinhs = new List<ChiTietThongTinCaiChinhViewModel>();
                                    logCaiChinh.ChiTietThongTinCaiChinhs.Add(new ChiTietThongTinCaiChinhViewModel()
                                    {
                                        CaiChinhId = item.CaiChinhId,
                                        ThongTinCaiChinhId = item.ThongTinCaiChinhId,
                                        ThongTinThayDoi = item.ThongTinThayDoi,
                                        ThongTinCu = item.ThongTinCu,
                                        ThongTinMoi = item.ThongTinMoi,
                                    });
                                    dataWithLogViewModel.ListOfCaiChinh.Add(logCaiChinh);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return dataWithLogViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TaoBanSaoTuBangGoc(HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel, int donViId, string ip)
        {
            //hocSinhTaoVanBangViewModel.SoLuongBanSao : so luong can tao ban sao
            try
            {


                if (hocSinhTaoVanBangViewModel.SoLuongBanSao.HasValue && hocSinhTaoVanBangViewModel.SoLuongBanSao.Value > 0)
                {
                    // Check trung so hieu bản sao
                    string sqlString_2 = @"Select Count(*) as 'TotalRow' From Bang Where SoHieu Like @SoHieu";
                    int totalRow = 0;
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoHieu", hocSinhTaoVanBangViewModel.SoHieu));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    totalRow = Convert.ToInt32(reader[0]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    if (totalRow > 0)
                    {
                        Exception exception = new Exception("Đã tồn tại số hiệu bản sao");
                        throw exception;
                    }

                    // get truong du lieu bang goc
                    string sqlString = @"Select * from [ThongTinVanBang] Where (BangId = @BangId) and (DonViId = @DonViId)";
                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", hocSinhTaoVanBangViewModel.BangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuTrongBangViewModels = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // Get bang goc va loai bang ban sao
                    string sqlString_1 = @"Select a.*,  b.Id as 'LoaiBangId'
                                        From Bang as a
                                        Left Join LoaiBang as b
                                        on a.LoaiBangId = b.LoaiBangGocId
                                       Where (a.Id = @BangId) and (a.DonViId = @DonViId)";
                    BangViewModel bangViewModel = new BangViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@BangId", hocSinhTaoVanBangViewModel.BangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                bangViewModel = MapDataHelper<BangViewModel>.Map(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    LoaiBangViewModel loaiBangViewModel = new LoaiBangViewModel();
                    string sqlString_3 = @"Select * From LoaiBang Where Id = @Id";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@Id", bangViewModel.LoaiBangId));
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

                    // Tao bang ban sao
                    if (bangViewModel.Id != 0)
                    {
                        NhomTaoVanBang nhomTaoVanBang = new NhomTaoVanBang();
                        nhomTaoVanBang.ChoPhepTaoLai = true;
                        nhomTaoVanBang.DonViId = donViId;
                        nhomTaoVanBang.IsDeleted = false;
                        nhomTaoVanBang.NgayTao = hocSinhTaoVanBangViewModel.NgayTao;
                        nhomTaoVanBang.NguoiTao = hocSinhTaoVanBangViewModel.NguoiTao;
                        nhomTaoVanBang.LoaiBangId = bangViewModel.LoaiBangId;
                        nhomTaoVanBang.NgayCapNhat = hocSinhTaoVanBangViewModel.NgayCapNhat;
                        nhomTaoVanBang.NguoiCapNhat = hocSinhTaoVanBangViewModel.NguoiCapNhat;
                        nhomTaoVanBang.Title = "Tạo bản sao cho học sinh " + bangViewModel.HoVaTen;
                        nhomTaoVanBang.TongSohocSinh = hocSinhTaoVanBangViewModel.SoLuongBanSao;
                        nhomTaoVanBang.TrangThaiBangId = 2;
                        nhomTaoVanBang.LoaiNhomTaoVanBangId = 3;
                        nhomTaoVanBang.TruongHocId = bangViewModel.TruongHocId;
                        DbContext.NhomTaoVanBangs.Add(nhomTaoVanBang);
                        DbContext.SaveChanges();

                        for (int i = 0; i < hocSinhTaoVanBangViewModel.SoLuongBanSao.Value; i++)
                        {
                            // tao bang ban sao
                            Bang bang = new Bang();
                            bang.TruongHocId = bangViewModel.TruongHocId;
                            bang.TruongHoc = bangViewModel.TruongHoc.Trim();
                            bang.LoaiBangId = bangViewModel.LoaiBangId;
                            bang.BangGocId = bangViewModel.Id;
                            bang.HocSinhId = bangViewModel.HocSinhId.HasValue ? bangViewModel.HocSinhId : -1;
                            bang.DonViId = donViId;
                            bang.NgayTao = hocSinhTaoVanBangViewModel.NgayTao;
                            bang.NguoiTao = hocSinhTaoVanBangViewModel.NguoiTao;
                            bang.NgayCapNhat = hocSinhTaoVanBangViewModel.NgayCapNhat;
                            bang.SoHieu = hocSinhTaoVanBangViewModel.SoHieu;
                            bang.NguoiCapNhat = hocSinhTaoVanBangViewModel.NguoiCapNhat;
                            bang.HoVaTen = bangViewModel.HoVaTen.Trim();
                            bang.SoVaoSo = bangViewModel.SoVaoSo.Trim();
                            bang.NamTotNghiep = bangViewModel.NamTotNghiep;
                            bang.IsDeleted = false;
                            bang.YeuCauId = -1;
                            bang.TrangThaiBangId = 2;
                            DbContext.Bangs.Add(bang);
                            DbContext.SaveChanges();

                            // tao thong tin truong du lieu
                            List<ThongTinVanBang> thongTinVanBangs = new List<ThongTinVanBang>();
                            foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                            {
                                ThongTinVanBang thongTinVanBang = new ThongTinVanBang();
                                thongTinVanBang.TruongDuLieuCode = truongDuLieuTrongBangViewModel.TruongDuLieuCode;
                                thongTinVanBang.BangId = bang.Id;
                                thongTinVanBang.GiaTri = truongDuLieuTrongBangViewModel.GiaTri;
                                thongTinVanBang.NgayTao = hocSinhTaoVanBangViewModel.NgayTao;
                                thongTinVanBang.NgayCapNhat = hocSinhTaoVanBangViewModel.NgayCapNhat;
                                thongTinVanBang.DonViId = donViId;
                                thongTinVanBangs.Add(thongTinVanBang);
                            }

                            //ThongTinVanBang thongTinVanBang_1 = new ThongTinVanBang();
                            //thongTinVanBang_1.TruongDuLieuCode = ;
                            //thongTinVanBang_1.BangId = bang.Id;
                            //thongTinVanBang_1.GiaTri = bang.SoHieu;
                            //thongTinVanBang_1.NgayTao = hocSinhTaoVanBangViewModel.NgayTao;
                            //thongTinVanBang_1.NgayCapNhat = hocSinhTaoVanBangViewModel.NgayCapNhat;
                            //thongTinVanBang_1.DonViId = donViId;
                            //thongTinVanBangs.Add(thongTinVanBang_1);
                            DbContext.ThongTinVanBangs.AddRange(thongTinVanBangs);
                            DbContext.SaveChanges();

                            string sqlString_4 = @"INSERT INTO [dbo].[ThongTinVanBang]
                                                               ([TruongDuLieuCode]
                                                               ,[BangId]
                                                               ,[GiaTri]
                                                               ,[DonViId])
                                                         VALUES
                                                               (@SoHieu
                                                               ,@BangId
                                                               ,@GiaTri
                                                               ,@DonViId)";
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString_4;
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.Add(new SqlParameter("@BangId", bang.Id));
                                    command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                                    command.Parameters.Add(new SqlParameter("@SoHieu", donViId + "-" + (loaiBangViewModel.IsChungChi.Value ? "1" : "0") + "-SOHIEU"));
                                    command.Parameters.Add(new SqlParameter("@GiaTri", bang.SoHieu));
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }

                            HocSinhTrongNhomTaoVanBang hocSinhTrongNhomTaoVanBang = new HocSinhTrongNhomTaoVanBang();
                            hocSinhTrongNhomTaoVanBang.BangId = bang.Id;
                            hocSinhTrongNhomTaoVanBang.HocSinhId = bangViewModel.HocSinhId;
                            hocSinhTrongNhomTaoVanBang.NhomTaoVanBangId = nhomTaoVanBang.Id;
                            hocSinhTrongNhomTaoVanBang.TrangThaiBangId = 2;
                            hocSinhTrongNhomTaoVanBang.DonViId = donViId;
                            DbContext.HocSinhTrongNhomTaoVanBangs.Add(hocSinhTrongNhomTaoVanBang);
                            DbContext.SaveChanges();
                        }

                        // add log
                        var objLog = new LogVanBang();
                        var user = new NguoiDungProvider().GetById(hocSinhTaoVanBangViewModel.NguoiCapNhat);
                        objLog.NguoiDungId = user.NguoiDungId;
                        objLog.HanhDong = "Đã tạo thông tin bản sao của học sinh " + bangViewModel.HoVaTen;
                        objLog.VanBangId = bangViewModel.Id;
                        objLog.ThoiGian = DateTime.Now;
                        objLog.HoTen = user.HoTen;
                        objLog.Ip = ip;
                        DbContext.LogVanBangs.Add(objLog);
                        DbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TaoAnhVanBang(TaoAnhVanBangViewModel taoAnhVanBangViewModel, int donViId)
        {
            try
            {
                // get nhom van bang
                string sqlString_4 = @"Select * From NhomTaoVanBangs Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                if (nhomTaoVanBangViewModel.Id == 0)
                {
                    Exception exception = new Exception("Không tìm thấy dữ liệu");
                    throw exception;
                }

                // check 2 folder anh 
                if (string.IsNullOrEmpty(nhomTaoVanBangViewModel.DuongDanFileAnh) || !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnh)))
                {
                    nhomTaoVanBangViewModel.DuongDanFileAnh = Path.Combine("Upload", "ThongTinVanBang", nhomTaoVanBangViewModel.Id + "_" + DateTime.Now.ToString("dd_MM_yyyy"));
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnh));
                    string sqlString_5 = @"Update NhomTaoVanBangs
                                        Set DuongDanFileAnh = @DuongDanFileAnh
                                        Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnh", nhomTaoVanBangViewModel.DuongDanFileAnh));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }
                if (string.IsNullOrEmpty(nhomTaoVanBangViewModel.DuongDanFileAnhDeIn) || !Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnhDeIn)))
                {
                    nhomTaoVanBangViewModel.DuongDanFileAnhDeIn = Path.Combine("Upload", "ThongTinVanBangDeIn", nhomTaoVanBangViewModel.Id + "_" + DateTime.Now.ToString("dd_MM_yyyy"));
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnhDeIn));
                    string sqlString_5 = @"Update NhomTaoVanBangs
                                        Set DuongDanFileAnhDeIn = @DuongDanFileAnhDeIn
                                        Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnhDeIn", nhomTaoVanBangViewModel.DuongDanFileAnhDeIn));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get danh sach hoc sinh de in anh
                string sqlString = @"Select c.*, d.LopHoc, d.GioiTinh, d.NgaySinh, d.DanToc, d.HK, d.HL, d.HinhThucDaoTao, d.SoLanXet,
                                            d.UT, d.XetHK, d.HL, d.XepLoaiTotNghiep, e.Ten as 'TrangThaiBang', e.MaMauTrangThai as 'MaMauTrangThaiBang'
                                     From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Left Join HocSinh as d
                                      on c.HocSinhId = d.Id
                                      Left Join TrangThaiBang as e
                                      on c.TrangThaiBangId = e.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    ((b.TrangThaiBangId = 3) or (b.TrangThaiBangId = 2))
                                    Order By RIGHT(c.HoVaTen,CHARINDEX(' ',REVERSE(c.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                List<BangViewModel> bangViewModels = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModels = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                foreach (BangViewModel bangViewModel in bangViewModels)
                {
                    // lay thong tin van bang cua tung hoc sinh
                    string sqlString_1 = @"Select a.*, b.X, b.Y, b.[Format], b.Font, b.Color, b.Bold, b.Italic, 
                                                b.Underline, b.Size, b.Width, b.Height, c.KieuDuLieu 
                                            From ThongTinVanBang as a
                                            Left Join TruongDuLieuLoaiBang as b
                                            on b.TruongDuLieuCode = a.TruongDuLieuCode
                                            Left Join TruongDuLieu as c
                                            on b.TruongDuLieuCode = c.Code
                                                Where (b.LoaiBangId = @LoaiBangId) and (a.BangId = @BangId) and (b.DonViId = @DonViId)";

                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", bangViewModel.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuTrongBangViewModels = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // lay thong tin loai bang
                    string sqlString_2 = @"Select a.*, b.Url as 'AnhLoaiBang' 
                                            From LoaiBang as a
                                            Left Join (Select * From AnhLoaiBang Where DonViId = @DonViId) as b
                                            on a.id = b.ObjectId 
                                            Where (a.Id = @LoaiBangId)";
                    LoaiBangViewModel loaiBang = new LoaiBangViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", bangViewModel.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                loaiBang = MapDataHelper<LoaiBangViewModel>.Map(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }


                    int width = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Width));
                    int height = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Height));
                    if (width == 0 || height == 0)
                    {
                        Exception exception = new Exception("Kích thước loại bằng phải khác 0");
                        throw exception;
                    }
                    // drawing text trong thong tin van bang  co them background phoi bang
                    Image img = null;
                    if (!string.IsNullOrEmpty(loaiBang.AnhLoaiBang))
                    {
                        string backgroundPath = Directory.GetCurrentDirectory() + loaiBang.AnhLoaiBang;
                        img = new Bitmap(Image.FromFile(backgroundPath), width, height);
                    }
                    else
                    {
                        img = new Bitmap(width, height);
                    }
                    Graphics drawing = Graphics.FromImage(img);
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = truongDuLieuTrongBangViewModel.X.Value;
                        int y = truongDuLieuTrongBangViewModel.Y.Value;
                        if (truongDuLieuTrongBangViewModel.KieuDuLieu == 1)
                        {
                            DrawText(drawing, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                        }
                        else if (truongDuLieuTrongBangViewModel.KieuDuLieu == 2)
                        {
                            // drawing QRCode
                            int qrCodeWidth = Convert.ToInt32(truongDuLieuTrongBangViewModel.Width.Value * 3.779527559);
                            int qrCodeHeight = Convert.ToInt32(truongDuLieuTrongBangViewModel.Height.Value * 3.779527559);
                            Image qrCode = Image.FromFile(Directory.GetCurrentDirectory() + truongDuLieuTrongBangViewModel.GiaTri);
                            drawing.DrawImage(qrCode, truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value, qrCodeWidth, qrCodeHeight);
                        }
                    }
                    drawing.Save();
                    drawing.Dispose();

                    // drawing text trong thong tin van bang de in
                    Image img_1 = new Bitmap(width, height);
                    Graphics drawing_1 = Graphics.FromImage(img_1);
                    drawing_1.Clear(Color.FromArgb(255, 255, 255));
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = truongDuLieuTrongBangViewModel.X.Value;
                        int y = truongDuLieuTrongBangViewModel.Y.Value;

                        if (truongDuLieuTrongBangViewModel.KieuDuLieu == 1)
                        {
                            DrawText(drawing_1, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                        }
                        else if (truongDuLieuTrongBangViewModel.KieuDuLieu == 2)
                        {
                            // drawing QRCode
                            int qrCodeWidth = Convert.ToInt32(truongDuLieuTrongBangViewModel.Width.Value * 3.779527559);
                            int qrCodeHeight = Convert.ToInt32(truongDuLieuTrongBangViewModel.Height.Value * 3.779527559);
                            Image qrCode = Image.FromFile(Directory.GetCurrentDirectory() + truongDuLieuTrongBangViewModel.GiaTri);
                            drawing_1.DrawImage(qrCode, truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value, qrCodeWidth, qrCodeHeight);
                        }
                    }
                    drawing_1.Save();
                    drawing_1.Dispose();

                    string fileName = bangViewModel.Id + "_" + bangViewModel.HocSinhId + ".png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnh, fileName);
                    string path_1 = Path.Combine(Directory.GetCurrentDirectory(), nhomTaoVanBangViewModel.DuongDanFileAnhDeIn, fileName);

                    // update trang thai da in trong bang
                    string sqlString_3 = @"Update Bang
                                            Set TrangThaiBangId = 3, DuongDanFileAnh = @DuongDanFileAnh, DuongDanFileDeIn = @DuongDanFileDeIn,
                                                NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat
                                            Where Id = @BangId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnh", "/" + Path.Combine(nhomTaoVanBangViewModel.DuongDanFileAnh, fileName)));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileDeIn", "/" + Path.Combine(nhomTaoVanBangViewModel.DuongDanFileAnhDeIn, fileName)));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@NgayCapNhat", taoAnhVanBangViewModel.NgayCapNhat));
                            command.Parameters.Add(new SqlParameter("@NguoiCapNhat", taoAnhVanBangViewModel.NguoiCapNhat));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // update nhom tao van bang va hoc sinh trong nhom tao van bang
                    string sqlString_5 = @"Update [NhomTaoVanBangs] 
                                            Set TrangThaiBangId = 3, NgayCapNhat = @NgayCapNhat, @NguoiCapNhat = @NguoiCapNhat,
                                            CanDelete = Case 
					                                        When (AddedByImport = 1) and (DonViIn Is Null ) and (TrangThaiBangId <= 3) Then 1
					                                        Else 0
				                                        End
                                            Where (Id = @NhomTaoVanBangId) and (DonViId =@DonViId);
                                            Update [HocSinhTrongNhomTaoVanBangs]
                                            Set TrangThaiBangId = 3 
                                            Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (DonViId =@DonViId); ";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@NgayCapNhat", taoAnhVanBangViewModel.NgayCapNhat));
                            command.Parameters.Add(new SqlParameter("@NguoiCapNhat", taoAnhVanBangViewModel.NguoiCapNhat));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    img.Save(path, ImageFormat.Png);
                    img_1.Save(path_1, ImageFormat.Png);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void TaoAnhBanSao(TaoAnhVanBangViewModel taoAnhVanBangViewModel, int donViId)
        {
            try
            {
                // get nhom van bang
                string sqlString_4 = @"Select * From NhomTaoVanBangs Where (Id = @NhomTaoVanBangId) and (DonViId = @DonViId)";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                if (nhomTaoVanBangViewModel.Id == 0)
                {
                    Exception exception = new Exception("Không tìm thấy dữ liệu");
                    throw exception;
                }

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBang", "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string folderPath_1 = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBangDeIn", "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value);
                if (!Directory.Exists(folderPath_1))
                {
                    Directory.CreateDirectory(folderPath_1);
                }


                // get danh sach hoc sinh de in anh
                string sqlString = @"Select c.*
                                      From [HocSinhTrongNhomTaoVanBangs] as a
                                      Left Join NhomTaoVanBangs as b
                                      on a.NhomTaoVanBangId = b.Id
                                      Left Join Bang as c 
                                      on a.BangId = c.Id
                                      Where (b.Id = @NhomTaoVanBangId) and (b.DonViId = @DonViId) and (b.IsDeleted = 0) and
		                                    ((b.TrangThaiBangId = 2) or (b.TrangThaiBangId = 3))";
                List<BangViewModel> bangViewModels = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            bangViewModels = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                foreach (BangViewModel bangViewModel in bangViewModels)
                {
                    // lay thong tin van bang cua tung hoc sinh
                    string sqlString_1 = @"Select a.*, b.X, b.Y, b.[Format], b.Font, b.Color, b.Bold, b.Italic, 
                                                b.Underline, b.Size, b.Width, b.Height, c.KieuDuLieu 
                                            From ThongTinVanBang as a
                                            Left Join TruongDuLieuLoaiBang as b
                                            on b.TruongDuLieuCode = a.TruongDuLieuCode
                                            Left Join TruongDuLieu as c
                                            on b.TruongDuLieuCode = c.Code
                                                Where (b.LoaiBangId = @LoaiBangId) and (a.BangId = @BangId) and (b.DonViId = @DonViId)";

                    List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", bangViewModel.LoaiBangId));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                truongDuLieuTrongBangViewModels = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // lay thong tin loai bang
                    string sqlString_2 = @"Select a.*, b.Url as 'AnhLoaiBang' From LoaiBang as a
                                            Left Join [AnhLoaiBang] as b
                                            on a.id = b.ObjectId Where a.Id = @LoaiBangId";
                    LoaiBangViewModel loaiBang = new LoaiBangViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@LoaiBangId", bangViewModel.LoaiBangId));
                            using (var reader = command.ExecuteReader())
                            {
                                loaiBang = MapDataHelper<LoaiBangViewModel>.Map(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }


                    int width = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Width));
                    int height = Convert.ToInt32(Math.Ceiling(3.779527559 * loaiBang.Height));

                    // drawing text trong thong tin van bang  co them background phoi bang
                    Image img = null;
                    if (!string.IsNullOrEmpty(loaiBang.AnhLoaiBang))
                    {
                        string backgroundPath = Directory.GetCurrentDirectory() + loaiBang.AnhLoaiBang;
                        img = new Bitmap(Image.FromFile(backgroundPath), width, height);
                    }
                    else
                    {
                        img = new Bitmap(width, height);
                    }
                    Graphics drawing = Graphics.FromImage(img);
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = truongDuLieuTrongBangViewModel.X.Value;
                        int y = truongDuLieuTrongBangViewModel.Y.Value;
                        if (truongDuLieuTrongBangViewModel.KieuDuLieu == 1)
                        {
                            DrawText(drawing, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                        }
                        else if (truongDuLieuTrongBangViewModel.KieuDuLieu == 2)
                        {
                            // drawing QRCode
                            int qrCodeWidth = Convert.ToInt32(truongDuLieuTrongBangViewModel.Width.Value * 3.779527559);
                            int qrCodeHeight = Convert.ToInt32(truongDuLieuTrongBangViewModel.Height.Value * 3.779527559);
                            Image qrCode = Image.FromFile(Directory.GetCurrentDirectory() + truongDuLieuTrongBangViewModel.GiaTri);
                            drawing.DrawImage(qrCode, truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value, qrCodeWidth, qrCodeHeight);
                        }
                    }
                    drawing.Save();
                    drawing.Dispose();

                    // drawing text trong thong tin van bang de in
                    Image img_1 = new Bitmap(width, height);
                    Graphics drawing_1 = Graphics.FromImage(img_1);
                    drawing_1.Clear(Color.FromArgb(255, 255, 255));
                    foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel in truongDuLieuTrongBangViewModels)
                    {
                        Font font;
                        if (truongDuLieuTrongBangViewModel.Bold == false && truongDuLieuTrongBangViewModel.Italic == false && truongDuLieuTrongBangViewModel.Underline == false)
                        {
                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value);
                        }
                        else
                        {
                            FontStyle fontStyle = FontStyle.Regular;
                            fontStyle = truongDuLieuTrongBangViewModel.Bold.Value ? fontStyle |= FontStyle.Bold : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Italic.Value ? fontStyle |= FontStyle.Italic : fontStyle;
                            fontStyle = truongDuLieuTrongBangViewModel.Underline.Value ? fontStyle |= FontStyle.Underline : fontStyle;

                            font = new Font(truongDuLieuTrongBangViewModel.Font, truongDuLieuTrongBangViewModel.Size.Value, fontStyle);
                        }
                        int x = truongDuLieuTrongBangViewModel.X.Value;
                        int y = truongDuLieuTrongBangViewModel.Y.Value;

                        if (truongDuLieuTrongBangViewModel.KieuDuLieu == 1)
                        {
                            DrawText(drawing_1, truongDuLieuTrongBangViewModel.GiaTri, font, ColorTranslator.FromHtml(truongDuLieuTrongBangViewModel.Color), truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value);
                        }
                        else if (truongDuLieuTrongBangViewModel.KieuDuLieu == 2)
                        {
                            // drawing QRCode
                            int qrCodeWidth = Convert.ToInt32(truongDuLieuTrongBangViewModel.Width.Value * 3.779527559);
                            int qrCodeHeight = Convert.ToInt32(truongDuLieuTrongBangViewModel.Height.Value * 3.779527559);
                            Image qrCode = Image.FromFile(Directory.GetCurrentDirectory() + truongDuLieuTrongBangViewModel.GiaTri);
                            drawing_1.DrawImage(qrCode, truongDuLieuTrongBangViewModel.X.Value, truongDuLieuTrongBangViewModel.Y.Value, qrCodeWidth, qrCodeHeight);
                        }
                    }
                    drawing_1.Save();
                    drawing_1.Dispose();

                    string fileName = bangViewModel.Id + "_" + bangViewModel.HocSinhId + "_" + ".png";
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBang", "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value, fileName);
                    string path_1 = Path.Combine(Directory.GetCurrentDirectory(), "Upload/ThongTinVanBangDeIn", "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value, fileName);

                    // update trang thai da in trong bang
                    string sqlString_3 = @"Update Bang
                                            Set TrangThaiBangId = 3, DuongDanFileAnh = @DuongDanFileAnh, DuongDanFileDeIn = @DuongDanFileDeIn,
                                                NgayCapNhat = @NgayCapNhat, NguoiCapNhat = @NguoiCapNhat
                                            Where Id = @BangId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DuongDanFileAnh", "/Upload/ThongTinVanBang/" + "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value + "/" + fileName));
                            command.Parameters.Add(new SqlParameter("@DuongDanFileDeIn", "/Upload/ThongTinVanBangDeIn/" + "DonViId_" + nhomTaoVanBangViewModel.TruongHocId.Value + "/" + fileName));
                            command.Parameters.Add(new SqlParameter("@BangId", bangViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@NgayCapNhat", taoAnhVanBangViewModel.NgayCapNhat));
                            command.Parameters.Add(new SqlParameter("@NguoiCapNhat", taoAnhVanBangViewModel.NguoiCapNhat));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // update nhom tao van bang va hoc sinh trong nhom tao van bang
                    string sqlString_5 = @"Update [NhomTaoVanBangs] 
                                            Set TrangThaiBangId = 3, NgayCapNhat = @NgayCapNhat, @NguoiCapNhat = @NguoiCapNhat
                                            Where (Id = @NhomTaoVanBangId) and (DonViId =@DonViId);
                                            Update [HocSinhTrongNhomTaoVanBangs]
                                            Set TrangThaiBangId = 3 
                                            Where (NhomTaoVanBangId = @NhomTaoVanBangId) and (DonViId =@DonViId); ";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", taoAnhVanBangViewModel.NhomTaoVanBangId));
                            command.Parameters.Add(new SqlParameter("@NgayCapNhat", taoAnhVanBangViewModel.NgayCapNhat));
                            command.Parameters.Add(new SqlParameter("@NguoiCapNhat", taoAnhVanBangViewModel.NguoiCapNhat));
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.ExecuteNonQuery();
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    img.Save(path, ImageFormat.Png);
                    img_1.Save(path_1, ImageFormat.Png);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddNhomTaoVanBang(NhomTaoVanBangViewModel nhomTaoVanBangViewModel, int donViId)
        {
            try
            {
                // get loai bang
                LoaiBang loaiBang = new LoaiBang();
                string sqlString_1 = @"Select * From LoaiBang Where Id = @Id";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", nhomTaoVanBangViewModel.LoaiBangId));
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


                // add bang
                List<Bang> bangs = new List<Bang>();
                foreach (HocSinhTaoVanBangViewModel hocSinhTaoVanBangViewModel in nhomTaoVanBangViewModel.HocSinhs)
                {
                    Bang bang = new Bang();
                    bang.HocSinhId = hocSinhTaoVanBangViewModel.Id;
                    bang.TruongHocId = nhomTaoVanBangViewModel.TruongHocId;
                    bang.TruongHoc = hocSinhTaoVanBangViewModel.TruongHoc.Trim();
                    bang.HoVaTen = hocSinhTaoVanBangViewModel.HoVaTen.Trim();
                    bang.SoVaoSo = hocSinhTaoVanBangViewModel.SoVaoSo.Trim();
                    bang.NamTotNghiep = hocSinhTaoVanBangViewModel.NamTotNghiep;
                    bang.YeuCauId = -1;
                    bang.LoaiBangId = nhomTaoVanBangViewModel.LoaiBangId;
                    bang.DonViId = donViId;
                    bang.TrangThaiBangId = 2;
                    bang.NgayTao = nhomTaoVanBangViewModel.NgayTao;
                    bang.NgayCapNhat = nhomTaoVanBangViewModel.NgayCapNhat;
                    bang.NguoiTao = nhomTaoVanBangViewModel.NguoiTao;
                    bang.NguoiCapNhat = nhomTaoVanBangViewModel.NguoiCapNhat;
                    bang.IsDeleted = false;
                    bang.IsChungChi = loaiBang.IsChungChi;
                    bang.SoLanCaiChinh = 0;
                    bangs.Add(bang);
                }
                DbContext.Bangs.AddRange(bangs);
                DbContext.SaveChanges();

                // add nhom tao van bang
                NhomTaoVanBang nhomTaoVanBang = new NhomTaoVanBang();
                nhomTaoVanBang.Title = nhomTaoVanBangViewModel.Title;
                nhomTaoVanBang.TruongHocId = nhomTaoVanBangViewModel.TruongHocId;
                nhomTaoVanBang.DonViId = nhomTaoVanBangViewModel.DonViId;
                nhomTaoVanBang.LoaiBangId = nhomTaoVanBangViewModel.LoaiBangId;
                nhomTaoVanBang.NgayTao = nhomTaoVanBangViewModel.NgayTao;
                nhomTaoVanBang.NguoiTao = nhomTaoVanBangViewModel.NguoiTao;
                nhomTaoVanBang.NgayCapNhat = nhomTaoVanBangViewModel.NgayCapNhat;
                nhomTaoVanBang.NguoiCapNhat = nhomTaoVanBangViewModel.NguoiCapNhat;
                nhomTaoVanBang.IsDeleted = nhomTaoVanBangViewModel.IsDeleted;
                nhomTaoVanBang.TrangThaiBangId = nhomTaoVanBangViewModel.TrangThaiBangId;
                nhomTaoVanBang.TongSohocSinh = nhomTaoVanBangViewModel.TongSohocSinh;
                nhomTaoVanBang.ChoPhepTaoLai = nhomTaoVanBangViewModel.ChoPhepTaoLai;
                nhomTaoVanBang.LoaiNhomTaoVanBangId = 1;
                nhomTaoVanBang.CanDelete = nhomTaoVanBangViewModel.CanDelete.HasValue ? nhomTaoVanBangViewModel.CanDelete : false;
                nhomTaoVanBang.AddedByImport = nhomTaoVanBangViewModel.AddedByImport.HasValue ? nhomTaoVanBangViewModel.AddedByImport : false;
                DbContext.NhomTaoVanBangs.Add(nhomTaoVanBang);
                DbContext.SaveChanges();

                // add hoc sinh trong nhom tao van bang
                List<HocSinhTrongNhomTaoVanBang> hocSinhTrongNhomTaoVanBangs = new List<HocSinhTrongNhomTaoVanBang>();
                foreach (Bang bang_1 in bangs)
                {
                    HocSinhTrongNhomTaoVanBang hocSinhTrongNhomTaoVanBang = new HocSinhTrongNhomTaoVanBang();
                    hocSinhTrongNhomTaoVanBang.NhomTaoVanBangId = nhomTaoVanBang.Id;
                    hocSinhTrongNhomTaoVanBang.HocSinhId = bang_1.HocSinhId;
                    hocSinhTrongNhomTaoVanBang.BangId = bang_1.Id;
                    hocSinhTrongNhomTaoVanBang.TrangThaiBangId = 1;
                    hocSinhTrongNhomTaoVanBang.DonViId = bang_1.DonViId;
                    hocSinhTrongNhomTaoVanBangs.Add(hocSinhTrongNhomTaoVanBang);
                }
                DbContext.HocSinhTrongNhomTaoVanBangs.AddRange(hocSinhTrongNhomTaoVanBangs);
                DbContext.SaveChanges();

                // get truong du lieu trong loai bang
                string sqlString_3 = @"Select c.Ten as 'Ten', b.*  From LoaiBang as a
                                            Left Join TruongDuLieuLoaiBang as b
                                            on a.Id = b.LoaiBangId
                                            left Join TruongDuLieu as c
                                            on b.TruongDuLieuCode = c.Code
                                            Where (a.Id = @LoaiBangId) and (b.DonViId = @DonViId)";
                List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", nhomTaoVanBangViewModel.LoaiBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            truongDuLieuTrongBangs = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // add thong tin van bang
                if (bangs.Count != 0)
                {
                    List<ThongTinVanBang> thongTinVanBangs = new List<ThongTinVanBang>();
                    foreach (HocSinhTaoVanBangViewModel hocSinh in nhomTaoVanBangViewModel.HocSinhs)
                    {
                        hocSinh.TruongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();
                        foreach (TruongDuLieuTrongBangViewModel truongDuLieuTrongBang in truongDuLieuTrongBangs)
                        {

                            TruongDuLieuTrongBangViewModel truongDuLieuTrongBangViewModel = new TruongDuLieuTrongBangViewModel();
                            truongDuLieuTrongBangViewModel.TenTruongDuLieu = truongDuLieuTrongBang.Ten;

                            ThongTinVanBang thongTinVanBang = new ThongTinVanBang();
                            thongTinVanBang.TruongDuLieuCode = truongDuLieuTrongBang.TruongDuLieuCode;
                            thongTinVanBang.BangId = bangs.Where(x => x.HocSinhId == hocSinh.Id).FirstOrDefault().Id;
                            thongTinVanBang.NgayTao = nhomTaoVanBangViewModel.NgayTao;
                            thongTinVanBang.NguoiTao = nhomTaoVanBangViewModel.NguoiTao;
                            thongTinVanBang.NgayCapNhat = nhomTaoVanBangViewModel.NgayCapNhat;
                            thongTinVanBang.NguoiCapNhat = nhomTaoVanBangViewModel.NguoiCapNhat;
                            thongTinVanBang.DonViId = donViId;

                            string sqlString = @"Select * From [dbo].[HocSinh] as a
                                    Where ([Id] = @Id) and ([DonViId] = @DonViId) and ([IsDeleted] = 0)";
                            List<SqlParameter> sqlParameters = new List<SqlParameter>();
                            sqlParameters.Add(new SqlParameter("@Id", hocSinh.Id));
                            sqlParameters.Add(new SqlParameter("@DonViId", nhomTaoVanBang.TruongHocId));
                            HocSinhViewModel hocSinhViewModel = new HocSinhViewModel();
                            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                            {
                                bool wasOpen = command.Connection.State == ConnectionState.Open;
                                if (!wasOpen) command.Connection.Open();

                                try
                                {
                                    command.CommandText = sqlString;
                                    command.CommandType = CommandType.Text;
                                    command.Parameters.AddRange(sqlParameters.ToArray());
                                    using (var reader = command.ExecuteReader())
                                    {
                                        hocSinhViewModel = MapDataHelper<HocSinhViewModel>.Map(reader);
                                    }
                                }
                                finally
                                {
                                    command.Connection.Close();
                                }
                            }

                            // qrcode
                            Type myType = hocSinhViewModel.GetType();
                            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                            PropertyInfo propertyInfo = props.Where(x => x.Name == truongDuLieuTrongBang.TenTruongDuLieu).FirstOrDefault();
                            if (!truongDuLieuTrongBang.DungChung.Value)
                            {
                                if (propertyInfo != null)
                                {
                                    if (truongDuLieuTrongBang.KieuDuLieu == 2)
                                    {
                                        thongTinVanBang.GiaTri = QRHelper.QRGenerator(Convert.ToString(propertyInfo.GetValue(hocSinhViewModel, null)) + "&bangId=" + thongTinVanBang.BangId);
                                    }
                                    else if (truongDuLieuTrongBang.KieuDuLieu == 1)
                                    {
                                        thongTinVanBang.GiaTri = Convert.ToString(propertyInfo.GetValue(hocSinhViewModel, null));
                                        if (truongDuLieuTrongBang.TenTruongDuLieu.ToLower() == "ngaysinh")
                                        {
                                            thongTinVanBang.GiaTri = DateTime.Parse(Convert.ToString(propertyInfo.GetValue(hocSinhViewModel, null))).ToString("dd/MM/yyyy");
                                        }
                                    }

                                }
                                else
                                {
                                    thongTinVanBang.GiaTri = "";
                                }
                            }
                            thongTinVanBangs.Add(thongTinVanBang);
                        }
                    }
                    DbContext.ThongTinVanBangs.AddRange(thongTinVanBangs);
                    DbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ImportHocSinhTotNghiepExcelFile(NhomTaoVanBangViewModel nhomTaoVanBangViewModel, int donViId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NhomTaoVanBangsViewModel GetNhomTaoVanBangs(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, int donViId, bool? isChungChi)
        {
            try
            {
                NhomTaoVanBangsViewModel nhomTaoVanBangsViewModel = new NhomTaoVanBangsViewModel();
                // get chua tao  TTVB
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'LoaiBang', d.Ten as 'TrangThaiBang'
                                    From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join LoaiBang as c
                                    on a.LoaiBangId = c.Id
                                    Left Join TrangThaiBang as d
                                    on a.TrangThaiBangId = d.Id
                                    Where ((@IsChungChi is null) or (c.IsChungChi = @IsChungChi)) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
		                                    and ((@LoaiBangId is null) or (a.LoaiBangId = @LoaiBangId))
		                                    and (a.TrangThaiBangId = @TrangThaiBangId)
		                                    and ((@Nam is null) or (Year(a.Ngaytao) = @Nam)) and (a.DonViId = @DonViId)
                                            and ((@LoaiNhomTaoVanBangId is null) or (a.LoaiNhomTaoVanBangId = @LoaiNhomTaoVanBangId))
                                    Order By a.NgayTao Desc";
                List<NhomTaoVanBangViewModel> nhomChuaTaoTTVB = new List<NhomTaoVanBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", LoaiBangId.HasValue ? LoaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", !trangThaiBangId.HasValue || (trangThaiBangId.HasValue && (trangThaiBangId.Value == 1)) ? 1 : -1));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiNhomTaoVanBangId", loaiNhomTaoVanBangId.HasValue ? loaiNhomTaoVanBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomChuaTaoTTVB = MapDataHelper<NhomTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }


                // get da tao TTVB
                string sqlString_2 = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'LoaiBang', d.Ten as 'TrangThaiBang'
                                    From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join LoaiBang as c
                                    on a.LoaiBangId = c.Id
                                    Left Join TrangThaiBang as d
                                    on a.TrangThaiBangId = d.Id
                                    Where ((@IsChungChi is null) or (c.IsChungChi = @IsChungChi)) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
		                                    and ((@LoaiBangId is null) or (a.LoaiBangId = @LoaiBangId))
		                                    and (a.TrangThaiBangId = @TrangThaiBangId)
		                                    and ((@Nam is null) or (Year(a.Ngaytao) = @Nam)) and (a.DonViId = @DonViId)
                                            and ((@LoaiNhomTaoVanBangId is null) or (a.LoaiNhomTaoVanBangId = @LoaiNhomTaoVanBangId))
                                    Order By a.NgayTao Desc";
                List<NhomTaoVanBangViewModel> nhomDaTaoTTVB = new List<NhomTaoVanBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", LoaiBangId.HasValue ? LoaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", !trangThaiBangId.HasValue || (trangThaiBangId.HasValue && (trangThaiBangId.Value == 2)) ? 2 : -1));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiNhomTaoVanBangId", loaiNhomTaoVanBangId.HasValue ? loaiNhomTaoVanBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomDaTaoTTVB = MapDataHelper<NhomTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get da tao tao anh
                string sqlString_3 = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'LoaiBang', d.Ten as 'TrangThaiBang'
                                    From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join LoaiBang as c
                                    on a.LoaiBangId = c.Id
                                    Left Join TrangThaiBang as d
                                    on a.TrangThaiBangId = d.Id
                                    Where ((@IsChungChi is null) or (c.IsChungChi = @IsChungChi)) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
		                                    and ((@LoaiBangId is null) or (a.LoaiBangId = @LoaiBangId))
		                                    and ( ((@TrangThaiBangId is null) and (a.TrangThaiBangId = 3)) or (a.TrangThaiBangId = @TrangThaiBangId) )
		                                    and ((@Nam is null) or (Year(a.Ngaytao) = @Nam)) and ((a.DonViId = @DonViId) or ((a.DonViIn = @DonViId)))
                                            and ((@LoaiNhomTaoVanBangId is null) or (a.LoaiNhomTaoVanBangId = @LoaiNhomTaoVanBangId))
                                    Order By a.NgayTao Desc";
                List<NhomTaoVanBangViewModel> nhomDaTaoAnh = new List<NhomTaoVanBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", LoaiBangId.HasValue ? LoaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", !trangThaiBangId.HasValue || (trangThaiBangId.HasValue && (trangThaiBangId.Value == 3)) ? DBNull.Value : -1));
                        command.Parameters.Add(new SqlParameter("@LoaiNhomTaoVanBangId", loaiNhomTaoVanBangId.HasValue ? loaiNhomTaoVanBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomDaTaoAnh = MapDataHelper<NhomTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                nhomTaoVanBangsViewModel.ListOfChuaTaoTTVB = nhomChuaTaoTTVB;
                nhomTaoVanBangsViewModel.ListOfDaTaoTTVB = nhomDaTaoTTVB;
                nhomTaoVanBangsViewModel.ListOfDaTaoAnh = nhomDaTaoAnh;
                return nhomTaoVanBangsViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NhomTaoVanBangViewModel GetNhomTaoVanBang(int nhomTaoVanBangId, int donViId)
        {
            try
            {
                string sqlString_3 = @"Select a.*, b.TenDonVi as 'TruongHoc' From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId Where (a.Id = @Id) and ((a.DonViId = @DonViId) or (a.DonViIn = @DonViId))";
                NhomTaoVanBangViewModel nhomTaoVanBangViewModel = new NhomTaoVanBangViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Id", nhomTaoVanBangId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangViewModel = MapDataHelper<NhomTaoVanBangViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return nhomTaoVanBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NhomTaoVanBangWithPaginationViewModel GetNhomTaoVanBangDaIns(int? truongHocId, int? LoaiBangId, int? loaiNhomTaoVanBangId, int? trangThaiBangId, int? nam, bool? isChungChi, int donViId, int currentPage)
        {
            try
            {
                NhomTaoVanBangWithPaginationViewModel nhomTaoVanBangWithPaginationViewModel = new NhomTaoVanBangWithPaginationViewModel();
                // get da in
                string sqlString_1 = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'LoaiBang', d.Ten as 'TrangThaiBang'
                                    From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join LoaiBang as c
                                    on a.LoaiBangId = c.Id
                                    Left Join TrangThaiBang as d
                                    on a.TrangThaiBangId = d.Id
                                    Where ((@IsChungChi is null) or (c.IsChungChi = @IsChungChi)) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
		                                    and ((@LoaiBangId is null) or (a.LoaiBangId = @LoaiBangId))
		                                    and ( ((@TrangThaiBangId is null) and ((a.TrangThaiBangId = 4) or (a.TrangThaiBangId = 6)) ) or (a.TrangThaiBangId = @TrangThaiBangId) )
		                                    and ((@Nam is null) or (Year(a.Ngaytao) = @Nam)) and ((a.DonViId = @DonViId) or (a.DonViIn = @DonViId))
                                            and ((@LoaiNhomTaoVanBangId is null) or (a.LoaiNhomTaoVanBangId = @LoaiNhomTaoVanBangId))
                                    Order By a.NgayCapNhat Desc
                                    OFFSET @Offset ROWS FETCH NEXT @Next ROWS ONLY";
                List<NhomTaoVanBangViewModel> nhomDaTaoAnh = new List<NhomTaoVanBangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", LoaiBangId.HasValue ? LoaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiNhomTaoVanBangId", loaiNhomTaoVanBangId.HasValue ? loaiNhomTaoVanBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));
                        using (var reader = command.ExecuteReader())
                        {
                            nhomTaoVanBangWithPaginationViewModel.ListOfDaInBang = MapDataHelper<NhomTaoVanBangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get totalpage
                string sqlString_2 = @"Select Count(*) as 'TotalRow'
                                    From NhomTaoVanBangs as a
                                    Left Join DonVi as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join LoaiBang as c
                                    on a.LoaiBangId = c.Id
                                    Left Join TrangThaiBang as d
                                    on a.TrangThaiBangId = d.Id
                                    Where ((@IsChungChi is null) or (c.IsChungChi = @IsChungChi)) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
		                                    and ((@LoaiBangId is null) or (a.LoaiBangId = @LoaiBangId))
		                                    and ( ((@TrangThaiBangId is null) and ((a.TrangThaiBangId = 4) or (a.TrangThaiBangId = 6)) ) or (a.TrangThaiBangId = @TrangThaiBangId) )
		                                    and ((@Nam is null) or (Year(a.Ngaytao) = @Nam)) and (a.DonViId = @DonViId)
                                            and ((@LoaiNhomTaoVanBangId is null) or (a.LoaiNhomTaoVanBangId = @LoaiNhomTaoVanBangId))";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi.HasValue ? isChungChi.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", LoaiBangId.HasValue ? LoaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Nam", nam.HasValue ? nam.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LoaiNhomTaoVanBangId", loaiNhomTaoVanBangId.HasValue ? loaiNhomTaoVanBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
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

                nhomTaoVanBangWithPaginationViewModel.CurrentPage = currentPage;
                nhomTaoVanBangWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));

                return nhomTaoVanBangWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DanhSachBangViewModel GetDanhSachBangChoTruong(int? trangThaiBangId, int? lopHocId, string hoVaTen, int? namTotNghiep, int currentPage, int donViId)
        {
            try
            {
                DanhSachBangViewModel danhSachBangViewModel = new DanhSachBangViewModel();
                string sqlString = @"Select a.*, c.NhomTaoVanBangId, d.Ten as 'TrangThaiBang', d.MaMauTrangThai as 'MaMauTrangThaiBang'
                                        From Bang as a
                                        Left Join HocSinh as b
                                        on a.HocSinhId = b.Id
                                        Left Join HocSinhTrongNhomTaoVanBangs as c
                                        on a.Id = c.BangId
                                        Left Join TrangThaiBang as d
                                        on a.TrangThaiBangId = d.Id
                                        Left Join NhomTaoVanBangs as e
										on c.NhomTaoVanBangId = e.Id
                                        Where ((c.NhomTaoVanBangId is null) or ((c.NhomTaoVanBangId is not null) and (e.DonViIn is null)))
                                            and ((@HoVaTen is null) or (a.HoVaTen Like N'%'+@HoVaTen+'%')) 
	                                        and (a.TruongHocId = @TruongHocId) and (c.Id = (Select Max(Id) From HocSinhTrongNhomTaoVanBangs Where BangId = a.Id))
	                                        and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep))
	                                        and ((@LopHocId is null) or (b.LopHocId = @LopHocId))
	                                        and (((@TrangThaiBangId is null) and (a.TrangThaiBangId >= 4)) or (a.TrangThaiBangId = @TrangThaiBangId))
                                            and (a.HocSinhId != -1) and (a.BangGocId is null)
                                        Order By RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc
                                        Offset @Offset Rows Fetch Next @Next Rows Only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen.Trim()));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", donViId));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));
                        using (var reader = command.ExecuteReader())
                        {
                            danhSachBangViewModel.VanBangs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get total page
                string sqlString_1 = @"Select Count(*) as 'TotalRow'
                                        From Bang as a
                                        Left Join HocSinh as b
                                        on a.HocSinhId = b.Id
                                        Left Join HocSinhTrongNhomTaoVanBangs as c
                                        on a.Id = c.BangId
                                        Left Join NhomTaoVanBangs as e
										on c.NhomTaoVanBangId = e.Id
                                        Where ((c.NhomTaoVanBangId is null) or ((c.NhomTaoVanBangId is not null) and (e.DonViIn is null)))
                                            and ((@HoVaTen is null) or (a.HoVaTen Like N'%'+@HoVaTen+'%')) 
	                                        and (a.TruongHocId = @TruongHocId) and (c.Id = (Select Max(Id) From HocSinhTrongNhomTaoVanBangs Where BangId = a.Id))
	                                        and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep))
	                                        and ((@LopHocId is null) or (b.LopHocId = @LopHocId))
	                                        and (((@TrangThaiBangId is null) and (a.TrangThaiBangId >= 4)) or (a.TrangThaiBangId = @TrangThaiBangId))
                                            and (a.HocSinhId != -1) and (a.BangGocId is null)";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen.Trim()));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TrangThaiBangId", trangThaiBangId.HasValue ? trangThaiBangId.Value : DBNull.Value));
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
                danhSachBangViewModel.CurrentPage = currentPage;
                danhSachBangViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                return danhSachBangViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ChuyenDonViSoIn(ChuyenDonViInViewModel chuyenDonViInViewModel, int donViId)
        {
            try
            {
                // get donviso
                string sqlString = @"Select * From DonVi Where DonViId = @DonViId";
                DonViViewModel donViViewModel = new DonViViewModel();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
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

                // update donvi trong nhomtaovanbang
                string sqlString_1 = @"Update NhomTaoVanBangs
                                     Set DonViIn = @DonViSo
                                     Where Id = @NhomTaoVanBangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViSo", donViViewModel.KhoaChaId));
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", chuyenDonViInViewModel.NhomTaoVanBangId));
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

        public void ChuyenDonViPhongIn(ChuyenDonViInViewModel chuyenDonViInViewModel, int donViId)
        {
            try
            {
                // update donvi trong nhomtaovanbang
                string sqlString_1 = @"Update NhomTaoVanBangs
                                     Set DonViIn = null
                                     Where Id = @NhomTaoVanBangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", chuyenDonViInViewModel.NhomTaoVanBangId));
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

        public void DeleteNhomTaoVanBang(int nhomTaoVanBangId, int donViId)
        {
            try
            {
                // update donvi trong nhomtaovanbang
                string sqlString_1 = @"Delete LogHocSinh Where HocSinhId In (Select HocSinhId From HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId);
                                        Delete LogVanBang Where VanBangId In (Select BangId From HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId);
                                        Delete ThongTinVanBang Where BangId In (Select BangId From HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId);
                                        Delete HocSinh Where Id In (Select HocSinhId From HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId);
                                        Delete Bang Where Id in (Select BangId From HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId);
                                        Delete NhomTaoVanBangs Where Id = @NhomTaoVanBangId;
                                        Delete HocSinhTrongNhomTaoVanBangs Where NhomTaoVanBangId = @NhomTaoVanBangId;";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NhomTaoVanBangId", nhomTaoVanBangId));
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
    }

}
