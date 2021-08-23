using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbcert.DATA.ViewModels
{
    public class LoaiBangViewModel
    {
        public int? Id { get; set; }

        public string Ten { get; set; }

        public int HinhThucCapId { get; set; }

        public string HinhThucCap { get; set; }

        public string CodeCapDonVi { get; set; }

        public string CapDonVi { get; set; }

        public int Level { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string AnhLoaiBang { get; set; }

        public bool? IsChungChi { get; set; }

        public DateTime? NgayTao { get; set; }

        public Guid? NguoiTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public Guid? NguoiCapNhat { get; set; }

        public int? LoaiBangGocId { get; set; }

        public List<TruongDuLieuLoaiBangViewModel> TruongDuLieuLoaiBangs { get; set; }

        public int? DonViId { get; set; }
    }
}
