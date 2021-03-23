using HotelListingAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData(
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

            builder.Entity<Hotel>().HasData(
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

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

       
    }
}
