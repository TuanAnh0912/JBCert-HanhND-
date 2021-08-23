using jbcert.DATA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Provider
{
    public class AddLienKetCapDonViChucNangProvider : ApplicationDbContext
    {
        public void Add(int capDonVi)
        {
            List<int> chucNangIds = DbContext.ChucNangs.Select(x => x.ChucNangId).ToList();
            List<int> capDonViIds = DbContext.CapDonVis.Where(x =>x.CapDonViId == capDonVi).Select(x => x.CapDonViId).ToList();

            List<LienKetCapDonViChucNang> lienKetCapDonViChucNangs = new List<LienKetCapDonViChucNang>();
            foreach (int chucNangId in chucNangIds)
            {
                foreach (var capDonViId in capDonViIds)
                {
                    LienKetCapDonViChucNang lienKetCapDonViChucNang = new LienKetCapDonViChucNang();
                    lienKetCapDonViChucNang.CapDonViId = capDonViId;
                    lienKetCapDonViChucNang.ChucNangId = chucNangId;
                    lienKetCapDonViChucNangs.Add(lienKetCapDonViChucNang);
                }
            }
            DbContext.LienKetCapDonViChucNangs.AddRange(lienKetCapDonViChucNangs);
            DbContext.SaveChanges();
        }
    }
}
