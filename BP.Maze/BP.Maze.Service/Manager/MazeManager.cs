namespace BP.Maze.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BP.Maze.Entity;
    using BP.Maze.Service.Exceptions;

    public interface IMazeManager
    {
        Maze Maze { get; set; }

        Maze CreateMaze(string mazeInput);

        BaseMazeObject GetMazeObjectByLocation(Location location);

        int GetObjCount(Type objType);

        Explorer TurnExplorer(TurnDirection direction);

        BaseMazeObject GetFaceDirectionMovementObject();

        bool CanMove();

        void Move();

        bool Finished();

        MoveHistory GetMoveHistory();

        IEnumerable<MovementOption> GetAvailableMovementOptions();

        Explorer GetCurrentExplorer();

        Explorer GetExplorer();

        BaseMazeObject GetFaceDirectionMovementObject(Explorer explorer);

        BaseMazeObject GetDirectionMovementObject(BaseMazeObject explorer, FaceDirection faceDirection);

        void SaveStepHistory();

        void MoveExplorer();

        IEnumerable<MovementOption> GetCurrentAvailableMovementOptions();
    }

    public class MazeManager : IMazeManager
    {
        private const char Wall = 'X';
        private const char Exit = 'F';
        private const char StartPoint = 'S';
        private const char EmptySpace = ' ';
        private const FaceDirection DefaultFaceDirection = FaceDirection.Right;

        public Maze Maze { get; set; }

        public Maze CreateMaze(string mazeInput)
        {
            var maze = new Maze();

            CheckNullForMazeInput(mazeInput);
            CheckInvalidCharacterForMazeInput(mazeInput);

            var lines = mazeInput.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    var mazeObj = GetMazeObjectFromInput(lines[y][x], x, y);
                    maze.MazeItems.Add(mazeObj);
                }
            }

            maze.MazeItems.Add(CreateExplorer(maze));

            this.Maze = maze;
            return maze;
        }

        public BaseMazeObject GetMazeObjectByLocation(Location location)
        {
            if (this.Maze == null)
            {
                throw new MazeNotFoundException();
            }

            return this.Maze.MazeItems.SingleOrDefault(x => x.Location.X == location.X && x.Location.Y == location.Y);
        }

        public int GetObjCount(Type objType)
        {
            return this.Maze.MazeItems.Count(x => x.GetType() == objType);
        }

        public Explorer TurnExplorer(TurnDirection direction)
        {
            var explorer = this.GetExplorer();

            if (explorer == null)
            {
                return null;
            }

            switch (direction)
            {
                case TurnDirection.Left:
                    switch (explorer.FaceDirection)
                    {
                        case FaceDirection.Up:
                            explorer.FaceDirection = FaceDirection.Left;
                            break;
                        case FaceDirection.Left:
                            explorer.FaceDirection = FaceDirection.Down;
                            break;
                        case FaceDirection.Down:
                            explorer.FaceDirection = FaceDirection.Right;
                            break;
                        case FaceDirection.Right:
                            explorer.FaceDirection = FaceDirection.Up;
                            break;
                    }

                    break;
                case TurnDirection.Right:
                    switch (explorer.FaceDirection)
                    {
                        case FaceDirection.Up:
                            explorer.FaceDirection = FaceDirection.Right;
                            break;
                        case FaceDirection.Left:
                            explorer.FaceDirection = FaceDirection.Up;
                            break;
                        case FaceDirection.Down:
                            explorer.FaceDirection = FaceDirection.Left;
                            break;
                        case FaceDirection.Right:
                            explorer.FaceDirection = FaceDirection.Down;
                            break;
                    }

                    break;
            }

            return explorer;
        }

        public BaseMazeObject GetFaceDirectionMovementObject()
        {
            return this.GetFaceDirectionMovementObject(this.GetExplorer());
        }

        public bool CanMove()
        {
            var faceDirectionObj = this.GetFaceDirectionMovementObject(this.GetExplorer());
            return faceDirectionObj != null && !(faceDirectionObj is Wall);
        }

        public void Move()
        {
            this.MoveExplorer();
            this.SaveStepHistory();
        }

        public bool Finished()
        {
            return this.GetFaceDirectionMovementObject(this.GetExplorer()) is Exit;
        }

        public MoveHistory GetMoveHistory()
        {
            return this.Maze.MoveHistory;
        }

        public IEnumerable<MovementOption> GetAvailableMovementOptions()
        {
            return this.GetCurrentAvailableMovementOptions();
        }

        public Explorer GetCurrentExplorer()
        {
            return this.GetExplorer();
        }

        private static Explorer CreateExplorer(Maze maze)
        {
            var startPoint = maze.MazeItems.OfType<StartPoint>().SingleOrDefault();
            if (startPoint == null)
            {
                throw new StartPointNotExistException();
            }

            return new Explorer
                                   {
                                       Location = new Location { X = startPoint.Location.X, Y = startPoint.Location.Y },
                                       FaceDirection = DefaultFaceDirection
                                   };
        }

        private static void CheckNullForMazeInput(string mazeInput)
        {
            if (mazeInput == null)
            {
                throw new MazeNotCreatedException();
            }
        }

        private static void CheckInvalidCharacterForMazeInput(string mazeInput)
        {
            var invalidCharExists = mazeInput.IndexOfAny(new[] { Wall, Exit, StartPoint, EmptySpace }) == -1;
            if (invalidCharExists)
            {
                throw new MazeNotCreatedException();
            }
        }

        private static BaseMazeObject GetMazeObjectFromInput(char input, int x, int y)
        {
            BaseMazeObject mazeObj;

            switch (input)
            {
                case Wall:
                    mazeObj = new Wall();
                    break;
                case Exit:
                    mazeObj = new Exit();
                    break;
                case StartPoint:
                    mazeObj = new StartPoint();
                    break;
                case EmptySpace:
                    mazeObj = new EmptySpace();
                    break;
                default:
                    mazeObj = null;
                    break;
            }

            if (mazeObj == null)
            {
                return null;
            }

            mazeObj.Location = new Location { X = x, Y = y };
            return mazeObj;
        }

        public Explorer GetExplorer()
        {
            return this.Maze.MazeItems.OfType<Explorer>().SingleOrDefault();
        }

        public BaseMazeObject GetFaceDirectionMovementObject(Explorer explorer)
        {
            return this.GetDirectionMovementObject(explorer, explorer.FaceDirection);
        }

        public BaseMazeObject GetDirectionMovementObject(BaseMazeObject explorer, FaceDirection faceDirection)
        {
            var faceDirectionLocation = new Location { X = explorer.Location.X, Y = explorer.Location.Y };

            switch (faceDirection)
            {
                case FaceDirection.Up:
                    faceDirectionLocation.Y -= 1;
                    break;
                case FaceDirection.Left:
                    faceDirectionLocation.X -= 1;
                    break;
                case FaceDirection.Down:
                    faceDirectionLocation.Y += 1;
                    break;
                case FaceDirection.Right:
                    faceDirectionLocation.X += 1;
                    break;
            }

            var faceDirectionObj =
                this.Maze.MazeItems.SingleOrDefault(
                    o => o.Location.X == faceDirectionLocation.X && o.Location.Y == faceDirectionLocation.Y);

            return faceDirectionObj;
        }

        public void SaveStepHistory()
        {
            var lastStepNo = (this.Maze.MoveHistory.Steps.Count == 0)
                                 ? 0
                                 : this.Maze.MoveHistory.Steps.Max(x => x.StepNo);

            var step = new Step
            {
                StepNo = lastStepNo + 1,
                Location =
                    new Location
                    {
                        X = this.GetExplorer().Location.X,
                        Y = this.GetExplorer().Location.Y
                    }
            };
            this.Maze.MoveHistory.Steps.Add(step);
        }

        public void MoveExplorer()
        {
            var faceDirectionObj = this.GetFaceDirectionMovementObject(this.GetExplorer());

            this.GetExplorer().Location = new Location
            {
                X = faceDirectionObj.Location.X,
                Y = faceDirectionObj.Location.Y
            };
        }

        public IEnumerable<MovementOption> GetCurrentAvailableMovementOptions()
        {
            var availableMovementOptions = new List<MovementOption>();

            var upObj = this.GetDirectionMovementObject(this.GetExplorer(), FaceDirection.Up);
            if (upObj != null && !(upObj is Wall))
            {
                availableMovementOptions.Add(MovementOption.Up);
            }

            var leftObj = this.GetDirectionMovementObject(this.GetExplorer(), FaceDirection.Left);
            if (leftObj != null && !(leftObj is Wall))
            {
                availableMovementOptions.Add(MovementOption.Left);
            }

            var downObj = this.GetDirectionMovementObject(this.GetExplorer(), FaceDirection.Down);
            if (downObj != null && !(downObj is Wall))
            {
                availableMovementOptions.Add(MovementOption.Down);
            }

            var rightObj = this.GetDirectionMovementObject(this.GetExplorer(), FaceDirection.Right);
            if (rightObj != null && !(rightObj is Wall))
            {
                availableMovementOptions.Add(MovementOption.Right);
            }

            return availableMovementOptions;
        }
    }
}