using FakeItEasy;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using MetroAPI.Services;
using MetroAPI.Services.Interfaces;

namespace MertoAPITests
{
    public class StationsServiceTests
    {
        [Fact]
        public async Task GetStationAsync_WhenIdIsZero_ThrowsArgumentNullException()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            Func<int, Task<Station>> func = async (f) => await sut.GetStationAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task GetStationAsync_WhenIdIsValidNumberAndThereIsNoLines_ReturnNull()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Get(A<int>.Ignored))
                .Returns(Task.FromResult<Station>(null));

            //Act
            var result = await sut.GetStationAsync(10);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStationAsync_WhenIdIsValidNumberAndThereIsLine_ReturnStation()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Get(A<int>.Ignored))
                .Returns(new Station() { Name = "Test" });

            //Act
            var result = await sut.GetStationAsync(10);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetStationsAsync_WhenThereIsNoStations_ReturnEmptyEnumerable()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.GetStationsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetStationsAsync_WhenThereIsStations_ReturnStations()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetAll())
                .Returns(new List<Station> { new Station() { Id = 1 }, new Station() { Name = "Test" } });

            //Act
            var result = await sut.GetStationsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Test", result.Last().Name);
        }

        [Fact]
        public async Task GetStationsWithLines_WhenThereIsNoStations_ReturnEmptyEnumerable()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = sut.GetStationsWithLines();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetStationsWithLines_WhenThereIsStations_ReturnStations()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(new List<Station> { new Station() { Id = 1 }, new Station() { Name = "Test" } });

            //Act
            var result = sut.GetStationsWithLines();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Test", result.Last().Name);
        }

        [Fact]
        public async Task AddStation_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Add(A<Station>.Ignored))
                .Returns(Task.FromResult<Station>(null));

            //Act
            var result = await sut.AddStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task AddStation_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Add(A<Station>.Ignored))
                .Returns(new Station());

            //Act
            var result = await sut.AddStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }

        [Fact]
        public async Task UpdateStation_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Update(A<Station>.Ignored))
                .Returns(null);

            //Act
            var result = await sut.UpdateStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task UpdateStation_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Update(A<Station>.Ignored))
                .Returns(new Station());

            //Act
            var result = await sut.UpdateStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }

        [Fact]
        public async Task DeleteStation_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Delete(A<Station>.Ignored))
                .Returns(null);

            //Act
            var result = await sut.DeleteStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while deleting.", result.Error);
        }

        [Fact]
        public async Task DeleteStation_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.Delete(A<Station>.Ignored))
                .Returns(new Station());

            //Act
            var result = await sut.DeleteStation(new Station());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }

        [Fact]
        public async Task GetStationLineAsync_WhenthereIsNoStations_ReturnEmptyList()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(Enumerable.Empty<Station>());

            //Act
            var result = await sut.GetStationLineAsync("Test");

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Any());
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetStationLineAsync_WhenthereIsStations_ReturnLinesList()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(new List<Station> { new Station(), new Station() });

            //Act
            var result = await sut.GetStationLineAsync("Test");

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetDistanceAsync_WhenValidInputs_ReturnDistance()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.GetDistanceAsync(30.12, 29.20, 31.118, 30.0157);
            var expected = 135.6731911427491;

            //Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task GetDistanceAsync_WhenInputsIsZero_ReturnZero()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.GetDistanceAsync(0, 0, 0, 0);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result, 0);
        }

        [Fact]
        public async Task GetDistanceAsync_WhenInputsAreEqual_ReturnZero()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.GetDistanceAsync(30.12, 29.20, 30.12, 29.20);
            var expected = 0;

            //Assert
            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task Deg2RadAsync_WhenValidInput_ReturnDeg2Rad()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.Deg2RadAsync(50.16);
            var expected = 0.87545715280035563;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task Deg2RadAsync_WhenInputIsZero_ReturnZero()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            //Act
            var result = await sut.Deg2RadAsync(0);
            var expected = 0;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task GetSharedStationsAsync_WhenThereIsSharedStations_ReturnSharedStations()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();

            var lineService = A.Fake<ILinesService>();

            var stations = new List<Station>()
                {
                new Station
                {
                    Name = "Station 1",
                    isShared = true,
                    SharedWith = 2,
                    LineId = 1
                },
                new Station
                {
                    Name = "Station 2",
                    isShared = true,
                    SharedWith = 1,
                    LineId = 2
                }
                }.AsQueryable();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored))
                .Returns(stations);

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetSharedStationsAsync(1, 2);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetSharedStationsAsync_WhenThereIsNoSharedStations_ReturnNoData()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();

            var lineService = A.Fake<ILinesService>();

            var stations = new List<Station>()
                {
                  new Station {Name = "Station 1",
                  isShared = false,
                  LineId = 1 },
                  new Station {  Name = "Station 2",
                  isShared = false,
                  LineId = 2}
                }.AsQueryable();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored))
                .Returns(stations);

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetSharedStationsAsync(1, 2);

            //Assert
            Assert.False(result.Any());
        }

        [Fact]
        public async Task FindStationByNameAsync_WhenThereIsNoStations_ReturnNoData()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.Ignored, A<int>.Ignored))
                .Returns(null);

            //Act
            var result = sut.FindStationByNameAsync("Test", 1);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindStationByNameAsync_WhenThereIsStations_ReturnStation()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new StationsService(uow, null);

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.Ignored, A<int>.Ignored))
             .Returns(new Station());

            //Act
            var result = sut.FindStationByNameAsync("Test", 1);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInTheSameLineAndTheFromStationIsBeforeTheToStation_ReturnPath()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var lineService = A.Fake<ILinesService>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations);

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 1"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.First());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 3"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.Last());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored))
                .Returns(lineOneStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 1", "Station 3");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInTheSameLineAndTheFromStationIsAfterTheToStation_ReturnPath()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var lineService = A.Fake<ILinesService>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations);

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 3"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.Last());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 1"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.First());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored))
                .Returns(lineOneStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 3", "Station 1");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheFromStationIsTheToStation_ReturnFromStation()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var lineService = A.Fake<ILinesService>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations);

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 3"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.Last());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 3"), A<int>.That.IsEqualTo<int>(1)))
               .Returns(lineOneStations.Last());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored))
                .Returns(lineOneStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 3", "Station 3");

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsBeforeTheSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 1"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.First());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 5"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.Last());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 1", "Station 5");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsBeforeTheSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 1"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.First());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 4"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.First());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 1", "Station 4");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsAfterTheSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 3"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.Last());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 5"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.Last());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 3", "Station 5");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsAfterTheSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 1"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.Last());

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 4"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.First());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 1", "Station 4");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(3, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 2"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(1));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 5"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.Last());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 2", "Station 5");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(2, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var lineOne = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };

            var linetwo = new Line
            {
                Name = "Line 2",
                LineNo = 2,
            };

            var lineOneStations = new List<Station>()
            {
                new Station { Name = "Station 1", Line = lineOne, StationNO = 1 },
                new Station { Name = "Station 2", Line = lineOne, isShared = true, SharedWith = 2, StationNO = 2 },
                new Station { Name = "Station 3", Line = lineOne, StationNO = 3  }
            };

            var lineTwoStations = new List<Station>()
            {
                new Station { Name = "Station 4", Line = linetwo, StationNO = 1 },
                new Station { Name = "Station 2", Line = linetwo, isShared = true, SharedWith = 1, StationNO = 2 },
                new Station { Name = "Station 5", Line = linetwo , StationNO = 3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 2"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(1));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 4"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.First());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 2", "Station 4");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(2, result.DistinctBy(s => s.Name).Count());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsAfterSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared12"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(2));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 17"), A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.ElementAt(4));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Station 17");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsBeforeSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared12"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(2));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 14"), A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.ElementAt(1));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Station 14");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsBeforeSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 2"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(1));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared12"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(2));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Shared12");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsAfterSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 5"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(4));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared12"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(2));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 5", "Shared12");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 2"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(1));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared23"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(3));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Station 5"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(4));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared23"), A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.ElementAt(3));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(2)))
                .Returns(lineTwoStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 5", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromAndToStationIsSharedStationsAndThereIsIntersect_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, Line = line3 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared13"), A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.ElementAt(2));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared23"), A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.ElementAt(3));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared13", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromAndToStationIsSharedStationsAndThereIsNoIntersect_ReturnPath()
        {
            //Arrange
            var lineService = A.Fake<ILinesService>();
            var uow = A.Fake<IUnitOfWork>();


            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            var line4 = new Line { Name = "Line 4", LineNo = 4 };

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, Line = line1 },
            new Station { Name = "Station 2", StationNO = 2, Line = line1 },
            new Station { Name = "Shared12", StationNO = 3, Line = line1, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, Line = line1, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, Line = line1 },
            new Station { Name = "Station 6", StationNO = 6, Line = line1 }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, Line = line2 },
            new Station { Name = "Station 8", StationNO = 2, Line = line2 },
            new Station { Name = "Shared12", StationNO = 3, Line = line2, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line2, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, Line = line2 },
            new Station { Name = "Station 12", StationNO = 6, Line = line2 }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, Line = line3 },
            new Station { Name = "Station 14", StationNO = 2, Line = line3 },
            new Station { Name = "Shared13", StationNO = 3, Line = line3, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, Line = line3, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared34", StationNO = 5, Line = line3, isShared = true, SharedWith = 4 },
            new Station { Name = "Station 18", StationNO = 6, Line = line3 },
            new Station { Name = "Station 19", StationNO = 7, Line = line3 }
            };

            var lineFourStations = new List<Station>
            {
            new Station { Name = "Station 19", StationNO = 1, Line = line4 },
            new Station { Name = "Station 20", StationNO = 2, Line = line4 },
            new Station { Name = "Shared34", StationNO = 3, Line = line4, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 22", StationNO = 4, Line = line4 },
            new Station { Name = "Station 23", StationNO = 5, Line = line4 }
            };

            A.CallTo(() => uow.Stations.GetStationsWithLines())
                .Returns(lineOneStations.Concat(lineTwoStations.Concat(lineThreeStations).Concat(lineFourStations)));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared12"), A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.ElementAt(2));

            A.CallTo(() => uow.Stations.GetStationByName(A<string>.That.IsEqualTo<string>("Shared34"), A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.ElementAt(4));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(1)))
                .Returns(lineOneStations.AsQueryable());

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo<int>(3)))
                .Returns(lineThreeStations.AsQueryable());

            var sut = new StationsService(uow, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Shared34");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInCorrect_ReturnNull()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();

            var sut = new StationsService(uow, null);

            // Act
            var result = await sut.GetPathAsync("Test 1", "Test 2");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPathPrice_WhenStationsCountIsZero_ReturnEight()
        {
            //arrange
            var sut = new StationsService(null, null);

            //act
            var result = sut.GetPathPrice(0);

            //assert
            Assert.NotNull(result);
            Assert.Equal(8, result);
        }

        [Fact]
        public void GetPathPrice_WhenStationsCountIsTen_ReturnTen()
        {
            //arrange
            var sut = new StationsService(null, null);

            //act
            var result = sut.GetPathPrice(10);

            //assert
            Assert.NotNull(result);
            Assert.Equal(10, result);
        }

        [Fact]
        public void GetPathPrice_WhenStationsCountIsTwenty_ReturnFifteen()
        {
            //arrange
            var sut = new StationsService(null, null);

            //act
            var result = sut.GetPathPrice(20);

            //assert
            Assert.NotNull(result);
            Assert.Equal(15, result);
        }

        [Fact]
        public void GetPathPrice_WhenStationsCountIsTwentyFive_ReturnTwenty()
        {
            //arrange
            var sut = new StationsService(null, null);

            //act
            var result = sut.GetPathPrice(25);

            //assert
            Assert.NotNull(result);
            Assert.Equal(20, result);
        }
    }
}