using System;
using System.Collections.Generic;
using System.Linq;
using TournoiRugby;

namespace RugbyTournament
{
    /// <summary>
    /// Main class of the program, simulates the scheduling of a tournament with two phases (in practive, morning classement pools and afternoon classement pools)
    /// </summary>
    /// <remarks>
    /// First phase (morning scheduling) :
    ///     - The user specifies the characteristics (nb of teams, nb of pools) of the tournament and the name of the teams
    ///     - The program will generate the pools and schedule the games of the tournament
    ///     - The program will create an Excel file with the games per pool
    ///     - The user will fill this file with the scores
    ///     - The program will use the filled file to classify the teams, display the results
    /// Second phase (afternoon scheduling)
    ///     - The program will use the results of the first phase to create new pools (based on the ranking of the teams)
    ///     - The program will generate a new Excel file with the games of the second phase
    ///     - The user will fill the file with the results
    ///     - The program will use the filled file to classify the teams and display the final results
    /// </remarks>
    class Program
    {
        static void Main(string[] args)
        {
            string inputLine;
            int nbTeams;
            int nbPools;

            //*********** FIRST PHASE *****************

            //Generate the Excel file
            ExcelManager excelManager = new ExcelManager();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TournoiMatin.xlsx";

            //Specify the tournament characteristics
            Console.WriteLine("Entrez le nombre d'équipes :");
            bool formatOk = int.TryParse(Console.ReadLine(), out nbTeams);
            while (!formatOk || nbTeams <= 0)
            {
                Console.Clear();
                Console.WriteLine("Le format d'entrée n'est pas correct, veuillez entrer un nombre (strictement positif)\nNombre d'équipes :");
                formatOk = int.TryParse(Console.ReadLine(), out nbTeams);
            }

            Console.WriteLine("Entrez le nombre de pools:");
            formatOk = int.TryParse(Console.ReadLine(), out nbPools);
            while (!formatOk || nbPools <= 0)
            {
                Console.Clear();
                Console.WriteLine("Le format d'entrée n'est pas correct, veuillez entrer un nombre (strictement positif)\nNombre de pools :");
                formatOk = int.TryParse(Console.ReadLine(), out nbPools);
            }

            while(nbPools > nbTeams/2 || nbPools == 0 || nbTeams == 0 || !formatOk)
            {
                Console.WriteLine("Trop de pools pour le nombre d'équipes, veuillez réessayer.\n");
                Console.WriteLine("Entrez le nombre d'équipes :");
                formatOk = int.TryParse(Console.ReadLine(), out nbTeams);
                while (!formatOk || nbTeams <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Le format d'entrée n'est pas correct, veuillez entrer un nombre (strictement positif)\nNombre d'équipes :");
                    formatOk = int.TryParse(Console.ReadLine(), out nbTeams);
                }

                Console.WriteLine("Entrez le nombre de pools:");
                formatOk = int.TryParse(Console.ReadLine(), out nbPools);
                while (!formatOk || nbPools <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Le format d'entrée n'est pas correct, veuillez entrer un nombre (strictement positif)\nNombre de pools :");
                    formatOk = int.TryParse(Console.ReadLine(), out nbPools);
                }
            }
            Pool[] poolsTab = CreatePoolTab(nbTeams,nbPools);

            //Specify the name of the teams
            Console.Clear();
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
            //Generate the pools randomly
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

            //Inform the user that a file has been created and that he must fill it
            Console.Clear();
            Console.WriteLine("Un fichier Excel a été généré sur votre bureau (" + path +")\nVeuillez remplir les résultats des matches et enregistrer le fichier sous le même emplacement (même nom et même dossier)\n\n Une fois ces deux étapes réalisées appuyez sur 'Entrée'");
            Console.ReadKey();
            Console.WriteLine("Êtes-vous sûr d'avoir renseigné tous les scores et d'avoir enregistré le fichier ? Si non, c'est le moment !\n Appuyez sur 'Entrée'");
            Console.ReadKey();
            //Read information from the excel file
            bool readingOk = excelManager.ReadResultsFromExcelFile(path, poolsTab);
            while(!readingOk)
            {
                Console.Clear();
                Console.WriteLine("Des résultats n'ont pas été correctement renseignés sur le fichier excel.\nVeuillez vous assurer que les scores sont inscrits sous forme de nombres entiers (ex: 10, 14, 25).\nVeuillez vous assurer que les colonnes et les lignes ont bien été respectées (pas d'insertion de ligne ou de colonne).\nVeuillez vous assurer que le nom du fichier est le même que celui qui a été généré.\n\nAppuyez sur 'Entrée' pour réessayer...");
                Console.ReadKey();
                readingOk = excelManager.ReadResultsFromExcelFile(path, poolsTab);
            }
           
            //Classify the teams and display the rank of each team
            List<List<Team>> tempList = ComputePoolsResults(poolsTab);
            excelManager.WriteIntermediaryResults(@path, tempList);
            Console.Clear();
            Console.WriteLine("Un onglet a été créé dans le fichier Excel pour afficher les résultats.\nAppuyez sur 'Entrée' pour continuer et former les pools de la deuxième partie du tournoi.");
            Console.ReadKey();

            //**************** SECOND PHASE ****************************
            //Generate the new pools from the ranking
            poolsTab = CreateNewPools(tempList, poolsTab);
            //Generate the new games
            foreach(Pool p in poolsTab)
            {
                p.OrderGames();
            }
            //Generate a new Excel file for the second phase
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\TournoiApresMidi.xlsx";
            excelManager.CreateExcelFile(@path, poolsTab);

            //Inform the user that a file has been created and that he must fill it
            Console.Clear();
            Console.WriteLine("Un fichier Excel a été généré sur votre bureau (" + path + ")\nATTENTION : Ce fichier est différent du fichier que vous avez rempli pour le matin !\nVeuillez remplir les résultats des matches et enregistrer le fichier sous le même emplacement (même nom et même dossier)\n\n Une fois ces deux étapes réalisées appuyez sur 'Entrée'");
            Console.ReadKey();
            Console.WriteLine("Êtes-vous sûr d'avoir renseigné tous les scores et d'avoir enregistré le fichier ? Si non, c'est le moment !\n Appuyez sur 'Entrée'");
            Console.ReadKey();
            //Read the information from the file
            readingOk = excelManager.ReadResultsFromExcelFile(path, poolsTab);
            while (!readingOk)
            {
                Console.Clear();
                Console.WriteLine("Des résultats n'ont pas été correctement renseignés sur le fichier excel.\nVeuillez vous assurer que les scores sont inscrits sous forme de nombres entiers (ex: 10, 14, 25).\nVeuillez vous assurer que les colonnes et les lignes ont bien été respectées (pas d'insertion de ligne ou de colonne).\nVeuillez vous assurer que le nom du fichier est le même que celui qui a été généré.\n\nAppuyez sur 'Entrée' pour réessayer...");
                Console.ReadKey();
                readingOk = excelManager.ReadResultsFromExcelFile(path, poolsTab);
            }
            
            //Classify the teams and display the results 
            tempList = ComputePoolsResults(poolsTab);
            excelManager.WriteFinalResults(@path, tempList);
            Console.Clear();
            Console.WriteLine("Un onglet a été créé dans le fichier Excel pour afficher les résultats.\nAppuyez sur 'Entrée' pour cloturer le tournoi.");
            Console.ReadKey();
        }

