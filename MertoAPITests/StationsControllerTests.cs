using FakeItEasy;
using MetroAPI.Controllers;
using MetroAPI.DTOs;
using MetroAPI.DTOS;
using MetroAPI.Helpers;
using MetroAPI.Models;
using MetroAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MertoAPITests
{
    public class StationsControllerTests
    {
        [Fact]
        public async Task GetAllStations_WhereStationsExists_ReturnStations()
        {
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationsAsync()).Returns(
                new List<Station>() {
                    new Station{ Name = "Test 1"},
                    new Station{ Name = "Test 2" },
                    new Station{ Name = "Test 3"},
                    new Station{ Name = "Test 4" }
                });

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.GetAllStations();
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var data = okResult.Value as List<Station>;
            Assert.NotNull(data);
            Assert.Equal(4, data.Count);
        }

        [Fact]
        public async Task GetStation_WhereIdIsZero_ReturnBadRequest()
        {
            //Arrange
            var sut = new StationsController(null);

            //Act
            var result = await sut.GetStation(0);
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Invalid id!!", outputMessage.Message);
        }

        [Fact]
        public async Task GetStation_WhereTheStationIsNotFound_ReturnNotFound()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored)).Returns(Task.FromResult<Station>(null));

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.GetStation(10);
            var notFoundResult = result as ObjectResult;

            //Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);

            var outputMessage = notFoundResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Invalid id!!", outputMessage.Message);
        }

        [Fact]
        public async Task GetStation_WhereTheStationIsExist_ReturnStation()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored)).Returns(new Station() { Name = "Test" });

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.GetStation(1);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var data = okResult.Value as Station;
            Assert.NotNull(data);
            Assert.Equal("Test", data.Name);
        }

        [Fact]
        public async Task AddStation_WhenThereIsAaErrorWhileAdding_ReturnBadRequest()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.AddStation(A<Station>.Ignored))
                .Returns(new Result { Succed = false, Error = "An error happened." });

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.AddStation(station);
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("An error happened.", outputMessage.Message);
        }

        [Fact]
        public async Task AddStation_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.AddStation(A<Station>.Ignored))
                .Returns(new Result { Succed = true });

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.AddStation(station);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Station has been added successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task UpdateStation_WhereTheLineIsNull_ReturnNotFound()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored))
                .Returns(Task.FromResult<Station>(null));

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.UpdateStation(2, station);
            var notFoundResult = result as ObjectResult;

            //Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);

            var outputMessage = notFoundResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Invalid id!!", outputMessage.Message);
        }

        [Fact]
        public async Task UpdateStation_WhenThereIsAaErrorWhileAdding_ReturnBadRequest()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.UpdateStation(A<Station>.Ignored))
                .Returns(new Result { Succed = false, Error = "An error happened." });

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.UpdateStation(1, station);
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("An error happened.", outputMessage.Message);
        }

        [Fact]
        public async Task UpdateStation_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored))
                .Returns(new Station());

            A.CallTo(() => stationService.UpdateStation(A<Station>.Ignored))
               .Returns(new Result { Succed = true });

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.UpdateStation(1, station);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Station has been updated successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task DeleteStation_WhereTheLineIsNull_ReturnNotFound()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored))
                .Returns(Task.FromResult<Station>(null));

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.DeleteStation(2);
            var notFoundResult = result as ObjectResult;

            //Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);

            var outputMessage = notFoundResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Invalid id!!", outputMessage.Message);
        }

        [Fact]
        public async Task DeleteStation_WhenThereIsAaErrorWhileAdding_ReturnBadRequest()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.DeleteStation(A<Station>.Ignored))
                .Returns(new Result { Succed = false, Error = "An error happened." });

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.DeleteStation(1);
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("An error happened.", outputMessage.Message);
        }

        [Fact]
        public async Task DeleteStation_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored))
                .Returns(new Station());


            A.CallTo(() => stationService.DeleteStation(A<Station>.Ignored))
               .Returns(new Result { Succed = true });

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.DeleteStation(2);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Station has been deleted successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task GetStationLine_WhereLineIsNull_ReturnBadRequest()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetStationLineAsync(A<string>.Ignored))
                .Returns(Enumerable.Empty<int>());

            var sut = new StationsController(stationService);
            //Act
            var result = await sut.GetStationLine("Test");
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal($"Station Test was not found on any line.", outputMessage.Message);
        }

        [Fact]
        public async Task GetStationLine_WhereLineIsExist_ReturnLine()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationLineAsync(A<string>.Ignored)).Returns(new List<int> { 1 });

            var sut = new StationsController(stationService);
            //Act
            var result = await sut.GetStationLine("Test");
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var lineProp = okResult.Value.GetType().GetProperty("Line");
            var line = (int)lineProp.GetValue(okResult.Value);

            Assert.NotNull(line);
            Assert.Equal(1, line);
        }

        [Fact]
        public async Task GetStationLine_WhereLineIsExistAndMultiValid_ReturnLines()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationLineAsync(A<string>.Ignored))
                .Returns(new List<int> { 1, 2, 3 });

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.GetStationLine("Test");
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var linesProp = okResult.Value.GetType().GetProperty("Lines");
            var returnedLines = (List<int>)linesProp.GetValue(okResult.Value);

            Assert.NotNull(returnedLines);
            Assert.Equal(1, returnedLines.ElementAt(0));
            Assert.Equal(2, returnedLines.ElementAt(1));
            Assert.Equal(3, returnedLines.ElementAt(2));
        }

        [Fact]
        public async Task GetPathWithTimeAndPrice_WherePathIsNull_ReturnBadRequest()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetPathAsync(A<string>.Ignored, A<string>.Ignored)).Returns(Task.FromResult<List<Station>>(null));

            var sut = new StationsController(stationService);
            //Act
            var result = await sut.GetPathWithTimeAndPrice("Test 1", "Test 2");
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("An error occurred.", outputMessage.Message);
        }

        [Fact]
        public async Task GetPathWithTimeAndPrice_WherePathIsExist_ReturnPath()
        {
            //Arrange
            var path = new List<Station>()
            {
                new Station{Name = "Test 1"},
                new Station{Name = "Test 2"},
                new Station{Name = "Test 3"},
                new Station{Name = "Test 4"}
            };

            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetPathAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(path);

            A.CallTo(() => stationService.GetPathPrice(A<int>.Ignored))
               .Returns(8);

            var sut = new StationsController(stationService);

            //Act
            var result = await sut.GetPathWithTimeAndPrice("Test 1", "Test 2");
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            var jsonData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

            var stations = jsonData.GetProperty("Stations").EnumerateArray()
                .Select(station => station.GetString())
                .ToList();

            var priceInEGP = jsonData.GetProperty("PriceInEGP").GetDecimal();
            var stationsCount = jsonData.GetProperty("StationsCount").GetInt32();
            var timeInMinutes = jsonData.GetProperty("TimeInMinutes").GetDouble();

            Assert.NotNull(stations);
            Assert.Contains("Test 1", stations);
            Assert.Contains("Test 2", stations);
            Assert.Contains("Test 3", stations);
            Assert.Contains("Test 4", stations);
            Assert.Equal(4, stations.Count);
            Assert.Equal(8, priceInEGP);
            Assert.Equal(4, stationsCount);
            Assert.Equal(14, timeInMinutes);

        }

        [Fact]
        public async Task GetNearestStation_WhenThereIsNearestStation_ReturnNearestStation()
        {
            //Arrange
            var stations = new List<Station>()
            {
                new Station{Name = "Test 1" , Latitude = 31.2,Longitude=31.2},
                new Station{Name = "Test 2",Latitude = 33.2,Longitude = 33.2},
                new Station{Name = "Test 3", Latitude = 32.2,Longitude = 32.2},
                new Station{Name = "Test 4", Latitude = 30.2 ,Longitude= 30.2 }
            };

            var stationService = A.Fake<IStationsService>();

            A.CallTo(() => stationService.GetNearestStation(A<LocationDTO>.Ignored))
                .Returns(new NearestStationDTO { Distance = 0 , StationName = "Test 1"});

            var sut = new StationsController(stationService);

            //Act
            var location = new LocationDTO
            {
                Latitude = 31.2,
                Longitude = 31.2
            };

            var result = await sut.GetNearestStation(location);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            var jsonData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

            var name = jsonData.GetProperty("name").GetString();
            var distance = jsonData.GetProperty("distance").GetDouble();

            Assert.NotNull(name);
            Assert.Equal("Test 1", name);
            Assert.Equal(0, distance);
        }
    }
}
