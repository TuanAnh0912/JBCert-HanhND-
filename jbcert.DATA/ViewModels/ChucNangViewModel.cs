using System;
using System.Collections.Generic;
using System.Text;

namespace jbcert.DATA.ViewModels
{
    public class ChucNangViewModel
    {
        public int ChucNangId { get; set; }
        public Nullable<int> KhoaChaId { get; set; }
        public string TenChucNang { get; set; }
        public string AuthCode { get; set; }
        public string Icon { get; set; }

        public string Alias { get; set; }
        public string Summary { get; set; }
        public string DefaultAlias { get; set; }
        public Nullable<bool> ShowOnMenu { get; set; }
    }
}
