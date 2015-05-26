namespace BP.Maze.Test
{
    using System.IO;
    using System.Linq;

    using BP.Maze.Entity;
    using BP.Maze.Service;
    using BP.Maze.Service.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class TestMazeService
    {
        private const string SampleMazePath = @"resources\ExampleMaze.txt";
        private const char Wall = 'X';
        private const char EmptySpace = ' ';

        private IMazeManager mazeManager;

        [TestInitialize]
        public void CreateMazeManager()
        {
            this.mazeManager = new MazeManager();
        }

        [TestMethod]
        public void WithMoq_CreateMaze_EmptyContent_MazeNotCreatedException()
        {
            // Arrange
            var mockMazeManager = new Mock<IMazeManager>();
            mockMazeManager.Setup(x => x.CreateMaze(It.IsAny<string>())).Returns(new Maze());

            var mazeService = new MazeService(mockMazeManager.Object);
            string mazeContent = null;

            // Act
            var maze = mazeService.CreateMaze(mazeContent);

            // Assert
            mockMazeManager.Verify(x => x.CreateMaze(null));
        }

        [TestMethod]
        [ExpectedException(typeof(MazeNotCreatedException))]
        public void CreateMaze_EmptyContent_MazeNotCreatedException()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            string mazeContent = null;

            // Act
            var maze = mazeService.CreateMaze(mazeContent);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(MazeNotCreatedException))]
        public void CreateMaze_IllegalCharacterInContent_MazeNotCreatedException()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            const string MazeContent = "I";

            // Act
            var maze = mazeService.CreateMaze(MazeContent);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(StartPointNotExistException))]
        public void CreateMaze_StartPointNotExist_ThrowsStartPointNotExistException()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            const string MazeContent = "X";

            // Act
            var maze = mazeService.CreateMaze(MazeContent);

            // Assert
        }

        [TestMethod]

        public void CreateMaze_ValidContent_MazeCreated()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            var mazeContent = File.ReadAllText(SampleMazePath);

            // Act
            var mazeResult = mazeService.CreateMaze(mazeContent);

            // Assert
            Assert.IsNotNull(mazeResult);
            Assert.IsNotNull(mazeResult.Maze);
        }

        [TestMethod]
        public void CreateMaze_ValidContent_NumberOfWallsCalculatedCorrect()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            var mazeContent = File.ReadAllText(SampleMazePath);

            var wallsCount = mazeContent.Count(x => x == Wall);

            // Act
            var mazeResult = mazeService.CreateMaze(mazeContent);

            // Assert
            Assert.IsNotNull(mazeResult);
            Assert.AreEqual(wallsCount, mazeResult.NumberofWalls);
        }

        [TestMethod]
        public void CreateMaze_ValidContent_NumberOfEmptySpacesCalculatedCorrect()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            var mazeContent = File.ReadAllText(SampleMazePath);

            var emptySpacesCount = mazeContent.Count(x => x == EmptySpace);

            // Act
            var mazeResult = mazeService.CreateMaze(mazeContent);

            // Assert
            Assert.IsNotNull(mazeResult);
            Assert.AreEqual(emptySpacesCount, mazeResult.NumberofEmptySpaces);
        }

        [TestMethod]
        [ExpectedException(typeof(MazeNotFoundException))]
        public void GetMazeObjectByLocation_MazeNotCreated_ThrowsMazeNotFoundException()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            var location = new Location { X = 0, Y = 0 };

            // Act
            mazeService.GetMazeObjectByLocation(location);

            // Assert
        }

        [TestMethod]
        public void GetMazeObjectByLocation_MazeCreated_MazeObjectReturned()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            const string MazeContent = "XS";
            mazeService.CreateMaze(MazeContent);
            var location = new Location { X = 0, Y = 0 };

            // Act
            var mazeObject = mazeService.GetMazeObjectByLocation(location);

            // Assert
            Assert.IsTrue(mazeObject is Wall);
            Assert.AreEqual(location.X, mazeObject.Location.X);
            Assert.AreEqual(location.Y, mazeObject.Location.Y);
        }

        [TestMethod]
        public void CreateMaze_MazeCreated_ExplorerInSameLocationWithStartPoint()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);

            // Act
            var maze = CreateMaze(mazeService);

            var startPoint = maze.MazeItems.OfType<StartPoint>().SingleOrDefault();
            var explorer = maze.MazeItems.OfType<Explorer>().SingleOrDefault();

            // Assert  
            Assert.IsNotNull(explorer);
            Assert.AreEqual(explorer.Location.X, startPoint.Location.X);
            Assert.AreEqual(explorer.Location.Y, startPoint.Location.Y);
        }

        [TestMethod]
        public void Move_FrontObjectIsEmptySpace_SucessMove()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var moveResult = mazeService.Move();

            // Assert  
            Assert.IsNotNull(moveResult);
            Assert.IsTrue(moveResult.Success);
        }

        [TestMethod]
        public void Move_FrontObjectIsEmptySpace_ReturnsFrontObject()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var moveResult = mazeService.Move();

            // Assert  
            Assert.IsNotNull(moveResult);
            Assert.IsNotNull(moveResult.FrontObject);
        }

        [TestMethod]
        public void Move_FrontObjectIsEmptySpace_ReturnsAvailableMovementOptions()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var moveResult = mazeService.Move();

            // Assert  
            Assert.IsNotNull(moveResult);
            Assert.IsNotNull(moveResult.AvailableMovementOptions);
        }

        [TestMethod]
        public void Move_FrontObjectIsEmptySpace_ReturnsMovementHistory()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var moveResult = mazeService.Move();

            // Assert  
            Assert.IsNotNull(moveResult);
            Assert.IsNotNull(moveResult.History);
        }

        [TestMethod]
        public void Turn_TurnLeft_SucessTurn()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var turnResult = mazeService.Turn(TurnDirection.Left);

            // Assert  
            Assert.IsNotNull(turnResult);
        }

        [TestMethod]
        public void Turn_TurnRight_SucessTurn()
        {
            // Arrange
            var mazeService = new MazeService(this.mazeManager);
            CreateMaze(mazeService);

            // Act
            var turnResult = mazeService.Turn(TurnDirection.Right);

            // Assert  
            Assert.IsNotNull(turnResult);
        }

        private static Maze CreateMaze(MazeService mazeService)
        {
            var mazeContent = File.ReadAllText(SampleMazePath);
            var mazeResult = mazeService.CreateMaze(mazeContent);

            return mazeResult.Maze;
        }
    }
}
