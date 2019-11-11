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

            ExcelManager excelManager = new ExcelManager();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TournoiMatin.xlsx";


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
            excelManager.CreateExcelFile(@path, poolsTab);
            bool b = false;
            Console.Clear();
            Console.WriteLine("Un fichier Excel a été généré sur votre bureau (" + path +")\nVeuillez remplir les résultats des matches et enregistrer le fichier sous le même emplacement (même nom et même dossier)\n\n Une fois ces deux étapes réalisées appuyez sur 'Entrée'");
            Console.ReadKey();
            Console.WriteLine("Êtes-vous sûr d'avoir renseigné tous les scores et d'avoir enregistré le fichier ? Si non, c'est le moment !\n Appuyez sur 'Entrée'");
            excelManager.ReadResultsFromExcelFile(path, poolsTab);
           
            List<List<Team>> tempList = ComputePoolsResults(poolsTab);
            DisplayIntermediaryResults(tempList);
            poolsTab = CreateNewPools(tempList, poolsTab);
            foreach(Pool p in poolsTab)
            {
                p.OrderGames();
            }
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TournoiApresMidi.xlsx";
            excelManager.CreateExcelFile(@path, poolsTab);
            Console.Clear();
            Console.WriteLine("Un fichier Excel a été généré sur votre bureau (" + path + ")\nATTENTION : Ce fichier est différent du fichier que vous avez rempli pour le matin !\nVeuillez remplir les résultats des matches et enregistrer le fichier sous le même emplacement (même nom et même dossier)\n\n Une fois ces deux étapes réalisées appuyez sur 'Entrée'");
            Console.ReadKey();
            Console.WriteLine("Êtes-vous sûr d'avoir renseigné tous les scores et d'avoir enregistré le fichier ? Si non, c'est le moment !\n Appuyez sur 'Entrée'");
            excelManager.ReadResultsFromExcelFile(path, poolsTab);
            
            tempList = ComputePoolsResults(poolsTab);
            DisplayFinalResults(tempList);
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
                    Game g = chosenPool.FindGameByIndex(0);
                    while(g != null)
                    {
                        DisplayGames(chosenPool);
                        g = ChooseGame(chosenPool);
                        if(g != null)
                        {
                            EnterGameScore(g);
                        }                    }
                }
                if(chosenPool == null)
                {
                    foreach(Pool p in poolsTab)
                    {
                        if(Convert.ToInt32(p.GetUnplayedGames()) > 0)
                        {
                            chosenPool = poolsTab[0];
                            Console.WriteLine("\n La Pool #" + p.PoolId + " ne possède pas de score pour tous ses matchs, veuillez les renseigner.");
                            Console.WriteLine("Appuyez sur 'Entrée' pour continuer ...");
                            Console.ReadKey();
                        }
                    }
                }
            }
        }
        
        static void DisplayPoolsToChoose(Pool[] pTab)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Les pools de votre tournoi : \n\n");
            foreach(Pool p in pTab)
            {
                Console.WriteLine(p.ToString() + " [" + p.GetUnplayedGames() + " score(s) non renseigné(s).]\n");
            }
        }

        static Pool ChoosePool(Pool[] pTab)
        {
            string inputLine;
            int res =0;
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\nTapez le numéro de la pool et appuyez sur 'Entrée' \n\n\n Une fois tous les scores renseignés,\nTapez '0' et appyuez sur 'Entrée' pour calculer le classement");
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
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\nPour renseigner le score, tapez le numéro du match et appuyez sur 'Entrée' \n\n\n Appuyez simplement sur 'Entrée' pour revenir à la liste des pools");
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
            Console.WriteLine("\nMatch validé\n" + g.TeamA + ": " + g.ScoreA + "\n" + g.TeamB + ": " + g.ScoreB + "\n\n" + "Vainqueur = " + g.GetVerbalWinner());
            Console.WriteLine("-------------------------------------"); 
            Console.WriteLine("\n\n Appuyez sur Entrée ...");
            Console.ReadKey();
        }

        static List<List<Team>> ComputePoolsResults(Pool[] pTab)
        {
            List<List<Team>> sortedTeamsLists = new List<List<Team>>();
            List<Team> tempTeamList;
            
            foreach (Pool p in pTab)
            {
                tempTeamList = p.ComputeResults();
                sortedTeamsLists.Add(tempTeamList);
            }
            return sortedTeamsLists;
        }
        static Pool[] CreateNewPools(List<List<Team>> sortedTeamsLists, Pool[] previousPools)
        {
            List<Pool> newPoolsList = new List<Pool>();
            List<Team> SortedList = sortedTeamsLists.SelectMany(teamList => teamList).OrderBy(team => team.PositionInPool).ThenByDescending(team => team.TotalScore).ToList();
            foreach (Pool p in previousPools)
            {
                Pool newP = new Pool(p.PoolId, p.NbTeamsInPool);
                newPoolsList.Add(newP);
            }
            foreach (Team t in SortedList)
            {
                Console.WriteLine(t.ToString());
            }
            foreach (Pool p in newPoolsList)
            {
                for (int i = 0; i < p.NbTeamsInPool; i++)
                {
                    SortedList[0].ClearResults();
                    p.AddTeam(SortedList[0], i);
                    SortedList.RemoveAt(0);
                }
                Console.WriteLine(p.ToString());
            }
            Pool[] poolTab = newPoolsList.ToArray();
            return poolTab;
        }

        static void DisplayFinalResults(List<List<Team>> listTeamsByPool)
        {
            Console.Clear();
            Console.WriteLine("Les résulats finaux sont : ");
            int NumPool = 1;
            int totalCount = 1;
            foreach(List<Team> listT in listTeamsByPool)
            {
                Console.WriteLine("---- Pool #" + NumPool);
                for(int k = 0; k < listT.Count; k++)
                {
                    Console.WriteLine(totalCount + ". " + listT[k].ToString());
                    totalCount++;
                }
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n\n Appuyez sur 'Entrée' pour terminer le tournoi.");
            Console.ReadLine();
        }
        static void DisplayIntermediaryResults(List<List<Team>> listTeamsByPool)
        {
            Console.Clear();
            Console.WriteLine("Les résulats intermédiaires sont : ");
            int NumPool = 1;
            int totalCount = 1;
            foreach (List<Team> listT in listTeamsByPool)
            {
                Console.WriteLine("---- Pool #" + NumPool);
                for (int k = 0; k < listT.Count; k++)
                {
                    Console.WriteLine((k+1) + ". " + listT[k].ToString());
                    totalCount++;
                }
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n\n Appuyez sur 'Entrée' pour continuer et passer aux pools de l'après midi.");
            Console.ReadLine();
        }

    }
}
