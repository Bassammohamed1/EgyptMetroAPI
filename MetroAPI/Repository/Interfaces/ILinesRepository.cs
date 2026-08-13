using MetroAPI.Models;
using System.Linq.Expressions;

namespace MetroAPI.Repository.Interfaces
{
    public interface ILinesRepository : IRepository<Line>
    {
        Task<Line> GetByNo(int lineNo);
        IQueryable<Line> GetSpecificLine(Expression<Func<Line, bool>> match);
        IQueryable<Station> GetLineStations(int lineID);
    }
}
