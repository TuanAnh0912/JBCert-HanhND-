using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbcert.DATA.Helpers;
using jbcert.DATA.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace jbcert.DATA.Provider
{
    public class BackUpProvider : ApplicationDbContext
    {
        public string GetAllThongTinSoGocByNam(int nam, int loaibangid)
        {
            try
            {
                var thongtinsogoc = new ThongTinSoGocViewModel();
                #region
                //string sql = @"select b.HocSinhId,SUBSTRING(b.SoVaoSo,15,1) as 'Tt',tdl.Code,tdl.Ten,ttvb.GiaTri
                //                from bang as b
                //                inner join ThongTinVanBang as ttvb on b.Id=ttvb.BangId
                //                inner join (select distinct TruongDuLieuCode,TenTruongDuLieu,DungChung from TruongDuLieuLoaiBang) as c on ttvb.TruongDuLieuCode=c.TruongDuLieuCode 
                //                inner join TruongDuLieu as tdl on ttvb.TruongDuLieuCode = tdl.Code
                //                where (b.TrangThaiBangId >=4) and (c.DungChung=0) and (b.NamTotNghiep=@Nam) and (b.BangGocId Is NULL) and (b.LoaiBangId=@LoaiBangId) and (tdl.KieuDuLieu=1)";
                //using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                //{
                //    bool wasOpen = command.Connection.State == ConnectionState.Open;
                //    if (!wasOpen) command.Connection.Open();
                //    try
                //    {
                //        command.CommandText = sql;
                //        command.CommandType = CommandType.Text;
                //        command.Parameters.Add(new SqlParameter("@Nam", nam));
                //        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaibangid));
                //        using (var reader = command.ExecuteReader())
                //        {
                //            thongtinsogoc.Sogocrieng = MapDataHelper<ThongTinSoGocRiengViewModel>.MapList(reader);
                //        }
                //    }
                //    finally
                //    {
                //        command.Connection.Close();
                //    }
                //}
                #endregion
                string sql2 = @"select b.HocSinhId,tdl.Code,tdl.Ten,ttvb.GiaTri
                                    from bang as b
                                    inner join ThongTinVanBang as ttvb on b.Id=ttvb.BangId
                                    inner join (select distinct LoaiBangId,TruongDuLieuCode,TenTruongDuLieu,DungChung from TruongDuLieuLoaiBang) as c on ttvb.TruongDuLieuCode=c.TruongDuLieuCode 
                                    inner join TruongDuLieu as tdl on ttvb.TruongDuLieuCode = tdl.Code
                                    where (b.TrangThaiBangId >=4) and (c.DungChung=1) and (b.NamTotNghiep=@Nam) and (b.BangGocId Is NULL) and (c.LoaiBangId=@LoaiBangId)
                                    order by b.HocSinhId";
                using (var command = DbContext.Database.GetDbConnection().CreateCommand())
                {
                    bool wasOpen = command.Connection.State == ConnectionState.Open;
                    if (!wasOpen) command.Connection.Open();
                    try
                    {
                        command.CommandText = sql2;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@Nam", nam));
                        command.Parameters.Add(new SqlParameter("@LoaiBangId", loaibangid));
                        using (var reader = command.ExecuteReader())
                        {
                            thongtinsogoc.Sogocchung = MapDataHelper<ThongTinSoGocChungViewModel>.MapList(reader);
                        }
                    }
                    finally
                    {
                        command.Connection.Close();
                    }
                }
                return NhomThongTinHocSinh(thongtinsogoc, nam, loaibangid);
            }
            catch (Exception e)
            {
                throw e;
                return e.Message;
            }
        }

        private string NhomThongTinHocSinh(ThongTinSoGocViewModel thongtinsogoc, int nam,int bangId)
        {
            var lstthongtin = new List<BangGocHocSinhModel>();
            int check = 0;
            int kt = 1;
            var tt = new BangGocHocSinhModel();
            tt.Code = new List<string>();
            tt.GiaTri = new List<string>();
            foreach (var x in thongtinsogoc.Sogocchung)
            {
                if (check == 0)
                {
                    tt.HocSinhId = x.HocSinhId;
                    tt.Code.Add(x.Code);
                    tt.GiaTri.Add(x.GiaTri);
                    check = 1;
                    kt++;
                }
                else
                {
                    if (x.HocSinhId == tt.HocSinhId)
                    {
                        tt.Code.Add(x.Code);
                        tt.GiaTri.Add(x.GiaTri);
                        if (kt == thongtinsogoc.Sogocchung.Count - 1)
                        {
                            lstthongtin.Add(tt);
                        }
                    }
                    if (x.HocSinhId != tt.HocSinhId)
                    {
                        lstthongtin.Add(tt);
                        tt = new BangGocHocSinhModel();
                        tt.Code = new List<string>();
                        tt.GiaTri = new List<string>();
                        tt.HocSinhId = x.HocSinhId;
                        tt.Code.Add(x.Code);
                        tt.GiaTri.Add(x.GiaTri);

                    }
                    kt++;
                }
            }
            return GroupBySoGoc(lstthongtin, nam, bangId);
        }


        public string GroupBySoGoc(List<BangGocHocSinhModel> lstthongtin, int nam,int bangId)
        {

            var lstNhomHocSinh = new List<NhomHocSinhSoGocModel>();
            foreach (var hs in lstthongtin)
            {
                if (hs.Checked == true) continue;
                else hs.Checked = true;
                var nhom = new NhomHocSinhSoGocModel();
                nhom.Hocsinh = new List<int>();
                nhom.Hocsinh.Add(hs.HocSinhId);                
                foreach (var hssame in lstthongtin)
                {
                    if (checksame(hs, hssame) &&  (hssame.Checked==false))
                    {
                        nhom.Hocsinh.Add(hssame.HocSinhId);
                        hssame.Checked = true;
                    }
                }
                lstNhomHocSinh.Add(nhom);
            }
            return ExportExcel(lstNhomHocSinh, nam, bangId);
        }

        private string ExportExcel(List<NhomHocSinhSoGocModel> lstNhomHocSinh, int nam,int bangId)
        {
            string url = Path.Combine("Upload", "SoGoc", "BangGoc" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            FileInfo file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), url));

            using (ExcelPackage package = new ExcelPackage(file))
            {
                int sheetstt = 1;
                if (lstNhomHocSinh.Count < 1) { 
                  var sheet1 = package.Workbook.Worksheets.Add("Bằng gốc " + sheetstt.ToString());
                    sheetstt++;
                    package.Save();
                    return url;
                }
                else
                {
                    foreach (var x in lstNhomHocSinh)
                    {
                        var sheet = package.Workbook.Worksheets.Add("Bằng gốc " + sheetstt.ToString());
                        int check = 0;
                        int row = 8;
                        int stt = 1;
                        sheetstt++;
                        var lstChung = new List<ThongTinSoGocChungViewModel>();
                        foreach (var hsid in x.Hocsinh)
                        {

                            var lstRieng = GetThongTinSoGocRiengByHocSinhId(hsid, nam, bangId);
                            if (check == 0)
                            {
                                lstChung = GetThongTinSoGocChungByHocSinhId(hsid, nam, bangId);
                                sheet.Cells["A1:D1"].Value = "SỞ GIÁO DỤC VÀ ĐÀO TẠO " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Hội đồng thi");
                                sheet.Cells["A1:D1"].Style.Font.Bold = true;
                                sheet.Cells["A1:D1"].Style.Font.Size = 14;
                                sheet.Cells["A1:D1"].Merge = true;
                                //  sheet.Cells["A1:D1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                sheet.Cells["E2:L2"].Value = "SỔ GỐC CẤP BẰNG TỐT NGHIỆP THPT NĂM " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Năm");
                                sheet.Cells["E2:L2"].Style.Font.Bold = true;
                                sheet.Cells["E2:L2"].Style.Font.Size = 14;
                                sheet.Cells["E2:L2"].Merge = true;
                                //  sheet.Cells["E2:L2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                sheet.Cells["D3:G3"].Value = "Quyết định công nhận tốt nghiệp số: ...";
                                sheet.Cells["D3:G3"].Merge = true;
                                //sheet.Cells["D3:G3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                sheet.Cells["D5"].Value = "Kỳ thi: THPT quốc gia năm " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Năm");
                                sheet.Cells["D4"].Value = "Năm tốt nghiệp: " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Năm");
                                sheet.Cells["J3:L3"].Value = "Hội đồng thi: Sở Giáo Dục và Đào tạo  " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Hội đồng thi");
                                sheet.Cells["J3:L3"].Merge = true;
                                sheet.Cells["J4"].Value = "Ngày thi: " + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Ngày") + "/" + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Tháng") + "/" + GetGiaTriByHocSinhIdcode(lstChung, hsid, "Năm");
                                sheet.Cells["J5"].Value = "Học sinh trường/trung tâm: ";
                                sheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                sheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                sheet.Cells[7, 1].Value = "STT";
                                sheet.Cells[7, 1].Style.Font.Bold = true;
                                sheet.Cells[7, 2].Style.Font.Bold = true;
                                sheet.Cells[7, 2].Value = "TT";
                                check = 1;
                            }
                            int colfirst = 3;
                            foreach (var t in lstRieng)
                            {
                                sheet.Cells[7, colfirst].Value = t.Ten;
                                sheet.Cells[7, colfirst].Style.Font.Bold = true;
                                sheet.Cells[row, 2].Value = t.Tt;
                                sheet.Cells[row, 1].Value = stt;
                                sheet.Cells[row, colfirst].Value = t.GiaTri;
                                colfirst++;
                            }
                            row++;
                            stt++;
                            sheet.Cells.AutoFitColumns();
                            sheet.Cells["A1:J5"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        }
                    }
                    package.Save();
                    return url;
                }

            }

            
        }
        public string GetGiaTriByHocSinhIdcode(List<ThongTinSoGocChungViewModel> lst, int hocsinhid, string Ten)
        {
            var rs = "";
            foreach (var x in lst)
            {
                if (x.Ten == Ten && x.HocSinhId == hocsinhid)
                {
                    rs = x.GiaTri;
                }
            }
            return rs;
        }
        public List<ThongTinSoGocRiengViewModel> GetThongTinSoGocRiengByHocSinhId(int hocsinhId, int nam,int bangId)
        {
            var rs = new List<ThongTinSoGocRiengViewModel>();
            string sql = @"select b.HocSinhId,SUBSTRING(b.SoVaoSo,15,1) as 'Tt',tdl.Code,tdl.Ten,ttvb.GiaTri
                                from bang as b
                                inner join ThongTinVanBang as ttvb on b.Id=ttvb.BangId
                                inner join (select distinct TruongDuLieuCode,TenTruongDuLieu,DungChung from TruongDuLieuLoaiBang) as c on ttvb.TruongDuLieuCode=c.TruongDuLieuCode 
                                inner join TruongDuLieu as tdl on ttvb.TruongDuLieuCode = tdl.Code
                                where (b.TrangThaiBangId >=4) and (c.DungChung=0) and (b.NamTotNghiep=@nam) and (b.BangGocId Is NULL) and (b.HocSinhId=@hocsinhId)and (tdl.KieuDuLieu=1) and (b.LoaiBangId=@bangId)
                                order by Code ";
            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
            {
                bool wasOpen = command.Connection.State == ConnectionState.Open;
                if (!wasOpen) command.Connection.Open();
                try
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@nam", nam));
                    command.Parameters.Add(new SqlParameter("@hocsinhId", hocsinhId));
                    command.Parameters.Add(new SqlParameter("@bangId", bangId));

                    using (var reader = command.ExecuteReader())
                    {
                        rs = MapDataHelper<ThongTinSoGocRiengViewModel>.MapList(reader);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
            return rs;
        }
        public List<ThongTinSoGocChungViewModel> GetThongTinSoGocChungByHocSinhId(int hocsinhId, int nam,int bangId)
        {
            var rs = new List<ThongTinSoGocChungViewModel>();
            string sql = @"select b.HocSinhId,tdl.Code,tdl.Ten,ttvb.GiaTri
                                    from bang as b
                                    inner join ThongTinVanBang as ttvb on b.Id=ttvb.BangId
                                    inner join (select distinct LoaiBangId,TruongDuLieuCode,TenTruongDuLieu,DungChung from TruongDuLieuLoaiBang) as c on ttvb.TruongDuLieuCode=c.TruongDuLieuCode 
                                    inner join TruongDuLieu as tdl on ttvb.TruongDuLieuCode = tdl.Code
                                    where (b.TrangThaiBangId >=4) and (c.DungChung=1) and (b.NamTotNghiep=@nam) and (b.BangGocId Is NULL) and (b.HocSinhId=@hocsinhId) and (c.LoaiBangId=@bangId) ";
            using (var command = DbContext.Database.GetDbConnection().CreateCommand())
            {
                bool wasOpen = command.Connection.State == ConnectionState.Open;
                if (!wasOpen) command.Connection.Open();
                try
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@nam", nam));
                    command.Parameters.Add(new SqlParameter("@hocsinhId", hocsinhId));
                    command.Parameters.Add(new SqlParameter("@bangId", bangId));

                    using (var reader = command.ExecuteReader())
                    {
                        rs = MapDataHelper<ThongTinSoGocChungViewModel>.MapList(reader);
                    }
                }
                finally
                {
                    command.Connection.Close();
                }
            }
            return rs;
        }

        public bool checksame(BangGocHocSinhModel hs, BangGocHocSinhModel hssame)
        {

            List<string> Code1 = hs.Code;
            List<string> Code2 = hssame.Code;
            List<string> GiaTri1 = hs.GiaTri;
            List<string> GiaTri2 = hssame.GiaTri;
            Code1.ForEach(g => g=g.Trim());
            Code2.ForEach(g => g=g.Trim());
            for(int i=0;i<=GiaTri1.Count()-1; i++)
            {
                GiaTri1[i] = GiaTri1[i].Trim();
            }

            IEnumerable<string> equalCode1 = Code1.Except(Code2);
            IEnumerable<string> equalCode2 = Code2.Except(Code1);
            IEnumerable<string> equalGiaTri1 = GiaTri1.Except(GiaTri2);
            IEnumerable<string> equalGiaTri2 = GiaTri2.Except(GiaTri1);
            bool CodeCheck = !equalCode1.Any() && !equalCode2.Any();
            bool GiaTriCheck = !equalGiaTri1.Any() && !equalGiaTri2.Any();
            if (CodeCheck && GiaTriCheck) return true;
            return false;
        }
    }
}
