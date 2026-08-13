using MetroAPI.Data;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MetroAPI.Repository
{
    public class StationsRepository : Repository<Station>, IStationsRepository
    {
        private readonly AppDbContext _context;

        public StationsRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Station GetStationByName(string name, int lineNo)
        {
            return _context.Stations.AsNoTracking().AsSplitQuery()
                .Include(l => l.Line).FirstOrDefault(s => s.Name.ToLower() == name.ToLower() && s.Line.LineNo == lineNo);
        }

        public IEnumerable<Station> GetStationsWithLines()
        {
            return _context.Stations.Include(s => s.Line)
                .AsNoTracking().AsSplitQuery();
        }
    }
}
