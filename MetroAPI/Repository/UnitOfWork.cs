using MetroAPI.Data;
using MetroAPI.Repository.Interfaces;

namespace MetroAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Lines = new LinesRepository(context);
            Stations = new StationsRepository(context);
        }

        public ILinesRepository Lines { get; private set; }

        public IStationsRepository Stations { get; private set; }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
