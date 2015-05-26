namespace BP.Maze.Entity
{
    using System.Collections.Generic;
    
    public class Maze
    {
        public Maze()
        {
            this.MazeItems = new List<BaseMazeObject>();
            this.MoveHistory = new MoveHistory();
        }

        public List<BaseMazeObject> MazeItems { get; set; }

        public MoveHistory MoveHistory { get; set; }
    }
}