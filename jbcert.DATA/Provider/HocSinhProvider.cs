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
    public class HocSinhProvider : ApplicationDbContext, IHocSinh
    {
        public void ImportSoGoc(List<HocSinhViewModel> hocSinhViewModels, int donViId)
        {
            try
            {
                List<NienKhoaViewModel> nienKhoaViewModels = new List<NienKhoaViewModel>();
                string sqlString_2 = @"";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        //using (var reader = command.ExecuteReader())
                        //{

                        //}
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

        public List<HocSinh> AddHocSinhs(List<HocSinhViewModel> hocSinhViewModels, int donViId, string ip)
        {
            try
            {
                List<HocSinh> hocSinhs = new List<HocSinh>();
                List<LogHocSinh> logHocSinhs = new List<LogHocSinh>();
                foreach (HocSinhViewModel hocSinhViewModel in hocSinhViewModels)
                {
                    HocSinh hocSinh = new HocSinh();
                    hocSinh.HoVaTen = string.IsNullOrEmpty(hocSinhViewModel.HoVaTen) ? "" : hocSinhViewModel.HoVaTen.Trim();
                    hocSinh.NgaySinh = hocSinhViewModel.NgaySinh.HasValue ? hocSinhViewModel.NgaySinh : DateTime.Now;
                    hocSinh.NoiSinh = string.IsNullOrEmpty(hocSinhViewModel.NoiSinh) ? "" : hocSinhViewModel.NoiSinh.Trim();
                    hocSinh.HoKhauThuongTru = string.IsNullOrEmpty(hocSinhViewModel.HoKhauThuongTru) ? "" : hocSinhViewModel.HoKhauThuongTru.Trim();
                    hocSinh.TruongHocId = hocSinhViewModel.TruongHocId;
                    hocSinh.TruongHoc = hocSinhViewModel.TruongHoc;
                    hocSinh.LopHoc = hocSinhViewModel.LopHoc;
                    hocSinh.LopHocId = hocSinhViewModel.LopHocId;
                    hocSinh.DanTocId = hocSinhViewModel.DanTocId;
                    hocSinh.GioiTinhId = hocSinhViewModel.GioiTinhId;
                    hocSinh.NamTotNghiep = hocSinhViewModel.NamTotNghiep;
                    hocSinh.XepLoaiTotNghiep = hocSinhViewModel.XepLoaiTotNghiep;
                    hocSinh.HinhThucDaoTao = hocSinhViewModel.HinhThucDaoTao;
                    hocSinh.GioiTinh = hocSinhViewModel.GioiTinh;
                    hocSinh.DanToc = hocSinhViewModel.DanToc;
                    hocSinh.SoVaoSo = hocSinhViewModel.SoVaoSo;
                    hocSinh.XetHK = hocSinhViewModel.XetHK;
                    hocSinh.UT = hocSinhViewModel.UT;
                    hocSinh.KK = hocSinhViewModel.KK;
                    hocSinh.HL = hocSinhViewModel.HL;
                    hocSinh.HK = hocSinhViewModel.HK;
                    hocSinh.KQ = hocSinhViewModel.KQ;
                    hocSinh.TT = hocSinhViewModel.TT;
                    hocSinh.CongNhanTotNghiep = hocSinhViewModel.CongNhanTotNghiep;
                    hocSinh.DaInBangGoc = false;
                    hocSinh.NgayTao = hocSinhViewModel.NgayTao;
                    hocSinh.NgayCapNhat = hocSinhViewModel.NgayCapNhat;
                    hocSinh.NguoiTao = hocSinhViewModel.NguoiTao;
                    hocSinh.NguoiCapNhat = hocSinhViewModel.NguoiCapNhat;
                    hocSinh.IsDeleted = false;
                    hocSinh.DonViId = hocSinhViewModel.TruongHocId;
                    hocSinh.SoLanXet = hocSinh.SoLanXet ;

                    if (hocSinhViewModel.DiemMonHocs != null)
                    {
                        foreach (DiemMonHocViewModel diemMonHoc in hocSinhViewModel.DiemMonHocs)
                        {
                            diemMonHoc.HocSinhId = hocSinh.Id;
                        }
                    }
                    if (hocSinhViewModel.DiemMonHocs != null && hocSinhViewModel.DiemMonHocs.Count != 0)
                    {
                        CapNhatDiemMonHoc(hocSinhViewModel.DiemMonHocs);
                    }

                    hocSinhs.Add(hocSinh);
                }
                DbContext.HocSinhs.AddRange(hocSinhs);
                DbContext.SaveChanges();

                foreach (HocSinh hocSinh in hocSinhs)
                {
                    var objLog = new LogHocSinh();
                    var user = new NguoiDungProvider().GetById(hocSinh.NguoiCapNhat.Value);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã thêm thêm mới thông tin học sinh " + hocSinh.HoVaTen;
                    objLog.HocSinhId = hocSinh.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    logHocSinhs.Add(objLog);
                }

                DbContext.LogHocSinhs.AddRange(logHocSinhs);
                DbContext.SaveChanges();

                return hocSinhs;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddHocSinh(HocSinhViewModel hocSinhViewModel, int donViId, string ip)
        {
            try
            {
                HocSinh hocSinh = new HocSinh();
                hocSinh.HoVaTen = hocSinhViewModel.HoVaTen.Trim();
                hocSinh.NgaySinh = hocSinhViewModel.NgaySinh;
                hocSinh.NoiSinh = hocSinhViewModel.NoiSinh.Trim();
                hocSinh.HoKhauThuongTru = hocSinhViewModel.HoKhauThuongTru.Trim();
                hocSinh.TruongHocId = hocSinhViewModel.TruongHocId;
                hocSinh.TruongHoc = hocSinhViewModel.TruongHoc;
                hocSinh.LopHoc = hocSinhViewModel.LopHoc;
                hocSinh.LopHocId = hocSinhViewModel.LopHocId;
                hocSinh.DanTocId = hocSinhViewModel.DanTocId;
                hocSinh.GioiTinhId = hocSinhViewModel.GioiTinhId;
                hocSinh.NamTotNghiep = hocSinhViewModel.NamTotNghiep;
                hocSinh.XepLoaiTotNghiep = hocSinhViewModel.XepLoaiTotNghiep;
                hocSinh.HinhThucDaoTao = hocSinhViewModel.HinhThucDaoTao;
                hocSinh.GioiTinh = hocSinhViewModel.GioiTinh;
                hocSinh.DanToc = hocSinhViewModel.DanToc;
                //hocSinh.SoVaoSo = hocSinhViewModel.SoVaoSo;
                hocSinh.XetHK = hocSinhViewModel.XetHK;
                hocSinh.UT = hocSinhViewModel.UT;
                hocSinh.KK = hocSinhViewModel.KK;
                hocSinh.HL = hocSinhViewModel.HL;
                hocSinh.HK = hocSinhViewModel.HK;
                hocSinh.TT = hocSinhViewModel.TT;
                hocSinh.KQ = hocSinhViewModel.KQ;
                hocSinh.DiemThi = hocSinhViewModel.DiemThi;
                hocSinh.HoiDongThi = hocSinhViewModel.HoiDongThi;
                hocSinh.DaInBangGoc = false;
                hocSinh.NgayTao = hocSinhViewModel.NgayTao;
                hocSinh.NgayCapNhat = hocSinhViewModel.NgayCapNhat;
                hocSinh.NguoiTao = hocSinhViewModel.NguoiTao;
                hocSinh.NguoiCapNhat = hocSinhViewModel.NguoiCapNhat;
                hocSinh.IsDeleted = false;
                hocSinh.DonViId = donViId;
                hocSinh.SoLanXet = 0;
                DbContext.HocSinhs.Add(hocSinh);
                DbContext.SaveChanges();
                if (hocSinhViewModel.DiemMonHocs != null)
                {
                    foreach (DiemMonHocViewModel diemMonHoc in hocSinhViewModel.DiemMonHocs)
                    {
                        diemMonHoc.HocSinhId = hocSinh.Id;
                    }
                }
                if (hocSinhViewModel.DiemMonHocs != null && hocSinhViewModel.DiemMonHocs.Count != 0)
                {
                    CapNhatDiemMonHoc(hocSinhViewModel.DiemMonHocs);
                }
                var objLog = new LogHocSinh();
                var user = new NguoiDungProvider().GetById(hocSinhViewModel.NguoiCapNhat);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm thêm mới thông tin học sinh " + hocSinh.HoVaTen;
                objLog.HocSinhId = hocSinh.Id;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                InsertLog(objLog);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<NienKhoaViewModel> GetNienKhoas(int donViId)
        {
            try
            {
                List<NienKhoaViewModel> nienKhoaViewModels = new List<NienKhoaViewModel>();
                string sqlString_2 = @" Select  b.Id, b.TenLop, b.NienKhoa,b.GiaoVien, b.DonViId, b.NgayTao, Convert(varchar(10), a.NamTotNghiep ) as 'Nam'
                                        From ( Select NamTotNghiep, LopHocId From HocSinh
			                                        Where (DonViId = @DonViId) and (IsDeleted = 0) 
			                                        Group by NamTotNghiep, LopHocId) as a
                                        Left Join LopHocs  as  b
                                        on a.LopHocId = b.Id
                                        order by NamTotNghiep desc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            var result = MapDataHelper<LopHocViewModel>.MapList(reader);
                            foreach (var item in result)
                            {
                                if (nienKhoaViewModels.Any(x => x.Nam == item.Nam))
                                {
                                    NienKhoaViewModel nienKhoaViewModel = nienKhoaViewModels.Where(x => x.Nam == item.Nam).FirstOrDefault();
                                    nienKhoaViewModel.LopHocs.Add(new LopHocViewModel()
                                    {
                                        Id = item.Id,
                                        TenLop = item.TenLop,
                                        NienKhoa = item.NienKhoa
                                    });

                                }
                                else
                                {
                                    NienKhoaViewModel nienKhoaViewModel = new NienKhoaViewModel();
                                    nienKhoaViewModel.Nam = item.Nam;
                                    nienKhoaViewModel.LopHocs = new List<LopHocViewModel>();
                                    nienKhoaViewModel.LopHocs.Add(new LopHocViewModel()
                                    {
                                        Id = item.Id,
                                        TenLop = item.TenLop,
                                        NienKhoa = item.NienKhoa
                                    });
                                    nienKhoaViewModels.Add(nienKhoaViewModel);
                                }
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return nienKhoaViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateHocSinh(HocSinhViewModel hocSinhViewModel, int donViId, string ip)
        {
            try
            {
                string sqlString = @"Select * From [dbo].[HocSinh]
                                      Where [Id] = @Id and [DonViId] = @DonViId and IsDeleted = 0";
                HocSinh hocSinh = DbContext.HocSinhs.FromSqlRaw(sqlString, new SqlParameter("@Id", hocSinhViewModel.Id), new SqlParameter("@DonViId", donViId)).FirstOrDefault();
                if (hocSinh != null)
                {
                    //string sqlString_2 = @"Select Count(*) as 'TotalRow' From HocSinh Where (SoVaoSo Like @SoVaoSo) and (Id != @HocSinhId)";
                    //int totalRow = 0;
                    //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    //{
                    //    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    //    if (!wasOpen) command.Connection.Open();

                    //    try
                    //    {
                    //        command.CommandText = sqlString_2;
                    //        command.CommandType = CommandType.Text;
                    //        command.Parameters.Add(new SqlParameter("@SoVaoSo", hocSinhViewModel.SoVaoSo));
                    //        command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinh.Id));
                    //        using (var reader = command.ExecuteReader())
                    //        {
                    //            totalRow = MapDataHelper<int>.Map(reader);
                    //        }
                    //    }
                    //    finally
                    //    {
                    //        command.Connection.Close();
                    //    }
                    //}

                    //if (totalRow > 0)
                    //{
                    //    Exception exception = new Exception("Số vào sổ đã tồn tại");
                    //    throw exception;
                    //}

                    string sqlString_1 = @"Select * From Bang Where HocSinhId = @HocSinhId";
                    List<Bang> bangs = DbContext.Bangs.FromSqlRaw(sqlString_1, new SqlParameter("@HocSinhId", hocSinh.Id)).ToList();
                    if (bangs != null && bangs.Any(x => x.TrangThaiBangId >= 4))
                    {
                        Exception exception = new Exception("Học sinh đã in văn bằng, không thể cập nhật thông tin của học sinh!");
                        throw exception;
                    }

                    hocSinh.HoVaTen = hocSinhViewModel.HoVaTen;
                    hocSinh.NgaySinh = hocSinhViewModel.NgaySinh;
                    hocSinh.NoiSinh = hocSinhViewModel.NoiSinh;
                    hocSinh.HoKhauThuongTru = hocSinhViewModel.HoKhauThuongTru;
                    //hocSinh.TruongHocId = donViId;
                    hocSinh.LopHoc = hocSinhViewModel.LopHoc;
                    hocSinh.LopHocId = hocSinhViewModel.LopHocId;
                    hocSinh.DanTocId = hocSinhViewModel.DanTocId;
                    hocSinh.GioiTinhId = hocSinhViewModel.GioiTinhId;
                    hocSinh.GioiTinh = hocSinhViewModel.GioiTinh;
                    hocSinh.DanToc = hocSinhViewModel.DanToc;
                    //hocSinh.TruongHoc = hocSinhViewModel.TruongHoc;
                    hocSinh.NamTotNghiep = hocSinhViewModel.NamTotNghiep;
                    hocSinh.XepLoaiTotNghiep = hocSinhViewModel.XepLoaiTotNghiep;
                    hocSinh.HinhThucDaoTao = hocSinhViewModel.HinhThucDaoTao;
                    //hocSinh.SoVaoSo = hocSinhViewModel.SoVaoSo;
                    hocSinh.XetHK = hocSinhViewModel.XetHK;
                    hocSinh.UT = hocSinhViewModel.UT;
                    hocSinh.KK = hocSinhViewModel.KK;
                    hocSinh.HL = hocSinhViewModel.HL;
                    hocSinh.HK = hocSinhViewModel.HK;
                    hocSinh.DiemThi = hocSinhViewModel.DiemThi;
                    hocSinh.HoiDongThi = hocSinhViewModel.HoiDongThi;
                    //hocSinh.KQ = hocSinhViewModel.KQ;
                    hocSinh.NgayCapNhat = hocSinhViewModel.NgayCapNhat;
                    hocSinh.NguoiCapNhat = hocSinhViewModel.NguoiCapNhat;
                    //hocSinh.IsDeleted = false;
                    //hocSinh.DonViId = donViId;
                    DbContext.SaveChanges();

                    if (hocSinhViewModel.DiemMonHocs != null)
                    {
                        foreach (DiemMonHocViewModel diemMonHoc in hocSinhViewModel.DiemMonHocs)
                        {
                            diemMonHoc.HocSinhId = hocSinh.Id;
                        }
                    }
                    CapNhatDiemMonHoc(hocSinhViewModel.DiemMonHocs);

                    var objLog = new LogHocSinh();
                    var user = new NguoiDungProvider().GetById(hocSinhViewModel.NguoiCapNhat);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã thêm cập nhật thông tin học sinh " + hocSinh.HoVaTen;
                    objLog.HocSinhId = hocSinh.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                }
                else
                {
                    Exception exception = new Exception("Không tìm thấy học sinh");
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateTrangThaiXetDuyet(HocSinhViewModel hocSinhViewModel, int donViId, string ip)
        {
            try
            {
                string sqlString = @"Select * From [dbo].[HocSinh]
                                      Where [Id] = @Id and [DonViId] = @DonViId and IsDeleted = 0";
                HocSinh hocSinh = DbContext.HocSinhs.FromSqlRaw(sqlString, new SqlParameter("@Id", hocSinhViewModel.Id), new SqlParameter("@DonViId", donViId)).FirstOrDefault();
                if (hocSinh == null)
                {
                    Exception exception = new Exception("Không tìm thấy học sinh!");
                    throw exception;
                }
                string sqlString_1 = @"Select * From Bang Where HocSinhId = @HocSinhId";
                List<Bang> bangs = DbContext.Bangs.FromSqlRaw(sqlString_1, new SqlParameter("@HocSinhId", hocSinh.Id)).ToList();
                if (bangs != null && bangs.Any(x => x.TrangThaiBangId >= 4))
                {
                    Exception exception = new Exception("Học sinh đã in văn bằng, không thể cập nhật thông tin của học sinh!");
                    throw exception;
                }

                hocSinh.SoLanXet = hocSinh.SoLanXet + 1;
                hocSinh.SoVaoSo = hocSinhViewModel.SoVaoSo;
                hocSinh.KQ = hocSinhViewModel.KQ;
                hocSinh.XepLoaiTotNghiep = hocSinhViewModel.XepLoaiTotNghiep;
                hocSinh.NgayCapNhat = hocSinhViewModel.NgayCapNhat;
                hocSinh.NguoiCapNhat = hocSinhViewModel.NguoiCapNhat;
                DbContext.SaveChanges();

                var objLog = new LogHocSinh();
                var user = new NguoiDungProvider().GetById(hocSinhViewModel.NguoiCapNhat);
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã thêm cập nhật trạng thái xét duyệt học sinh: " + hocSinh.HoVaTen;
                objLog.HocSinhId = hocSinh.Id;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                objLog.Ip = ip;
                InsertLog(objLog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetSTTHocSinhDuocXet(int namTotNghiep, int donViId)
        {
            try
            {
                int stt = 0;
                //string sqlString = @"Select Count(*) as 'TotalRow' From HocSinh Where (KQ = 1) and (CongNhanTotNghiep = 1) and (DonViId = @DonViId) and (NamTotNghiep = @NamTotNghiep)";
                string sqlString = @"Select IsNull(Max(TT),0) as 'Max' From HocSinh Where (KQ = 1) and (CongNhanTotNghiep = 1) and (DonViId = @DonViId) and (NamTotNghiep = @NamTotNghiep)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep));

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                stt = Convert.ToInt32(reader[0]) + 1;
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

                return stt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel ExportDanhSachDaXetDuyetTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                List<HocSinhViewModel> hocSinhViewModels = new List<HocSinhViewModel>();
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep)) and ((@CongNhanTotNghiep is null) or (a.CongNhanTotNghiep = @CongNhanTotNghiep))
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                    Order By e.NienKhoa Desc , a.LopHocId, RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@CongNhanTotNghiep", congNhanTotNghiep.HasValue ? congNhanTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs.Count == 0)
                {
                    Exception exception = new Exception("Không có học sinh!!!");
                    throw exception;
                }

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachHocSinhDuXetCapTruong(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, int? namTotNghiep, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                List<HocSinhViewModel> hocSinhViewModels = new List<HocSinhViewModel>();
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep)) and (a.CongNhanTotNghiep is null)
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and (a.Id not in ( Select HocSinhId From LienKetHocSinhYeuCau as a
					                                Left Join YeuCau as b
					                                on a.YeuCauId = b.Id	
					                                Where ((b.MaTrangThaiYeuCau like 'approved') or (b.MaTrangThaiYeuCau like 'waiting')) and (b.DonViYeuCauId = @DonViId) ))         
                                            Order By e.NienKhoa Desc , a.LopHocId, RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@CongNhanTotNghiep", congNhanTotNghiep.HasValue ? congNhanTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel ExportDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, int? namTotNghiep, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                List<HocSinhViewModel> hocSinhViewModels = new List<HocSinhViewModel>();
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
                                            and ((@NamTotNghiep is null) or (a.NamTotNghiep = @NamTotNghiep)) and (f.KhoaChaId = @DonViId)
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                    Order By e.NienKhoa Desc , a.SoVaoSo, RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep.HasValue ? namTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs.Count == 0)
                {
                    Exception exception = new Exception("Không có học sinh");
                    throw exception;
                }

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                hocSinhsWithPaginationViewModel.TongSoHocSinhCongNhanTotNghiep = hocSinhsWithPaginationViewModel.HocSinhs.Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiGioi = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep.ToLower() == "giỏi").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiKha = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep.ToLower() == "khá").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiTrungBinh = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep.ToLower() == "trung bình").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhNam = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 1).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhNu = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 2).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhUT = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.UT.HasValue).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhKK = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => !string.IsNullOrEmpty(x.KK)).Count();
                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel ExportDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId))
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and ((@DaInBangGoc is null) or (a.DaInBangGoc = @DaInBangGoc))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@DaInBangGoc", daInBangGoc.HasValue ? daInBangGoc.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs.Count == 0)
                {
                    Exception exception = new Exception("Không có học sinh");
                    throw exception;
                }

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachDuXetCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, string nienKhoa, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and (a.CongNhanTotNghiep is null)
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and (a.Id not in ( Select HocSinhId From LienKetHocSinhYeuCau as a
					                                Left Join YeuCau as b
					                                on a.YeuCauId = b.Id	
					                                Where ((b.MaTrangThaiYeuCau like 'approved') or (b.MaTrangThaiYeuCau like 'waiting')) and (b.DonViYeuCauId = @DonViId) ))
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
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get tong so row
                string sqlString_4 = @"Select Count(*) as 'TotalRow' From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and (a.CongNhanTotNghiep is null)
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

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
                    hocSinhsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    hocSinhsWithPaginationViewModel.CurrentPage = currentPage;
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachDaXetDuyet(string hoVaTen, int? namSinh, int? gioiTinhId, int? lopHocId, string nienKhoa, int? danTocId, string hL, string hK, bool? kQ, string xepLoaiTotNghiep, bool? congNhanTotNghiep, int currentPage, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@CongNhanTotNghiep is null) or (a.CongNhanTotNghiep = @CongNhanTotNghiep))
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
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
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@CongNhanTotNghiep", congNhanTotNghiep.HasValue ? congNhanTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get tong so row
                string sqlString_4 = @"Select Count(*) as 'TotalRow' From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Left Join LopHocs as e
                                    on a.LopHocId = e.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and ((@LopHocId is null) or (a.LopHocId = @LopHocId)) 
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@CongNhanTotNghiep is null) or (a.CongNhanTotNghiep = @CongNhanTotNghiep))
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((a.KQ is not null) and ((@KQ is null) or (a.KQ = @KQ))) and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null )or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@CongNhanTotNghiep", congNhanTotNghiep.HasValue ? congNhanTotNghiep.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@KQ", kQ.HasValue ? kQ.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

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
                    hocSinhsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    hocSinhsWithPaginationViewModel.CurrentPage = currentPage;
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachDaCongNhanTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa,
		                                    g.TrangThaiBangId, h.MaMauTrangThai as 'MaMauTrangThaiBang', h.Ten as 'TrangThaiBang'
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and (g.BangGocId is null)
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
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
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get tong so row
                string sqlString_4 = @"Select Count(*) as 'TotalRow' From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and (g.BangGocId is null)
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null or (a.GioiTinhId = @GioiTinhId)))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

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
                    hocSinhsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    hocSinhsWithPaginationViewModel.CurrentPage = currentPage;
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int currentPage, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa,
		                                    g.TrangThaiBangId, h.MaMauTrangThai as 'MaMauTrangThaiBang', h.Ten as 'TrangThaiBang'
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and (g.BangGocId is null)
                                            and ((@NienKhoa is null) or(a.NamTotNghiep = @NienKhoa)) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and ((@LopHocId is null) or (a.LopHocId = @LopHocId))
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
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : Convert.ToInt32(nienKhoa)));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get tong so row
                string sqlString_4 = @"Select Count(*) as 'TotalRow' From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and (g.BangGocId is null)
                                            and ((@NienKhoa is null) or(a.NamTotNghiep = @NienKhoa)) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null or (a.GioiTinhId = @GioiTinhId)))
                                            and ((@LopHocId is null) or (a.LopHocId = @LopHocId))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : Convert.ToInt32(nienKhoa)));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

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
                    hocSinhsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    hocSinhsWithPaginationViewModel.CurrentPage = currentPage;
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel ExportDanhSachDaCongNhanTotNghiepTheoDonVi(string hoVaTen, int? namSinh, int? lopHocId, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa,
		                                    g.TrangThaiBangId, h.MaMauTrangThai as 'MaMauTrangThaiBang', h.Ten as 'TrangThaiBang'
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) and (g.BangGocId is null)
                                            and ((@NienKhoa is null) or(a.NamTotNghiep = @NienKhoa)) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and ((@LopHocId is null) or (a.LopHocId = @LopHocId))
                                    Order By e.NienKhoa Desc , a.LopHocId, RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS Asc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId.HasValue ? lopHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : Convert.ToInt32(nienKhoa)));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                hocSinhsWithPaginationViewModel.TongSoHocSinhCongNhanTotNghiep = hocSinhsWithPaginationViewModel.HocSinhs.Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiGioi = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "giỏi") == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "giỏi").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiKha = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "khá") == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "khá").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhLoaiTrungBinh = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "trung bình")== null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.XepLoaiTotNghiep != null && x.XepLoaiTotNghiep.ToLower() == "trung bình").Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhNam = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 1) == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 1).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhNu = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 2) == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.GioiTinhId == 2).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhUT = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.UT.HasValue) == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => x.UT.HasValue).Count();
                hocSinhsWithPaginationViewModel.TongSoHocSinhKK = hocSinhsWithPaginationViewModel.HocSinhs.Where(x => !string.IsNullOrEmpty(x.KK)) == null ? 0 : hocSinhsWithPaginationViewModel.HocSinhs.Where(x => !string.IsNullOrEmpty(x.KK)).Count();

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachDaTotNghiep(string hoVaTen, int? namSinh, int? gioiTinhId, int? truongHocId, string nienKhoa, int? danTocId, string hL, string hK, string xepLoaiTotNghiep, bool? daInBangGoc, int currentPage, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh', e.NienKhoa From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId))
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null) or (a.GioiTinhId = @GioiTinhId))
                                            and ((@DaInBangGoc is null) or (a.DaInBangGoc = @DaInBangGoc))
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
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@DaInBangGoc", daInBangGoc.HasValue ? daInBangGoc.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@Offset", (currentPage - 1) * 12));
                        command.Parameters.Add(new SqlParameter("@Next", 12));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
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

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                // get tong so row
                string sqlString_4 = @"Select Count(*) as 'TotalRow' From [dbo].[HocSinh] as a
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
                                    Where (a.IsDeleted = 0) and (a.KQ = 1) and (a.CongNhanTotNghiep = 1) and ((@TruongHocId is null) or (a.TruongHocId = @TruongHocId)) 
                                            and ((@NienKhoa is null) or(e.NienKhoa like N'%'+@NienKhoa+'%')) and ((@DonViId is null) or (f.KhoaChaId = @DonViId)) 
		                                    and ((@HoVaTen is null) or (a.HoVaTen like N'%'+@HoVaTen+'%')) and ((@DanTocId is null) or (a.DanTocId = @DanTocId))
		                                    and ((@HL is null) or (a.HL like N'%'+@HL+'%')) and ((@HK is null) or (a.HK like N'%'+@HK+'%'))
		                                    and ((@XepLoaiTotNghiep is null) or (a.XepLoaiTotNghiep like N'%'+@XepLoaiTotNghiep+'%'))
                                            and ((@NamSinh is null) or (YEAR(a.NgaySinh) = @NamSinh)) and ((@GioiTinhId is null or (a.GioiTinhId = @GioiTinhId)))
                                            and ((@DaInBangGoc is null) or (a.DaInBangGoc = @DaInBangGoc))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    int totalRow = 0;
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DaInBangGoc", daInBangGoc.HasValue ? daInBangGoc.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@DonViId", truongHocId.HasValue ? DBNull.Value : donViId));
                        command.Parameters.Add(new SqlParameter("@TruongHocId", truongHocId.HasValue ? truongHocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@NienKhoa", string.IsNullOrEmpty(nienKhoa) ? DBNull.Value : nienKhoa));
                        command.Parameters.Add(new SqlParameter("@HoVaTen", string.IsNullOrEmpty(hoVaTen) ? DBNull.Value : hoVaTen));
                        command.Parameters.Add(new SqlParameter("@DanTocId", danTocId.HasValue ? danTocId.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@HL", string.IsNullOrEmpty(hL) ? DBNull.Value : hL));
                        command.Parameters.Add(new SqlParameter("@HK", string.IsNullOrEmpty(hK) ? DBNull.Value : hK));
                        command.Parameters.Add(new SqlParameter("@XepLoaiTotNghiep", string.IsNullOrEmpty(xepLoaiTotNghiep) ? DBNull.Value : xepLoaiTotNghiep));
                        command.Parameters.Add(new SqlParameter("@NamSinh", namSinh.HasValue ? namSinh.Value : DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@GioiTinhId", gioiTinhId.HasValue ? gioiTinhId.Value : DBNull.Value));

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
                    hocSinhsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / 12));
                    hocSinhsWithPaginationViewModel.CurrentPage = currentPage;
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CapNhatDiemMonHoc(List<DiemMonHocViewModel> diemMonHocViewModels)
        {
            try
            {
                if (diemMonHocViewModels == null || diemMonHocViewModels.Count == 0)
                {
                    return;
                }
                string sqlString = "Delete From DiemMonHocs Where HocSinhId = @HocSinhId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HocSinhId", diemMonHocViewModels.FirstOrDefault().HocSinhId));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                if (diemMonHocViewModels != null)
                {
                    List<DiemMonHoc> diemMonHocs = new List<DiemMonHoc>();
                    foreach (DiemMonHocViewModel diemMonHocViewModel in diemMonHocViewModels)
                    {
                        DiemMonHoc diemMonHoc = new DiemMonHoc();
                        diemMonHoc.CodeMonHoc = diemMonHocViewModel.CodeMonHoc;
                        diemMonHoc.HocSinhId = diemMonHocViewModel.HocSinhId.Value;
                        diemMonHoc.Diem = diemMonHocViewModel.Diem;
                        diemMonHocs.Add(diemMonHoc);
                    }
                    DbContext.DiemMonHocs.AddRange(diemMonHocs);
                    DbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResultModel AddAttachFile(List<HocSinhFileDinhKemViewModel> attachFileViewModels, int HocSinhId)
        {
            try
            {
                var lst = new List<HocSinhFileDinhKem>();
                foreach (var item in attachFileViewModels)
                {
                    HocSinhFileDinhKem hocSinhFileDinhKem = new HocSinhFileDinhKem();
                    hocSinhFileDinhKem.FileId = item.FileId;
                    hocSinhFileDinhKem.Url = item.Url;
                    hocSinhFileDinhKem.TenFile = item.TenFile;
                    hocSinhFileDinhKem.HocSinhId = HocSinhId;
                    hocSinhFileDinhKem.NguoiTao = item.NguoiTao;
                    hocSinhFileDinhKem.NgayTao = item.NgayTao;
                    hocSinhFileDinhKem.DonViId = item.DonViId;

                    switch (item.Ext.ToLower())
                    {
                        case ".jpg":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/jpg.png";
                            break;
                        case ".jpeg":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/jpg.png";
                            break;
                        case ".png":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/png.png";
                            break;
                        case ".pdf":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/pdf.png";
                            break;
                        case ".xlsx":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/excel.png";
                            break;
                        case ".xls":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/excel.png";
                            break;
                        case ".doc":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/doc.png";
                            break;
                        case ".docx":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/doc.png";
                            break;
                        case ".ppt":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/ppt.png";
                            break;
                        case ".txt":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/txt.png";
                            break;
                        case ".zip":
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/zip.png";
                            break;
                        default:
                            hocSinhFileDinhKem.IconFile = "http://certapi.jbtech.vn/Assets/FileTypes/unknow.png";
                            break;
                    }

                    lst.Add(hocSinhFileDinhKem);
                }
                DbContext.HocSinhFileDinhKems.AddRange(lst);
                DbContext.SaveChanges();

                return new ResultModel(true, "Đính kèm file thành công");
            }
            catch (Exception ex)
            {
                return new ResultModel(false, ex.Message);
            }
        }

        public HocSinhsWithPaginationViewModel GetHocSinhs(int lopHocId, int namTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh' From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and (a.LopHocId = @LopHocId) and (a.NamTotNghiep = @NamTotNghiep)
                                    Order By RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HocSinhsWithPaginationViewModel GetDanhSachCanPheDuyet(int lopHocId, int namTotNghiep, int donViId)
        {
            try
            {
                HocSinhsWithPaginationViewModel hocSinhsWithPaginationViewModel = new HocSinhsWithPaginationViewModel();
                // get danh sach hoc sinh
                string sqlString = @"Select a.*, b.TenDonVi as 'TruongHoc', c.Ten as 'DanToc', d.Ten as 'GioiTinh' From [dbo].[HocSinh] as a
                                    Left Join [dbo].[DonVi] as b
                                    on a.TruongHocId = b.DonViId
                                    Left Join [dbo].[DanToc] as c
                                    on a.DanTocId = c.Id
                                    Left Join [dbo].[GioiTinh] as d
                                    on a.GioiTinhId = d.Id
                                    Where (a.IsDeleted = 0) and (a.DonViId = @DonViId) and (a.LopHocId = @LopHocId) and (a.CongNhanTotNghiep is null) and (a.NamTotNghiep = @NamTotNghiep)
                                    Order By RIGHT(a.HoVaTen,CHARINDEX(' ',REVERSE(a.HoVaTen ))-1) COLLATE Vietnamese_CI_AS";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", namTotNghiep));

                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhsWithPaginationViewModel.HocSinhs = MapDataHelper<HocSinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (hocSinhsWithPaginationViewModel.HocSinhs != null && hocSinhsWithPaginationViewModel.HocSinhs.Count() != 0)
                {
                    // get danh sach diem mon hoc trong tung hoc sinh
                    foreach (HocSinhViewModel hocSinhViewModel in hocSinhsWithPaginationViewModel.HocSinhs)
                    {
                        string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                                using (var reader = command.ExecuteReader())
                                {
                                    hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }

                    // get danh sach mon hoc
                    string sqlString_3 = string.Format(@"Select * From [MonHocs] 
                                                        Where CodeCapDonVi Like (Select b.Code From DonVi as a 
                                                                                    Left Join CapDonVi as b 
                                                                                    on a.CapDonViId = b.CapDonViId 
                                                                                    Where DonViId = @DonViId )");
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));

                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhsWithPaginationViewModel.MonHocs = MapDataHelper<MonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResultModel DeleteAttachFile(HocSinhFileDinhKemViewModel model, NguoiDung user)
        {
            try
            {
                var obj = DbContext.HocSinhFileDinhKems.FirstOrDefault(f => f.FileId == model.FileId);
                DbContext.HocSinhFileDinhKems.Remove(obj);
                DbContext.SaveChanges();
                var objLog = new LogHocSinh();
                objLog.NguoiDungId = user.NguoiDungId;
                objLog.HanhDong = "Đã xóa file: " + model.TenFile;
                objLog.HocSinhId = model.HocSinhId;
                objLog.ThoiGian = DateTime.Now;
                objLog.HoTen = user.HoTen;
                InsertLog(objLog);
                return new ResultModel(true, "Xóa file thành công");
            }
            catch (Exception ex)
            {
                return new ResultModel(false, ex.Message);
            }
        }

        public HocSinhViewModel GetHocSinh(int hocSinhId, int donViId)
        {
            try
            {

                string sqlString = @";With T1 
                                    as (
	                                    Select * From DonVi Where DonViId = (Select DonViId From HocSinh Where Id = @HocSinhId)
	                                    Union all
	                                    Select a.* From DonVi as a
	                                    inner Join T1 as b
	                                    on a.DonViId = b.KhoaChaId
                                    )
                                    Select * From HocSinh Where (Id = @HocSinhId) and (@DonViId in (Select DonViId From T1))";
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                sqlParameters.Add(new SqlParameter("@HocSinhId", hocSinhId));
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

                

                string sqlString_2 = @"Select * From [DiemMonHocs] Where HocSinhId = @HocSinhId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhId));
                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhViewModel.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (hocSinhViewModel != null)
                {
                    string sqlString_1 = @";With T1 
                                        as (
	                                        Select * From DonVi Where DonViId = (Select DonViId From HocSinh Where Id = @HocSinhId)
	                                        Union all
	                                        Select a.* From DonVi as a
	                                        inner Join T1 as b
	                                        on a.DonViId = b.KhoaChaId
                                        )
                                       Select * From [dbo].[HocSinhFileDinhKem] Where (HocSinhId = @HocSinhId) and (@DonViId in (Select DonViId From T1))";
                    List<SqlParameter> sqlParameters_1 = new List<SqlParameter>();
                    sqlParameters_1.Add(new SqlParameter("@HocSinhId", hocSinhId));
                    sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));

                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddRange(sqlParameters_1.ToArray());
                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhViewModel.Files = MapDataHelper<HocSinhFileDinhKemViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                }

                return hocSinhViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteHocSinhs(List<HocSinhViewModel> hocSinhViewModels, int donViId, string ip)
        {
            try
            {

                int i = 0;
                string[] paramIds = hocSinhViewModels.Select(x => "@Id_" + i++).ToArray();
                string sqlString = string.Format(@"Select * From [dbo].[HocSinh]
                                      Where [Id] In ({0}) and [DonViId] = @DonViId and IsDeleted = 0", string.Join(",", paramIds));
                i = 0;
                List<SqlParameter> sqlParameters = hocSinhViewModels.Select(x => new SqlParameter("@Id_" + i++, x.Id)).ToList();
                sqlParameters.Add(new SqlParameter("@DonViId", donViId));
                List<HocSinh> hocSinhs = DbContext.HocSinhs.FromSqlRaw(sqlString, sqlParameters.ToArray()).ToList();
                foreach (HocSinh hocSinh in hocSinhs)
                {
                    string sqlString_1 = @"Select * From Bang Where HocSinhId = @HocSinhId";
                    List<Bang> bangs = DbContext.Bangs.FromSqlRaw(sqlString_1, new SqlParameter("@HocSinhId", hocSinh.Id)).ToList();
                    if (bangs != null && bangs.Any(x => x.TrangThaiBangId >= 4))
                    {
                        Exception exception = new Exception("Học sinh đã in văn bằng, không thể xóa học sinh " + hocSinh.HoVaTen);
                        throw exception;
                    }

                    hocSinh.NgayCapNhat = hocSinhViewModels.Where(x => hocSinhs.Any(y => y.Id == x.Id)).FirstOrDefault().NgayCapNhat;
                    hocSinh.NguoiCapNhat = hocSinhViewModels.Where(x => hocSinhs.Any(y => y.Id == x.Id)).FirstOrDefault().NguoiCapNhat;
                    hocSinh.IsDeleted = true;
                    DbContext.SaveChanges();

                    //string sqlString_1 = string.Format(@"Select * From [dbo].[HocSinhFileDinhKem]
                    //                                    Where ([DonViId] = @DonViId) and ([IsDeleted] = 0) and ([Id] In ({0}))", string.Join(",", paramIds));
                    //i = 0;
                    //List<SqlParameter> sqlParameters_1 = hocSinhViewModels.Select(x => new SqlParameter("@Id_" + i++, x.Id)).ToList();
                    //sqlParameters_1.Add(new SqlParameter("@DonViId", donViId));
                    //List<HocSinhFileDinhKem> hocSinhFileDinhKems = DbContext.HocSinhFileDinhKems.SqlQuery(sqlString_1, sqlParameters_1.ToArray()).ToList();
                    //foreach (HocSinhFileDinhKem hocSinhFileDinhKem in hocSinhFileDinhKems)
                    //{
                    //    hocSinhFileDinhKem.IsDeleted = true;
                    //    hocSinhFileDinhKem.NgayCapNhat = hocSinhViewModels.Where(x => hocSinhs.Any(y => y.Id == x.Id)).FirstOrDefault().NgayCapNhat;
                    //    hocSinhFileDinhKem.NguoiCapNhat = hocSinhViewModels.Where(x => hocSinhs.Any(y => y.Id == x.Id)).FirstOrDefault().NguoiCapNhat;
                    //    DbContext.SaveChanges();
                    //}

                    var objLog = new LogHocSinh();
                    var user = new NguoiDungProvider().GetById(hocSinhViewModels.Where(x => hocSinhs.Any(y => y.Id == x.Id)).FirstOrDefault().NguoiCapNhat);
                    objLog.NguoiDungId = user.NguoiDungId;
                    objLog.HanhDong = "Đã thêm xóa học sinh " + hocSinh.HoVaTen;
                    objLog.HocSinhId = hocSinh.Id;
                    objLog.ThoiGian = DateTime.Now;
                    objLog.HoTen = user.HoTen;
                    objLog.Ip = ip;
                    InsertLog(objLog);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddAttachFile(List<HocSinhFileDinhKemViewModel> attachFileViewModels)
        {
            throw new NotImplementedException();
        }

        public void DeleteAttachFile(List<AttachFileViewModel> attachFileViewModels)
        {
            throw new NotImplementedException();
        }

        public void UpdateSoVaoSo(List<HocSinhViewModel> hocSinhViewModels, int donViId)
        {
            try
            {
                foreach (HocSinhViewModel hocSinhViewModel in hocSinhViewModels)
                {
                    string sqlString = @"Update HocSinh Set SoVaoSo = @SoVaoSo Where Id = @Id";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@SoVaoSo", hocSinhViewModel.SoVaoSo));
                            command.Parameters.Add(new SqlParameter("@Id", hocSinhViewModel.Id));
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

        public void CongNhanTotNghiepHocSinhTrongYeuCau(int yeuCauId, string maTruongHoc, string maDonViSo, int donViId)
        {
            try
            {
                // get danh sach hoc sinh trong yeu cau
                string sqlString = @"Select b.*
                                      From [LienKetHocSinhYeuCau] as a
                                      Left Join HocSinh as b
                                      on a.HocSinhId = b.Id
                                      Where YeuCauId = @YeuCauId";
                List<HocSinhViewModel> hocSinhViewModels = new List<HocSinhViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@YeuCauId", yeuCauId));
                        using (var reader = command.ExecuteReader())
                        {
                            hocSinhViewModels = MapDataHelper<HocSinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }

                foreach (HocSinhViewModel hocSinhViewModel in hocSinhViewModels)
                {
                    string sqlString_1 = @"Update HocSinh
                                            Set CongNhanTotNghiep = 1
                                            Where Id = @HocSinhId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhViewModels = MapDataHelper<HocSinhViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            if (!wasOpen) command.Connection.Close();
                        }
                    }

                    int tt = GetSTTHocSinhDuocXet(hocSinhViewModel.NamTotNghiep.Value, hocSinhViewModel.TruongHocId);
                    hocSinhViewModel.SoVaoSo = maDonViSo + "/" + maTruongHoc + "/" + tt + "/" + hocSinhViewModel.NamTotNghiep;
                    hocSinhViewModel.TT = tt;
                    string sqlString_2 = @"Update HocSinh
                                            Set SoVaoSo = @SoVaoSo, TT = @TT
                                            Where Id = @HocSinhId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = System.Data.CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@TT", hocSinhViewModel.TT.Value));
                            command.Parameters.Add(new SqlParameter("@HocSinhId", hocSinhViewModel.Id));
                            command.Parameters.Add(new SqlParameter("@SoVaoSo", hocSinhViewModel.SoVaoSo));
                            using (var reader = command.ExecuteReader())
                            {
                                hocSinhViewModels = MapDataHelper<HocSinhViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            if (!wasOpen) command.Connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region LOG
        public ResultModel InsertLog(LogHocSinh model)
        {
            try
            {
                DbContext.LogHocSinhs.Add(model);
                DbContext.SaveChanges();
                return new ResultModel(true, "Log success");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }

        public List<LogHocSinhViewModel> GetByHocSinhId(int HocSinhId)
        {
            try
            {
                List<LogHocSinhViewModel> logHocSinhViewModels = new List<LogHocSinhViewModel>();
                string sqlString = "select * from LogHocSinh where HocSinhId = " + HocSinhId + " Order by ThoiGian DESC";
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
                            logHocSinhViewModels = MapDataHelper<LogHocSinhViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return logHocSinhViewModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //public void AddMonHoc(MonHocViewModel monHocViewModel, int donViId)
        //{
        //    try
        //    {
        //        MonHoc monHoc = new MonHoc();
        //        monHoc.TenMonHoc = monHocViewModel.TenMonHoc;
        //        monHoc.CodeMonHoc = monHocViewModel.CodeMonHoc;
        //        monHoc.DonViId = donViId;
        //        DbContext.MonHocs.Add(monHoc);
        //        DbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public void UpdateMonHoc(MonHocViewModel monHocViewModel, int donViId)
        //{
        //    try
        //    {
        //        MonHoc monHoc = DbContext.MonHocs.Where(x => x.CodeMonHoc == monHocViewModel.CodeMonHoc).FirstOrDefault();
        //        monHoc.TenMonHoc = monHocViewModel.TenMonHoc;
        //        DbContext.MonHocs.Add(monHoc);
        //        DbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        //public void DeleteMonHoc(MonHocViewModel monHocViewModel, int donViId)
        //{
        //    try
        //    {
        //        MonHoc monHoc = DbContext.MonHocs.Where(x => x.CodeMonHoc == monHocViewModel.CodeMonHoc).FirstOrDefault();
        //        DbContext.MonHocs.Remove(monHoc);
        //        DbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        public List<MonHocViewModel> ListMonHoc(int donViId)
        {
            try
            {
                List<MonHocViewModel> monHocViewModels = new List<MonHocViewModel>();
                string sqlString = "select * from MonHoc where DonViId = @DonViID";
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
                            monHocViewModels = MapDataHelper<MonHocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return monHocViewModels;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AddLopHoc(LopHocViewModel lopHocViewModel, int donViId)
        {
            try
            {
                LopHoc lopHoc = new LopHoc();
                lopHoc.GiaoVien = lopHocViewModel.GiaoVien;
                lopHoc.TenLop = lopHocViewModel.TenLop;
                lopHoc.DonViId = donViId;
                DbContext.LopHocs.Add(lopHoc);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateLopHoc(LopHocViewModel lopHocViewModel, int donViId)
        {
            try
            {
                LopHoc lopHoc = DbContext.LopHocs.Find(lopHocViewModel.Id);
                if (lopHoc == null)
                {
                    Exception exception = new Exception("Không tồn tại lớp học!!!");
                    throw exception;
                }

                lopHoc.TenLop = lopHocViewModel.TenLop;
                lopHoc.GiaoVien = lopHocViewModel.GiaoVien;
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteLopHoc(int lopHocId)
        {
            try
            {
                LopHoc lopHoc = DbContext.LopHocs.Find(lopHocId);
                if (lopHoc == null)
                {
                    Exception exception = new Exception("Không tồn tại lớp học!!!");
                    throw exception;
                }
                DbContext.LopHocs.Remove(lopHoc);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LopHocViewModel> GetLopHocByGiaoVien(Guid nguoiDungId, int donViId)
        {
            try
            {
                List<LopHocViewModel> lopHocViewModels = new List<LopHocViewModel>();
                string sqlString = @"SELECT *, NienKhoa as 'nam'
                                    FROM [dbo].[LopHocs]
                                    Where (GiaoVien Like @NguoiDungId) and (DonViId = @DonViId)
                                    order by Convert(int, NienKhoa) desc, TenLop asc";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@NguoiDungId", nguoiDungId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
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

        public List<LopHocViewModel> GetAllLopHocTrongTruong(int donViId)
        {
            try
            {
                List<LopHocViewModel> lopHocViewModels = new List<LopHocViewModel>();
                string sqlString = @"SELECT *, NienKhoa as 'nam'
                                     FROM [dbo].[LopHocs]
                                     Where (DonViId = @DonViId)
                                    order by Convert(int, NienKhoa) desc, TenLop asc";
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
        public List<MonHocViewModel> GetMonHocsByCapDonVi(string codeCapDonVi)
        {
            try
            {
                List<MonHocViewModel> monHocViewModels = new List<MonHocViewModel>();
                string sqlString = @"Select * From MonHocs Where CodeCapDonVi = @CodeCapDonVi";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@CodeCapDonVi", codeCapDonVi));
                        using (var reader = command.ExecuteReader())
                        {
                            monHocViewModels = MapDataHelper<MonHocViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return monHocViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LopHocViewModel GetLopHoc(int lopHocId, int donViId)
        {
            try
            {
                LopHocViewModel lopHocViewModel = new LopHocViewModel();
                string sqlString = @"Select * From LopHocs Where (Id = @LopHocId) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@LopHocId", lopHocId));
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            lopHocViewModel = MapDataHelper<LopHocViewModel>.Map(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return lopHocViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
