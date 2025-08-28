using FakeItEasy;
using MetroAPI.Models;
using MetroAPI.Services.Lines;
using MetroAPI.Services.Stations;
using Microsoft.EntityFrameworkCore;

namespace MetroAPI.Tests
{
    public class StationsServiceTests
    {
        [Fact]
        public async Task GetStationAsync_WhenIdIsZero_ThrowsArgumentNullException()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

            //Act
            Func<int, Task<Station>> func = async (f) => await sut.GetStationAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task AddStation_WhenAddValidStation_AddStationSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var station = new Station()
            {
                Name = "Station"
            };
            var sut = new StationsService(context, null);

            //Act
            await sut.AddStation(station);

            //Assert
            Assert.True(station.Id > 0);
        }

        [Fact]
        public async Task UpdateStation_WhenUpdateValidStation_UpdateStationSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var station = new Station()
            {
                Name = "Station"
            };

            var sut = new StationsService(context, null);

            await context.Stations.AddAsync(station);
            await context.SaveChangesAsync();

            context.Entry(station).State = EntityState.Detached;

            var newStation = new Station()
            {
                Id = station.Id,
                Name = "Updated name"
            };

            //Act
            await sut.UpdateStation(newStation);

            //Assert
            var updatedStation = await context.Stations.FindAsync(station.Id);

