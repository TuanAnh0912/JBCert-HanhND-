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
    public class BaoCaoThongKeProvider : ApplicationDbContext, IBaoCaoThongKe
    {
        public SoLuongThongKeBangDaInTungNamViewModel GetSoLuongBangDaInTungNam(int donViId)
        {
            try
            {
                SoLuongThongKeBangDaInTungNamViewModel soLuongThongKeBangDaInTungNamViewModel = new SoLuongThongKeBangDaInTungNamViewModel();

                // tong so luong bang goc da in tung nam
                string sqlString = @"Select YEAR(NgayInBang) as 'Nam', Count(*) as 'SoLuong' From Bang 
                                        Where (DonViId = @DonViId) and ((HocSinhId != -1)) and (BangGocId is null)
			                                    and ( ((TrangThaiBangId >= 4) and (SoLanCaiChinh = 0)) or (SoLanCaiChinh > 0))
                                    Group by YEAR(NgayInBang)";
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
                            soLuongThongKeBangDaInTungNamViewModel.BangGocs = MapDataHelper<SoLuongThongKeTungNamViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong ban sao da in tung nam
                string sqlString_1 = @"Select YEAR(NgayInBang) as 'Nam', Count(*) as 'SoLuong' From Bang 
                                            Where (TrangThaiBangId >= 4) and (DonViId = @DonViId)
                                            and (BangGocId is not null)
                                        Group by YEAR(NgayInBang)";
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
                            soLuongThongKeBangDaInTungNamViewModel.BanSaos = MapDataHelper<SoLuongThongKeTungNamViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong cai chinh da in tung nam
                string sqlString_2 = @"Select a.Nam, ISNULL(a.SoLuong, 0) - ISNULL(b.SoLuong, 0) as 'SoLuong' From (Select YEAR(NgayInBang) as 'Nam', Sum(SoLanCaiChinh) as 'SoLuong' From Bang 
                                            Where (DonViId = @DonViId) and (BangGocId is null)
			                                        and (SoLanCaiChinh > 0)
                                        Group by YEAR(NgayInBang)) as a
                                        Left Join (Select YEAR(NgayInBang) as 'Nam', Count(*) as 'SoLuong' From Bang 
                                            Where (DonViId = @DonViId) and (BangGocId is null)
			                                        and ((TrangThaiBangId < 4) and (SoLanCaiChinh > 0))
		                                        Group by YEAR(NgayInBang)) as b
                                        on a.Nam = b.Nam";
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
                            soLuongThongKeBangDaInTungNamViewModel.CaiChinhs = MapDataHelper<SoLuongThongKeTungNamViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // get max / min
                string sqlString_3 = @"Select ISNULL(YEAR(Max(NgayInBang)), 0) as 'Nam' From Bang 
                                        Where (TrangThaiBangId >= 4) or (SoLanCaiChinh > 0) and (DonViId = @DonViId)";
                int min = 0;
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
                            if (reader.Read())
                            {
                                min = Convert.ToInt32(reader["Nam"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong bang goc da in
                string sqlString_4 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where  (DonViId = @DonViId) and ((HocSinhId != -1))  and (BangGocId is null)
                                        and ( ((TrangThaiBangId >= 4) and (SoLanCaiChinh = 0)) or (SoLanCaiChinh > 0))";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                soLuongThongKeBangDaInTungNamViewModel.TongSoBangGocDaIn = Convert.ToInt32(reader["SoLuong"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong bang goc da in
                string sqlString_5 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId >= 4) and (DonViId = @DonViId)
                                        and (BangGocId is not null)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_5;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                soLuongThongKeBangDaInTungNamViewModel.TongSoBanSaoDaIn = Convert.ToInt32(reader["SoLuong"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong cai chinh
                string sqlString_6 = @"Select ISNULL(a.SoLuong, 0) - ISNULL(b.SoLuong, 0) as 'SoLuong' 
                                        From (Select Sum(SoLanCaiChinh) as 'SoLuong' From Bang 
                                                Where (DonViId = @DonViId) and (BangGocId is null)
			                                            and (SoLanCaiChinh > 0)) as a
										,( Select Count(*) as 'SoLuong' From Bang 
                                        Where (DonViId = @DonViId) and (BangGocId is null)
			                            and ((TrangThaiBangId < 4) and (SoLanCaiChinh > 0)) ) as b";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_6;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                soLuongThongKeBangDaInTungNamViewModel.TongSoLanCaiChinh = Convert.ToInt32(reader["SoLuong"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // tong so luong so hoa
                string sqlString_7 = @"Select Count(*) as 'SoLuong' From Bang
                                        Where (HocSinhId = -1) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_7;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                soLuongThongKeBangDaInTungNamViewModel.TongSoSoHoa = Convert.ToInt32(reader["SoLuong"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                soLuongThongKeBangDaInTungNamViewModel.Min = min == 0 ? 2020 : min;
                soLuongThongKeBangDaInTungNamViewModel.Max = DateTime.Now.Year;

                return soLuongThongKeBangDaInTungNamViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ThongKeChungTheoDonViViewModel ThongKeChungTheoDonVi(int donViId)
        {
            try
            {
                ThongKeChungTheoDonViViewModel thongKeChungTheoDonViViewModel = new ThongKeChungTheoDonViViewModel();
                thongKeChungTheoDonViViewModel.SoBangDaInTrongNam = 0;
                thongKeChungTheoDonViViewModel.TongSoHSTotNghiepTrongNam = 0;
                thongKeChungTheoDonViViewModel.SoPhoiDaNhanTrongNam = 0;
                thongKeChungTheoDonViViewModel.SoLanDinhChinhTrongNam = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps = new ThongKeYeuCauCongNhanTotNghiepTheoDonViViewModel();
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauBiTuChoi = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDangCho = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDuocCongNhan = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois = new ThongKeYeuCauCapPhoiTheoDonViViewModel();
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauBiTuChoi = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDangCho = 0;
                thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDuocCongNhan = 0;
                thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam = new List<SoLuongThongKeTungNamViewModel>();
                thongKeChungTheoDonViViewModel.Max = 0;
                thongKeChungTheoDonViViewModel.Min = 0;
                // get code cap don vi
                string codeCapDonVi = "";
                string sqlString = @"Select b.Code From DonVi as a
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
                                codeCapDonVi += Convert.ToString(reader["Code"]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // SOGD
                if (codeCapDonVi == "SOGD")
                {
                    // get So Bang Da In Trong Nam
                    string sqlString_1 = @"Select ISNULL(SoLanCaiChinh, 0) as 'SoLanCaiChinh', Count(*) as 'SoLuong', TrangThaiBangId,
                                    Case
	                                    When TrangThaiBangId < 4 Then ISNULL(SoLanCaiChinh, 0) * Count(*)
	                                    When TrangThaiBangId >= 4 Then (ISNULL(SoLanCaiChinh, 0) + 1) * Count(*)
                                    End as 'TongSo'
                                    From Bang Where DonViId in (Select a.DonViId From DonVi as a
									                                    left Join CapDonVi as b
									                                    on a.CapDonViId = b.CapDonViId
									                                    Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											                                    and (b.[Level] > 1)) and not ((ISNULL(SoLanCaiChinh, 0) = 0) and TrangThaiBangId < 4)
                                    Group By ISNULL(SoLanCaiChinh, 0), TrangThaiBangId";
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
                                    thongKeChungTheoDonViViewModel.SoBangDaInTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get Tong So HS Tot Nghiep Trong Nam
                    string sqlString_2 = @"Select Count(*) as 'TongSo' From HocSinh 
                                        Where (CongNhanTotNghiep = 1) and (NamTotNghiep = @NamTotNghiep)
	                                    and TruongHocId in (Select a.DonViId From DonVi as a
									                                    left Join CapDonVi as b
									                                    on a.CapDonViId = b.CapDonViId
									                                    Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											                                    and (b.[Level] > 1)) and (IsDeleted = 0)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NamTotNghiep", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.TongSoHSTotNghiepTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get So Phoi Da Nhan Trong Nam
                    string sqlString_3 = @"Select Count(*) as 'TongSo' From Phoi Where DonViId in (Select a.DonViId From DonVi as a
									    left Join CapDonVi as b
									    on a.CapDonViId = b.CapDonViId
									    Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											    and (b.[Level] > 1)) and (IsDeleted = 0) and (Year(NgayTao) = @NgayTao)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NgayTao", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.SoPhoiDaNhanTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get So Lan Dinh Chinh Trong Nam
                    string sqlString_4 = @"Select Count(*) as 'TongSo' From CaiChinhs Where DonViThucHien in (Select a.DonViId From DonVi as a
										left Join CapDonVi as b
										on a.CapDonViId = b.CapDonViId
										Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = 46), '%') )
												and (b.[Level] > 1)) and (Year(ThoiGianThucHien) = @ThoiGianThucHien)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_4;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.SoLanDinhChinhTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // thong ke yeu cau cong nhan tot nghiep
                    string sqlString_5 = @"Select MaTrangThaiYeuCau, Count(*) as 'TongSo' From YeuCau 
                                            Where DonViId in (Select a.DonViId From DonVi as a
									                                            left Join CapDonVi as b
									                                            on a.CapDonViId = b.CapDonViId
									                                            Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											                                            and (b.[Level] > 1))
	                                            and (LoaiYeuCauId = 6) and (MaTrangThaiYeuCau in ('approved', 'rejected', 'waiting'))
                                            Group By MaTrangThaiYeuCau";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    switch (Convert.ToString(reader["MaTrangThaiYeuCau"]).ToLower())
                                    {
                                        case "approved":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDuocCongNhan
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "rejected":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauBiTuChoi
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "waiting":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDangCho
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // thong ke yeu cau cap phoi
                    string sqlString_6 = @"Select MaTrangThaiYeuCau, Count(*) as 'TongSo' From YeuCau 
                                            Where DonViId in (Select a.DonViId From DonVi as a
									                                            left Join CapDonVi as b
									                                            on a.CapDonViId = b.CapDonViId
									                                            Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											                                            and (b.[Level] > 1))
	                                            and ((LoaiYeuCauId = 2) or (LoaiYeuCauId = 7)) and (MaTrangThaiYeuCau in ('approved', 'rejected', 'waiting'))
                                            Group By MaTrangThaiYeuCau";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_6;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    switch (Convert.ToString(reader["MaTrangThaiYeuCau"]).ToLower())
                                    {
                                        case "approved":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDuocCongNhan
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "rejected":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauBiTuChoi
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "waiting":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDangCho
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get so luong hoc sinh tot nghiep qua tung nam
                    string sqlString_7 = @"Select NamTotNghiep, Count(NamTotNghiep) as 'TongSo' From HocSinh 
                                            Where (CongNhanTotNghiep = 1) and (NamTotNghiep = @NamTotNghiep)
	                                            and TruongHocId in (Select a.DonViId From DonVi as a
									                                            left Join CapDonVi as b
									                                            on a.CapDonViId = b.CapDonViId
									                                            Where (DiaGioiHanhChinh Like CONCAT((Select DiaGioiHanhChinh From DonVi Where DonViId = @DonViId), '%') )
											                                            and (b.[Level] > 1)) and (IsDeleted = 0)
                                            Group By NamTotNghiep";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_7;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NamTotNghiep", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Add(new SoLuongThongKeTungNamViewModel()
                                    {
                                        Nam = Convert.ToInt32(reader["NamTotNghiep"]),
                                        SoLuong = Convert.ToInt32(reader["TongSo"])
                                    });
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    thongKeChungTheoDonViViewModel.IsTruong = false;
                }
                else if (codeCapDonVi == "PHONGGD")
                {
                    // get So Bang Da In Trong Nam
                    string sqlString_1 = @"Select ISNULL(SoLanCaiChinh, 0) as 'SoLanCaiChinh', Count(*) as 'SoLuong', TrangThaiBangId,
                                    Case
	                                    When TrangThaiBangId < 4 Then ISNULL(SoLanCaiChinh, 0) * Count(*)
	                                    When TrangThaiBangId >= 4 Then (ISNULL(SoLanCaiChinh, 0) + 1) * Count(*)
                                    End as 'TongSo'
                                    From Bang 
                                    Where DonViId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId)) 
                                    and not ((ISNULL(SoLanCaiChinh, 0) = 0) and TrangThaiBangId < 4)
                                    Group By ISNULL(SoLanCaiChinh, 0), TrangThaiBangId";
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
                                    thongKeChungTheoDonViViewModel.SoBangDaInTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get Tong So HS Tot Nghiep Trong Nam
                    string sqlString_2 = @"Select Count(*) as 'TongSo' From HocSinh 
                                        Where (CongNhanTotNghiep = 1) and (NamTotNghiep = @NamTotNghiep)
	                                    and TruongHocId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId)) 
                                        and (IsDeleted = 0)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NamTotNghiep", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.TongSoHSTotNghiepTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get So Phoi Da Nhan Trong Nam
                    string sqlString_3 = @"Select Count(*) as 'TongSo' From Phoi 
                                        Where DonViId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId)) 
                                        and (IsDeleted = 0) and (Year(NgayTao) = @NgayTao)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_3;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NgayTao", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.SoPhoiDaNhanTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get So Lan Dinh Chinh Trong Nam
                    string sqlString_4 = @"Select Count(*) as 'TongSo' From CaiChinhs 
                                            Where DonViThucHien in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId)) 
                                            and (Year(ThoiGianThucHien) = @ThoiGianThucHien)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_4;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.SoLanDinhChinhTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // thong ke yeu cau cong nhan tot nghiep
                    string sqlString_5 = @"Select MaTrangThaiYeuCau, Count(*) as 'TongSo' From YeuCau 
                                            Where DonViId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId))
	                                            and (LoaiYeuCauId = 6) and (MaTrangThaiYeuCau in ('approved', 'rejected', 'waiting'))
                                            Group By MaTrangThaiYeuCau";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_5;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    switch (Convert.ToString(reader["MaTrangThaiYeuCau"]).ToLower())
                                    {
                                        case "approved":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDuocCongNhan
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "rejected":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauBiTuChoi
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "waiting":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCongCongNhanTotNghieps.SoLuongYeuCauDangCho
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // thong ke yeu cau cap phoi
                    string sqlString_6 = @"Select MaTrangThaiYeuCau, Count(*) as 'TongSo' From YeuCau 
                                            Where DonViId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId))
	                                            and ((LoaiYeuCauId = 2) or (LoaiYeuCauId = 7)) and (MaTrangThaiYeuCau in ('approved', 'rejected', 'waiting'))
                                            Group By MaTrangThaiYeuCau";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_6;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@ThoiGianThucHien", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    switch (Convert.ToString(reader["MaTrangThaiYeuCau"]).ToLower())
                                    {
                                        case "approved":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDuocCongNhan
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "rejected":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauBiTuChoi
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                        case "waiting":
                                            thongKeChungTheoDonViViewModel.ThongKeYeuCauCapPhois.SoLuongYeuCauDangCho
                                                = Convert.ToInt32(reader["TongSo"]);
                                            break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get so luong hoc sinh tot nghiep qua tung nam
                    string sqlString_7 = @"Select NamTotNghiep, Count(NamTotNghiep) as 'TongSo' From HocSinh 
                                            Where (TruongHocId in (Select DonViId From DonVi Where (KhoaChaId = @DonViId) or (DonViId = @DonViId)) )
                                            and (IsDeleted = 0)
                                            Group By NamTotNghiep";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_7;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Add(new SoLuongThongKeTungNamViewModel()
                                    {
                                        Nam = Convert.ToInt32(reader["NamTotNghiep"]),
                                        SoLuong = Convert.ToInt32(reader["TongSo"])
                                    });
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    thongKeChungTheoDonViViewModel.IsTruong = false;
                }
                else if (codeCapDonVi == "THPT" || codeCapDonVi == "THCS" || codeCapDonVi == "TTGD")
                {
                    // get So Bang Da In Trong Nam
                    string sqlString_1 = @"Select ISNULL(SoLanCaiChinh, 0) as 'SoLanCaiChinh', Count(*) as 'SoLuong', TrangThaiBangId,
                                    Case
	                                    When TrangThaiBangId < 4 Then ISNULL(SoLanCaiChinh, 0) * Count(*)
	                                    When TrangThaiBangId >= 4 Then (ISNULL(SoLanCaiChinh, 0) + 1) * Count(*)
                                    End as 'TongSo'
                                    From Bang 
                                    Where TruongHocId = @DonViId
                                    and not ((ISNULL(SoLanCaiChinh, 0) = 0) and TrangThaiBangId < 4)
                                    Group By ISNULL(SoLanCaiChinh, 0), TrangThaiBangId";
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
                                    thongKeChungTheoDonViViewModel.SoBangDaInTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get Tong So HS Tot Nghiep Trong Nam
                    string sqlString_2 = @"Select Count(*) as 'TongSo' From HocSinh 
                                        Where (CongNhanTotNghiep = 1) and (NamTotNghiep = @NamTotNghiep)
	                                    and TruongHocId = @DonViId 
                                        and (IsDeleted = 0)";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_2;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            command.Parameters.Add(new SqlParameter("@NamTotNghiep", DateTime.Now.Year));
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.TongSoHSTotNghiepTrongNam += Convert.ToInt32(reader["TongSo"]);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    // get so luong hoc sinh tot nghiep qua tung nam
                    string sqlString_7 = @"Select NamTotNghiep, Count(NamTotNghiep) as 'TongSo' From HocSinh 
                                            Where (TruongHocId = @DonViId)
                                            and (IsDeleted = 0)
                                            Group By NamTotNghiep";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_7;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Add(new SoLuongThongKeTungNamViewModel()
                                    {
                                        Nam = Convert.ToInt32(reader["NamTotNghiep"]),
                                        SoLuong = Convert.ToInt32(reader["TongSo"])
                                    });
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }

                    thongKeChungTheoDonViViewModel.IsTruong = true;
                }

                if (thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Count > 0)
                {
                    thongKeChungTheoDonViViewModel.Max = thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Select(x => x.Nam).Max();
                    thongKeChungTheoDonViViewModel.Min = thongKeChungTheoDonViViewModel.HocSinhTotNghiepQuaTungNam.Select(x => x.Nam).Min();
                }
                return thongKeChungTheoDonViViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ThongKeVanBangChungChiTheoNamViewModel ThongKeVanBangChungChiTheoNam(int? nam, int donViId)
        {
            try
            {
                ThongKeVanBangChungChiTheoNamViewModel thongKeVanBangChungChiTheoNamViewModel = new ThongKeVanBangChungChiTheoNamViewModel();
                //Số lượng bằng gốc đã in trong năm
                string sqlString = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId >= 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value ));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongVanBangDaInTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng chứng chi đã in trong năm
                string sqlString_1 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId >= 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 1) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongChungChiDaInTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng bản sao văn bằng đã in trong năm
                string sqlString_2 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId >= 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is not null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongBanSaoVanBangDaInTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng văn bằng đã cải chính trong năm
                string sqlString_3 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (SoLanCaiChinh is not null) and (SoLanCaiChinh > 0) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_3;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongVanBangDaCaiChinhTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng chứng chỉ đã cải chính trong năm
                string sqlString_4 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (SoLanCaiChinh is not null) and (SoLanCaiChinh > 0) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 1) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_4;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongChungChiCaiChinhTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng văn bằng đã phát trong năm
                string sqlString_5 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId = 6) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_5;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongVanBangDaCaiChinhTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng chứng chỉ đã phát trong năm
                string sqlString_6 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId = 6) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 1) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_6;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongChungChiCaiChinhTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng văn bằng chưa phát trong năm
                string sqlString_7 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId = 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_7;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongVanBangChuaPhatTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng chứng chỉ chưa phát trong năm
                string sqlString_8 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId = 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 1) and (BangGocId is null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_8;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongChungChiChuaPhatTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                //Số lượng chứng chỉ chưa phát trong năm
                string sqlString_9 = @"Select Count(*) as 'SoLuong' From Bang 
                                        Where (TrangThaiBangId = 4) and ((@NamTotNghiep is null) or (NamTotNghiep = @NamTotNghiep)) 
                                                and (IsChungChi = 0) and (BangGocId is not null) and (DonViId = @DonViId)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_9;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViId", donViId));
                        command.Parameters.Add(new SqlParameter("@NamTotNghiep", nam.HasValue ? nam.Value : DBNull.Value));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thongKeVanBangChungChiTheoNamViewModel.SoLuongBanSaoVanBangChuaPhatTrongNam = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return thongKeVanBangChungChiTheoNamViewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
