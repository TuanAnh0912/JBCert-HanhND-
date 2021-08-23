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

namespace jbcert.DATA.Provider
{
    public class DonViProvider : ApplicationDbContext
    {
        public ListReturnViewModel GetAll(string keyword, string DiaGioiHC)
        {
            var result = new ListReturnViewModel();
            try
            {
                if (DiaGioiHC.Length == 4)
                {
                    DiaGioiHC = DiaGioiHC.Substring(0, 2);
                }
                else if (DiaGioiHC.Length == 7)
                {
                    DiaGioiHC = DiaGioiHC.Substring(0, 4);
                }
                string sqlString = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa', e.TenCapDonVi, f.TenDonVi as 'TenDonViCha'
									from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
									Left Join CapDonVi as e
									on a.CapDonViId = e.CapDonViId
									Left Join DonVi as f
									on a.KhoaChaId = f.DonViId
                                    where (a.DiaGioiHanhChinh Like @DiaGioiHanhChinh+'%') and ((@Keyword is null) or (a.TenDonVi like N'%'+@Keyword+'%'))";

                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DiaGioiHanhChinh", DiaGioiHC));
                        command.Parameters.Add(new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword.Trim()));
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

                result.Data = donViViewModels;
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public DonViViewModel GetDonViSo(int donViId)
        {
            try
            {
                return GetDonViSoByDonViId(donViId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DonViViewModel GetDonViSoByDonViId(int donViId)
        {
            try
            {
                DonViViewModel donViViewModel = new DonViViewModel();
                string sqlString = @"Select a.*, b.Level From DonVi as a
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
                            if (donViViewModel.KhoaChaId.HasValue && donViViewModel.Level > 2)
                            {
                                donViViewModel = GetDonViSoByDonViId(donViViewModel.KhoaChaId.Value);
                            }
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
        public List<DonViViewModel> GetTruongHocsByDonViCha(int donViChaId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa',
		                                    f.Ten as 'LoaiBang', f.Id as 'LoaiBangId'
                                    from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
                                    Left Join CapDonVi as e
                                    on a.CapDonViId = e.CapDonViId
                                    Left Join LoaiBang as f
                                    on e.Code = f.CodeCapDonVi
                                    where (a.KhoaChaId = @DonViChaId) and (f.IsDeleted = 0) and (f.LoaiBangGocId is null)";
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
                            foreach (var item in result)
                            {
                                if (donViViewModels.Any(x => x.DonViId == item.DonViId))
                                {
                                    DonViViewModel donViViewModel = donViViewModels.Where(x => x.DonViId == item.DonViId).FirstOrDefault();
                                    donViViewModel.LoaiBangs.Add(new LoaiBangViewModel()
                                    {
                                        Id = item.LoaiBangId,
                                        Ten = item.LoaiBang
                                    });
                                }
                                else
                                {
                                    DonViViewModel donViViewModel = new DonViViewModel();
                                    donViViewModel.DonViId = item.DonViId;
                                    donViViewModel.TenDonVi = item.TenDonVi;
                                    donViViewModel.KhoaChaId = item.KhoaChaId;
                                    donViViewModel.CapDonViId = item.CapDonViId;
                                    donViViewModel.MaDonVi = item.MaDonVi;
                                    donViViewModel.DiaGioiHanhChinh = item.DiaGioiHanhChinh;
                                    donViViewModel.TinhId = item.TinhId;
                                    donViViewModel.Tinh = item.Tinh;
                                    donViViewModel.HuyenId = item.HuyenId;
                                    donViViewModel.Huyen = item.Huyen;
                                    donViViewModel.XaId = item.XaId;
                                    donViViewModel.Xa = item.Xa;
                                    donViViewModel.LoaiBangs = new List<LoaiBangViewModel>();
                                    donViViewModel.LoaiBangs.Add(new LoaiBangViewModel()
                                    {
                                        Id = item.LoaiBangId,
                                        Ten = item.LoaiBang
                                    });
                                    donViViewModels.Add(donViViewModel);
                                }
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

        public List<DonViViewModel> GetTruongHocsByDonViChaVaKieuBang(int donViChaId, bool isChungChi)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.DonViId, a.TenDonVi
                                    from DonVi as a
                                    Left Join CapDonVi as b
                                    on a.CapDonViId = b.CapDonViId
                                    Left Join LoaiBang as c
                                    on b.Code = c.CodeCapDonVi
                                    where (a.KhoaChaId = @DonViId) and (c.IsDeleted = 0) and (c.LoaiBangGocId is null) and (c.IsChungChi = @IsChungChi)
                                    Group by a.DonViId, a.TenDonVi";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViChaId", donViChaId));
                        command.Parameters.Add(new SqlParameter("@IsChungChi", isChungChi));
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
        private List<DonViViewModel> FindDonViCon(List<DonViViewModel> donViViewModels)
        {
            try
            {
                List<DonViViewModel> data = new List<DonViViewModel>();

                foreach (DonViViewModel donViViewModel in donViViewModels)
                {
                    string sqlString_1 = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa', e.TenCapDonVi, f.TenDonVi as 'TenDonViCha'
									from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
									Left Join CapDonVi as e
									on a.CapDonViId = e.CapDonViId
									Left Join DonVi as f
									on a.KhoaChaId = f.DonViId
                                    where a.KhoaChaid = @ChaId";
                    using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                    {
                        bool wasOpen = command.Connection.State == ConnectionState.Open;
                        if (!wasOpen) command.Connection.Open();
                        try
                        {
                            command.CommandText = sqlString_1;
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SqlParameter("@ChaId", donViViewModel.DonViId));
                            using (var reader = command.ExecuteReader())
                            {
                                var result = MapDataHelper<DonViViewModel>.MapList(reader);
                                if (result != null)
                                {
                                    data.AddRange(result);
                                }
                            }
                        }
                        finally
                        {
                            command.Connection.Close();
                        }
                    }
                    if (data.Count() > 0)
                    {
                        FindDonViCon(data);
                    }

                }
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DonViViewModel GetDonViById(int donViId)
        {
            try
            {
                DonViViewModel donViViewModel = new DonViViewModel();
                string sqlString = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa' from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
                                    where a.DonViId = @DonViId";

                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
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
        public ListReturnViewModel GetByNguoiDung(NguoiDung model)
        {
            var result = new ListReturnViewModel();
            try
            {
                int donViId = model.DonViId.Value;
                string sqlString = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa', e.TenCapDonVi, f.TenDonVi as 'TenDonViCha'
									from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
									Left Join CapDonVi as e
									on a.CapDonViId = e.CapDonViId
									Left Join DonVi as f
									on a.KhoaChaId = f.DonViId
                                    where (a.DonViId = @DonViId)";

                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
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

                List<DonViViewModel> donViCons = FindDonViCon(donViViewModels);
                if (donViCons != null)
                {
                    donViViewModels.AddRange(donViCons);
                }

                result.Data = donViViewModels;
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public ListReturnViewModel GetDonViByNguoiDung(NguoiDung model)
        {
            var result = new ListReturnViewModel();
            try
            {
                string sqlString = "select * from DonVi where DonViId = " + model.PhongBan.DonViId;
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
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
                            donViViewModels = MapDataHelper<DonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                result.Data = donViViewModels;
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public ResultModel Insert(DonViViewModel model)
        {
            using var transaction = DbContext.Database.BeginTransaction();
            try
            {
                var obj = new DonVi();
                obj.TenDonVi = model.TenDonVi;
                obj.KhoaChaId = model.KhoaChaId;
                obj.MaDonVi = model.MaDonVi;
                obj.CapDonViId = model.CapDonViId;
                obj.DiaGioiHanhChinh = model.DiaGioiHanhChinh;
                DbContext.DonVis.Add(obj);
                DbContext.SaveChanges();

                PhongBan phongBan = new PhongBan();
                phongBan.TenPhongBan = "Phòng ban quản trị hệ thống";
                phongBan.DonViId = obj.DonViId;
                phongBan.IsDelete = false;
                DbContext.PhongBans.Add(phongBan);
                DbContext.SaveChanges();

                NhomNguoiDung nhomNguoiDung = new NhomNguoiDung();
                nhomNguoiDung.TenNhomNguoiDung = "Quản trị hệ thống";
                nhomNguoiDung.PhongBanId = phongBan.PhongBanId;
                DbContext.NhomNguoiDungs.Add(nhomNguoiDung);
                DbContext.SaveChanges();

                var res = new ChucNangProvider().GetByCapDonDonVi(obj.CapDonViId.Value);
                var lstlk = new List<LienKetNhomNguoiDungChucNang>();
                foreach (var item in res)
                {
                    var o = new LienKetNhomNguoiDungChucNang();
                    o.ChucNangid = item.ChucNangId;
                    o.NhomNguoiDungId = nhomNguoiDung.NhomNguoiDungId;
                    lstlk.Add(o);
                }
                DbContext.LienKetNhomNguoiDungChucNangs.AddRange(lstlk);
                DbContext.SaveChanges();

                transaction.Commit();
                return new ResultModel(true, "Thêm mới đơn vị thành công");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResultModel(false, e.Message);
            }
        }
        public DonVi GetById(int DonViId)
        {
            try
            {
                return DbContext.DonVis.FirstOrDefault(d => d.DonViId == DonViId);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public ResultModel Update(DonViViewModel model)
        {
            try
            {
                var obj = GetById(model.DonViId);
                obj.TenDonVi = model.TenDonVi;
                obj.KhoaChaId = model.KhoaChaId;
                obj.CapDonViId = model.CapDonViId;
                obj.DiaGioiHanhChinh = model.DiaGioiHanhChinh;
                DbContext.SaveChanges();
                return new ResultModel(true, "Cập nhật đơn vị thành công");
            }
            catch (Exception e)
            {
                return new ResultModel(false, e.Message);
            }
        }
        public ListReturnViewModel GetAllCapDonVi()
        {
            var result = new ListReturnViewModel();
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

                result.Data = capDonViViewModels;
                result.Message = "Get data success!";
                result.Status = true;
                return result;
            }
            catch (Exception e)
            {
                result.Data = null;
                result.Status = false;
                result.Message = e.Message;
                return result;
            }
        }
        public List<int> GetDonViIdByChaId(int chaId)
        {
            try
            {
                List<int> donViIds = new List<int>();
                string sqlString = @"Select DonViId From [DonVi] Where KhoaChaId = @ChaId Group By DonViId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();

                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@ChaId", chaId));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                donViIds.Add(Convert.ToInt32(reader["DonViId"]));
                            }
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                return donViIds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DonViViewModel> GetPhongGiaoDucsBySo(int donViChaId)
        {
            try
            {
                List<DonViViewModel> donViViewModels = new List<DonViViewModel>();
                string sqlString = @"Select a.*, b.Id as 'TinhId', b.Ten as 'Tinh', c.Id as 'HuyenId', c.Ten as 'Huyen', d.Id as 'XaId', d.Ten as 'Xa' from DonVi as a
                                    Left Join Tinh as b
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,2) = b.Id
                                    Left Join Huyen as c
                                    on SUBSTRING(a.DiaGioiHanhChinh,1,4) = c.Id
                                    Left Join Huyen as d
                                    on a.DiaGioiHanhChinh = d.Id
                                    Left Join CapDonVi as e
                                    on a.CapDonViId = e.CapDonViId
                                    where (a.KhoaChaId = @DonViChaId) and (e.Code Like @CodeCapDonVi)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@DonViChaId", donViChaId));
                        command.Parameters.Add(new SqlParameter("@CodeCapDonVi", "PHONGGD"));
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
        public List<CapDonViViewModel> GetCapDonVis(int Level)
        {
            try
            {
                string sqlString = @"select * from CapDonVi where Level > " + Level;
                var lst = new List<CapDonViViewModel>();
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
                            lst = MapDataHelper<CapDonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        if (!wasOpen) command.Connection.Close();
                    }
                }
                return lst;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<DonViViewModel> GetDonVis(int DonViId)
        {
            try
            {
                var model = DbContext.DonVis.Include("CapDonVi").FirstOrDefault(d => d.DonViId == DonViId);

                var donVi = new DonViViewModel();
                donVi.DonViId = model.DonViId;
                donVi.TenDonVi = model.TenDonVi;
                donVi.KhoaChaId = model.KhoaChaId;
                donVi.CapDonViId = model.CapDonViId;
                donVi.MaDonVi = model.MaDonVi;
                donVi.Code = model.CapDonVi.Code;
                donVi.DiaGioiHanhChinh = model.DiaGioiHanhChinh;
                var sqlString = @"select d.*,c.Code, c.CapDonViId from DonVi as d 
                            inner join CapDonVi as c on d.CapDonViId = c.CapDonViId 
                            where (c.Level > @Level) and (d.KhoaChaId = @DonViId)";
                var donVis = new List<DonViViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Level", model.CapDonVi.Level));
                        command.Parameters.Add(new SqlParameter("@DonViId", DonViId));
                        using (var reader = command.ExecuteReader())
                        {
                            donVis = MapDataHelper<DonViViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                donVis.Add(donVi);
                return donVis;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
