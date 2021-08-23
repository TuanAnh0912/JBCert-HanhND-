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
using System.Threading.Tasks;

namespace jbcert.DATA.Provider
{
    public class SearchProvider : ApplicationDbContext
    {
        public SearchedCertsWithPaginationViewModel SearchBang(string keyword, int pageNum, int pageSize)
        {
            try
            {
                SearchedCertsWithPaginationViewModel searchedCertsWithPaginationViewModel = new SearchedCertsWithPaginationViewModel();
                List<SearchedCertsViewModel> searchedCerts = new List<SearchedCertsViewModel>();
                string sqlString = @"Select a.*, b.GiaTri, d.X, d.Y, d.Color, d.Font, d.Italic, d.Bold, d.Underline, d.Size, d.TruongDuLieuCode, d.TenTruongDuLieu as 'TenTruongDuLieuHocSinh',
									i.Ten as 'TenTruongDuLieu', i.KieuDuLieu, 
		                            g.Ten as 'LoaiBang', g.Width, g.Height, h.[Url] as 'AnhLoaiBang'
                                    From Bang as a
                                    Left Join ThongTinVanBang as b
                                    on a.Id = b.BangId
                                    Left Join TrangThaiBang as c
                                    on a.TrangThaiBangId = c.Id
                                    Left Join TruongDuLieuLoaiBang as d
                                    on a.LoaiBangId = d.LoaiBangId and b.TruongDuLieuCode = d.TruongDuLieuCode
                                    Left Join LoaiBang as g
                                    on a.LoaiBangId = g.Id
                                    Left Join AnhLoaiBang as h
                                    on a.LoaiBangId = h.ObjectId
									Left Join TruongDuLieu as i
									on d.TruongDuLieuCode = i.Code
                                    Where a.Id in (Select BangId From ThongTinVanBang as a
				                    Left Join Bang as b
				                    on a.BangId = b.Id
				                    Where ((@Keyword is null) or (a.GiaTri Like N'%'+@Keyword+'%') or (b.SoHieu like '%'+@Keyword+'%')) and (b.TrangThaiBangId >= 4)
                                    and (b.IsDeleted = 0) 
				                    Group By BangId
				                    Order by BangId
				                    Offset @Offset Rows Fetch Next @Next Rows Only)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));
                        command.Parameters.Add(new SqlParameter("@Offset", (pageNum - 1) * pageSize));
                        command.Parameters.Add(new SqlParameter("@Next", pageSize));

