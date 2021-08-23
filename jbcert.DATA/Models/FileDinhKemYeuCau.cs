using System;
using System.Collections.Generic;

#nullable disable

namespace jbcert.DATA.Models
{
    public partial class FileDinhKemYeuCau
    {
        public string FileId { get; set; }
        public string Url { get; set; }
        public string TenFile { get; set; }
        public int? YeuCauId { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? DonViId { get; set; }
        public string Ext { get; set; }
        public string IconFile { get; set; }
        public string MimeTypes { get; set; }

        public virtual YeuCau YeuCau { get; set; }
    }
}
