using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class HocSinhFileDinhKemViewModel
    {
        public string FileId { get; set; }
        public string Url { get; set; }
        public string TenFile { get; set; }
        public Nullable<int> HocSinhId { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<System.Guid> NguoiTao { get; set; }
        public string Ext { get; set; }
        public string IconFile { get; set; }
        public Nullable<int> DonViId { get; set; }
    }
}
