
namespace BP.Maze.Entity
{
    using System.Collections.Generic;

    public class MoveHistory 
    {
        public MoveHistory()
        {
            this.Steps = new List<Step>();
        }

        public List<Step> Steps { get; set; }
    }
}
