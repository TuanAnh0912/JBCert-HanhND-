using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LienKetHocSinhYeuCauViewModel
    {
        public int LienKetId { get; set; }
        public Nullable<int> YeuCauId { get; set; }
        public Nullable<int> HocSinhId { get; set; }
        public Nullable<System.Guid> NguoiTao { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
    }
}
