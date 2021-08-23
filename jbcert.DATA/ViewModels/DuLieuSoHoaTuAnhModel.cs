using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class DuLieuSoHoaTuAnhModel
    {
        public int loaiBangId { get; set; }
        public string TruongDuLieuCode { get; set; }
        public float Margin { get; set; }
        public ToaDoModel ToaDo { get; set; }
        public List<string> GiaTri { get; set; }
    }
}
