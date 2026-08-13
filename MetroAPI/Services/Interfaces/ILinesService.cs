using MetroAPI.Helpers;
using MetroAPI.Models;
using System.Linq.Expressions;

namespace MetroAPI.Services.Interfaces
{
    public interface ILinesService
    {
        Task<Line> GetLineAsync(int id);
        Task<Line> GetLineByNoAsync(int lineNo);
        IQueryable<Line> GetSpecificLine(Expression<Func<Line, bool>> match);
        Task<IEnumerable<Line>> GetLinesAsync();
        Task<IQueryable<Station>> GetLineStationsAsync(int LineNo);
        Task<Result> AddLine(Line data);
        Task<Result> UpdateLine(Line data);
        Task<Result> DeleteLine(Line data);
    }
}
