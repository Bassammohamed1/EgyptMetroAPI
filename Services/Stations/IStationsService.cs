using MetroAPI.Models;

namespace MetroAPI.Services.Stations
{
    public interface IStationsService
    {
        Task<IEnumerable<Station>> GetStationsAsync();
        Task<IEnumerable<Station>> GetStationsWithLinesAsync();
        Task<Station> GetStationAsync(int id);
        Task AddStation(Station data);
        Task UpdateStation(Station data);
        Task DeleteStation(Station data);
        Task<List<int>> GetStationLineAsync(string station);
        Task<List<Station>> GetPathAsync(string fromStation, string toStation);
        Task<double> GetDistanceAsync(double lat1, double lon1, double lat2, double lon2);
    }
}
