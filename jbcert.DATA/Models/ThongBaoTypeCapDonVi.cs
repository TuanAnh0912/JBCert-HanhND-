using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class ThongBaoTypeCapDonVi
    {
        [Key]
        public int Id { get; set; }

        public int ThongBaoTypeId { get; set; }

        public string CodeCapDonVi { get; set; }
    }
}
