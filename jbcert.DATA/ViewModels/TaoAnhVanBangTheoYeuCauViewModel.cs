using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class TaoAnhVanBangTheoYeuCauViewModel
    {
        public int YeuCauId { get; set; }

        public int BangId { get; set; }

        public Guid NguoiCapNhat { get; set; }

        public DateTime NgayCapNhat { get; set; }
    }
}
