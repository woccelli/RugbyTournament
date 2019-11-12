
namespace RugbyTournament
{
    /// <summary>
    /// This class represents a Team in a tournament. 
    /// The Team has an Id, a Name and various information about the points it won (score) and its position in its pool.
    /// </summary>
    class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PointsOfVictoriesAndDraws;
        // Total of the points the team has scored during its games
        public int ScoresSum;
        // TotalScore = PointsOfVictoriesAndDraws + ScoresSum*0.001 
        public double TotalScore;
        public int PositionInPool;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public Team(string name, int id)
        {
            this.Name = name; 
            this.Id = id;
            this.PointsOfVictoriesAndDraws = 0;
            this.ScoresSum = 0;
            this.TotalScore = 0;
            this.PositionInPool = 0;
        }
        /// <summary>
        /// Override of the ToString method
        /// </summary>
        /// <returns>Name of the team and its TotalScore</returns>
        public override string ToString()
        {
            return this.Name + " (Points:" + this.TotalScore + ")";
        }

        /// <summary>
        /// Method used to compute, update and return the total score of the team
        /// TotalScore = PointsOfVictoriesAndDraws + ScoresSum*0.001 
        /// </summary>
        /// <returns>Returns the TotalScore of the Team</returns>
        public double ComputeTotalScore()
        {
            this.TotalScore = PointsOfVictoriesAndDraws + (ScoresSum*0.001);
            return TotalScore;
        }

        /// <summary>
        /// Method used to clear all the results of the team
        /// </summary>
        /// <remarks>
        /// Usually called when the team enters a new Pool
        /// </remarks>
        public void ClearResults ()
        {
            this.PointsOfVictoriesAndDraws = 0;
            this.ScoresSum = 0;
            this.TotalScore = 0;
        }
    }
}
