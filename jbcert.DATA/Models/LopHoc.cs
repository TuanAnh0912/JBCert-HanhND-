using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class LopHoc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenLop { get; set; }
        public Guid GiaoVien { get; set; }
        public string NienKhoa { get; set; }
        public int DonViId { get; set; }
        public Nullable<DateTime> NgayTao { get; set; }
    }
}
