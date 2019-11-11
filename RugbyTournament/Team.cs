using System;
using System.Collections.Generic;
using System.Text;

namespace RugbyTournament
{
    class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PointsOfVictoriesAndDraws;
        public int ScoresSum;
        public double TotalScore;
        public int PositionInPool;

        public Team(string name, int id)
        {
            this.Name = name; 
            this.Id = id;
            this.PointsOfVictoriesAndDraws = 0;
            this.ScoresSum = 0;
            this.TotalScore = 0;
            this.PositionInPool = 0;
        }
        public override string ToString()
        {
            return this.Name + " (Points:" + this.TotalScore + ")";
        }

        public double ComputeTotalScore()
        {
            this.TotalScore = PointsOfVictoriesAndDraws + (ScoresSum*0.001);
            return TotalScore;
        }

        public void ClearResults ()
        {
            this.PointsOfVictoriesAndDraws = 0;
            this.ScoresSum = 0;
            this.TotalScore = 0;
        }
    }
}
