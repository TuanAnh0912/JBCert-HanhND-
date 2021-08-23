using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class YeuCau
    {
        public YeuCau()
        {
            FileDinhKemYeuCaus = new HashSet<FileDinhKemYeuCau>();
            LienKetHocSinhYeuCaus = new HashSet<LienKetHocSinhYeuCau>();
            LogYeuCaus = new HashSet<LogYeuCau>();
        }

        public int Id { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTaoYeuCau { get; set; }
        public int? DonViYeuCauId { get; set; }
        public int? LoaiYeuCauId { get; set; }
        public int? DonViDichId { get; set; }
        public DateTime? NgayTao { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public Guid? NguoiCapNhat { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DonViId { get; set; }
        public string MaTrangThaiYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string MaYeuCau { get; set; }
        public string TenYeuCau { get; set; }
        public DateTime? NgayGuiYeuCau { get; set; }
        public int? LoaiVanBangId { get; set; }
        public int? DonViChuyenTiepId { get; set; }
        public virtual LoaiYeuCau LoaiYeuCau { get; set; }
        public virtual TrangThaiYeuCau MaTrangThaiYeuCauNavigation { get; set; }
        public virtual ICollection<FileDinhKemYeuCau> FileDinhKemYeuCaus { get; set; }
        public virtual ICollection<LienKetHocSinhYeuCau> LienKetHocSinhYeuCaus { get; set; }
        public virtual ICollection<LogYeuCau> LogYeuCaus { get; set; }
    }
}
