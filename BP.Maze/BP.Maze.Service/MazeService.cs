namespace BP.Maze.Service
{
    using BP.Maze.Entity;

    public class MazeService
    {
        public MazeService(IMazeManager mazeManager)
        {
            this.MazeManager = mazeManager;
        }

        private IMazeManager MazeManager { get; set; }

        public CreateMazeResult CreateMaze(string mazeInput)
        {
            var result = new CreateMazeResult();

            var maze = this.MazeManager.CreateMaze(mazeInput);
            result.Maze = maze;

            result.NumberofEmptySpaces = this.MazeManager.GetObjCount(typeof(EmptySpace));
            result.NumberofWalls = this.MazeManager.GetObjCount(typeof(Wall));

            return result;
        }

        public BaseMazeObject GetMazeObjectByLocation(Location location)
        {
            return this.MazeManager.GetMazeObjectByLocation(location);
        }
        
        public TurnResult Turn(TurnDirection direction)
        {
            var explorer = this.MazeManager.TurnExplorer(direction);
            var faceDirectionObj = this.MazeManager.GetFaceDirectionMovementObject();

            var turnResult = new TurnResult { FrontObject = faceDirectionObj, Explorer = explorer };

            return turnResult;
        }

        public MoveResult Move()
        {
            var canMove = this.MazeManager.CanMove();
            if (!canMove)
            {
                return new MoveResult { Success = false };
            }

            this.MazeManager.Move();

            var moveResult = new MoveResult
                                 {
                                     Success = true,
                                     History = this.MazeManager.GetMoveHistory(),
                                     CurrentExplorer = this.MazeManager.GetCurrentExplorer(),
                                     FrontObject = this.MazeManager.GetFaceDirectionMovementObject(),
                                     AvailableMovementOptions = this.MazeManager.GetAvailableMovementOptions(),
                                     Finished = this.MazeManager.Finished()
                                 };

            return moveResult;
        }
    }
}
