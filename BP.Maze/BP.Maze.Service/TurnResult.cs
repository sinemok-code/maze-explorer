namespace BP.Maze.Service
{
    using BP.Maze.Entity;

    public class TurnResult
    {
        public BaseMazeObject FrontObject { get; set; }

        public Explorer Explorer { get; set; }
    }
}
