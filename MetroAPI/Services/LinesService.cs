using MetroAPI.Helpers;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using MetroAPI.Services.Interfaces;
using System.Linq.Expressions;

namespace MetroAPI.Services
{
    public class LinesService : ILinesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LinesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Line> GetLineAsync(int id)
        {
            return id is 0 ? throw new ArgumentNullException("ID can't be zero") :
                await _unitOfWork.Lines.Get(id);
        }

        public async Task<Line> GetLineByNoAsync(int lineNo)
        {
            return lineNo is 0 ? throw new ArgumentNullException("LineNo can't be zero") :
                await _unitOfWork.Lines.GetByNo(lineNo);
        }

        public IQueryable<Line> GetSpecificLine(Expression<Func<Line, bool>> match)
        {
            return _unitOfWork.Lines.GetSpecificLine(match);
        }

        public async Task<IEnumerable<Line>> GetLinesAsync()
        {
            return await _unitOfWork.Lines.GetAll();
        }

        public async Task<IQueryable<Station>> GetLineStationsAsync(int LineNo)
        {
            var line = await _unitOfWork.Lines.GetByNo(LineNo);

            return line is not null ? _unitOfWork.Lines.GetLineStations(line.Id) :
                Enumerable.Empty<Station>().AsQueryable();
        }

        public async Task<Result> AddLine(Line data)
        {
            var result = await _unitOfWork.Lines.Add(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while adding." };
        }

        public async Task<Result> UpdateLine(Line data)
        {
            var result = _unitOfWork.Lines.Update(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while updating." };
        }

        public async Task<Result> DeleteLine(Line data)
        {
            var result = _unitOfWork.Lines.Delete(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while deleting." };
        }
    }
}