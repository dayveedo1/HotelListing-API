using HotelListingAPI.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.Data.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext context;
        private IGenericRepository<Country> countries;
        private IGenericRepository<Hotel> hotels;

        public UnitOfWork(HotelDbContext context)
        {
            this.context = context;
        }
        public IGenericRepository<Country> Countries => countries ??= new GenericRepository<Country>(context);

        public IGenericRepository<Hotel> Hotels => hotels ??= new GenericRepository<Hotel>(context);

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
