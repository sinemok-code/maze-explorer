namespace BP.Maze.Service
{
    using System.Collections.Generic;

    using BP.Maze.Entity;

    public class MoveResult
    {
        public BaseMazeObject FrontObject { get; set; }

        public bool Success { get; set; }

        public Explorer CurrentExplorer { get; set; }

        public bool Finished { get; set; }

        public MoveHistory History { get; set; }

        public IEnumerable<MovementOption> AvailableMovementOptions { get; set; }
    }
}
