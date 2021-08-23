using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class SoGocFileDinhKem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string FileId { get; set; }
        public string Url { get; set; }
        public string TenFile { get; set; }
        public int? SoHoaId { get; set; }
        public Guid? NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? DonViId { get; set; }
        public string Ext { get; set; }
        public string IconFile { get; set; }
        public string MimeTypes { get; set; }
    }
}
