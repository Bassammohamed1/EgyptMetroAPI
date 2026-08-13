using MetroAPI.Data;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MetroAPI.Repository
{
    public class LinesRepository : Repository<Line>, ILinesRepository
    {
        private readonly AppDbContext _context;

        public LinesRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Line> GetByNo(int lineNo)
        {
            return await _context.Lines.AsNoTracking().FirstOrDefaultAsync(l => l.LineNo == lineNo);
        }

        public IQueryable<Station> GetLineStations(int lineID)
        {
            return _context.Stations
                .Include(s => s.Line).Where(s => s.LineId == lineID)
                .AsNoTracking().AsSplitQuery();
        }

        public IQueryable<Line> GetSpecificLine(Expression<Func<Line, bool>> match)
        {
            return _context.Lines.Where(match).AsNoTracking();
        }
    }
}
