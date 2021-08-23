using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class LienKetHocSinhYeuCau
    {
        public int LienKetId { get; set; }
        public int? YeuCauId { get; set; }
        public int? HocSinhId { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool? TrangThaiKiemTraTaoHoSo { get; set; }
        public int? TrangThaiCapPhatBang { get; set; }

        public virtual HocSinh HocSinh { get; set; }
        public virtual YeuCau YeuCau { get; set; }
    }
}
