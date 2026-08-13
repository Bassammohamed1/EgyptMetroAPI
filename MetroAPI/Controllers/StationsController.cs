using MetroAPI.DTOS;
using MetroAPI.Models;
using MetroAPI.Services.Interfaces;
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

            var result = await _stationsService.AddStation(station);

            return result.Succed ? Ok(new OutputMessage { Message = "Station has been added successfully!" }) :
                BadRequest(new OutputMessage { Message = result.Error });
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

            var result = await _stationsService.UpdateStation(station);

            return result.Succed ? Ok(new OutputMessage { Message = "Station has been updated successfully!" }) :
                BadRequest(new OutputMessage { Message = result.Error });
        }

        [HttpDelete("DeleteStation/{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            var station = await _stationsService.GetStationAsync(id);

            if (station is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            var result = await _stationsService.DeleteStation(station);

            return result.Succed ? Ok(new OutputMessage { Message = "Station has been deleted successfully!" }) :
                BadRequest(new OutputMessage { Message = result.Error });
        }

        [HttpGet("GetStationLine")]
        public async Task<IActionResult> GetStationLine(string station)
        {
            var lines = await _stationsService.GetStationLineAsync(station);

            if (lines.Any())
            {
                if (lines.Count() > 1)
                {
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

            if (data is not null)
            {
                var path = new
                {
                    Stations = data.Select(s => s.Name).Distinct(),
                    PriceInEGP = _stationsService.GetPathPrice(data.Count()),
                    StationsCount = data.Distinct().Count(),
                    TimeInMinutes = data.Distinct().Count() * 3.5
                };

                return Ok(path);
            }

            return BadRequest(new OutputMessage { Message = "An error occurred." });
        }

        [HttpPost("GetNearestStation")]
        public async Task<IActionResult> GetNearestStation([FromBody] LocationDTO location)
        {
            var result = await _stationsService.GetNearestStation(location);

            var response = new
            {
                name = result.StationName,
                distance = result.Distance
            };

            return Ok(response);
        }
    }
}