using MetroAPI.Data;
using MetroAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MetroAPI.Services.Lines
{
    public class LinesService : ILinesService
    {
        private readonly AppDbContext _context;
        public LinesService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Line> GetLineAsync(int id)
        {
            if (id is 0)
                throw new ArgumentNullException("Id can't be zero");

            return await _context.Lines.AsNoTracking().SingleOrDefaultAsync(L => L.Id == id);
        }
        public async Task<Line> GetLineByNoAsync(int lineNo)
        {
            if (lineNo is 0)
                throw new ArgumentNullException("LineNo can't be zero");

            return await _context.Lines.AsNoTracking().SingleOrDefaultAsync(l => l.LineNo == lineNo);
        }
        public async Task<IQueryable<Line>> GetSpecificLineAsync(Expression<Func<Line, bool>> match)
        {
            return _context.Lines.Where(match).AsNoTracking();
        }
        public async Task<IEnumerable<Line>> GetLinesAsync()
        {
            return _context.Lines.AsNoTracking().AsEnumerable();
        }
        public async Task AddLine(Line data)
        {
            await _context.Lines.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateLine(Line data)
        {
            _context.Lines.Update(data);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteLine(Line data)
        {
            _context.Lines.Remove(data);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Station>> GetLineStationsAsync(int LineNo)
        {
            var line = await this.GetLineByNoAsync(LineNo);

            if (line is not null)
            {
                var lineId = line.Id;
                var stations = _context.Stations.Include(s => s.Line).Where(s => s.LineId == lineId).AsNoTracking().AsSplitQuery();
                if (stations is not null)
                    return stations.ToList();
            }

            return Enumerable.Empty<Station>().ToList();
        }
    }
}