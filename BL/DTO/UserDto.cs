﻿using DAL.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.DTO
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        [DisplayName("Country")]
        public int CountryId { get; set; }
        public string? Country { get; set; }

        [ValidateNever]
        public string Role { get; set; }
    }
}
