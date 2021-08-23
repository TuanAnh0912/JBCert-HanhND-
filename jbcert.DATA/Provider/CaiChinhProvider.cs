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
    public class CaiChinhProvider : ApplicationDbContext
    {
        public ResultModel CaiChinhVanBang(CaiChinhVanBangViewModel model)
        {
            // ko cần in bằng sau cải chính nên ko cần check
            // check bang goc da phat chua
            //string sqlString = @"Select * From Bang Where Id = @BangId";
            //BangViewModel bangViewModel = new BangViewModel();
            //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
            //{
            //    bool wasOpen = command.Connection.State == ConnectionState.Open;
            //    if (!wasOpen) command.Connection.Open();
            //    try
            //    {
            //        command.CommandText = sqlString;
            //        command.CommandType = CommandType.Text;
            //        command.Parameters.Add(new SqlParameter("@BangId", model.VanBang.Id));
            //        using (var reader = command.ExecuteReader())
            //        {
            //            bangViewModel = MapDataHelper<BangViewModel>.Map(reader);
            //        }
            //    }
            //    finally
            //    {
            //        command.Connection.Close();
            //    }
            //}
            //if (bangViewModel.TrangThaiBangId < 6)
            //{
            //    Exception exception = new Exception("Bằng phải được phát mới được cải chính");
            //    throw exception;
            //}


            using var transaction = DbContext.Database.BeginTransaction();
            try
            {
                /// Insert văn bằng cũ
                var bangCu = new BangCu();
                bangCu.LoaiBangId = model.VanBang.LoaiBangId;
                bangCu.PhoiId = model.VanBang.PhoiId;
                bangCu.TrangThaiBangId = model.VanBang.TrangThaiBangId;
                bangCu.HocSinhId = model.VanBang.HocSinhId;
                bangCu.TruongHocId = model.VanBang.TruongHocId;
                bangCu.SoVaoSo = model.VanBang.SoVaoSo;
                bangCu.TruongHoc = model.VanBang.TruongHoc;
                bangCu.SoHieu = model.VanBang.SoHieu;
                bangCu.HoVaTen = model.VanBang.HoVaTen;
                bangCu.DuongDanFileAnh = model.VanBang.DuongDanFileAnh;
                bangCu.DuongDanFileDeIn = model.VanBang.DuongDanFileDeIn;
                bangCu.CmtnguoiLayBang = model.VanBang.CmtnguoiLayBang;
                bangCu.SoDienThoaiNguoiLayBang = model.VanBang.SoDienThoaiNguoiLayBang;
                bangCu.QuanHeVoiNguoiDuocCapBang = model.VanBang.QuanHeVoiNguoiDuocCapBang;
                bangCu.HinhThucNhan = model.VanBang.HinhThucNhan;
                bangCu.DonViId = model.VanBang.DonViId;
                bangCu.NgayInBang = model.VanBang.NgayInBang;
                bangCu.NgayPhatBang = model.VanBang.NgayPhatBang;
                bangCu.NgayTao = model.VanBang.NgayTao;
                bangCu.NguoiTao = model.VanBang.NguoiTao;
                bangCu.NgayCapNhat = model.VanBang.NgayCapNhat;
                bangCu.NguoiCapNhat = model.VanBang.NguoiCapNhat;
                bangCu.NamTotNghiep = model.VanBang.NamTotNghiep;
                bangCu.IsDeleted = model.VanBang.IsDeleted;
                DbContext.BangCus.Add(bangCu);
                DbContext.SaveChanges();
                /// Insert thông tin văn bằng cũ
                var lstThongTinVB = DbContext.ThongTinVanBangs.Where(t => t.BangId == model.VanBang.Id);
                var lstThongTinCu = new List<ThongTinVanBangCu>();
                foreach(var item in lstThongTinVB)
                {
                    var o = new ThongTinVanBangCu();
                    o.BangId = bangCu.Id;
                    o.TruongDuLieuCode = item.TruongDuLieuCode;
                    o.GiaTri = item.GiaTri;
                    o.NgayTao = DateTime.Now;
                    o.NguoiTao = model.NguoiThucHien;
                    o.DonViId = item.DonViId;
                    lstThongTinCu.Add(o);
                }
                DbContext.ThongTinVanBangCus.AddRange(lstThongTinCu);
                DbContext.SaveChanges();
              
                /// Insert cải chính
                var caiChinh = new CaiChinh();
                var lst = DbContext.CaiChinhs.Where(c => c.BangId == model.VanBang.Id);
                
                caiChinh.LanCaiChinh = lst == null ? 0 : lst.Count() + 1;
                caiChinh.BangId = model.VanBang.Id;
                caiChinh.BangCuId = bangCu.Id;
                caiChinh.NguoiThucHien = model.NguoiThucHien;
                caiChinh.NoiThucHien = model.CaiChinh.NoiThucHien;
                caiChinh.DonViThucHien = model.DonViId;
                caiChinh.ThoiGianThucHien = DateTime.Now;
                caiChinh.LyDoCaiChinh = model.CaiChinh.LyDoCaiChinh;
                DbContext.CaiChinhs.Add(caiChinh);
                DbContext.SaveChanges();

                /// Insert thông tin cải chính
                var lstThongTinCaiChinh = new List<ThongTinCaiChinh>();
                foreach(var item in model.CaiChinh.ThongTinCaiChinhs) {
                    var obj = new ThongTinCaiChinh();
                    obj.CaiChinhId = caiChinh.CaiChinhId;
                    obj.ThongTinThayDoi = item.ThongTinThayDoi;
                    obj.ThongTinCu = item.ThongTinCu;
                    obj.ThongTinMoi = item.ThongTinMoi;
                    lstThongTinCaiChinh.Add(obj);
                }
                DbContext.ThongTinCaiChinhs.AddRange(lstThongTinCaiChinh);
                DbContext.SaveChanges();

                /// Cập nhật thông tin văn bằng
                foreach(var item in model.VanBang.ThongTinVanBangs)
                {
                    var obj = DbContext.ThongTinVanBangs.FirstOrDefault(t => t.Id == item.Id);
                    obj.GiaTri = item.GiaTri;
                }
                DbContext.SaveChanges();
                var bang = DbContext.Bangs.FirstOrDefault(b => b.Id == model.VanBang.Id);
                bang.SoLanCaiChinh = bang.SoLanCaiChinh == null ? 1 : bang.SoLanCaiChinh + 1;
                DbContext.SaveChanges();

                ///// Tạo lệnh in
                //var lenhIn = new NhomTaoVanBang();
                //lenhIn.Title = "Cải chính_" + bang.HoVaTen;
                //lenhIn.TruongHocId = bang.TruongHocId;
                //lenhIn.LoaiBangId = bang.LoaiBangId;
                //lenhIn.TrangThaiBangId = 2;
                //lenhIn.TongSohocSinh = 1;
                //lenhIn.ChoPhepTaoLai = true;
                //lenhIn.LoaiNhomTaoVanBangId = 4;
                //lenhIn.NgayTao = DateTime.Now;
                //lenhIn.NguoiTao = model.NguoiThucHien;
                //lenhIn.NgayCapNhat = DateTime.Now;
                //lenhIn.DonViId = model.DonViId;
                //lenhIn.IsDeleted = false;
                //DbContext.NhomTaoVanBangs.Add(lenhIn);
                //DbContext.SaveChanges();

                ///// Thêm học sinh vào lệnh in
                //var hocSinh = new HocSinhTrongNhomTaoVanBang();
                //hocSinh.BangId = bang.Id;
                //hocSinh.NhomTaoVanBangId = lenhIn.Id;
                //hocSinh.HocSinhId = bang.HocSinhId;
                //hocSinh.TrangThaiBangId = 2;
                //hocSinh.DonViId = bang.DonViId;
                //DbContext.HocSinhTrongNhomTaoVanBangs.Add(hocSinh);
                //DbContext.SaveChanges();

                /// Insert file quyết định cải chính
                var lstFile = new List<FileDinhKemCaiChinh>();
                foreach(var item in model.CaiChinh.Files)
                {
                    var file = new FileDinhKemCaiChinh();
                    file.FileId = item.FileId;
                    file.Url = item.Url;
                    file.TenFile = item.TenFile;
                    file.CaiChinhId = caiChinh.CaiChinhId;
                    file.NgayTao = item.NgayTao;
                    file.NguoiTao = item.NguoiTao;
                    file.Ext = item.Ext;
                    file.IconFile = item.IconFile;
                    file.DonViId = item.DonViId;
                    lstFile.Add(file);
                }
                DbContext.FileDinhKemCaiChinhs.AddRange(lstFile);
                DbContext.SaveChanges();

                transaction.Commit();

                // update bang cu id cua hocsinhtrongnhomtaovanbang
                string sqlString_1 = @"Update HocSinhTrongNhomTaoVanBangs 
                                        Set BangCuId = @BangCuId
                                        Where (BangCuId is null) and (BangId = @BangId) and (TrangThaiBangId = 6)";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sqlString_1;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@BangCuId", bangCu.Id));
                        command.Parameters.Add(new SqlParameter("@BangId", model.VanBang.Id));
                        command.ExecuteNonQuery();
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }

                // todo
                // update thong tin bang sau khi cai chinh
                    // get gia tri co truong du lieu hoc sinh
                //List<TruongDuLieuTrongBangViewModel> truongDuLieuTrongBangViewModels = new List<TruongDuLieuTrongBangViewModel>();
                //string sqlString_2 = @"Select b.*, c.TenTruongDuLieu as 'TenTruongDuLieuHocSinh' From Bang as a
                //                        Left Join ThongTinVanBang as b
                //                        on a.Id = b.BangId
                //                        Left Join TruongDuLieuLoaiBang as c
                //                        on b.TruongDuLieuCode = c.TruongDuLieuCode and a.LoaiBangId = c.LoaiBangId
                //                        Where (a.Id = @BangId) and (c.TenTruongDuLieu is not null)";
                //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                //{
                //    bool wasOpen = command.Connection.State == ConnectionState.Open;
                //    if (!wasOpen) command.Connection.Open();
                //    try
                //    {
                //        command.CommandText = sqlString_2;
                //        command.CommandType = CommandType.Text;
                //        command.Parameters.Add(new SqlParameter("@BangId", model.VanBang.Id));
                //        using (var reader = command.ExecuteReader())
                //        {
                //            truongDuLieuTrongBangViewModels = MapDataHelper<TruongDuLieuTrongBangViewModel>.MapList(reader);
                //        }
                //    }
                //    finally
                //    {
                //        command.Connection.Close();
                //    }
                //}
                    
                    // update thong tin van bang
                return new ResultModel(true, "Lưu thông tin cải chính thành công");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return new ResultModel(false, "Lưu thông tin cải chính thất bại: " + e.Message);
                // TODO: Handle failure
            }
        }
        public List<BangViewModel> GetVanBangDaCaiChinh(string keyword, int donViId)
        {
            try
            {
                var sql = @"Select a.*, l.Ten as TenLoaiBang From Bang as a 
                                inner join LoaiBang as l on a.LoaiBangId = l.Id 
                                Where a.Id in (Select a.Id From Bang as a
                                Left Join CaiChinhs as b
                                on a.Id = b.BangId Where a.SoLanCaiChinh > 0 and b.DonViThucHien = @DonViThucHien and ((@keyword is null) or (a.HoVaTen Like N'%'+@keyword+'%')) Group by a.Id)";
                var lstVanBangs = new List<BangViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    var lst = new List<BangViewModel>();
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword));
                        command.Parameters.Add(new SqlParameter("@DonViThucHien", donViId));
                        using (var reader = command.ExecuteReader())
                        {
                            lstVanBangs = MapDataHelper<BangViewModel>.MapList(reader);
                        }
                    } 
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return lstVanBangs;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public BangViewModel GetChiTietVanBangCaiChinh(int id)
        {
            try
            {
                var sql = @"select * from Bang where Id = " + id;
                var vanBang = new BangViewModel();
                var lstCaiChinh = new List<CaiChinhViewModel>();
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    var lst = new List<CaiChinhViewModel>();
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            vanBang = MapDataHelper<BangViewModel>.Map(reader);
                        }

                        command.CommandText = @"select c.*, d.TenDonVi as TenDonViThucHien, n.HoTen as TenNguoiThucHien from CaiChinhs as c 
                                                inner join DonVi as d on c.DonViThucHien = d.DonViId 
                                                inner join NguoiDung as n on c.NguoiThucHien = n.NguoiDungId where BangId = " + vanBang.Id + " Order by c.ThoiGianThucHien DESC";
                        command.CommandType = CommandType.Text;
                        using (var reader2 = command.ExecuteReader())
                        {
                            lst = MapDataHelper<CaiChinhViewModel>.MapList(reader2);
                        }

                        command.CommandText = @"select t.*, tl.Ten as TenTruongDuLieu, tl.KieuDuLieu from ThongTinVanBang as t inner join TruongDuLieu as tl on t.TruongDuLieuCode = tl.Code where BangId = " + vanBang.Id;
                        command.CommandType = CommandType.Text;
                        using (var reader6 = command.ExecuteReader())
                        {
                            vanBang.ThongTinVanBangs = MapDataHelper<ThongTinVBViewModel>.MapList(reader6);
                        }

                        foreach (var item in lst)
                        {
                            command.CommandText = @"select * from FileDinhKemCaiChinhs where CaiChinhId = " + item.CaiChinhId;
                            command.CommandType = CommandType.Text;
                            using (var reader3 = command.ExecuteReader())
                            {
                                item.Files = MapDataHelper<FileDinhKemCaiChinhViewModel>.MapList(reader3);
                            }

                            command.CommandText = @"select * from ThongTinCaiChinhs where CaiChinhId = " + item.CaiChinhId;
                            command.CommandType = CommandType.Text;
                            using (var reader4 = command.ExecuteReader())
                            {
                                item.ThongTinCaiChinhs = MapDataHelper<ThongTinCaiChinhVewModel>.MapList(reader4);
                            }

                            lstCaiChinh.Add(item);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                    vanBang.CaiChinhs = lstCaiChinh;
                }
                return vanBang;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
