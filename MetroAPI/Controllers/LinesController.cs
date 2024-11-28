using MetroAPI.Models;
using MetroAPI.Models.DTOS;
using MetroAPI.Services.Lines;
using Microsoft.AspNetCore.Mvc;

namespace MetroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinesController : ControllerBase
    {
        private readonly ILinesService _linesService;
        public LinesController(ILinesService linesService)
        {
            _linesService = linesService;
        }

        [HttpGet("GetAllLines")]
        public async Task<IActionResult> GetAllLines()
        {
            var data = await _linesService.GetLinesAsync();
            return Ok(data);
        }
        [HttpGet("GetLine/{id}")]
        public async Task<IActionResult> GetLine(int id)
        {
            if (id == 0 || id == null)
                return BadRequest(new OutputMessage { Message = "Invalid id!!" });

            var data = await _linesService.GetLineAsync(id);
            if (data is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            return Ok(data);
        }
        [HttpPost("AddLine")]
        public async Task<IActionResult> AddLine(LineDTO data)
        {
            var line = new Line()
            {
                Name = data.Name,
                LineNo = data.LineNo,
            };
            await _linesService.AddLine(line);
            return Ok(new OutputMessage { Message = "Line has been added successfully!" });
        }
        [HttpPut("UpdateLine/{id}")]
        public async Task<IActionResult> UpdateLine(int id, LineDTO data)
        {
            var line = await _linesService.GetLineAsync(id);
            if (line is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            line.Name = data.Name;
            line.LineNo = data.LineNo;

            await _linesService.UpdateLine(line);
            return Ok(new OutputMessage { Message = "Line has been updated successfully!" });
        }
        [HttpDelete("DeleteLine/{id}")]
        public async Task<IActionResult> DeleteLine(int id)
        {
            var line = await _linesService.GetLineAsync(id);

            if (line is null)
                return NotFound(new OutputMessage { Message = "Invalid id!!" });

            await _linesService.DeleteLine(line);
            return Ok(new OutputMessage { Message = "Line has been deleted successfully!" });
        }
        [HttpGet("GetLineStations")]
        public async Task<IActionResult> GetLineStations(int LineNo)
        {
            var stations = await _linesService.GetLineStationsAsync(LineNo);

            if (stations is not null)
            {
                var data = new
                {
                    Stations = stations.Select(s => s.Name),
                    StationsCount = stations.Count()
                };
                return Ok(data);
            }
            else
                return BadRequest(new OutputMessage { Message = "Invalid line!!" });
        }
    }
}