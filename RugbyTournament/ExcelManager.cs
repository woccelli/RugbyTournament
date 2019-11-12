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
                var headerRow = new List<string[]>()
                {
                    new string[] { "NumMatch", "Équipe 1", "Équipe 2", "Score Équipe 1", "Score Équipe 2" }
                };

                // Determine the header range (e.g. A1:E1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";


                foreach (Pool p in pTab)
                {
                    excel.Workbook.Worksheets.Add("Pool #" + p.PoolId ); //Each pool has a different worksheet
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
                        worksheet.Cells[i + 2, 1].Value = i+1;
                        worksheet.Cells[i + 2, 2].Value = p.GameList[i].TeamA.Name;
                        worksheet.Cells[i + 2, 3].Value = p.GameList[i].TeamB.Name;
                    }
                }

                FileInfo excelFile = new FileInfo(fileName);
                excel.SaveAs(excelFile); //save the created file
            }
        }

        /// <summary>
        /// Method used to read the scores of the tournament and directly update the data contained in the objects of this program
        /// </summary>
        /// <param name="fileName">Path of the Excel file to read</param>
        /// <param name="pTab">Pools of the tournament</param>
        public void ReadResultsFromExcelFile(string fileName, Pool[] pTab)
        {
            FileInfo existingFile = new FileInfo(fileName);
            using (ExcelPackage excel = new ExcelPackage(existingFile))
            {
                foreach (Pool p in pTab)
                {
                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Pool #" + p.PoolId];

                    for (int i = 0; i < p.GetNbOfGames(); i++) //read the scores
                    {
                        int scoreA = Convert.ToInt32(worksheet.Cells[i + 2, 4].Value.ToString()); 
                        int scoreB = Convert.ToInt32(worksheet.Cells[i + 2, 5].Value.ToString());
                        p.GameList[i].EnterScore(scoreA, scoreB);
                    }
                }
            }
        }
    }
}
