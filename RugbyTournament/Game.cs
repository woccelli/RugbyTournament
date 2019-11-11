using System;
using System.Collections.Generic;
using System.Text;

namespace RugbyTournament
{
    class Game
    {
        public Team TeamA { get; }
        public Team TeamB { get; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; } 
        public string Winner;

        public Game (Team teamA, Team teamB)
        {
            this.TeamA = teamA;
            this.TeamB = teamB;
        }

        public override string ToString()
        {
            string s;
            if (Winner != null)
            {
                s = TeamA.Name + "(" + ScoreA+ ") VS " + TeamB.Name + "(" + ScoreB + ")";
            } else
            {
                s = TeamA.Name + " VS " + TeamB.Name;
            }
            return s;
        }

        public void EnterScore(int scoreA, int scoreB)
        {
            if(Winner != null)
            {
                CancelPreviousEnterScore();
            }
            this.ScoreA = scoreA;
            this.ScoreB = scoreB;
            TeamA.ScoresSum += scoreA;
            TeamB.ScoresSum += scoreB;
            if (ScoreA - ScoreB > 0)
            {
                Winner = Convert.ToString(TeamA.Id);
                TeamA.PointsOfVictoriesAndDraws += 3;
            } 
            else if (ScoreA - ScoreB == 0)
            {
                Winner = "Match nul";
                TeamA.PointsOfVictoriesAndDraws += 1;
                TeamB.PointsOfVictoriesAndDraws += 1;
            }
            else
            {
                Winner = Convert.ToString(TeamB.Id);
                TeamB.PointsOfVictoriesAndDraws += 3;
            }
            TeamA.ComputeTotalScore();
            TeamB.ComputeTotalScore();
        }

        public string GetVerbalWinner ()
        {
            if (Winner == "Match nul")
            {
                return "Match nul";
            } else if (Winner == Convert.ToString(TeamA.Id))
            {
                return TeamA.Name;
            } else
            {
                return TeamB.Name;
            }
        }
        public void CancelPreviousEnterScore()
        {
            Winner = null;
            TeamA.ScoresSum -= ScoreA;
            TeamB.ScoresSum -= ScoreB;
            if (ScoreA - ScoreB > 0)
            {
                TeamA.PointsOfVictoriesAndDraws -= 3;
            }
            else if (ScoreA - ScoreB == 0)
            {
                TeamA.PointsOfVictoriesAndDraws -= 1;
                TeamB.PointsOfVictoriesAndDraws -= 1;
            }
            else
            {
                TeamB.PointsOfVictoriesAndDraws -= 3;
            }
        }
    }
}
