using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class ChiTietThongTinVanBangViewModel<T,M,K/*,I*/>
    {
        public List<T> ListOfData { get; set; }

        public List<M> ListOfLog { get; set; }

        public List<K> ListOfCaiChinh { get; set; }

        //public List<I> ListOfYeuCauPhatBang { get; set; }
    }
}
