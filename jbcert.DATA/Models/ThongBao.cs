using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace jbcert.DATA.Models
{
    public class ThongBao
    {
        [Key]
        public string Id { get; set; }

        public string NoiDung { get; set; }

        public string Title { get; set; }

        public int? ThongBaoTypeId { get; set; }

        public string NguoiGuiId { get; set; }

        public int? PhongBanGuiId { get; set; }

        public int? DonViGuiId { get; set; }

        public string NguoiNhanId { get; set; }

        public int? PhongBanNhanId { get; set; }

        public int? DonViNhanId { get; set; }

        public bool? DaDoc { get; set; }

        public string Code { get; set; }

        public string Url { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}
