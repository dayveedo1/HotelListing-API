﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Dto
{
    public class UserDto : LoginUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public ICollection<string> Roles { get; set; }
        //[Required]
        //[DataType(DataType.EmailAddress)]
        //public string Email { get; set; }
        //[StringLength(15, ErrorMessage ="Your password is limited to {2} to {1} characters", MinimumLength=5)]
        //public string Password { get; set; }
    }

    public class LoginUserDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 5)]
        public string Password { get; set; }
    }
}
