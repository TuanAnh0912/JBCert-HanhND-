using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface IBaoCaoThongKe
    {
        public SoLuongThongKeBangDaInTungNamViewModel GetSoLuongBangDaInTungNam(int donViId);

        public ThongKeVanBangChungChiTheoNamViewModel ThongKeVanBangChungChiTheoNam(int? nam, int donViId);

        public ThongKeChungTheoDonViViewModel ThongKeChungTheoDonVi(int donViId);
    }
}
