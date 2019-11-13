using System;
using System.Collections.Generic;

using OfficeOpenXml;
using System.IO;
using TournoiRugby;

namespace RugbyTournament
{
    /// <summary>
    /// This class allows to manipulate (create, write, read) Excel files.
    /// </summary>
    /// <remarks>
    /// Source code originated from : https://www.codebyamir.com/blog/create-excel-files-in-c-sharp
    /// </remarks>
    class ExcelManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExcelManager() { }
        /// <summary>
        /// Create an Excel file with a specific header that will be used to enter the scores of the tournament
        /// </summary>
        /// <param name="fileName">Path of the created Excel file</param>
        /// <param name="pTab">Pools of the tournament</param>
        public void CreateExcelFile(string fileName, Pool[] pTab)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                FileInfo excelFile = new FileInfo(fileName);
                if (excelFile.Exists)
                {
                    while (IsFileLocked(excelFile))
                    {
                        Console.WriteLine("Veuillez fermer le fichier excel :" + fileName + "\nEt appuyer sur 'Entrée'");
                        Console.ReadKey();
                    }
                }
                
                var headerRow = new List<string[]>()
                {
                    new string[] { "NumMatch", "Équipe 1", "Équipe 2", "Score Équipe 1", "Score Équipe 2" }
                };

                // Determine the header range (e.g. A1:E1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";


                foreach (Pool p in pTab)
                {
                    excel.Workbook.Worksheets.Add("Pool #" + p.PoolId); //Each pool has a different worksheet
                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Pool #" + p.PoolId];

                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                    worksheet.Cells[headerRange].Style.Font.Bold = true;
                    worksheet.Cells[headerRange].Style.Font.Size = 14;
                    worksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[headerRange].AutoFitColumns();

                    for (int i = 0; i < p.GetNbOfGames(); i++) //Write the games in order
                    {
                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = p.GameList[i].TeamA.Name;
                        worksheet.Cells[i + 2, 3].Value = p.GameList[i].TeamB.Name;
                    }
                }

                excel.SaveAs(excelFile); //save the created file
            }
        }

        /// <summary>
        /// Method used to read the scores of the tournament and directly update the data contained in the objects of this program
        /// </summary>
        /// <param name="fileName">Path of the Excel file to read</param>
        /// <param name="pTab">Pools of the tournament</param>
        public bool ReadResultsFromExcelFile(string fileName, Pool[] pTab)
        {
            bool readingIsOk = true;
            FileInfo existingFile = new FileInfo(fileName);
            
            if (existingFile.Exists)
            {
                while (IsFileLocked(existingFile))
                {
                    Console.WriteLine("Veuillez fermer le fichier excel :" + fileName + "\nEt appuyer sur 'Entrée'");
                    Console.ReadKey();
                }
                using (ExcelPackage excel = new ExcelPackage(existingFile))
                {
                    foreach (Pool p in pTab)
                    {
                        // Target a worksheet
                        var worksheet = excel.Workbook.Worksheets["Pool #" + p.PoolId];

                        for (int i = 0; i < p.GetNbOfGames(); i++) //read the scores
                        {
                            if (worksheet.Cells[i + 2, 4].Value != null && worksheet.Cells[i + 2, 5].Value != null)
                            {
                                if (!int.TryParse(worksheet.Cells[i + 2, 4].Value.ToString(), out int scoreA))
                                {
                                    readingIsOk = false;
                                    break;
                                }
                                if (!int.TryParse(worksheet.Cells[i + 2, 5].Value.ToString(), out int scoreB))
                                {
                                    readingIsOk = false;
                                    break;
                                }
                                p.GameList[i].EnterScore(scoreA, scoreB);
                            }
                            else
                            {
                                readingIsOk = false;
                                break;
                            }

                        }
                    }
                }
            }
            else
            {
                readingIsOk = false;
            }
            return readingIsOk;
        }

        public void WriteFinalResults(string fileName, List<List<Team>> listTeamsByPool)
        {
            FileInfo existingFile = new FileInfo(fileName);
            if (existingFile.Exists)
            {
                while (IsFileLocked(existingFile))
                {
                    Console.WriteLine("Veuillez fermer le fichier excel :" + fileName + "\nEt appuyer sur 'Entrée'");
                    Console.ReadKey();
                }
                using (ExcelPackage excel = new ExcelPackage(existingFile))
                {
                    excel.Workbook.Worksheets.Add("Résultats");
                    var worksheet = excel.Workbook.Worksheets["Résultats"];

                    var headerRow = new List<string[]>()
                    {
                        new string[] { "Classement général", "Équipe", "Pool", "Points", "Goal average" }
                    };

                    string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                    worksheet.Cells[headerRange].Style.Font.Bold = true;
                    worksheet.Cells[headerRange].Style.Font.Size = 14;
                    worksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[headerRange].AutoFitColumns();

                    int numPool = 1;
                    int totalCount = 1;
                    foreach (List<Team> listT in listTeamsByPool)
                    {
                        for (int k = 0; k < listT.Count; k++)
                        {
                            worksheet.Cells[totalCount + 1, 1].Value = totalCount;
                            worksheet.Cells[totalCount + 1, 2].Value = listT[k].Name;
                            worksheet.Cells[totalCount + 1, 3].Value = numPool;
                            worksheet.Cells[totalCount + 1, 4].Value = listT[k].PointsOfVictoriesAndDraws;
                            worksheet.Cells[totalCount + 1, 5].Value = listT[k].ScoresSum;
                            totalCount++;
                        }
                        numPool++;
                    }
                    excel.SaveAs(existingFile);
                }
            }
            
        }

        public void WriteIntermediaryResults(string fileName, List<List<Team>> listTeamsByPool)
        {
            FileInfo existingFile = new FileInfo(fileName);
            if (existingFile.Exists)
            {
                using (ExcelPackage excel = new ExcelPackage(existingFile))
                {
                    excel.Workbook.Worksheets.Add("Résultats");
                    var worksheet = excel.Workbook.Worksheets["Résultats"];

                    int numPool = 1;
                    int totalCount = 1;
                    int firstColumn = 1;
                    foreach (List<Team> listT in listTeamsByPool)
                    {

                        var headerRow = new List<string[]>()
                        {
                            new string[] { "Pool #" + numPool },
                            new string[] { "Classement", "Équipe", "Points", "Goal average" }
                        };
                        string headerRange = Char.ConvertFromUtf32(firstColumn + 64)+ "1:" + Char.ConvertFromUtf32(firstColumn + headerRow[1].Length + 64) + "2";
                        worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                        worksheet.Cells[headerRange].Style.Font.Bold = true;
                        worksheet.Cells[headerRange].Style.Font.Size = 14;
                        worksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                        worksheet.Cells[headerRange].AutoFitColumns();

                        for (int k = 0; k < listT.Count; k++)
                        {
                            worksheet.Cells[k+3, firstColumn].Value = totalCount;
                            worksheet.Cells[k+3, firstColumn+1].Value = listT[k].Name;
                            worksheet.Cells[k+3, firstColumn+2].Value = listT[k].PointsOfVictoriesAndDraws;
                            worksheet.Cells[k+3, firstColumn+3].Value = listT[k].ScoresSum;
                            totalCount++;
                        }
                        numPool++;
                        firstColumn += headerRow[1].Length + 1;
                    }
                    excel.SaveAs(existingFile);
                }
            }

        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}
