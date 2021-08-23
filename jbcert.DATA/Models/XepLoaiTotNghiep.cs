using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class XepLoaiTotNghiep
    {
        [Key]
        public int Id { get; set; }

        public string XepLoai { get; set; }

        public string CodeCapDonVi { get; set; }
    }
}
