using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace jbcert.DATA.IdentityModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
