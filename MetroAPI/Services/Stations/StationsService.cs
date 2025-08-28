using MetroAPI.Data;
using MetroAPI.Models;
using MetroAPI.Services.Lines;
using Microsoft.EntityFrameworkCore;

namespace MetroAPI.Services.Stations
{
    public class StationsService : IStationsService
    {
        private readonly AppDbContext _context;
        private readonly ILinesService _linesService;

        public StationsService(AppDbContext context, ILinesService linesService)
        {
            _context = context;
            _linesService = linesService;
        }

        public async Task<Station> GetStationAsync(int id)
        {
            if (id is 0)
                throw new ArgumentNullException("Id can't be zero");

            return await _context.Stations.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Station>> GetStationsAsync()
        {
            return _context.Stations.AsNoTracking().AsEnumerable();
        }

        public async Task<IEnumerable<Station>> GetStationsWithLinesAsync()
        {
            return _context.Stations.Include(s => s.Line).AsNoTracking().AsSplitQuery().AsEnumerable();
        }

        public async Task AddStation(Station data)
        {
            await _context.Stations.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStation(Station data)
        {
            _context.Stations.Update(data);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStation(Station data)
        {
            _context.Stations.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetStationLineAsync(string station)
        {
            var stations = await this.GetStationsWithLinesAsync();
            List<int> lines = new List<int>();

            foreach (var item in stations)
            {
                if (item.Name == station)
                {
                    lines.Add(item.Line.LineNo);
                }
            }

            return lines;
        }

        public async Task<List<Station>> GetPathAsync(string fromStation, string toStation)
        {
            int stationsLine, fromStationLine, toStationLine;
            var data = new List<Station>();

            var fromStationLineNo = await this.GetStationLineAsync(fromStation);
            var toStationLineNo = await this.GetStationLineAsync(toStation);

            if (fromStationLineNo.Count() == 1 && toStationLineNo.Count() == 1)
            {
                if (fromStationLineNo.First() == toStationLineNo.First())
                {
                    stationsLine = fromStationLineNo.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.First();

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else
                        return tempFromStation.ToList();
                }
                else
                {
                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.First();

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                }
            }
            else if (fromStationLineNo.Count() > 1 && toStationLineNo.Count() == 1)
            {
                var temp = fromStationLineNo.Intersect(toStationLineNo).ToList();

                if (temp.Any())
                {
                    stationsLine = temp.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.Where(s => s.Line.LineNo == stationsLine).First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.First();

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.Where(s => s.Line.LineNo == fromStationLine).First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.First();

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                }
            }
            else if (fromStationLineNo.Count() == 1 && toStationLineNo.Count() > 1)
            {
                var temp = toStationLineNo.Intersect(fromStationLineNo).ToList();
                if (temp.Any())
                {
                    stationsLine = temp.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.Where(s => s.Line.LineNo == stationsLine).First();

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.Where(s => s.Line.LineNo == toStationLine).First();

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 == 0)
                        {
                            return data;
                        }
                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 == 0)
                        {
                            return data;
                        }
                        return data;
                    }
                }
            }
            else if (fromStationLineNo.Count() > 1 && toStationLineNo.Count() > 1)
            {
                var temp = toStationLineNo.Intersect(fromStationLineNo).ToList();
                if (temp.Count() >= 1)
                {
                    stationsLine = temp.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.Where(s => s.Line.LineNo == stationsLine).First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.Where(s => s.Line.LineNo == stationsLine).First();

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var tempFromStation = await this.FindStationByNameAsync(fromStation);
                    if (tempFromStation is null)
                        return null;

                    var FromStation = tempFromStation.Where(s => s.Line.LineNo == fromStationLine).First();

                    var tempToStation = await this.FindStationByNameAsync(toStation);
                    if (tempToStation is null)
                        return null;

                    var ToStation = tempToStation.Where(s => s.Line.LineNo == toStationLine).First();

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO).ToList();

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 == 0)
                        {
                            return data;
                        }
                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO).ToList();

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO).ToList();

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO).ToList();

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 == 0)
                        {
                            return data;
                        }
                        return data;
                    }
                }
            }
            return null;
        }

        public async Task<double> GetDistanceAsync(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371;
            var dLat = await this.Deg2RadAsync(lat2 - lat1);
            var dLon = await this.Deg2RadAsync(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(await this.Deg2RadAsync(lat1)) * Math.Cos(await this.Deg2RadAsync(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;
            return distance;
        }

        public async Task<double> Deg2RadAsync(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public async Task<List<Station>> GetSharedStationsAsync(int line1, int line2)
        {
            var data = new List<Station>();
            var FirstLineStations = await _linesService.GetLineStationsAsync(line1);
            var SecondLineStations = await _linesService.GetLineStationsAsync(line2);

            FirstLineStations = FirstLineStations.Where(s => s.isShared && s.SharedWith == line2).ToList();
            data.AddRange(FirstLineStations);

            SecondLineStations = SecondLineStations.Where(s => s.isShared && s.SharedWith == line1).ToList();
            data.AddRange(SecondLineStations);

            return data;
        }

        public async Task<IQueryable<Station>> FindStationByNameAsync(string station)
        {
            var Station = _context.Stations.Include(l => l.Line).Where(s => s.Name.ToLower() == station.ToLower()).AsNoTracking().AsSplitQuery();

            return await Station.AnyAsync() ? Station : Enumerable.Empty<Station>().AsQueryable();
        }
    }
}