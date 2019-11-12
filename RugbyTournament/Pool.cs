using RugbyTournament;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TournoiRugby
{
    /// <summary>
    /// This class represents a Pool in a tournament. 
    /// A pool is composed of teams and possesses a list of games between theses teams.
    /// </summary>
    class Pool
    {
        public int PoolId;
        public int NbTeamsInPool;
        Team[] TeamTab;
        public List<Game> GameList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// We initially know the number of teams in the Pool but we don't know which team yet
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="nbTeamsInPool"></param>
        public Pool(int id,int nbTeamsInPool)
        {
            this.PoolId = id;
            this.NbTeamsInPool = nbTeamsInPool;
            this.TeamTab = new Team[nbTeamsInPool];
            this.GameList = new List<Game>();
        }

        /// <summary>
        /// Method used to add a team to the team tab thanks to an index.
        /// If there already is a team at the given index, we replace it by the given team.
        /// </summary>
        /// <param name="teamToAdd"></param>
        /// <param name="index"></param>
        public void AddTeam(Team teamToAdd, int index)
        {
            this.TeamTab[index] = teamToAdd;
        }

        /// <summary>
        /// Override of the ToString method
        /// </summary>
        /// <returns>Id of the pool and all the teams of the Pool</returns>
        public override string ToString()
        {
            string s = this.PoolId + ". - Pool #" + Convert.ToString(this.PoolId) ;
            foreach(Team t in TeamTab)
            {
                if(t != null)
                {
                    s += ", " + t.ToString();
                }
            }
            s += "\n";
            return s;
        }

        /// <summary>
        /// Method used to create and order the games between the teams such that : 
        ///     - Each team encounters every other team of the Pool once
        ///     - Avoid 2 or more successive games for a team (if possible)
        /// Erases the previous games in the GameList of the Pool.
        /// Adds each game one by one in the GameList to preserve the order
        /// </summary>
        /// <remarks>
        /// The algorithm used for the game ordering is a Round Robin (Knuth) algorithm
        /// https://stackoverflow.com/questions/1293058/round-robin-tournament-algorithm-in-c-sharp
        /// </remarks>
        public void OrderGames()
        {
            //Erase all previous games
            this.GameList.Clear();
            List<int> teamList = new List<int>();
            int halfsize;
            foreach(Team t in TeamTab)
            {
                teamList.Add(t.Id);
            }
            if (this.NbTeamsInPool % 2 != 0) //if the number of teaams in the Pool isn't evem, we add a dummy. Games created with the dummy won't be considered (=pause for the other team).
            {
                teamList.Add(-1); //dummy
                halfsize = (int)(NbTeamsInPool/2) + 1;
            } else
            {
                halfsize = NbTeamsInPool / 2;
            }

            int numIterations = (halfsize*2 - 1); //Nb of rounds necessary for each team to encounter every other team

            List<int> teams = new List<int>();

            teams.AddRange(teamList); // Copy all the elements.
            teams.RemoveAt(0); // Exclude the first team (necessity of the algorithm, the first team remains "unmovable").

            int teamsSize = teams.Count;

            for (int it = 0; it < numIterations; it++) 
            {
                Game g;
                int teamIdx = it % teamsSize;
                if (teams[teamIdx] != -1 && teamList[0] != -1) 
                {
                    g = new Game(FindTeamById(teams[teamIdx]), FindTeamById(teamList[0]));
                    GameList.Add(g);
                }
                for (int idx = 1; idx < halfsize; idx++) 
                {
                    int firstTeam = (it + idx) % teamsSize;
                    int secondTeam = (it + teamsSize - idx) % teamsSize;
                    if (teams[firstTeam] != -1 && teams[secondTeam] != -1)
                    {
                        g = new Game(FindTeamById(teams[firstTeam]), FindTeamById(teams[secondTeam]));
                        GameList.Add(g);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Find a team by its Id in the TeamTab of the Pool
        /// The last occurence is returned
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Last occurence of the team that possesses the given Id int the TeamTab of the Pool.
        /// If no occurence, returns null.
        /// </returns>
        private Team FindTeamById (int id)
        {
            Team t = null;
            foreach(Team team in TeamTab)
            {
                if(team.Id == id)
                {
                    t = team;
                }
            }
            if( t != null)
            {
                return t;
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// Method used to generate a string with all the games of the Pool
        /// </summary>
        /// <returns>
        /// Returns a string with the games of the pool in ascending order
        /// if no game has been set for the Pool, returns empty string
        /// </returns>
        public string DisplayPoolGames ()
        {
            string s = "";
            if(GameList != null)
            {
                foreach(Game g in GameList)
                {
                    //index +1 for display purposes 
                    s += "\n" + (GameList.IndexOf(g)+1) + ". " + g.ToString();
                }
            }
            return s;
        }

        /// <summary>
        /// Method used to find and return a Game by its index in the GameList of the Pool
        /// </summary>
        /// <param name="index"></param>
        /// <returns>
        /// Returns the Game at the given Index in the GameList
        /// Returns null if the Game does'nt exist
        /// </returns>
        public Game FindGameByIndex(int index)
        {
            if(GameList[index] != null)
            {
                return GameList[index];
            } else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Method used to know the number of games in the pool
        /// </summary>
        /// <returns>
        /// Number of games in the pool.
        /// Returns 0 if no games set.
        /// </returns>
        public int GetNbOfGames()
        {
            return GameList.Count;
        }

        /// <summary>
        /// Method used to know the number of games in the pool that don't have been set yet (set = scores entered)
        /// We count the number of unplayed games thanks to its attribute "Winner".
        /// If the Winner is null, then we know that the scores have not been entered (since then Winner is automatically set when the scores are entered)
        /// </summary>
        /// <returns>Number of games that don't have their scores entered</returns>
        public int GetUnplayedGames()
        {
            int unplayedGames = 0;
            if(GameList.Count > 0)
            {
                foreach (Game g in GameList ) {
                    if(g.Winner == null)
                    {
                        unplayedGames++;
                    }
                }
            }
            return unplayedGames;
            
        }

        /// <summary>
        /// Method used to order the teams in the pool by their TotalScore
        /// </summary>
        /// <returns>Ordered list of the teams in the pool by descending TotalScore of the team</returns>
        public List<Team> ComputeResults()
        {
            List<Team> updatedTeams = new List<Team>();
            foreach(Team t in TeamTab) //Compute the total score of each team in the pool
            {
                t.ComputeTotalScore();
                updatedTeams.Add(t);
            }
            List<Team> SortedList = updatedTeams.OrderByDescending(o => o.TotalScore).ToList(); //Linq method
            for(int i = 0; i< SortedList.Count; i++)
            {
                SortedList[i].PositionInPool = i + 1;
            }
            return SortedList;
        }
    }
}
