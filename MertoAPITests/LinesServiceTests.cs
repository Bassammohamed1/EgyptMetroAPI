using MetroAPI.Models;
using MetroAPI.Services.Lines;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace MetroAPI.Tests
{
    public class LinesServiceTests
    {
        [Fact]
        public async Task GetLineAsync_WhenIdIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new LinesService(context);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task GetLineByNoAsync_WhenLineNoIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var sut = new LinesService(context);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineByNoAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task AddLine_WhenAddValidLine_AddLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            //Act
            await sut.AddLine(line);

            //Assert
            Assert.True(line.Id > 0);
        }

        [Fact]
        public async Task UpdateLine_WhenUpdateValidLine_UpdateLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            await context.Lines.AddAsync(line);
            await context.SaveChangesAsync();

            context.Entry(line).State = EntityState.Detached;

            var newLine = new Line()
            {
                Id = line.Id,
                Name = "Updated name",
                LineNo = 1
            };

            //Act
            await sut.UpdateLine(newLine);

            //Assert
            var updatedLine = await context.Lines.FindAsync(line.Id);
            Assert.Equal("Updated name", updatedLine.Name);
        }

        [Fact]
        public async Task DeleteLine_WhenDeleteValidLine_DeleteLineSuccessfully()
        {
            //Arrange
            var context = new InMemoryDbContext();
            var line = new Line()
            {
                Name = "Test",
                LineNo = 1
            };
            var sut = new LinesService(context);

            await context.Lines.AddAsync(line);
            await context.SaveChangesAsync();

            //Act
            await sut.DeleteLine(line);

            //Assert
            Assert.Null(context.Lines.Find(line.Id));
        }

        [Fact]
        public async Task GetLineStationsAsync_WhenLineAndStationsIsNotNull_ReturnLineStations()
        {
            //Arrange
            var context = new InMemoryDbContext();

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
        public async Task GetLineStationsAsync_WhenLineIsNotNull_ReturnEmptyEnumerable()
        {
            //Arrange
            var context = new InMemoryDbContext();

            var sut = new LinesService(context);

            int lineNo = 100;

            //Act
            var result = sut.GetLineStationsAsync(lineNo);

            //Assert
            Assert.False(result.Result.Any());
        }
    }
}