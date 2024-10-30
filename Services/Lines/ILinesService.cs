using MetroAPI.Models;
using System.Linq.Expressions;

namespace MetroAPI.Services.Lines
{
    public interface ILinesService
    {
        Task<IEnumerable<Line>> GetLinesAsync();
        Task<Line> GetLineAsync(int id);
        Task<Line> GetLineByNoAsync(int lineNo);
        Task<IQueryable<Line>> GetSpecificLineAsync(Expression<Func<Line, bool>> match);
        Task AddLine(Line data);
        Task UpdateLine(Line data);
        Task DeleteLine(Line data);
        Task<List<Station>> GetLineStationsAsync(int LineNo);
    }
}
