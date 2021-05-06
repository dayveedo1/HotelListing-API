using HotelListingAPI.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Security
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDto userDto);
        Task<string> CreateToken();
    }
}