            Assert.Equal("Updated name", updatedStation.Name);
        }

        [Fact]
        public async Task DeleteStation_WhenDeleteValidStation_DeleteStationSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var station = new Station()
            {
                Name = "Station"
            };
            var sut = new StationsService(context, null);

            await context.Stations.AddAsync(station);
            await context.SaveChangesAsync();

            //Act
            await sut.DeleteStation(station);

            //Assert
            Assert.Null(context.Stations.Find(station.Id));
        }

        [Fact]
        public async Task GetStationLineAsync_WhenInvalidStation_ReturnNoData()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

            //Act
            var result = await sut.GetStationLineAsync("Test");

            //Assert
            Assert.False(result.Any());
        }

        [Fact]
        public async Task GetDistanceAsync_WhenValidInputs_ReturnDistance()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

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
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

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
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

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
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

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
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

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
            var context = new InMemoryDbContext();

            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(new List<Station>()
            {
                new Station {Name = "Station 1",
                isShared = true,
                SharedWith = 2,
                LineId = 1 },
                new Station {  Name = "Station 2",
                isShared = true,
                SharedWith = 1,
                LineId = 2}
            });

            var sut = new StationsService(context, lineService);

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
            var context = new InMemoryDbContext();

            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(new List<Station>()
            {
                new Station {Name = "Station 1",
                isShared = false,
                LineId = 1 },
                new Station {  Name = "Station 2",
                isShared = false,
                LineId = 2}
            });

            var sut = new StationsService(context, lineService);

            //Act
            var result = await sut.GetSharedStationsAsync(1, 2);

            //Assert
            Assert.False(result.Any());
        }

        [Fact]
        public async Task FindStationByNameAsync_WhenThereIsNoStations_ReturnNoData()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new StationsService(context, null);

            //Act
            var result = await sut.FindStationByNameAsync("Test");

            //Assert
            Assert.False(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInTheSameLineAndTheFromStationIsBeforeTheToStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };
            context.Lines.Add(line);
            context.SaveChanges();

            for (int i = 1; i <= 5; i++)
            {
                var station = new Station()
                {
                    Name = "Station " + i,
                    StationNO = i,
                    LineId = line.Id
                };
                context.Stations.Add(station);
            }
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(new List<Station>()
             {
                 new Station {  Name = "Test 0",
                StationNO=9,
                LineId = 1},
                new Station
                {
                    Name = "Station 1",
                    StationNO = 1,
                    LineId = line.Id
                },
                new Station
                {
                    Name = "Station 2",
                    StationNO = 2,
                    LineId = line.Id
                },
                 new Station
                 {
                     Name = "Station 3",
                     StationNO = 3,
                     LineId = line.Id
                 },
                  new Station
                  {
                      Name = "Station 4",
                      StationNO = 4,
                       LineId = line.Id
                  },
                  new Station
                  {
                      Name = "Station 5",
                      StationNO = 5,
                      LineId = line.Id
                  }
            });

            var sut = new StationsService(context, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 1", "Station 5");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInTheSameLineAndTheFromStationIsAfterTheToStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };
            context.Lines.Add(line);
            context.SaveChanges();

            for (int i = 1; i <= 5; i++)
            {
                var station = new Station()
                {
                    Name = "Station " + i,
                    StationNO = i,
                    LineId = line.Id
                };
                context.Stations.Add(station);
            }
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(new List<Station>()
             {
                 new Station {  Name = "Test 0",
                StationNO=9,
                LineId = 1},
                new Station
                {
                    Name = "Station 1",
                    StationNO = 1,
                    LineId = line.Id
                },
                new Station
                {
                    Name = "Station 2",
                    StationNO = 2,
                    LineId = line.Id
                },
                 new Station
                 {
                     Name = "Station 3",
                     StationNO = 3,
                     LineId = line.Id
                 },
                  new Station
                  {
                      Name = "Station 4",
                      StationNO = 4,
                       LineId = line.Id
                  },
                  new Station
                  {
                      Name = "Station 5",
                      StationNO = 5,
                      LineId = line.Id
                  }
            });

            var sut = new StationsService(context, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 4", "Station 2");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheFromStationIsTheToStation_ReturnFromStation()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line = new Line
            {
                Name = "Line 1",
                LineNo = 1,
            };
            context.Lines.Add(line);
            context.SaveChanges();

            for (int i = 1; i <= 5; i++)
            {
                var station = new Station()
                {
                    Name = "Station " + i,
                    StationNO = i,
                    LineId = line.Id
                };
                context.Stations.Add(station);
            }
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();
            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.Ignored)).Returns(new List<Station>()
             {
                 new Station {  Name = "Test 0",
                StationNO=9,
                LineId = 1},
                new Station
                {
                    Name = "Station 1",
                    StationNO = 1,
                    LineId = line.Id
                },
                new Station
                {
                    Name = "Station 2",
                    StationNO = 2,
                    LineId = line.Id
                },
                 new Station
                 {
                     Name = "Station 3",
                     StationNO = 3,
                     LineId = line.Id
                 },
                  new Station
                  {
                      Name = "Station 4",
                      StationNO = 4,
                       LineId = line.Id
                  },
                  new Station
                  {
                      Name = "Station 5",
                      StationNO = 5,
                      LineId = line.Id
                  }
            });

            var sut = new StationsService(context, lineService);

            //Act
            var result = await sut.GetPathAsync("Station 2", "Station 2");

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsBeforeTheSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Station 9");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 2");
            Assert.Contains(result, s => s.Name == "Station 9");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsBeforeTheSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Station 7");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 2");
            Assert.Contains(result, s => s.Name == "Station 7");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsAfterTheSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 4", "Station 9");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 4");
            Assert.Contains(result, s => s.Name == "Station 9");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsAfterTheSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 4", "Station 7");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 4");
            Assert.Contains(result, s => s.Name == "Station 7");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared", "Station 9");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 9");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            context.Lines.AddRange(line1, line2);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 4", StationNO = 4, LineId = line1.Id },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 6", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 7", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Station 9", StationNO = 4, LineId = line2.Id },
            new Station { Name = "Station 10", StationNO = 5, LineId = line2.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared", "Station 7");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 7");
            Assert.Contains(result, s => s.Name == "Shared");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsAfterSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(3)))
                .Returns(Task.FromResult(lineThreeStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Station 17");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 17");
            Assert.Contains(result, s => s.Name == "Shared12");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromStationIsSharedStationAndTheToStationIsBeforeSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(3)))
                .Returns(Task.FromResult(lineThreeStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Station 14");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 14");
            Assert.Contains(result, s => s.Name == "Shared12");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsBeforeSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Shared12");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 2");
            Assert.Contains(result, s => s.Name == "Shared12");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsAfterSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 5", "Shared12");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 5");
            Assert.Contains(result, s => s.Name == "Shared12");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsBeforeTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 2", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 2");
            Assert.Contains(result, s => s.Name == "Shared23");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheToStationIsSharedStationAndTheFromStationIsAfterTheSharedStation_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(2)))
                .Returns(Task.FromResult(lineTwoStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Station 5", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Station 5");
            Assert.Contains(result, s => s.Name == "Shared23");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromAndToStationIsSharedStationsAndThereIsIntersect_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            context.Lines.AddRange(line1, line2, line3);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Station 17", StationNO = 5, LineId = line3.Id },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(3)))
                .Returns(Task.FromResult(lineThreeStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(3)))
                .Returns(Task.FromResult(lineThreeStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared13", "Shared23");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Shared13");
            Assert.Contains(result, s => s.Name == "Shared23");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInDifferentLinesAndTheFromAndToStationIsSharedStationsAndThereIsNoIntersect_ReturnPath()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var line1 = new Line { Name = "Line 1", LineNo = 1 };
            var line2 = new Line { Name = "Line 2", LineNo = 2 };
            var line3 = new Line { Name = "Line 3", LineNo = 3 };
            var line4 = new Line { Name = "Line 4", LineNo = 4 };
            context.Lines.AddRange(line1, line2, line3, line4);
            context.SaveChanges();

            var lineOneStations = new List<Station>
            {
            new Station { Name = "Station 1", StationNO = 1, LineId = line1.Id },
            new Station { Name = "Station 2", StationNO = 2, LineId = line1.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line1.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared13", StationNO = 4, LineId = line1.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 5", StationNO = 5, LineId = line1.Id },
            new Station { Name = "Station 6", StationNO = 6, LineId = line1.Id }
            };

            var lineTwoStations = new List<Station>
            {
            new Station { Name = "Station 7", StationNO = 1, LineId = line2.Id },
            new Station { Name = "Station 8", StationNO = 2, LineId = line2.Id },
            new Station { Name = "Shared12", StationNO = 3, LineId = line2.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line2.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 11", StationNO = 5, LineId = line2.Id },
            new Station { Name = "Station 12", StationNO = 6, LineId = line2.Id }
            };

            var lineThreeStations = new List<Station>
            {
            new Station { Name = "Station 13", StationNO = 1, LineId = line3.Id },
            new Station { Name = "Station 14", StationNO = 2, LineId = line3.Id },
            new Station { Name = "Shared13", StationNO = 3, LineId = line3.Id, isShared = true, SharedWith = 1 },
            new Station { Name = "Shared23", StationNO = 4, LineId = line3.Id, isShared = true, SharedWith = 2 },
            new Station { Name = "Shared34", StationNO = 5, LineId = line3.Id, isShared = true, SharedWith = 4 },
            new Station { Name = "Station 18", StationNO = 6, LineId = line3.Id },
            new Station { Name = "Station 19", StationNO = 7, LineId = line3.Id }
            };

            var lineFourStations = new List<Station>
            {
            new Station { Name = "Station 19", StationNO = 1, LineId = line4.Id },
            new Station { Name = "Station 20", StationNO = 2, LineId = line4.Id },
            new Station { Name = "Shared34", StationNO = 3, LineId = line4.Id, isShared = true, SharedWith = 3 },
            new Station { Name = "Station 22", StationNO = 4, LineId = line4.Id },
            new Station { Name = "Station 23", StationNO = 5, LineId = line4.Id }
            };

            context.Stations.AddRange(lineOneStations);
            context.Stations.AddRange(lineTwoStations);
            context.Stations.AddRange(lineThreeStations);
            context.Stations.AddRange(lineFourStations);
            context.SaveChanges();

            var lineService = A.Fake<ILinesService>();

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(1)))
                .Returns(Task.FromResult(lineOneStations));

            A.CallTo(() => lineService.GetLineStationsAsync(A<int>.That.IsEqualTo(3)))
                .Returns(Task.FromResult(lineThreeStations));

            var sut = new StationsService(context, lineService);

            // Act
            var result = await sut.GetPathAsync("Shared12", "Shared34");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, s => s.Name == "Shared12");
            Assert.Contains(result, s => s.Name == "Shared34");
        }

        [Fact]
        public async Task GetPathAsync_WhenTheStationsIsInCorrect_ReturnNull()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var sut = new StationsService(context, null);

            // Act
            var result = await sut.GetPathAsync("Test 1", "Test 2");

            // Assert
            Assert.Null(result);
        }
    }
}