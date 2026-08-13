using MetroAPI.DTOs;
using MetroAPI.DTOS;
using MetroAPI.Helpers;
using MetroAPI.Models;

namespace MetroAPI.Services.Interfaces
{
    public interface IStationsService
    {
        Task<IEnumerable<Station>> GetStationsAsync();
        IEnumerable<Station> GetStationsWithLines();
        Task<Station> GetStationAsync(int id);
        Task<Result> AddStation(Station data);
        Task<Result> UpdateStation(Station data);
        Task<Result> DeleteStation(Station data);
        Task<IEnumerable<int>> GetStationLineAsync(string station);
        Task<List<Station>> GetPathAsync(string fromStation, string toStation);
        Task<double> GetDistanceAsync(double lat1, double lon1, double lat2, double lon2);
        Task<NearestStationDTO> GetNearestStation(LocationDTO location);
        int GetPathPrice(int stationsCount);
    }
}
