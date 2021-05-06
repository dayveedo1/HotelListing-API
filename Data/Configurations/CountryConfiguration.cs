using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
               new Country
               {
                   CountryId = 1,
                   Name = "Nigeria",
                   ShortName = "NGN"
               },
                  new Country
                  {
                      CountryId = 2,
                      Name = "Unites States of America",
                      ShortName = "USA"
                  },

                  new Country
                  {
                      CountryId = 3,
                      Name = "Ghana",
                      ShortName = "GHN"
                  },

                  new Country
                  {
                      CountryId = 4,
                      Name = "Spain",
                      ShortName = "ESP"
                  }

               );
        }
    }
}
