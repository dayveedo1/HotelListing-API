using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    HotelId = 1,
                    Name = "Eko Atlantic Hotel",
                    Address = "Along Lekki-Epe expressway, Lekki",
                    CountryId = 1,
                    Rating = 4.5
                },

                new Hotel
                {
                    HotelId = 2,
                    Name = "Pearl Garden Hotel",
                    Address = "Washington D.C",
                    CountryId = 2,
                    Rating = 4.7
                },

                new Hotel
                {
                    HotelId = 3,
                    Name = "Hotel Excelencio",
                    Address = "Madrid drive",
                    CountryId = 3,
                    Rating = 4.3
                }
              );
        }
    }
}