                        using (var reader = command.ExecuteReader())
                        {
                            var items = MapDataHelper<SearchedCertsViewModel>.MapList(reader);
                            foreach (SearchedCertsViewModel item in items)
                            {
                                if (searchedCerts.Any(x => x.Id == item.Id))
                                {
                                    SearchedCertsViewModel searchedCertsViewModel = searchedCerts.Where(x => x.Id == item.Id).FirstOrDefault();
                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    searchedCertsViewModel.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);
                                }
                                else
                                {
                                    SearchedCertsViewModel searchedCert = new SearchedCertsViewModel();
                                    searchedCert.Id = item.Id;
                                    searchedCert.LoaiBangId = item.LoaiBangId;
                                    searchedCert.PhoiId = item.PhoiId;
                                    searchedCert.TruongHocId = item.TruongHocId;
                                    searchedCert.TruongHoc = item.TruongHoc;
                                    searchedCert.SoVaoSo = item.SoVaoSo;
                                    searchedCert.NgayInBang = item.NgayInBang;
                                    searchedCert.NgayPhatBang = item.NgayPhatBang;
                                    searchedCert.HoVaTen = item.HoVaTen;
                                    searchedCert.LoaiBang = item.LoaiBang;
                                    searchedCert.TrangThaiBangId = item.TrangThaiBangId;
                                    searchedCert.SoHieu = item.SoHieu;
                                    searchedCert.DuongDanFileAnh = item.DuongDanFileAnh;
                                    searchedCert.CmtnguoiLayBang = item.CmtnguoiLayBang;
                                    searchedCert.SoDienThoaiNguoiLayBang = item.SoDienThoaiNguoiLayBang;
                                    searchedCert.HinhThucNhan = item.HinhThucNhan;
                                    searchedCert.Width = item.Width;
                                    searchedCert.Height = item.Height;
                                    searchedCert.AnhLoaiBang = item.AnhLoaiBang;
                                    searchedCert.TruongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();

                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    searchedCert.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);

                                    searchedCerts.Add(searchedCert);
                                }
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get totalPage
                int totalRow = 0;
                string sqlString_1 = @"Select COUNT(*) OVER () as 'TotalRow' From ThongTinVanBang as a
				                                    Left Join Bang as b
				                                    on a.BangId = b.Id
				                                    Where ((@Keyword is null) or (a.GiaTri Like N'%'+@Keyword+'%') or (b.SoHieu like '%'+@Keyword+'%')) and (b.TrangThaiBangId >= 4)
                                                            and (b.IsDeleted = 0) 
				                                    Group By BangId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));

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

                searchedCertsWithPaginationViewModel.SearchedCerts = searchedCerts;
                searchedCertsWithPaginationViewModel.Total = totalRow;
                searchedCertsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / pageSize));
                searchedCertsWithPaginationViewModel.PageNum = pageNum;

                return searchedCertsWithPaginationViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SearchedCertsWithPaginationViewModel AdvancedSearch(string keyword, int pageNum, int pageSize, string hvt, int? nam, int? idtruong)
        {
            try
            {
                SearchedCertsWithPaginationViewModel searchedCertsWithPaginationViewModel = new SearchedCertsWithPaginationViewModel();
                List<SearchedCertsViewModel> searchedCerts = new List<SearchedCertsViewModel>();
                string sql = @"Select a.*, b.GiaTri, d.X, d.Y, d.Color, d.Font, d.Italic, d.Bold, d.Underline, d.Size, d.TruongDuLieuCode, d.TenTruongDuLieu as 'TenTruongDuLieuHocSinh',
									i.Ten as 'TenTruongDuLieu', i.KieuDuLieu, 
		                            g.Ten as 'LoaiBang', g.Width, g.Height, h.[Url] as 'AnhLoaiBang'
                                    From Bang as a
                                    Left Join ThongTinVanBang as b
                                    on a.Id = b.BangId
                                    Left Join TrangThaiBang as c
                                    on a.TrangThaiBangId = c.Id
                                    Left Join TruongDuLieuLoaiBang as d
                                    on a.LoaiBangId = d.LoaiBangId and b.TruongDuLieuCode = d.TruongDuLieuCode
                                    Left Join LoaiBang as g
                                    on a.LoaiBangId = g.Id
                                    Left Join AnhLoaiBang as h
                                    on a.LoaiBangId = h.ObjectId
									Left Join TruongDuLieu as i
									on d.TruongDuLieuCode = i.Code
                                    Where a.Id in (Select BangId From ThongTinVanBang as a
				                    Left Join Bang as b
				                    on a.BangId = b.Id
				                    Where (((@keyword is null) or (a.GiaTri Like N'%'+@keyword+'%')) or (b.SoHieu like N'%'+@keyword+'%') ) and (b.TrangThaiBangId >= 4)
                                    and (b.IsDeleted = 0) 
									and (( @hvt is null) or (b.HoVaTen like N'%'+@hvt+'%')) and ((@idtruong is null) or (b.DonViId=@idtruong)) and ((@nam is null) or (b.NamTotNghiep=@nam))
				                    Group By BangId
				                    Order by BangId
				                    Offset @offset Rows Fetch Next @next Rows Only)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));
                        command.Parameters.Add(new SqlParameter("@hvt", string.IsNullOrEmpty(hvt) ? DBNull.Value : hvt.Trim()));
                        command.Parameters.Add(new SqlParameter("@idtruong", idtruong == null ? DBNull.Value : idtruong.Value));
                        command.Parameters.Add(new SqlParameter("@nam", nam == null ? DBNull.Value : nam.Value));
                        command.Parameters.Add(new SqlParameter("@offset", (pageNum - 1) * pageSize));
                        command.Parameters.Add(new SqlParameter("@next", pageSize));

                        using (var reader = command.ExecuteReader())
                        {
                            var items = MapDataHelper<SearchedCertsViewModel>.MapList(reader);
                            foreach (SearchedCertsViewModel item in items)
                            {
                                if (searchedCerts.Any(x => x.Id == item.Id))
                                {
                                    SearchedCertsViewModel searchedCertsViewModel = searchedCerts.Where(x => x.Id == item.Id).FirstOrDefault();
                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    searchedCertsViewModel.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);
                                }
                                else
                                {
                                    SearchedCertsViewModel searchedCert = new SearchedCertsViewModel();
                                    searchedCert.Id = item.Id;
                                    searchedCert.LoaiBangId = item.LoaiBangId;
                                    searchedCert.PhoiId = item.PhoiId;
                                    searchedCert.TruongHocId = item.TruongHocId;
                                    searchedCert.TruongHoc = item.TruongHoc;
                                    searchedCert.SoVaoSo = item.SoVaoSo;
                                    searchedCert.NgayInBang = item.NgayInBang;
                                    searchedCert.NgayPhatBang = item.NgayPhatBang;
                                    searchedCert.HoVaTen = item.HoVaTen;
                                    searchedCert.LoaiBang = item.LoaiBang;
                                    searchedCert.TrangThaiBangId = item.TrangThaiBangId;
                                    searchedCert.SoHieu = item.SoHieu;
                                    searchedCert.DuongDanFileAnh = item.DuongDanFileAnh;
                                    searchedCert.CmtnguoiLayBang = item.CmtnguoiLayBang;
                                    searchedCert.SoDienThoaiNguoiLayBang = item.SoDienThoaiNguoiLayBang;
                                    searchedCert.HinhThucNhan = item.HinhThucNhan;
                                    searchedCert.Width = item.Width;
                                    searchedCert.Height = item.Height;
                                    searchedCert.AnhLoaiBang = item.AnhLoaiBang;
                                    searchedCert.TruongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();

                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    searchedCert.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);

                                    searchedCerts.Add(searchedCert);
                                }
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                string sqlcout = @"Select count(*) OVER() as 'TotalRow' From ThongTinVanBang as a
				                    Left Join Bang as b
				                    on a.BangId = b.Id
				                    Where (((@Keyword is null) or (a.GiaTri Like N'%'+@Keyword+'%')) or  (b.SoHieu like '%'+@Keyword+'%')) and (b.TrangThaiBangId >= 4)
                                    and (b.IsDeleted = 0) 
									and ((@hvt is null) or (b.HoVaTen like N'%'+@hvt+'%')) and ((@idtruong is null) or (b.DonViId=@idtruong)) and ((@nam is null) or (b.NamTotNghiep=@nam))
				                    Group By BangId";
                int totalRow = 0;
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlcout;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));
                        command.Parameters.Add(new SqlParameter("@hvt", string.IsNullOrEmpty(hvt) ? DBNull.Value : hvt.Trim()));
                        command.Parameters.Add(new SqlParameter("@nam", nam==null ? DBNull.Value : nam.Value));
                        command.Parameters.Add(new SqlParameter("@idtruong", idtruong == null ? DBNull.Value : idtruong.Value));
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

                searchedCertsWithPaginationViewModel.SearchedCerts = searchedCerts;
                searchedCertsWithPaginationViewModel.Total = totalRow;
                searchedCertsWithPaginationViewModel.TotalPage = Convert.ToInt32(Math.Ceiling(totalRow * 1.0 / pageSize));
                searchedCertsWithPaginationViewModel.PageNum = pageNum;

                return searchedCertsWithPaginationViewModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public SearchedCertsViewModel Detail(string keyword, int bangId)
        {
            try
            {
                // get detail
                List<SearchedCertsViewModel> searchedCerts = new List<SearchedCertsViewModel>();
                string sqlString = @"Select a.*, b.GiaTri, d.X, d.Y, d.Color, d.Font, d.Italic, d.Bold, d.Underline, d.Size, d.TruongDuLieuCode, d.TenTruongDuLieu as 'TenTruongDuLieuHocSinh',
											i.Ten as 'TenTruongDuLieu', i.KieuDuLieu, 
		                                   g.Ten as 'LoaiBang', g.Width, g.Height, h.[Url] as 'AnhLoaiBang'
                                    From Bang as a
                                    Left Join ThongTinVanBang as b
                                    on a.Id = b.BangId
                                    Left Join TrangThaiBang as c
                                    on a.TrangThaiBangId = c.Id
                                    Left Join TruongDuLieuLoaiBang as d
                                    on a.LoaiBangId = d.LoaiBangId and b.TruongDuLieuCode = d.TruongDuLieuCode
                                    Left Join LoaiBang as g
                                    on a.LoaiBangId = g.Id
                                    Left Join AnhLoaiBang as h
                                    on a.LoaiBangId = h.ObjectId
									Left Join TruongDuLieu as i
									on d.TruongDuLieuCode = i.Code
                                    Where (a.Id = @BangId) and (h.IsDeleted = 0)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        //command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));
                        command.Parameters.Add(new SqlParameter("@BangId", bangId));

                        using (var reader = command.ExecuteReader())
                        {
                            var items = MapDataHelper<SearchedCertsViewModel>.MapList(reader);
                            foreach (SearchedCertsViewModel item in items)
                            {
                                if (searchedCerts.Any(x => x.Id == item.Id))
                                {
                                    SearchedCertsViewModel searchedCertsViewModel = searchedCerts.Where(x => x.Id == item.Id).FirstOrDefault();
                                    if (searchedCertsViewModel.TruongDuLieuTrongBangs.Any(x => x.TruongDuLieuCode == item.TruongDuLieuCode))
                                    {
                                        continue;
                                    }
                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    searchedCertsViewModel.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);
                                }
                                else
                                {
                                    SearchedCertsViewModel searchedCert = new SearchedCertsViewModel();
                                    searchedCert.Id = item.Id;
                                    searchedCert.LoaiBangId = item.LoaiBangId;
                                    searchedCert.PhoiId = item.PhoiId;
                                    searchedCert.TruongHocId = item.TruongHocId;
                                    searchedCert.HocSinhId = item.HocSinhId;
                                    searchedCert.TruongHoc = item.TruongHoc;
                                    searchedCert.LoaiBang = item.LoaiBang;
                                    searchedCert.SoHieu = item.SoHieu;
                                    searchedCert.NgayInBang = item.NgayInBang;
                                    searchedCert.NgayPhatBang = item.NgayPhatBang;
                                    searchedCert.SoVaoSo = item.SoVaoSo;
                                    searchedCert.TrangThaiBangId = item.TrangThaiBangId;
                                    searchedCert.HoVaTen = item.HoVaTen;
                                    searchedCert.DuongDanFileAnh = item.DuongDanFileAnh;
                                    searchedCert.CmtnguoiLayBang = item.CmtnguoiLayBang;
                                    searchedCert.SoDienThoaiNguoiLayBang = item.SoDienThoaiNguoiLayBang;
                                    searchedCert.HinhThucNhan = item.HinhThucNhan;
                                    searchedCert.Width = item.Width;
                                    searchedCert.Height = item.Height;
                                    searchedCert.AnhLoaiBang = item.AnhLoaiBang;
                                    searchedCert.TruongDuLieuTrongBangs = new List<TruongDuLieuTrongBangViewModel>();

                                    TruongDuLieuTrongBangViewModel truongDuLieuTrongBang = new TruongDuLieuTrongBangViewModel();
                                    truongDuLieuTrongBang.X = item.X;
                                    truongDuLieuTrongBang.Y = item.Y;
                                    truongDuLieuTrongBang.GiaTri = item.GiaTri;
                                    truongDuLieuTrongBang.Color = item.Color;
                                    truongDuLieuTrongBang.Font = item.Font;
                                    truongDuLieuTrongBang.Italic = item.Italic;
                                    truongDuLieuTrongBang.Bold = item.Bold;
                                    truongDuLieuTrongBang.Underline = item.Underline;
                                    truongDuLieuTrongBang.Size = item.Size;
                                    truongDuLieuTrongBang.TenTruongDuLieu = item.TenTruongDuLieu;
                                    truongDuLieuTrongBang.TenTruongDuLieuHocSinh = item.TenTruongDuLieuHocSinh;
                                    truongDuLieuTrongBang.KieuDuLieu = item.KieuDuLieu;
                                    truongDuLieuTrongBang.TruongDuLieuCode = item.TruongDuLieuCode;
                                    searchedCert.TruongDuLieuTrongBangs.Add(truongDuLieuTrongBang);

                                    searchedCerts.Add(searchedCert);
                                }
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                if (searchedCerts.FirstOrDefault() != null && searchedCerts.FirstOrDefault().HocSinhId.HasValue && searchedCerts.FirstOrDefault().HocSinhId != -1)
                {
                    // get thong tin hoc sinh
                    string sqlString_1 = @"Select * From [dbo].[HocSinh] as a
                                    Where ([Id] = @Id) and ([IsDeleted] = 0)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    sqlParameters.Add(new SqlParameter("@Id", searchedCerts.FirstOrDefault().HocSinhId));
                    HocSinhViewModel hocSinhViewModel = new HocSinhViewModel();
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddRange(sqlParameters.ToArray());
                            using (var reader = command.ExecuteReader())
                            {
                                searchedCerts.FirstOrDefault().HocSinh = MapDataHelper<HocSinhViewModel>.Map(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get diem mon hoc
                    string sqlString_3 = @"Select a.*, b.TenMonHoc From [DiemMonHocs] as a 
                                            Left Join MonHocs as b
                                            on a.CodeMonHoc = b.CodeMonHoc
                                            Left Join CapDonVi as c
                                            on b.CodeCapDonVi = c.Code
                                            Left Join DonVi as d
                                            on c.CapDonViId = d.CapDonViId 
                                            Where (a.HocSinhId = @HocSinhId) and (d.DonViId = @TruongHocId)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();

                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@HocSinhId", searchedCerts.FirstOrDefault().HocSinh.Id));
                            command.Parameters.Add(new SqlParameter("@TruongHocId", searchedCerts.FirstOrDefault().TruongHocId));
                            using (var reader = command.ExecuteReader())
                            {
                                searchedCerts.FirstOrDefault().HocSinh.DiemMonHocs = MapDataHelper<DiemMonHocViewModel>.MapList(reader);
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get log
                    if (searchedCerts.FirstOrDefault() != null)
                    {
                        string sqlString_2 = @"Select a.* From LogVanBang as a
                                        Left Join Bang as b
                                        on a.VanBangId = b.Id
                                        Where (b.SoVaoSo Like @Keyword) and (b.IsDeleted = 0)
                                        Order By ThoiGian DESC";
                        using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                        {
                            bool wasOpen = command.Connection.State == ConnectionState.Open;
                            if (!wasOpen) command.Connection.Open();

                            try
                            {
                                command.CommandText = sqlString_2;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new SqlParameter("@Keyword", searchedCerts.FirstOrDefault().SoVaoSo));

                                using (var reader = command.ExecuteReader())
                                {
                                    searchedCerts.FirstOrDefault().LogVanBangs = MapDataHelper<LogVanBangViewModel>.MapList(reader);
                                }
                            }
                            finally
                            {
                                command.Connection.Close();
                            }
                        }
                    }
                   
                }

                return searchedCerts == null ? null : searchedCerts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DonViViewModel> GetDanhSachTruongHoc(string tenDonVi, int maximumReturn)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.* From DonVi as a
                                    left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    Where (a.TenDonVi Like N'%'+@TenDonVi+'%') and  (b.Code in ('TIEUHOC', 'THCS', 'THPT', 'TTGD'))
                                    Order by a.TenDonVi
                                    Offset 0 Rows
                                    Fetch Next @Next Rows only";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@TenDonVi", string.IsNullOrEmpty(tenDonVi) ? "" : tenDonVi.Trim()));
                        command.Parameters.Add(new SqlParameter("@Next", maximumReturn));
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
    }
}
