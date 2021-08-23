using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LopHocViewModel
    {
        public int Id { get; set; }
        public string TenLop { get; set; }
        public Guid GiaoVien { get; set; }
        public string TruongHocId { get; set; }
        public string Nam { get; set; }
        public string TruongHoc { get; set; }
        public string TenGiaoVien { get; set; }
        public string DienThoaiGiaoVien { get; set; }
        public int DonViId { get; set; }
        public string NienKhoa { get; set; }
        public Nullable<DateTime> NgayTao { get; set; }
    }
}
