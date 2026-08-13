using MetroAPI.DTOs;
using MetroAPI.DTOS;
using MetroAPI.Helpers;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using MetroAPI.Services.Interfaces;

namespace MetroAPI.Services
{
    public class StationsService : IStationsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILinesService _linesService;

        public StationsService(IUnitOfWork unitOfWork, ILinesService linesService)
        {
            _unitOfWork = unitOfWork;
            _linesService = linesService;
        }

        public async Task<Station> GetStationAsync(int id)
        {
            return id is 0 ? throw new ArgumentNullException("Id can't be zero") :
                await _unitOfWork.Stations.Get(id);
        }

        public async Task<IEnumerable<Station>> GetStationsAsync()
        {
            return await _unitOfWork.Stations.GetAll();
        }

        public IEnumerable<Station> GetStationsWithLines()
        {
            return _unitOfWork.Stations.GetStationsWithLines();
        }

        public async Task<Result> AddStation(Station data)
        {
            var result = await _unitOfWork.Stations.Add(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while adding." };
        }

        public async Task<Result> UpdateStation(Station data)
        {
            var result = _unitOfWork.Stations.Update(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while updating." };
        }

        public async Task<Result> DeleteStation(Station data)
        {
            var result = _unitOfWork.Stations.Delete(data);

            await _unitOfWork.Commit();

            return result is not null ? new Result { Succed = true } :
                new Result { Succed = false, Error = "An error occured while deleting." };
        }

        public async Task<IEnumerable<int>> GetStationLineAsync(string station)
        {
            var stations = _unitOfWork.Stations.GetStationsWithLines();

            var lines = new List<int>();

            foreach (var item in stations)
            {
                if (item.Name == station)
                    lines.Add(item.Line.LineNo);
            }

            return lines.Count > 1 ? lines.Distinct().ToList() : lines;
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

                    var FromStation = this.FindStationByNameAsync(fromStation, stationsLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, stationsLine);
                    if (ToStation is null)
                        return null;

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else
                        return Enumerable.Empty<Station>().ToList();
                }
                else
                {
                    var FromStation = this.FindStationByNameAsync(fromStation, fromStationLineNo.First());
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, toStationLineNo.First());
                    if (ToStation is null)
                        return null;

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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

                    var FromStation = this.FindStationByNameAsync(fromStation, stationsLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, stationsLine);
                    if (ToStation is null)
                        return null;

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var FromStation = this.FindStationByNameAsync(fromStation, fromStationLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, toStationLine);
                    if (ToStation is null)
                        return null;

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

                            var takenStations2 = Math.Abs((ToStation.StationNO - sharedStationOfSecondLine.StationNO)) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }

                        return data;
                    }
                    else if (pathLength1 > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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

                    var FromStation = this.FindStationByNameAsync(fromStation, stationsLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, stationsLine);
                    if (ToStation is null)
                        return null;

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var FromStation = this.FindStationByNameAsync(fromStation, fromStationLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, toStationLine);
                    if (ToStation is null)
                        return null;

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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

                    var FromStation = this.FindStationByNameAsync(fromStation, stationsLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, stationsLine);
                    if (ToStation is null)
                        return null;

                    var pathLength = FromStation.StationNO - ToStation.StationNO;

                    if (pathLength < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (ToStation.StationNO - FromStation.StationNO) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                    else if (pathLength > 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(stationsLine);
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((ToStation.StationNO - FromStation.StationNO)) + 1;

                        return allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                    }
                }
                else
                {
                    fromStationLine = fromStationLineNo.First();
                    toStationLine = toStationLineNo.First();

                    var FromStation = this.FindStationByNameAsync(fromStation, fromStationLine);
                    if (FromStation is null)
                        return null;

                    var ToStation = this.FindStationByNameAsync(toStation, toStationLine);
                    if (ToStation is null)
                        return null;

                    var sharedStations = await this.GetSharedStationsAsync(FromStation.Line.LineNo, ToStation.Line.LineNo);
                    var sharedStationOfFirstLine = sharedStations.Where(s => s.LineId == FromStation.LineId).First();
                    var sharedStationOfSecondLine = sharedStations.Where(s => s.Name == sharedStationOfFirstLine.Name && s.LineId == ToStation.LineId).First();

                    var pathLength1 = FromStation.StationNO - sharedStationOfFirstLine.StationNO;

                    if (pathLength1 < 0)
                    {
                        var allLineStations = await _linesService.GetLineStationsAsync(FromStation.Line.LineNo);
                        allLineStations = allLineStations.OrderBy(s => s.StationNO);

                        var takenStations = (sharedStationOfFirstLine.StationNO - FromStation.StationNO) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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
                        allLineStations = allLineStations.OrderByDescending(s => s.StationNO);

                        var takenStations = Math.Abs((sharedStationOfFirstLine.StationNO - FromStation.StationNO)) + 1;

                        var firstStations = allLineStations.SkipWhile(s => s.Name != FromStation.Name).Take(takenStations).ToList();
                        data.AddRange(firstStations);

                        var pathLength2 = sharedStationOfSecondLine.StationNO - ToStation.StationNO;
                        if (pathLength2 < 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderBy(s => s.StationNO);

                            var takenStations2 = (ToStation.StationNO - sharedStationOfSecondLine.StationNO) + 1;

                            var secondStations = allSecondLineStations.SkipWhile(s => s.Name != sharedStationOfSecondLine.Name).Take(takenStations2).ToList();
                            data.AddRange(secondStations);
                        }
                        else if (pathLength2 > 0)
                        {
                            var allSecondLineStations = await _linesService.GetLineStationsAsync(ToStation.Line.LineNo);
                            allSecondLineStations = allSecondLineStations.OrderByDescending(s => s.StationNO);

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

            FirstLineStations = FirstLineStations.Where(s => s.isShared && s.SharedWith == line2);
            data.AddRange(FirstLineStations);

            SecondLineStations = SecondLineStations.Where(s => s.isShared && s.SharedWith == line1);
            data.AddRange(SecondLineStations);

            return data;
        }

        public Station FindStationByNameAsync(string station, int lineNo)
        {
            return _unitOfWork.Stations.GetStationByName(station, lineNo);
        }

        public async Task<NearestStationDTO> GetNearestStation(LocationDTO location)
        {
            var stations = await _unitOfWork.Stations.GetAll();

            var nearestStation = stations
                .Select(async station => new
                {
                    Station = station,
                    Distance = await this.GetDistanceAsync(location.Latitude, location.Longitude, station.Latitude, station.Longitude)
                })
                .Select(task => task.Result)
                .OrderBy(x => x.Distance)
                .FirstOrDefault();

            return new NearestStationDTO
            {
                StationName = nearestStation.Station.Name,
                Distance = nearestStation.Distance
            };
        }

        public int GetPathPrice(int stationsCount)
        {
            int price;

            if (stationsCount <= 9)
                price = 8;
            else if (stationsCount <= 16)
                price = 10;
            else if (stationsCount <= 23)
                price = 15;
            else
                price = 20;

            return price;
        }
    }
}