        /// <summary>
        /// This method is used to create nbPools and fill them with the correct number of teams
        /// This method evenly distributes the nb of teams by pool, when nbTeams%nbPools!=0, the addtionnal teams are added to the first created pools
        /// </summary>
        /// <param name="nbTeams"></param>
        /// <param name="nbPools"></param>
        /// <returns></returns>
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
                for(int i = 0; i< nbTeams%nbPools; i++) // fills the first pools with an additional team
                {
                    pTab[i] = new Pool(i+1, (int)(nbTeams / nbPools)+1);
                }
                for (int i = nbTeams%nbPools; i < nbPools; i++) // fills the remaining pools 
                {
                    pTab[i] = new Pool(i+1, (int)(nbTeams/nbPools));
                }
            }
            return pTab;
        }

        /// <summary>
        /// Shuffles the members of a tab thanks to the Knuth algorithm
        /// </summary>
        /// <param name="tab"></param>
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

        /// <summary>
        /// Method used to sort the teams in each pool by their TotalScore (descending), and return all the sorted lists of the tournament (one for each pool) 
        /// </summary>
        /// <param name="pTab"></param>
        /// <returns>Returns a list of the sorted lists of each Pool. The sorted lists are ordered by descending TotalScore</returns>
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

        /// <summary>
        /// Method used to create new Pools from the results of the pools of the first phase. 
        /// This method will fill the first Pool with the NbTeamsInPool first teams of the tournament, the second pool with the next NbTeamsInPool first teams and so on.
        /// </summary>
        /// <param name="sortedTeamsLists"></param>
        /// <param name="previousPools"></param>
        /// <returns>Returns a tab with the newly created pools. The first pool contains the "best" teams of the tournament.</returns>
        static Pool[] CreateNewPools(List<List<Team>> sortedTeamsLists, Pool[] previousPools)
        {
            List<Pool> newPoolsList = new List<Pool>();

            //Create one sorted list from all the sorted lists. 
            //We order by positionInPool (e.g. to fill the first pool with all the first teams of the tournament) and then by TotalScore (to find the best second team of the tournament for example)
            List<Team> SortedList = sortedTeamsLists.SelectMany(teamList => teamList).OrderBy(team => team.PositionInPool).ThenByDescending(team => team.TotalScore).ToList();
            foreach (Pool p in previousPools)
            {
                Pool newP = new Pool(p.PoolId, p.NbTeamsInPool);
                newPoolsList.Add(newP);
            }
            foreach (Pool p in newPoolsList)
            {
                for (int i = 0; i < p.NbTeamsInPool; i++)
                {
                    SortedList[0].ClearResults();
                    p.AddTeam(SortedList[0], i);
                    SortedList.RemoveAt(0);
                }
            }
            Pool[] poolTab = newPoolsList.ToArray();
            return poolTab;
        }

        /// <summary>
        /// Method used to display the final rank of each team in the console.
        /// </summary>
        /// <param name="listTeamsByPool"></param>
        static void DisplayFinalResults(List<List<Team>> listTeamsByPool)
        {
            Console.Clear();
            Console.WriteLine("Les résulats finaux sont : ");
            int numPool = 1;
            int totalCount = 1;
            foreach(List<Team> listT in listTeamsByPool)
            {
                Console.WriteLine("---- Pool #" + numPool);
                for(int k = 0; k < listT.Count; k++)
                {
                    Console.WriteLine(totalCount + ". " + listT[k].ToString());
                    totalCount++;
                    
                }
                numPool++;
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n\n Appuyez sur 'Entrée' pour terminer le tournoi.");
            Console.ReadLine();
        }

        /// <summary>
        /// Method used to display the intermedirary (=of the first phase) rank of each team
        /// </summary>
        /// <param name="listTeamsByPool"></param>
        static void DisplayIntermediaryResults(List<List<Team>> listTeamsByPool)
        {
            Console.Clear();
            Console.WriteLine("Les résulats intermédiaires sont : ");
            int numPool = 1;
            int totalCount = 1;
            foreach (List<Team> listT in listTeamsByPool)
            {
                Console.WriteLine("---- Pool #" + numPool);
                for (int k = 0; k < listT.Count; k++)
                {
                    Console.WriteLine((k+1) + ". " + listT[k].ToString());
                    totalCount++;
                    
                }
                numPool++;
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n\n Appuyez sur 'Entrée' pour continuer et passer aux pools de l'après midi.");
            Console.ReadLine();
        }




     //******************* CONSOLE INTERACTIONS ***************************

        /// <summary>
        /// This method displays the pools of the tournament, allows the user to chose a pool thanks to a number 
        /// once the Pool is chosen, the method displays the games of the pool and the user can choose a game thanks to a number to specify the score of each team
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="poolsTab"></param>
        static void EnterGamesScore(Pool[] poolsTab)
        {
            Pool chosenPool = poolsTab[0];
            while (chosenPool != null)
            {
                //Display the pools of the tournament along with a number
                DisplayPoolsToChoose(poolsTab);
                //The user choses a pools thanks to its number
                chosenPool = ChoosePool(poolsTab);
                if (chosenPool != null)
                {
                    Game g = chosenPool.FindGameByIndex(0);
                    while (g != null) //while loop to come back to the games display after entering the scores of a game
                    {
                        DisplayGames(chosenPool);
                        g = ChooseGame(chosenPool);
                        if (g != null)
                        {
                            EnterGameScore(g);
                        }
                    }
                }
                //Verify that there is no missing data to continue and compute the ranking
                if (chosenPool == null)
                {
                    foreach (Pool p in poolsTab)
                    {
                        if (Convert.ToInt32(p.GetUnplayedGames()) > 0)
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

        /// <summary>
        /// Method used to display the pools of the tournament in the console along with the number of their unplayed games.
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="pTab"></param>
        static void DisplayPoolsToChoose(Pool[] pTab)
        {
            Console.Clear();
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Les pools de votre tournoi : \n\n");
            foreach (Pool p in pTab)
            {
                Console.WriteLine(p.ToString() + " [" + p.GetUnplayedGames() + " score(s) non renseigné(s).]\n");
            }
        }

        /// <summary>
        /// Method used to choose a pool from its displayed number.
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="pTab"></param>
        /// <returns>Returns the pool chosen by the user, null otherwise.</returns>
        static Pool ChoosePool(Pool[] pTab)
        {
            string inputLine;
            int res;
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\nTapez le numéro de la pool et appuyez sur 'Entrée' \n\n\n Une fois tous les scores renseignés,\nTapez '0' et appyuez sur 'Entrée' pour calculer le classement");
            inputLine = Console.ReadLine();
            if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
            else { res = 0; }

            while (res < 0 || res > pTab.Length) //if there is no pool corresponding to the entered number
            {
                Console.WriteLine("\nNuméro incorrect. Veuillez choisir un numéro de pool existant");
                inputLine = Console.ReadLine();
                if (inputLine.Length > 0) { res = Convert.ToInt32(inputLine); }
                else { res = 0; }
            }

            if (res > 0)
            {
                return pTab[res - 1];
            }
            else
            {
                return null;
            }


        }

        /// <summary>
        /// Method used to display the games of the pool in the console.
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="pool"></param>
        static void DisplayGames(Pool pool)
        {
            Console.Clear();
            Console.Write("~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
            Console.Write("Matchs de la pool #" + pool.PoolId + " :\n");
            Console.WriteLine(pool.DisplayPoolGames());
        }

        /// <summary>
        /// Method used to select a game for the game list of the pool thanks to a number.
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="pool"></param>
        /// <returns>Returns the game corresponding to the enterd number, null otherwise</returns>
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
                return pool.FindGameByIndex(res - 1);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Method used to specify the points scored by each team of a given game
        /// </summary>
        /// <remarks>
        /// Not used in this version
        /// </remarks>
        /// <param name="g"></param>
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


    }
}
