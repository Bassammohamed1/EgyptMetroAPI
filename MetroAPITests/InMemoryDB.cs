using MetroAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroAPI.Tests
{
    internal class InMemoryDB : AppDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
        public override void Dispose()
        {
            Database.EnsureDeleted();
            base.Dispose();
        }
    }
}
