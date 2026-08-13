using MetroAPI.Models;

namespace MetroAPI.Repository.Interfaces
{
    public interface IStationsRepository : IRepository<Station>
    {
        IEnumerable<Station> GetStationsWithLines();
        Station GetStationByName(string name, int lineNo);
    }
}