using FakeItEasy;
using MetroAPI.Controllers;
using MetroAPI.Models;
using MetroAPI.Models.DTOS;
using MetroAPI.Services.Lines;
using Microsoft.AspNetCore.Mvc;

namespace MetroAPI.Tests
{
    public class LinesControllerTests
    {
        [Fact]
        public async Task GetAllLines_WhereLinesExists_ReturnLines()
        {
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLinesAsync()).Returns(
                new List<Line>() {
                    new Line{ Name = "Test 1", LineNo = 1 },
                    new Line{ Name = "Test 2", LineNo = 3 },
                    new Line{ Name = "Test 3", LineNo = 3 },
                    new Line{ Name = "Test 4", LineNo = 4 }
                });

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.GetAllLines();
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var data = okResult.Value as List<Line>;
            Assert.NotNull(data);
            Assert.Equal(4, data.Count);
        }

        [Fact]
        public async Task GetLine_WhereIdIsZero_ReturnBadRequest()
        {
            //Arrange
            var sut = new LinesController(null);

            //Act
            var result = await sut.GetLine(0);
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
        public async Task GetLine_WhereTheLineIsNotFound_ReturnNotFound()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored)).Returns(Task.FromResult<Line>(null));

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.GetLine(10);
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
        public async Task GetLine_WhereTheLineIsExist_ReturnLine()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored)).Returns(new Line() { Name = "Test", LineNo = 1 });

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.GetLine(1);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var data = okResult.Value as Line;
            Assert.NotNull(data);
            Assert.Equal(1, data.LineNo);
            Assert.Equal("Test", data.Name);
        }

        [Fact]
        public async Task AddLine_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.AddLine(A<Line>.Ignored));

            var sut = new LinesController(lineService);

            var line = new LineDTO()
            {
                Name = "Test",
                LineNo = 1
            };

            //Act
            var result = await sut.AddLine(line);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Line has been added successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task UpdateLine_WhereTheLineIsNull_ReturnNotFound()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored)).Returns(Task.FromResult<Line>(null));

            var sut = new LinesController(lineService);

            var line = new LineDTO()
            {
                Name = "Test",
                LineNo = 1
            };

            //Act
            var result = await sut.UpdateLine(2, line);
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
        public async Task UpdateLine_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored));

            var sut = new LinesController(lineService);

            var line = new LineDTO()
            {
                Name = "Test",
                LineNo = 1
            };

            //Act
            var result = await sut.UpdateLine(2, line);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Line has been updated successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task DeleteLine_WhereTheLineIsNull_ReturnNotFound()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored)).Returns(Task.FromResult<Line>(null));

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.DeleteLine(2);
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
        public async Task DeleteLine_WhereTheLineIsValid_ReturnOk()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineAsync(A<int>.Ignored));

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.DeleteLine(2);
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            var outputMessage = okResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Line has been deleted successfully!", outputMessage.Message);
        }

        [Fact]
        public async Task GetLineStations_WhereStationsIsNull_ReturnBadRequest()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(Task.FromResult<List<Station>>(null));

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.GetLineStations(2);
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);

            var outputMessage = badRequestResult.Value as OutputMessage;
            Assert.NotNull(outputMessage);
            Assert.Equal("Invalid line!!", outputMessage.Message);
        }

        [Fact]
        public async Task GetLineStations_WhereStationsIsExist_ReturnStations()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();

            var Stations = new List<Station>()
            {
                new Station(){Name ="Test 1" },
                new Station(){Name ="Test 2" },
                new Station(){Name ="Test 3" },
                new Station(){Name ="Test 4" },
            };

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(Stations);

            var sut = new LinesController(lineService);

            //Act
            var result = await sut.GetLineStations(2);
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

            var stationsCount = jsonData.GetProperty("StationsCount").GetInt32();

            Assert.NotNull(stations);
            Assert.Equal(4, stations.Count);
            Assert.Equal(4, stationsCount); 
            Assert.Contains("Test 1", stations); 
            Assert.Contains("Test 2", stations); 
            Assert.Contains("Test 3", stations); 
            Assert.Contains("Test 4", stations); 
        }
    }
}
