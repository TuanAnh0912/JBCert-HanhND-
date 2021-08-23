using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.Models
{
    public class NhomTaoVanBang
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public int? TruongHocId { get; set; }

        public int? DonViId { get; set; }

        public int? LoaiBangId { get; set; }

        public int? TrangThaiBangId { get; set; }

        public int? TongSohocSinh { get; set; }

        public bool? ChoPhepTaoLai { get; set; }

        public int? LoaiNhomTaoVanBangId { get; set; }

        public int? DonViIn { get; set; }

        public DateTime? NgayTao { get; set; }

        public Guid? NguoiTao { get; set; }

        public DateTime NgayCapNhat { get; set; }
    
        public Guid? NguoiCapNhat { get; set; }

        public bool? CanDelete { get; set; }

        public bool? AddedByImport { get; set; }

        public bool IsDeleted { get; set; }

        public string DuongDanFileAnh { get; set; }
        public string DuongDanFileAnhDeIn { get; set; }
    }
}
