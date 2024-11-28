using MetroAPI.Models;
using MetroAPI.Services.Lines;

namespace MetroAPI.Tests
{
    public class LinesServiceTests
    {
        [Fact]
        public void GetLineAsync_WhenIdIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var context = new InMemoryDB();
            var sut = new LinesService(context);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineAsync(0);

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public void GetLineByNoAsync_WhenLineNoIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var context = new InMemoryDB();
            var sut = new LinesService(context);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineByNoAsync(0);

            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public void AddLine_WhenAddValidLine_AddLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDB();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            //Act
            sut.AddLine(line);

            //Assert
            Assert.True(line.Id > 0);
        }

        [Fact]
        public void UpdateLine_WhenUpdateValidLine_UpdateLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDB();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            //Act
            sut.UpdateLine(line);

            //Assert
            Assert.True(line.Id > 0);
        }

        [Fact]
        public void DeleteLine_WhenDeleteValidLine_DeleteLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDB();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            //Act
            sut.DeleteLine(line);

            //Assert
            Assert.True(line.Id > 0);
        }

        [Fact]
        public async Task GetLineStationsAsync_WhenLineAndStationsIsNotNull_ReturnLineStations()
        {
            //Arrange
            var context = new InMemoryDB();

            var sut = new LinesService(context);

            int lineNo = 1;
            var line = new Line
            {
                Name = "TestLine",
                LineNo = lineNo
            };

            var stations = new List<Station>()
            {
            new Station { Name="Station 1", LineId = lineNo },
            new Station { Name="Station 2", LineId = lineNo }
            };

            context.Lines.Add(line);
            context.Stations.AddRange(stations);
            context.SaveChanges();

            //Act
            var result = await sut.GetLineStationsAsync(lineNo);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetLineStationsAsync_WhenLineIsNotNull_ReturnNodata()
        {
            //Arrange
            var context = new InMemoryDB();

            var sut = new LinesService(context);

            int lineNo = 100;

            //Act
            var result = sut.GetLineStationsAsync(lineNo);

            //Assert
            Assert.False(result.Result.Any());
        }
    }
}