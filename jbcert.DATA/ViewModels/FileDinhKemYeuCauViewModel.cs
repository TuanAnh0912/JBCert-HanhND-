using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class FileDinhKemYeuCauViewModel
    {
        public string FileId { get; set; }
        public string Url { get; set; }
        public string TenFile { get; set; }
        public Nullable<int> YeuCauId { get; set; }
        public Nullable<System.Guid> NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> DonViId { get; set; }
        public string Ext { get; set; }
        public string IconFile { get; set; }
        public string MimeTypes { get; set; }
        public string FileKey { get; set; }
    }
}
