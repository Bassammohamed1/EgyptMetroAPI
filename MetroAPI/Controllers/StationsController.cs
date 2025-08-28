using MetroAPI.Models.DTOS;
using MetroAPI.Models;
using MetroAPI.Services.Stations;
using Microsoft.AspNetCore.Mvc;

namespace MetroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly IStationsService _stationsService;

        public StationsController(IStationsService stationsService)
        {
            _stationsService = stationsService;
        }

        [HttpGet("GetAllStations")]
        public async Task<IActionResult> GetAllStations()
        {
            var data = await _stationsService.GetStationsAsync();
            return Ok(data);
        }

        [HttpGet("GetStation/{id}")]
        public async Task<IActionResult> GetStation(int id)
        {
            if (id == 0 || id == null)
                return BadRequest(new OutputMessage { Message = "Invalid id!!" });
            var data = await _stationsService.GetStationAsync(id);
            if (data is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });
            return Ok(data);
        }

        [HttpPost("AddStation")]
        public async Task<IActionResult> AddStation(StationDTO data)
        {
            var station = new Station()
            {
                Name = data.Name,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                StationNO = data.StationNO,
                isShared = data.isShared,
                SharedWith = data.SharedWith,
                LineId = data.LineId
            };
            await _stationsService.AddStation(station);
            return Ok(new OutputMessage { Message = "Station has been added successfully!" });
        }

        [HttpPut("UpdateStation/{id}")]
        public async Task<IActionResult> UpdateStation(int id, StationDTO data)
        {
            var station = await _stationsService.GetStationAsync(id);
            if (station is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            station.Name = data.Name;
            station.Latitude = data.Latitude;
            station.Longitude = data.Longitude;
            station.StationNO = data.StationNO;
            station.isShared = data.isShared;
            station.SharedWith = data.SharedWith;
            station.LineId = data.LineId;

            await _stationsService.UpdateStation(station);
            return Ok(new OutputMessage { Message = "Station has been updated successfully!" });
        }

        [HttpDelete("DeleteStation/{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var station = await _stationsService.GetStationAsync(id);
            if (station is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            await _stationsService.DeleteStation(station);
            return Ok(new OutputMessage { Message = "Station has been deleted successfully!" });
        }

        [HttpGet("GetStationLine")]
        public async Task<IActionResult> GetStationLine(string station)
        {
            var lines = await _stationsService.GetStationLineAsync(station);

            if (lines is not null)
            {
                if (lines.Count > 1)
                {
                    lines = lines.Distinct().ToList();

                    var Lines = new
                    {
                        Lines = lines
                    };

                    return Ok(Lines);
                }
                else
                {
                    var line = new
                    {
                        Line = lines.First()
                    };

                    return Ok(line);
                }
            }
            else
                return BadRequest(new OutputMessage { Message = $"Station {station} was not found on any line." });
        }

        [HttpGet("GetPathWithTimeAndPrice")]
        public async Task<IActionResult> GetPathWithTimeAndPrice(string From, string To)
        {
            var data = await _stationsService.GetPathAsync(From, To);
            int price;

            if (data is not null)
            {
                if (data.Count <= 9)
                    price = 8;
                else if (data.Count <= 16)
                    price = 10;
                else if (data.Count <= 23)
                    price = 15;
                else
                    price = 20;

                var path = new
                {
                    Stations = data.Select(s => s.Name).Distinct(),
                    PriceInEGP = price,
                    StationsCount = data.Distinct().Count(),
                    TimeInMinutes = data.Distinct().Count() * 3.5
                };

                return Ok(path);
            }
            return BadRequest(new OutputMessage { Message = "An error occurred." });
        }

        [HttpPost("GetNearestStation")]
        public async Task<IActionResult> GetNearestStation([FromBody] LocationDto location)
        {
            var stations = await _stationsService.GetStationsAsync();

            var nearestStation = stations
                .Select(async station => new
                {
                    Station = station,
                    Distance = await _stationsService.GetDistanceAsync(location.Latitude, location.Longitude, station.Latitude, station.Longitude)
                })
                .Select(task => task.Result)
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            var response = new
            {
                name = nearestStation.Station.Name,
                distance = nearestStation.Distance
            };

            return Ok(response);
        }
    }
}