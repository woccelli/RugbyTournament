using System;
using System.Collections.Generic;
using TournoiRugby;
using System.Linq;

namespace RugbyTournament
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputLine;
            int nbTeams;
            int nbPools;

            Console.WriteLine("Entrez le nombre d'équipes :");
            inputLine = Console.ReadLine();
            if(inputLine.Length > 0) { nbTeams = Convert.ToInt32(inputLine); }
            else { nbTeams = 0; }    

            Console.WriteLine("Entrez le nombre de pools:");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { nbPools = Convert.ToInt32(inputLine); }
            else { nbPools = 0; }

            while(nbPools > nbTeams/2 || nbPools == 0 || nbTeams == 0)
            {
                Console.WriteLine("Trop de pools demandées pour le nombre d'équipes.");
                Console.WriteLine("Entrez le nombre d'équipes :");
                inputLine = Console.ReadLine();
                if (inputLine.Length > 0) { nbTeams = Convert.ToInt32(inputLine); }
                else { nbTeams = 0; }

                Console.WriteLine("Entrez le nombre de pools:");
                inputLine = Console.ReadLine();
                if (inputLine.Length > 0) { nbPools = Convert.ToInt32(inputLine); }
                else { nbPools = 0; }
            }
            Pool[] poolsTab = CreatePoolTab(nbTeams,nbPools);
           
            Team[] tempTeamTab = new Team[nbTeams];
            for (int i = 0; i < nbTeams ; i++)
            {
                Console.WriteLine("Veuillez rentrer le nom de l'équipe #" + (i+1));
                inputLine = Console.ReadLine();
                while(inputLine.Length <= 0)
                {
                    Console.WriteLine("Nom incorrect, veuillez rentrer le nom de l'équipe #" + (i + 1));
                    inputLine = Console.ReadLine();
                }
                Team t = new Team(inputLine,i);
                tempTeamTab[i] = t;
            }
            Shuffle(tempTeamTab);
            int countTeams = 0;
            for (int k = 0; k<nbPools; k++)
            {
                for (int i = 0; i<poolsTab[k].NbTeamsInPool ;i++)
                {
                    poolsTab[k].AddTeam(tempTeamTab[countTeams], i);
                    countTeams++;
                }
                poolsTab[k].OrderGames();
            }
            EnterGamesScore(poolsTab);
            ComputePoolsResults(poolsTab);
        }

        static Pool[] CreatePoolTab (int nbTeams, int nbPools)
        {
            Pool[] pTab = new Pool[nbPools];
            if(nbTeams%nbPools == 0)
            {
                for(int i = 0; i<nbPools; i++)
                {
                    pTab[i] = new Pool(i+1, nbTeams / nbPools);
                }
            } else 
            {
                for(int i = 0; i< nbTeams%nbPools; i++)
                {
                    pTab[i] = new Pool(i+1, (int)(nbTeams / nbPools)+1);
                }
                for (int i = nbTeams%nbPools; i < nbPools; i++)
                {
                    pTab[i] = new Pool(i+1, (int)(nbTeams/nbPools));
                }
            }
            return pTab;
        }

        static void Shuffle(Team[] tab)
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < tab.Length; t++)
            {
                Team tmp = tab[t];
                Random rnd = new Random();
                int r = rnd.Next(t, tab.Length);
                tab[t] = tab[r];
                tab[r] = tmp;
            }
        }

        static void EnterGamesScore (Pool[] poolsTab)
        {
            Pool chosenPool = poolsTab[0];
            while (chosenPool != null)
            {
                DisplayPoolsToChoose(poolsTab);
                chosenPool = ChoosePool(poolsTab);
                if(chosenPool != null)
                {
                    DisplayGames(chosenPool);
                    Game g = ChooseGame(chosenPool);
                    if(g != null)
                    {
                        EnterGameScore(g);
                    }
                }
            }
        }
        
        static void DisplayPoolsToChoose(Pool[] pTab)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Les pools de votre tournoi : \n");
            foreach(Pool p in pTab)
            {
                Console.WriteLine(p.ToString() + " [" + p.GetUnplayedGames() + "]");
            }
        }

        static Pool ChoosePool(Pool[] pTab)
        {
            string inputLine;
            int res =0;
            Console.WriteLine("\nTapez le numéro de la pool et appuyez sur 'Entrée' \n Entrez '0' pour calculer le classement");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
            else { res = 0; }

            while(res<0 || res>pTab.Length)
            {
                Console.WriteLine("\nNuméro incorrect. Veuillez choisir un numéro de pool existant");
                inputLine = Console.ReadLine();
                if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
                else { res = 0; }
            }

            if(res > 0)
            {
                return pTab[res - 1];
            } else
            {
                return null;
            }
                

        }

        static void DisplayGames(Pool pool)
        {
            Console.Clear();
            Console.Write("~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
            Console.Write("Matchs de la pool #" + pool.PoolId + " :\n");
            Console.WriteLine(pool.DisplayPoolGames());
        }

        static Game ChooseGame(Pool pool)
        {
            string inputLine;
            int res = 0;
            Console.WriteLine("\nTapez le numéro du match et appuyez sur 'Entrée' \n Entrez '0' pour calculer le classement");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
            else { res = 0; }

            while (res < 0 || res > pool.GetNbOfGames())
            {
                Console.WriteLine("\nNuméro incorrect. Veuillez choisir un numéro de match existant");
                inputLine = Console.ReadLine();
                if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
                else { res = 0; }
            }

            if (res > 0)
            {
                //res-1 because games were displayed at index+1
                return pool.FindGameByIndex(res-1);
            }
            else
            {
                return null;
            }
        }
        static void EnterGameScore(Game g)
        {
            string inputLine;
            int scoreA;
            int scoreB;

            Console.Clear();
            Console.WriteLine("[ " + g.TeamA + " VS " + g.TeamB + " ]\n");
            Console.WriteLine("Entrez le score de l'équipe : " + g.TeamA + " et appuyez sur 'Entrée'");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { scoreA = Convert.ToInt32(inputLine); }
            else { scoreA = 0; }
            Console.WriteLine("Entrez le score de l'équipe : " + g.TeamB + " et appuyez sur 'Entrée'");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { scoreB = Convert.ToInt32(inputLine); }
            else { scoreB = 0; }

            g.EnterScore(scoreA, scoreB);
            Console.Clear();
            Console.WriteLine("\nMatch validé\n" + g.TeamA + ": " + g.ScoreA +"\n"+g.TeamB+": "+g.ScoreB+"\n\n"+ "Vainqueur = " + g.GetVerbalWinner() + "\n\n Appuyez sur Entrée ...");
            Console.ReadLine();
        }

        static List<Pool> ComputePoolsResults(Pool[] pTab)
        {
            List<List<Team>> sortedTeamsLists = new List<List<Team>>();
            List<Team> tempTeamList;
            List<Pool> newPoolsList = new List<Pool>();
            foreach(Pool p in pTab)
            {
                Pool newP = new Pool(p.PoolId, p.NbTeamsInPool);
                newPoolsList.Add(newP);
            }
            foreach(Pool p in pTab)
            {
                tempTeamList = p.ComputeResults();
                sortedTeamsLists.Add(tempTeamList);
            }
            List<Team> SortedList = sortedTeamsLists.SelectMany(teamList => teamList).OrderBy(team => team.PositionInPool).ThenByDescending(team => team.TotalScore).ToList();
            foreach(Team t in SortedList)
            {
                Console.WriteLine(t.ToString());
            }
            foreach(Pool p in newPoolsList)
            {
                for(int i = 0; i < p.NbTeamsInPool; i++)
                {
                    p.AddTeam(SortedList[0], i);
                    SortedList.RemoveAt(0);
                }
                Console.WriteLine(p.ToString());
            }
            
            return newPoolsList;
        }
    }
}
