using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class MauCongVan
    {
        [Key]
        public int Id { get; set; }

        public string TenCongVan { get; set; }

        public string NoiDung { get; set; }

        public int LoaiYeuCauId { get; set; }
    }
}
