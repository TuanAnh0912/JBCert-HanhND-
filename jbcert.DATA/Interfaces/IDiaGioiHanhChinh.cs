using jbcert.DATA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Interfaces
{
    public interface IDiaGioiHanhChinh
    {
        List<TinhViewModel> GetTinhs(string name);
        TinhViewModel GetTinhById(string id);

        List<HuyenViewModel> GetHuyens(string id, string name);

        HuyenViewModel GetHuyenByXa(string id);

        List<XaViewModel> GetXas(string id, string name);
    }
}
