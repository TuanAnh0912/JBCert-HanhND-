﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace jbcert.DATA.IdentityModels
{
    public class VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
