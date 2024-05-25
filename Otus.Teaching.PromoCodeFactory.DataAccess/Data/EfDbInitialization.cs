using Microsoft.EntityFrameworkCore;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions;
using Otus.Teaching.PromoCodeFactory.DataAccess.DataContext;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Data
{
    public class EfDbInitialization : IDbInitialization
    {
        private readonly DataBaseContext _context;
 
        public EfDbInitialization(DataBaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public void Init()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            //_context.Roles.AddRange(FakeDataFactory.Roles);
            //_context.SaveChanges();


            //_context.Employees.AddRange(FakeDataFactory.Employees);
            //_context.SaveChanges();


            //_context.Preferences.AddRange(FakeDataFactory.Preferences);
            //_context.SaveChanges();

            //_context.Customers.AddRange(FakeDataFactory.Customers);
            //_context.SaveChanges();

            //_context.PromoCodes.AddRange(FakeDataFactory.PromoCodes);

            //_context.SaveChanges();
        }
    }
}
