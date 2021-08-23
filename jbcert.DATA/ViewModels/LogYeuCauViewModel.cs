using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LogYeuCauViewModel
    {
        public int LogId { get; set; }
        public Nullable<System.Guid> NguoiDungId { get; set; }
        public string HoTen { get; set; }
        public string HanhDong { get; set; }
        public Nullable<int> YeuCauId { get; set; }
        public Nullable<System.DateTime> ThoiGian { get; set; }
    }
}
