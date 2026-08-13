using FakeItEasy;
using MetroAPI.Models;
using MetroAPI.Repository.Interfaces;
using MetroAPI.Services;

namespace MertoAPITests
{
    public class LinesServiceTests
    {
        [Fact]
        public async Task GetLineAsync_WhenIdIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task GetLineAsync_WhenIdIsValidNumberAndThereIsNoLines_ReturnNull()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Get(A<int>.Ignored))
                .Returns(Task.FromResult<Line>(null));

            //Act
            var result = await sut.GetLineAsync(10);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLineAsync_WhenIdIsValidNumberAndThereIsLine_ReturnLine()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Get(A<int>.Ignored))
                .Returns(new Line() { Name = "Test" });

            //Act
            var result = await sut.GetLineAsync(10);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetLineByNoAsync_WhenIdIsZero_ThrowArgumentNullException()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            //Act
            Func<int, Task<Line>> func = async (f) => await sut.GetLineByNoAsync(0);

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => func(0));
        }

        [Fact]
        public async Task GetLineByNoAsync_WhenLineNoIsValidNumberAndThereIsNoLines_ReturnNull()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.GetByNo(A<int>.Ignored))
                .Returns(Task.FromResult<Line>(null));

            //Act
            var result = await sut.GetLineByNoAsync(10);

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLineByNoAsync_WhenLineNoIsValidNumberAndThereIsNoLines_ReturnLine()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.GetByNo(A<int>.Ignored))
              .Returns(new Line() { Name = "Test" });

            //Act
            var result = await sut.GetLineByNoAsync(10);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetLinesAsync_WhenThereIsNoLines_ReturnEmptyEnumerable()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            //Act
            var result = await sut.GetLinesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetLinesAsync_WhenThereIsLines_ReturnLines()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.GetAll())
                .Returns(new List<Line> { new Line() { Id = 1 }, new Line() { Name = "Test" } });

            //Act
            var result = await sut.GetLinesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal("Test", result.Last().Name);
        }

        [Fact]
        public async Task GetLineStationsAsync_WhenLineIsNull_ReturnEmptyQueryable()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.GetByNo(A<int>.Ignored))
                .Returns(Task.FromResult<Line>(null));

            //Act
            var result = await sut.GetLineStationsAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task GetLineStationsAsync_WhenLineIsNotNull_ReturnLineStations()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.GetByNo(A<int>.Ignored))
                .Returns(new Line());

            A.CallTo(() => uow.Lines.GetLineStations(A<int>.Ignored))
              .Returns(new List<Station>() { new Station(), new Station() }.AsQueryable());

            //Act
            var result = await sut.GetLineStationsAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddLine_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Add(A<Line>.Ignored))
                .Returns(Task.FromResult<Line>(null));

            //Act
            var result = await sut.AddLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while adding.", result.Error);
        }

        [Fact]
        public async Task AddLine_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Add(A<Line>.Ignored))
                .Returns(new Line());

            //Act
            var result = await sut.AddLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }

        [Fact]
        public async Task UpdateLine_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Update(A<Line>.Ignored))
                .Returns(null);

            //Act
            var result = await sut.UpdateLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while updating.", result.Error);
        }

        [Fact]
        public async Task UpdateLine_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Update(A<Line>.Ignored))
                .Returns(new Line());

            //Act
            var result = await sut.UpdateLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }

        [Fact]
        public async Task DeleteLine_WhenThereIsErrorWhileAdding_ReturnError()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Delete(A<Line>.Ignored))
                .Returns(null);

            //Act
            var result = await sut.DeleteLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.Succed);
            Assert.Equal("An error occured while deleting.", result.Error);
        }

        [Fact]
        public async Task DeleteLine_WhenThereIsNoErrorsWhileAdding_ReturnSucceed()
        {
            //Arrange
            var uow = A.Fake<IUnitOfWork>();
            var sut = new LinesService(uow);

            A.CallTo(() => uow.Lines.Delete(A<Line>.Ignored))
                .Returns(new Line());

            //Act
            var result = await sut.DeleteLine(new Line());

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Succed);
        }
    }
}