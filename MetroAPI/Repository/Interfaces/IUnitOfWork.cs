using MetroAPI.Models;

namespace MetroAPI.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        ILinesRepository Lines { get; }
        IStationsRepository Stations { get; }
        Task Commit();
    }
}
