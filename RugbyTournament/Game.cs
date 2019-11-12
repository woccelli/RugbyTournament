using System;

namespace RugbyTournament
{
    /// <summary>
    /// This class represents a game between 2 teams : TeamA and TeamB.
    /// It possesses the scores of the 2 teams and a Winner.
    /// </summary>
    class Game
    {
        public Team TeamA { get; }
        public Team TeamB { get; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public string Winner { get; set; } 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="teamA"></param>
        /// <param name="teamB"></param>
        public Game (Team teamA, Team teamB)
        {
            this.TeamA = teamA;
            this.TeamB = teamB;
        }

        /// <summary>
        /// Override of the ToString method
        /// </summary>
        /// <returns>
        /// Names of the 2 teams if the Game hasn't taken place 
        /// Name + scores of the 2 teams if the results of the Game have been set
        /// </returns>
        /// <returns></returns>
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
        /// <summary>
        /// Method to enter the scores of the Game (once the game is finished).
        /// It automatically calculates the Winner (or Draw) and adds points to the winning team.
        /// If the scores were already entered, it updates the scores, the winner and recalculates the points given to each team (cancels the previouses and adds the new ones)
        /// </summary>
        /// <remarks>
        /// 3 points for a victory
        /// 1 point for a draw
        /// 0 point for a defeat
        /// + score*0.001 for each team
        /// </remarks>
        /// <param name="scoreA"></param>
        /// <param name="scoreB"></param>
        public void EnterScore(int scoreA, int scoreB)
        {
            //If the scores have already been entered, we delete them to enter new scores
            if(Winner != null)
            {
                CancelPreviousEnterScore();
            }

            this.ScoreA = scoreA;
            this.ScoreB = scoreB;
            TeamA.ScoresSum += ScoreA;
            TeamB.ScoresSum += ScoreB;

            if (ScoreA - ScoreB > 0) //TeamA wins
            {
                Winner = Convert.ToString(TeamA.Id);
                TeamA.PointsOfVictoriesAndDraws += 3;
            } 
            else if (ScoreA - ScoreB == 0) //Draw
            {
                Winner = "Match nul";
                TeamA.PointsOfVictoriesAndDraws += 1;
                TeamB.PointsOfVictoriesAndDraws += 1;
            }
            else //TeamB wins
            {
                Winner = Convert.ToString(TeamB.Id);
                TeamB.PointsOfVictoriesAndDraws += 3;
            }
            TeamA.ComputeTotalScore();
            TeamB.ComputeTotalScore();
        }

        /// <summary>
        /// Method to get the Name of the Winner
        /// </summary>
        /// <returns>Name of the Winner. "Match nul" if draw. Information message if no winner yet.</returns>
        public string GetVerbalWinner ()
        {
            if (Winner == "Match nul")
            {
                return "Match nul";
            } else if (Winner == Convert.ToString(TeamA.Id))
            {
                return TeamA.Name;
            } else if (Winner == Convert.ToString(TeamB.Id))
            {
                return TeamB.Name;
            } else
            {
                return "Le match n'a pas encore été renseigné";
            }
        }
        /// <summary>
        /// Method to cancel the information entered. It removes the previously given points to the teams and clears the scores.
        /// </summary>
        public void CancelPreviousEnterScore()
        {
            Winner = null;
            //We remove the points given by the scores
            TeamA.ScoresSum -= ScoreA; 
            TeamB.ScoresSum -= ScoreB;
            if (ScoreA - ScoreB > 0) //TeamA won, we remove 3 points
            {
                TeamA.PointsOfVictoriesAndDraws -= 3;
            }
            else if (ScoreA - ScoreB == 0) // Draw, we remove 1 point to each team
            {
                TeamA.PointsOfVictoriesAndDraws -= 1;
                TeamB.PointsOfVictoriesAndDraws -= 1;
            }
            else //TeamB won, we remove 3 points
            {
                TeamB.PointsOfVictoriesAndDraws -= 3;
            }
        }
    }
}
