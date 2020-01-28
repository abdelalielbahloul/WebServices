﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebServices.ModelViews
{
    public class RegisterModel
    {
        [StringLength(256), Required]
        public string UserName { get; set; }
        [StringLength(256), Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
