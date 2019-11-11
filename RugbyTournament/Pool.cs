using RugbyTournament;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TournoiRugby
{
    class Pool
    {
        public int PoolId;
        public int NbTeamsInPool;
        Team[] TeamTab;
        List<Game> GameList;

        public Pool(int id,int nbTeamsInPool)
        {
            this.PoolId = id;
            this.NbTeamsInPool = nbTeamsInPool;
            this.TeamTab = new Team[nbTeamsInPool];
            this.GameList = new List<Game>();
        }

        public void AddTeam(Team teamToAdd, int index)
        {
            this.TeamTab[index] = teamToAdd;
        }

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

        public void OrderGames()
        {
            this.GameList.Clear();
            List<int> teamList = new List<int>();
            int halfsize;
            foreach(Team t in TeamTab)
            {
                teamList.Add(t.Id);
            }
            if (this.NbTeamsInPool % 2 != 0)
            {
                teamList.Add(-1);
                halfsize = (int)(NbTeamsInPool/2) + 1;
            } else
            {
                halfsize = NbTeamsInPool / 2;
            }

            int numIterations = (halfsize*2 - 1);

            List<int> teams = new List<int>();

            teams.AddRange(teamList); // Copy all the elements.
            teams.RemoveAt(0); // To exclude the first team.

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

        public Game FindGameByIndex(int index)
        {
            return GameList[index];
        }

        public int GetNbOfGames()
        {
            return GameList.Count;
        }

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

        public List<Team> ComputeResults()
        {
            List<Team> orderedTeams = new List<Team>();
            foreach(Team t in TeamTab)
            {
                t.ComputeTotalScore();
                orderedTeams.Add(t);
            }
            List<Team> SortedList = orderedTeams.OrderByDescending(o => o.TotalScore).ToList();
            for(int i = 0; i< SortedList.Count; i++)
            {
                SortedList[i].PositionInPool = i + 1;
            }
            return SortedList;
        }
    }
}
