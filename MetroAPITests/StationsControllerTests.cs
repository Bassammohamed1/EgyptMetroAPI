using FakeItEasy;
using MetroAPI.Controllers;
using MetroAPI.Models.DTOS;
using MetroAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MetroAPI.Services.Stations;

namespace MetroAPI.Tests
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
        public async Task AddStation_WhereTheStationIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.AddStation(A<Station>.Ignored));

            var sut = new StationsController(stationService);

            var line = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.AddStation(line);
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
        public async Task UpdateStation_WhereTheStationIsNull_ReturnNotFound()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored)).Returns(Task.FromResult<Station>(null));

            var sut = new StationsController(stationService);

            var line = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.UpdateStation(2, line);
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
        public async Task UpdateStation_WhereTheStationIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored));

            var sut = new StationsController(stationService);

            var station = new StationDTO()
            {
                Name = "Test"
            };

            //Act
            var result = await sut.UpdateStation(2, station);
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
        public async Task DeleteStation_WhereTheStationIsNull_ReturnNotFound()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored)).Returns(Task.FromResult<Station>(null));

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
        public async Task DeleteStation_WhereTheStationIsValid_ReturnOk()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationAsync(A<int>.Ignored));

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
            A.CallTo(() => stationService.GetStationLineAsync(A<string>.Ignored)).Returns(Task.FromResult<List<int>>(null));

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

            var output = Convert.ToInt64(okResult.Value);
            Assert.NotNull(output);
            Assert.Equal(1, output);

        }

        [Fact]
        public async Task GetStationLine_WhereLineIsExistAndMultiValid_ReturnLines()
        {
            //Arrange
            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationLineAsync(A<string>.Ignored)).Returns(new List<int> { 1, 2, 2, 3 });

            var sut = new StationsController(stationService);
            //Act
            var result = await sut.GetStationLine("Test");
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var output = okResult.Value as List<int>;
            Assert.NotNull(output);
            Assert.Equal(1, output.ElementAt(0));
            Assert.Equal(2, output.ElementAt(1));
            Assert.Equal(3, output.ElementAt(2));
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
            A.CallTo(() => stationService.GetPathAsync(A<string>.Ignored, A<string>.Ignored)).Returns(path);

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
            var context = new InMemoryDB();

            var stations = new List<Station>()
            {
                new Station{Name = "Test 1" , Latitude = 31.2,Longitude=31.2},
                new Station{Name = "Test 2",Latitude = 33.2,Longitude = 33.2},
                new Station{Name = "Test 3", Latitude = 32.2,Longitude = 32.2},
                new Station{Name = "Test 4", Latitude = 30.2 ,Longitude= 30.2 }
            };

            context.Stations.AddRange(stations);
            context.SaveChanges();

            var stationService = A.Fake<IStationsService>();
            A.CallTo(() => stationService.GetStationsAsync()).Returns(stations);

            A.CallTo(() => stationService.GetDistanceAsync(A<double>.Ignored, A<double>.Ignored, A<double>.Ignored, A<double>.Ignored)).Returns(0);

            var sut = new StationsController(stationService);
            //Act
            var location = new LocationDto
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
