namespace BP.Maze.Service
{
    public class CreateMazeResult
    {
        public Entity.Maze Maze { get; set; }

        public int NumberofWalls { get; set; }

        public int NumberofEmptySpaces { get; set; }
    }
}