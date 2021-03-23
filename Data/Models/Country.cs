using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data
{
    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public virtual IList<Hotel> Hotels { get; set; }
    }
}
