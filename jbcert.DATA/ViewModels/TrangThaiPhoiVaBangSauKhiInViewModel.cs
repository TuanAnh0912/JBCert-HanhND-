using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class TrangThaiPhoiVaBangSauKhiInViewModel
    {
        public int BangId { get; set; }

        public string SoHieu { get; set; }

        public int LoaiBangId { get; set; }

        public DateTime NgayCapNhat { get; set; }

        public Guid NguoiCapNhat { get; set; }
    }
}
