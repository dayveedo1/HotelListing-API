using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Dto
{
    public class CreateCountryDto
    {
        [Required]
        [StringLength(maximumLength:50, ErrorMessage = "Country Name is too long")]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 10, ErrorMessage = "Short Country Name is too long")]
        public string ShortName { get; set; }
    }

    public class UpdateCountryDto : CreateCountryDto
    {

    }

    public class CountryDto : CreateCountryDto
    {
        public int CountryId { get; set; }
        public IList<HotelDto> Hotels { get; set; }
    }
}
